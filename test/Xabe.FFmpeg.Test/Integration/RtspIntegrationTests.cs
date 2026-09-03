using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg.Test.Common.Fixtures;
using Xunit;

namespace Xabe.FFmpeg.Test.Integration
{
    [Collection(RtspIntegrationSuite.CollectionName)]
    public class RtspIntegrationTests(StorageFixture storageFixture, RtspServerFixture rtspServer) : IClassFixture<StorageFixture>
    {
        [Fact]
        public async Task GetMediaInfo_RTSP_CorrectDataIsShown()
        {
            await rtspServer.Publish(Resources.BunnyMp4, "bunny2");

            var result = await FFmpeg.GetMediaInfo(rtspServer.GetStreamUri("bunny2").OriginalString);

            Assert.Single(result.VideoStreams);
            Assert.Single(result.AudioStreams);
            Assert.Empty(result.SubtitleStreams);
            Assert.Equal("h264", result.VideoStreams.First().Codec);
            Assert.Equal(23.976, result.VideoStreams.First().Framerate);
            Assert.Equal(640, result.VideoStreams.First().Width);
            Assert.Equal(360, result.VideoStreams.First().Height);
            Assert.Equal("aac", result.AudioStreams.First().Codec);
        }

        [Fact]
        public async Task GetMediaInfo_MissingRtspStream_ThrowsArgumentException()
        {
            var exception = await Record.ExceptionAsync(
                async () => await FFmpeg.GetMediaInfo(rtspServer.GetStreamUri("notExisting").OriginalString));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public async Task GetMediaInfo_MissingRtspStream_CallerCancelsProbe_ThrowsArgumentException()
        {
            using var cancellationTokenSource = new CancellationTokenSource(2000);
            var exception = await Record.ExceptionAsync(
                async () => await FFmpeg.GetMediaInfo(rtspServer.GetStreamUri("notExisting").OriginalString, cancellationTokenSource.Token));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public async Task GetMediaInfo_UnavailableRtspEndpoint_ThrowsArgumentException()
        {
            var uri = new Uri($"rtsp://127.0.0.1:{TakeClosedLoopbackPort()}/notExisting");
            var exception = await Record.ExceptionAsync(
                async () => await FFmpeg.GetMediaInfo(uri.OriginalString));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public async Task SendToRtspServer_MinimumConfiguration_FileIsBeingStreamed()
        {
            // Arrange
            var output = rtspServer.GetStreamUri("newFile");
            using var cancellationTokenSource = new CancellationTokenSource();

            // Act
            var conversion = await FFmpeg.Conversions.FromSnippet.SendToRtspServer(Resources.Mp4, output);
            var task = conversion.Start(cancellationTokenSource.Token);
            await rtspServer.WaitForStreamAsync(output);

            try
            {
                // Assert
                var info = await MediaInfo.Get(output.OriginalString);

                Assert.Single(info.Streams);
            }
            finally
            {
                cancellationTokenSource.Cancel();
                try
                {
                    await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(15)));
                }
                catch
                {
                    // The publisher dies by cancellation; its completion shape is not the subject of this test.
                }
            }
        }

        [Fact]
        public async Task Rtsp_GotTwoStreams_SaveEverything()
        {
            var output = storageFixture.GetTempFileName(FileExtensions.Mp4);
            var streamUri = rtspServer.GetStreamUri("bunny");
            await rtspServer.Publish(Resources.BunnyMp4, "bunny");

            var mediaInfo = await FFmpeg.GetMediaInfo(streamUri.OriginalString);
            await FFmpeg.Conversions.New()
                                  .AddStream(mediaInfo.Streams)
                                  .SetInputTime(TimeSpan.FromSeconds(3))
                                  .SetOutput(output)
                                  .Start();

            IMediaInfo result = await FFmpeg.GetMediaInfo(output);
            Assert.True(result.Duration > TimeSpan.FromSeconds(0));
            Assert.Single(result.VideoStreams);
            Assert.Single(result.AudioStreams);
            Assert.Empty(result.SubtitleStreams);
            Assert.Equal("h264", result.VideoStreams.First().Codec);
            Assert.Equal(23, (int)result.VideoStreams.First().Framerate);
            Assert.Equal(640, result.VideoStreams.First().Width);
            Assert.Equal(360, result.VideoStreams.First().Height);
            Assert.Equal("aac", result.AudioStreams.First().Codec);
        }

        private static int TakeClosedLoopbackPort()
        {
            // Bind an ephemeral loopback port, then release it: the result is a port that reliably refuses RTSP
            // connections on this machine, without relying on any external network.
            using var probe = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            probe.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            return ((IPEndPoint)probe.LocalEndPoint).Port;
        }
    }
}
