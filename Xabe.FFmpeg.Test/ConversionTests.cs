using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xabe.FFmpeg.Enums;
using Xabe.FFmpeg.Exceptions;
using Xabe.FFmpeg.Model;
using Xabe.FFmpeg.Streams;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class ConversionTests
    {
        [Theory]
        [InlineData(Position.UpperRight)]
        [InlineData(Position.BottomRight)]
        [InlineData(Position.Left)]
        [InlineData(Position.Right)]
        [InlineData(Position.Up)]
        [InlineData(Position.BottomLeft)]
        [InlineData(Position.UpperLeft)]
        [InlineData(Position.Center)]
        [InlineData(Position.Bottom)]
        public async Task WatermarkTest(Position position)
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            IVideoStream stream = inputFile.VideoStreams.First()
                                  .SetWatermark(Resources.PngSample, position);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .SetPreset(ConversionPreset.UltraFast)
                                                                 .AddStream(stream)
                                                                 .SetOutput(outputPath)
                                                                 .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            Assert.Contains("overlay", conversionResult.Arguments);
            Assert.Contains(Resources.PngSample, conversionResult.Arguments);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task SetVideoCodecTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.Mpeg4);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(videoStream)
                                                                 .SetOutput(output)
                                                                 .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal("mpeg4", resultFile.VideoStreams.First().Format);
        }

        [Fact]
        public async Task SetAudioCodecTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.Ac3);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(audioStream)
                                                                 .SetOutput(output)
                                                                 .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal("ac3", resultFile.AudioStreams.First().Format);
        }

        [Fact]
        public async Task OverwriteFilesTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.Ac3);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(audioStream)
                                                                 .SetOutput(output)
                                                                 .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            Assert.Contains("-n ", conversionResult.Arguments);

            IConversionResult secondConversionResult = await Conversion.New()
                                                                 .AddStream(audioStream)
                                                                 .SetOverwriteOutput(true)
                                                                 .SetOutput(output)
                                                                 .Start().ConfigureAwait(false);

            Assert.True(secondConversionResult.Success);
            Assert.Contains(" -y ", secondConversionResult.Arguments);
            Assert.DoesNotContain(" -n ", secondConversionResult.Arguments);
        }

        [Fact]
        public async Task OverwriteFilesExceptionTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.Ac3);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(audioStream)
                                                                 .SetOutput(output)
                                                                 .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            Assert.Contains("-n ", conversionResult.Arguments);

            await Assert.ThrowsAsync<ConversionException>(() => Conversion.New()
                                                                          .AddStream(audioStream)
                                                                          .SetOutput(output)
                                                                          .Start()).ConfigureAwait(false);
        }

        [RunnableInDebugOnly]
        public async Task UseHardwareAcceleration()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);

            IConversionResult conversionResult = await Conversion.Convert(Resources.MkvWithAudio, output).UseHardwareAcceleration(HardwareAccelerator.Auto, VideoCodec.H264_cuvid, VideoCodec.H264_nvenc, 0).Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal("h264", resultFile.VideoStreams.First().Format);
        }


        [Theory]
        [InlineData("a16f0cb5c0354b6197e9f3bc3108c017")]
        public async Task MissingHardwareAccelerator(string hardwareAccelerator)
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);

            await Assert.ThrowsAsync<HardwareAcceleratorNotFoundException>(async () =>
            {
                await Conversion.Convert(Resources.MkvWithAudio, output).UseHardwareAcceleration(new HardwareAccelerator(hardwareAccelerator), VideoCodec.H264_cuvid, VideoCodec.H264_nvenc).Start().ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task UnknownDecoderException()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.Mpeg4);

            await Assert.ThrowsAsync<UnknownDecoderException>(async () =>
            {
                await Conversion.Convert(Resources.MkvWithAudio, output).UseHardwareAcceleration(HardwareAccelerator.Auto, VideoCodec.H264_nvenc, VideoCodec.H264_cuvid).Start().ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task RTSP_NotExistingStream_CancelledSucesfully()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.WebM);
            var cancellationToken = new CancellationTokenSource();
            var conversion = Conversion.New().AddStream(new WebStream(new Uri(@"rtsp://192.168.1.123:554/"), "M3U8", TimeSpan.FromMinutes(5))).SetOutput(output);
            var conversionTask = conversion.Start(cancellationToken.Token);
            cancellationToken.CancelAfter(2000);
            await Task.Delay(500).ConfigureAwait(false);
            var ffmpegProcesses = System.Diagnostics.Process.GetProcessesByName("ffmpeg");
            ffmpegProcesses.Any(_ => _.Id == conversion.FFmpegProcessId && !_.HasExited).Should().BeTrue();

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await conversionTask.ConfigureAwait(false)).ConfigureAwait(false);

            conversion.FFmpegProcessId.Should().BeGreaterThan(0);

            ffmpegProcesses = System.Diagnostics.Process.GetProcessesByName("ffmpeg");
            ffmpegProcesses.Any(_ => _.Id == conversion.FFmpegProcessId && !_.HasExited).Should().BeFalse();
        }

        [Fact]
        public async Task Conversion_CancellationOccurs_ExeptionWasThrown()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.WebM);
            var cancellationTokenSource = new CancellationTokenSource();

            var task = Conversion.ToWebM(Resources.Mp4WithAudio, output)
                    .SetPreset(ConversionPreset.UltraFast)
                    .Start(cancellationTokenSource.Token);

            cancellationTokenSource.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await task.ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(8)]
        [InlineData(0)]
        public async Task UseMultithreadTest(int expectedThreadsCount)
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(info.VideoStreams.First())
                                                                 .UseMultiThread(expectedThreadsCount)
                                                                 .SetOutput(output)
                                                                 .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal("h264", resultFile.VideoStreams.First().Format);
            Assert.Contains($"-threads {expectedThreadsCount}", conversionResult.Arguments);
        }

        [Fact]
        public async Task UseMultithreadTest_WithoutThreadCount_AllThreads()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(info.VideoStreams.First())
                                                                 .UseMultiThread(true)
                                                                 .SetOutput(output)
                                                                 .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal("h264", resultFile.VideoStreams.First().Format);
            Assert.Contains($"-threads {Environment.ProcessorCount}", conversionResult.Arguments);
        }

        [Fact]
        public async Task UseMultithreadTest_WithoutMultithread_OneThreadOnly()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(info.VideoStreams.First())
                                                                 .UseMultiThread(false)
                                                                 .SetOutput(output)
                                                                 .Start()
                                                                 .ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal("h264", resultFile.VideoStreams.First().Format);
            Assert.Contains($"-threads 1", conversionResult.Arguments);
        }
    }
}


