using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Xabe.FFmpeg;
using Xunit;

namespace Xabe.FFmpeg.Test.Common.Fixtures
{
    /// <summary>
    ///     Spins up one pinned <c>aler9/rtsp-simple-server</c> container on a random free host port, owns the FFmpeg
    ///     publishers feeding its streams, and tears everything down in a bounded way. See the "Testing" section in the
    ///     README for prerequisites (Docker on an x64 machine, FFmpeg/FFprobe on PATH).
    /// </summary>
    public sealed class RtspServerFixture : IAsyncLifetime
    {
        private const string ImageRepository = "aler9/rtsp-simple-server";
        private const string ImageRegistry = "docker.io";
        private const string ImageTag = "v1.12.2";

        // Immutable pin of the linux/amd64 manifest of the multi-arch manifest list
        // sha256:691bd1d4cef49226d1d3c86db2e9abe1fe8c7f98547cdc77d1c464637adadb6e.
        // Resolved from the registry on 2026-09-01; the pull can never drift from the exact tested image.
        private const string ImageDigest = "sha256:e51ed14e59ca157622470ff6084fe3e2aac385e456dce80ffd26ba068cf88f16";
        private const string ImagePlatform = "linux/amd64";

        private const int ContainerPort = 8554;
        private static readonly TimeSpan StartupBudget = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan StreamReadinessBudget = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan PublisherDrainBudget = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan ContainerStopBudget = TimeSpan.FromSeconds(30);

        private readonly Dictionary<string, Publisher> _publishers = new(StringComparer.OrdinalIgnoreCase);
        private readonly object _publishersLock = new();
        private readonly LogCapturing _logs = new();
        private IContainer _container;
        private string _endpointHost;
        private int _port;
        private volatile bool _initialized;
        private volatile bool _disposed;

        private sealed class Publisher
        {
            public string Name { get; set; }
            public string Arguments { get; set; }
            public CancellationTokenSource Cancellation { get; set; }
            public Task<IConversionResult> Task { get; set; }
        }

        public async Task InitializeAsync()
        {
            EnsureUsableWorkingDirectory();
            EnsureNativeX64();

            var image = new DockerImage(ImageRepository, ImageRegistry, ImageTag, ImageDigest, ImagePlatform);
            _container = new ContainerBuilder(image)
                                  .WithName($"xabertsp-{Guid.NewGuid():N}".Substring(0, 20))
                                  .WithEnvironment("RTSP_PROTOCOLS", "tcp")
                                  .WithPortBinding(ContainerPort, assignRandomHostPort: true)
                                  .WithAutoRemove(false)
                                  .WithOutputConsumer(_logs.Consumer)
                                  // The image is distroless (no shell), so exec-based waits cannot work.
                                  // The server logs this line as soon as its RTSP listener is bound.
                                  .WithWaitStrategy(Wait.ForUnixContainer()
                                                          .UntilMessageIsLogged("listener opened on :8554",
                                                              strategy => strategy
                                                                          .WithTimeout(TimeSpan.FromSeconds(30))
                                                                          .WithInterval(TimeSpan.FromMilliseconds(250))))
                                  .Build();

            using var startup = new CancellationTokenSource(StartupBudget);
            try
            {
                await _container.StartAsync(startup.Token);
            }
            catch (Exception ex)
            {
                await CleanupContainerSilentlyAsync();

                if (LooksLikeUnreachableDocker(ex))
                {
                    throw new InvalidOperationException(
                        "Could not reach a Docker daemon, but the RTSP integration tests require one. " +
                        "Install Docker, make sure the daemon is running, and re-run the tests. " +
                        "The fixture locates Docker via TESTCONTAINERS_HOST_OVERRIDE, DOCKER_HOST, or the default local socket. " +
                        $"Cause: {ex}",
                        ex);
                }

                throw new InvalidOperationException(
                    $"The rtsp-simple-server container did not start within {StartupBudget.TotalMinutes:0} minutes. {await DiagnosticsAsync()}",
                    ex);
            }

            _endpointHost = _container.Hostname;
            _port = _container.GetMappedPublicPort(ContainerPort);
            _initialized = true;

            await WaitUntilServerRespondsAsync();
        }

