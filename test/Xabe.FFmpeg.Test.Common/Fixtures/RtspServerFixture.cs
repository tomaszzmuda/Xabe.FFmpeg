using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Xabe.FFmpeg;
using Xunit;

namespace Xabe.FFmpeg.Test.Common.Fixtures
{
    /// <summary>
    ///     Runs one pinned <c>aler9/rtsp-simple-server</c> container on a random free host port and owns the
    ///     FFmpeg publishers that feed its streams. Needs a running Docker daemon.
    /// </summary>
    public sealed class RtspServerFixture : IAsyncLifetime
    {
        private const string IMAGE = "aler9/rtsp-simple-server:v1.12.2";
        private const int CONTAINER_PORT = 8554;
        private static readonly TimeSpan STARTUP_BUDGET = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan STREAM_READINESS_BUDGET = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan PROBE_BUDGET = TimeSpan.FromSeconds(8);
        private static readonly TimeSpan PUBLISHER_DRAIN_BUDGET = TimeSpan.FromSeconds(10);

        private sealed class Publisher
        {
            public CancellationTokenSource Cancellation { get; init; }
            public Task<IConversionResult> Task { get; init; }
        }

        private readonly Dictionary<string, Publisher> _publishers = new();
        private IContainer _container;
        private string _host;
        private int _port;

        public async Task InitializeAsync()
        {
            _container = new ContainerBuilder(new DockerImage(IMAGE))
                          .WithEnvironment("RTSP_PROTOCOLS", "tcp")
                          .WithPortBinding(CONTAINER_PORT, assignRandomHostPort: true)
                          .WithWaitStrategy(Wait.ForUnixContainer()
                                                  .UntilMessageIsLogged("listener opened on :8554", o => o.WithTimeout(STARTUP_BUDGET)))
                          .Build();

            await _container.StartAsync();

            _host = _container.Hostname;
            _port = _container.GetMappedPublicPort(CONTAINER_PORT);
        }

        public async Task DisposeAsync()
        {
            try
            {
                foreach (var publisher in _publishers.Values)
                {
                    publisher.Cancellation.Cancel();
                }

                await Task.WhenAll(_publishers.Values.Select(publisher => Task.WhenAny(publisher.Task, Task.Delay(PUBLISHER_DRAIN_BUDGET))));

                if (_container != null)
                {
                    await _container.DisposeAsync();
                }
            }
            catch
            {
                // Teardown must never mask the outcome of a test.
            }
        }

        public Uri GetStreamUri(string name)
        {
            return new Uri($"rtsp://{_host}:{_port}/{name}");
        }

        /// <summary>
        ///     Streams <paramref name="filePath"/> to the server under <paramref name="name"/> and blocks until
        ///     the stream is readable. Publishing an existing name replaces the running publisher.
        /// </summary>
        public async Task Publish(string filePath, string name)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The file to publish does not exist.", filePath);
            }

            if (_publishers.TryGetValue(name, out var existing))
            {
                existing.Cancellation.Cancel();
            }

            var streamUri = GetStreamUri(name);
            var cts = new CancellationTokenSource();
            var publisher = new Publisher
            {
                Cancellation = cts,
                Task = FFmpeg.Conversions.New().AddParameter(BuildPublisherArguments(filePath, streamUri)).Start(cts.Token)
            };
            _publishers[name] = publisher;

            try
            {
                await WaitForStreamAsync(streamUri);
            }
            catch
            {
                cts.Cancel();
                await Task.WhenAny(publisher.Task, Task.Delay(PUBLISHER_DRAIN_BUDGET));
                throw;
            }
        }

        /// <summary>
        ///     Blocks until the stream at <paramref name="streamUri"/> answers a probe.
        /// </summary>
        public async Task WaitForStreamAsync(Uri streamUri, TimeSpan? budget = null)
        {
            budget ??= STREAM_READINESS_BUDGET;
            using var deadline = new CancellationTokenSource(budget.Value);

            while (true)
            {
                try
                {
                    using var probe = CancellationTokenSource.CreateLinkedTokenSource(deadline.Token);
                    probe.CancelAfter(PROBE_BUDGET);
                    _ = await FFmpeg.GetMediaInfo(streamUri.OriginalString, probe.Token);
                    return;
                }
                catch (ArgumentException)
                {
                    // The server knows no such stream yet; the publisher is still warming up.
                }
                catch (OperationCanceledException)
                {
                    if (deadline.IsCancellationRequested)
                    {
                        throw new TimeoutException($"The RTSP stream '{streamUri}' was not readable within {budget.Value.TotalSeconds:0} seconds.");
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
    }
}