        public async Task DisposeAsync()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            try
            {
                Publisher[] publishers;
                lock (_publishersLock)
                {
                    publishers = _publishers.Values.ToArray();
                }

                // Publishers stop before the server so FFmpeg can terminate gracefully through 'q'.
                foreach (var publisher in publishers)
                {
                    publisher.Cancellation.Cancel();
                }

                await Task.WhenAll(publishers
                                           .Select(publisher => Task.WhenAny(publisher.Task, Task.Delay(PublisherDrainBudget))));

                foreach (var publisher in publishers)
                {
                    publisher.Cancellation.Dispose();
                }
            }
            catch
            {
                // Tear-down must never mask the outcome of a test.
            }

            try
            {
                if (_container != null)
                {
                    using var stop = new CancellationTokenSource(ContainerStopBudget);
                    await _container.StopAsync(stop.Token);
                }
            }
            catch
            {
                // Container reaping falls back to the Testcontainers resource reaper; do not mask test outcomes.
            }

            try
            {
                if (_container != null)
                {
                    await _container.DisposeAsync();
                }
            }
            catch
            {
                // See above.
            }

        }

        public Uri GetStreamUri(string name)
        {
            EnsureInitialized();
            ValidateStreamName(name);

            return new Uri($"rtsp://{_endpointHost}:{_port}/{name}");
        }

        /// <summary>
        ///     Streams <paramref name="filePath"/> to the RTSP server under <paramref name="name"/> and blocks until
        ///     the stream is readable. Republishing an existing name replaces the running publisher.
        /// </summary>
        public async Task Publish(string filePath, string name)
        {
            EnsureInitialized();
            ValidateStreamName(name);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The file to publish does not exist.", filePath);
            }

            var streamUri = GetStreamUri(name);
            var arguments = BuildPublisherArguments(filePath, streamUri);

            Publisher publisher;
            lock (_publishersLock)
            {
                if (_publishers.TryGetValue(name, out var existing) && !existing.Task.IsCompleted)
                {
                    // Superseded: request a graceful stop; its drain happens in parallel.
                    existing.Cancellation.Cancel();
                }

                publisher = new Publisher
                {
                    Name = name,
                    Arguments = arguments,
                    Cancellation = new CancellationTokenSource(),
                };
                publisher.Task =
                    FFmpeg.Conversions.New().AddParameter(arguments).Start(publisher.Cancellation.Token);
                _publishers[name] = publisher;
            }

            try
            {
                await WaitForStreamAsync(streamUri);
            }
            catch
            {
                await AbortPublisherAsync(publisher);
                throw;
            }
        }

        public async Task StopPublisherAsync(string name)
        {
            EnsureInitialized();

            Publisher publisher;
            lock (_publishersLock)
            {
                if (!_publishers.TryGetValue(name, out publisher))
                {
                    return;
                }

                _publishers.Remove(name);
            }

            await DrainPublisherAsync(publisher);
        }

        /// <summary>
        ///     Waits until an RTSP stream answers a probe, or the budget runs out.
        /// </summary>
        public async Task WaitForStreamAsync(Uri streamUri, TimeSpan? budget = null)
        {
            EnsureInitialized();
            budget ??= StreamReadinessBudget;

            using var cts = new CancellationTokenSource(budget.Value);
            while (true)
            {
                var publisher = FindPublisherByUri(streamUri);
                if (publisher != null && publisher.Task.IsFaulted)
                {
                    var failure = publisher.Task.Exception.GetBaseException();
                    throw new InvalidOperationException(
                        $"The FFmpeg publisher for '{streamUri}' terminated before the stream became readable. " +
                        $"Details: {failure.Message}{await DiagnosticsAsync()}",
                        failure);
                }

                try
                {
                    using var probe = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);
                    probe.CancelAfter(TimeSpan.FromSeconds(8));
                    _ = await FFmpeg.GetMediaInfo(streamUri.OriginalString, probe.Token);
                    return;
                }
                catch (ArgumentException)
                {
                    // The server knows no such stream yet; the publisher is still warming up.
                }
                catch (OperationCanceledException)
                {
                    if (cts.IsCancellationRequested)
                    {
                        throw new InvalidOperationException(
                            $"The RTSP stream '{streamUri}' never became readable within {budget.Value.TotalSeconds:0} seconds. {await DiagnosticsAsync()}");
                    }
                }

                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }
        }

        private async Task WaitUntilServerRespondsAsync()
        {
            var probeUri = GetStreamUri("_fixture_probe");
            using var cts = new CancellationTokenSource(StreamReadinessBudget);

            while (true)
            {
                try
                {
                    using var probe = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);
                    probe.CancelAfter(TimeSpan.FromSeconds(8));
                    _ = await FFmpeg.GetMediaInfo(probeUri.OriginalString, probe.Token);
                    throw new InvalidOperationException(
                        $"The RTSP server unexpectedly served '{probeUri}', although nothing publishes that stream.");
                }
                catch (ArgumentException)
                {
                    // The server refused the unknown path: it is alive and speaking RTSP.
                    return;
                }
                catch (OperationCanceledException)
                {
                    if (cts.IsCancellationRequested)
                    {
                        throw new InvalidOperationException(
                            $"The rtsp-simple-server container started, but never answered an RTSP probe within " +
                            $"{StreamReadinessBudget.TotalSeconds:0} seconds. {await DiagnosticsAsync()}");
                    }
                }

                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }
        }

        private static string BuildPublisherArguments(string filePath, Uri streamUri)
        {
            return $"-re -stream_loop -1 -i \"{filePath}\" -pix_fmt yuv420p -vsync 1 -vcodec libx264 -r 23.976 " +
                   $"-threads 0 -b:v: 1024k -bufsize 1024k -preset veryfast -profile:v baseline -tune film -g 48 " +
                   $"-x264opts no-scenecut -acodec aac -b:a 192k -f rtsp {streamUri}";
        }

        private static bool LooksLikeUnreachableDocker(Exception ex)
        {
            for (var e = ex; e != null; e = e.InnerException)
            {
                if (e is DockerUnavailableException)
                {
                    return true;
                }

                var message = e.Message ?? string.Empty;
                if (message.Contains("Initialization has been cancelled", StringComparison.Ordinal)
                    || message.Contains("connection refused", StringComparison.OrdinalIgnoreCase)
                    || message.Contains("unable to find", StringComparison.OrdinalIgnoreCase)
                    || message.Contains("timed out", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<string> DiagnosticsAsync()
        {
            var diagnostics = new StringBuilder();

            try
            {
                if (_container != null)
                {
                    var (stdOut, stdErr) =
                        await _container.GetLogsAsync(DateTime.UtcNow.AddHours(-1), DateTime.UtcNow, false, CancellationToken.None);
                    AppendSection(diagnostics, "container log since start", (stdErr ?? string.Empty) + (stdOut ?? string.Empty));
                }
            }
            catch
            {
                // Best effort; the consumer capture below still applies.
            }

            AppendSection(diagnostics, "container stderr (live capture)", _logs.StderrTail());
            AppendSection(diagnostics, "container stdout (live capture)", _logs.StdoutTail());

            string publisherStates;
            lock (_publishersLock)
            {
                publisherStates = _publishers
                                          .Values
                                          .Select(publisher => $"{publisher.Name}: {publisher.Task.Status}, arguments: {publisher.Arguments}")
                                          .Aggregate((first, second) => first + Environment.NewLine + second);
            }

            if (!string.IsNullOrEmpty(publisherStates))
            {
                AppendSection(diagnostics, "FFmpeg publishers", publisherStates);
            }

            return diagnostics.Length > 0 ? Environment.NewLine + diagnostics : string.Empty;
        }

        private static void AppendSection(StringBuilder builder, string title, string content)
        {
            if (string.IsNullOrWhiteSpace(content) || content.Trim() == "<empty>")
            {
                return;
            }

            var trimmed = content.Trim();
            if (trimmed.Length > 16 * 1024)
            {
                trimmed = trimmed[^16384..];
            }

            builder.Append(Environment.NewLine).Append("--- ").Append(title).Append(" ---").Append(Environment.NewLine).Append(trimmed);
        }

        private static void EnsureUsableWorkingDirectory()
        {
            string cwd = null;
            try
            {
                cwd = Directory.GetCurrentDirectory();
            }
            catch (IOException)
            {
                cwd = null;
            }

            if (cwd == null || !Directory.Exists(cwd))
            {
                // Testcontainers 4.14 snapshots the CWD inside a one-shot type initializer
                // (TestcontainersClient.OSRootDirectory); a getcwd failure there permanently
                // latches TypeInitializationException into every RTSP test. A parallel
                // collection that parked the CWD in a temp dir and had it removed would do
                // exactly that, so pin a CWD that lives as long as the process.
                Directory.SetCurrentDirectory(AppContext.BaseDirectory);
            }
        }

        private static void EnsureNativeX64()
        {
            if (RuntimeInformation.OSArchitecture != Architecture.X64)
            {
                throw new NotSupportedException(
                    $"The RTSP fixture requires an x64 machine; found '{RuntimeInformation.OSArchitecture}'. " +
                    "Running an amd64 container under emulation would exceed the startup budget.");
            }
        }

        private static void ValidateStreamName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)
                || name.Any(character => !(char.IsLetterOrDigit(character) || character is '.' or '-' or '_')))
            {
                throw new ArgumentException(
                    $"Stream name '{name}' may only contain letters, digits, '.', '-' and '_'.",
                    nameof(name));
            }
        }

        private Publisher FindPublisherByUri(Uri streamUri)
        {
            var name = streamUri.AbsolutePath.Trim('/');
            lock (_publishersLock)
            {
                return name.Length > 0 && _publishers.TryGetValue(name, out var publisher) ? publisher : null;
            }
        }

        private async Task AbortPublisherAsync(Publisher publisher)
        {
            lock (_publishersLock)
            {
                _publishers.Remove(publisher.Name);
            }

            await DrainPublisherAsync(publisher);
        }

        private static async Task DrainPublisherAsync(Publisher publisher)
        {
            publisher.Cancellation.Cancel();

            // The publisher task completes with OperationCanceledException once cancelled -
            // awaiting it directly is intentionally avoided.
            await Task.WhenAny(publisher.Task, Task.Delay(PublisherDrainBudget));
            publisher.Cancellation.Dispose();
        }

        private async Task CleanupContainerSilentlyAsync()
        {
            try
            {
                if (_container != null)
                {
                    await _container.DisposeAsync();
                }
            }
            catch
            {
                // Best effort; the resource reaper is the backstop.
            }
        }

        private void EnsureInitialized()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (!_initialized)
            {
                throw new InvalidOperationException("The RTSP server fixture has not been initialized.");
            }
        }

#pragma warning disable CA1001 // Managed-memory sinks; reclaimed by the GC together with the fixture.
        private sealed class LogCapturing
        {
            private readonly BoundedByteSink _stdout;
            private readonly BoundedByteSink _stderr;

            public LogCapturing()
            {
                _stdout = new BoundedByteSink(64 * 1024);
                _stderr = new BoundedByteSink(64 * 1024);
                Consumer = new OutputConsumerImpl(_stdout, _stderr);
            }

            public IOutputConsumer Consumer { get; }

            public string StderrTail()
            {
                return _stderr.Tail(8 * 1024);
            }

            public string StdoutTail()
            {
                return _stdout.Tail(2 * 1024);
            }

            private sealed class OutputConsumerImpl : IOutputConsumer
            {
                private readonly BoundedByteSink _stdout;
                private readonly BoundedByteSink _stderr;

                public OutputConsumerImpl(BoundedByteSink stdout, BoundedByteSink stderr)
                {
                    _stdout = stdout;
                    _stderr = stderr;
                }

                public bool Enabled => true;

                public Stream Stdout => _stdout;

                public Stream Stderr => _stderr;

                public void Dispose()
                {
                }
            }
        }
#pragma warning restore CA1001

        private sealed class BoundedByteSink : Stream
        {
            private readonly object _sync = new();
            private readonly byte[] _buffer;
            private int _length;

            public BoundedByteSink(int capacity)
            {
                _buffer = new byte[Math.Max(capacity, 1)];
            }

            public override bool CanRead => true;

            public override bool CanSeek => false;

            public override bool CanWrite => true;

            public override long Length
            {
                get
                {
                    lock (_sync)
                    {
                        return _length;
                    }
                }
            }

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                lock (_sync)
                {
                    var take = Math.Min(count, _length);
                    Buffer.BlockCopy(_buffer, 0, buffer, offset, take);
                    if (take < _length)
                    {
                        Buffer.BlockCopy(_buffer, take, _buffer, 0, _length - take);
                    }

                    _length -= take;
                    return count == 0 || take > 0 ? take : -1;
                }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                lock (_sync)
                {
                    if (_length + count > _buffer.Length)
                    {
                        var drop = _length + count - _buffer.Length;
                        Buffer.BlockCopy(_buffer, drop, _buffer, 0, _length - drop);
                        _length -= drop;
                    }

                    Buffer.BlockCopy(buffer, offset, _buffer, _length, count);
                    _length += count;
                }
            }

            public string Tail(int maxLength)
            {
                lock (_sync)
                {
                    if (_length == 0)
                    {
                        return "<empty>";
                    }

                    var start = _length - Math.Min(maxLength, _length);
                    var end = Math.Min(maxLength, _length);
                    return Encoding.Latin1.GetString(_buffer, start, end);
                }
            }
        }
    }
}
