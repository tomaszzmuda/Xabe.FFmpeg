using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg.Exceptions;
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
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            IVideoStream stream = inputFile.VideoStreams.First()
                                  .SetWatermark(Resources.PngSample, position);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .SetPreset(ConversionPreset.UltraFast)
                                                                 .AddStream(stream)
                                                                 .SetOutput(outputPath)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            Assert.Contains("overlay", conversionResult.Arguments);
            Assert.Contains(Resources.PngSample, conversionResult.Arguments);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task SetInputFormatTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mpeg);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.Mpeg4);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .SetInputFormat(MediaFormat.Matroska)
                                                                 .AddStream(videoStream)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal("mpeg4", resultFile.VideoStreams.First().Format);
        }

        [Fact]
        public async Task SetOutputFormatTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Ts);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.Mpeg4);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(videoStream)
                                                                 .SetOutputFormat(MediaFormat.Mpegts)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal(".ts", Path.GetExtension(resultFile.Path));
        }

        [Fact]
        public async Task SetInputAndOutputFormatTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Avi);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.Mpeg4);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .SetInputFormat(MediaFormat.Matroska)
                                                                 .AddStream(videoStream)
                                                                 .SetOutputFormat(MediaFormat.Avi)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal("mpeg4", resultFile.VideoStreams.First().Format);
            Assert.Equal(".avi", Path.GetExtension(resultFile.Path));
        }

        [Fact]
        public async Task SetOutputPixelFormatTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.Mpeg4);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(videoStream)
                                                                 .SetOutputPixelFormat(PixelFormat.Yuv420P)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal("yuv420p", resultFile.VideoStreams.First().PixelFormat);
        }

        [Theory]
        [InlineData(HashFormat.MD5, 37L)]
        [InlineData(HashFormat.murmur3, 41L)]
        [InlineData(HashFormat.RIPEMD128, 43L)]
        [InlineData(HashFormat.RIPEMD160, 51L)]
        [InlineData(HashFormat.RIPEMD256, 75L)]
        [InlineData(HashFormat.RIPEMD320, 91L)]
        [InlineData(HashFormat.SHA160, 48L)]
        [InlineData(HashFormat.SHA224, 64L)]
        [InlineData(HashFormat.SHA256, 72L)]
        [InlineData(HashFormat.SHA512_224, 68L)]
        [InlineData(HashFormat.SHA512_256, 76L)]
        [InlineData(HashFormat.SHA384, 104L)]
        [InlineData(HashFormat.SHA512, 136L)]
        [InlineData(HashFormat.CRC32, 15L)]
        [InlineData(HashFormat.adler32, 17L)]
        public async Task SetHashFormatTest(HashFormat hashFormat, long expectedLength)
        {
            string fileExtension = FileExtensions.Txt;
            if (hashFormat == HashFormat.SHA256)
                fileExtension = FileExtensions.Sha256;

            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + fileExtension);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.Copy);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.Copy);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(videoStream)
                                                                 .AddStream(audioStream)
                                                                 .SetOutputFormat(MediaFormat.Hash)
                                                                 .SetHashFormat(hashFormat)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            FileInfo fi = new FileInfo(output);
            
            Assert.Equal(fileExtension, fi.Extension);
            Assert.Equal(expectedLength, fi.Length);
        }

        [Fact]
        public async Task SetHashFormat_HashInString_CorrectLenght()
        {
            string fileExtension = FileExtensions.Txt;

            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + fileExtension);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.Copy);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.Copy);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(videoStream)
                                                                 .AddStream(audioStream)
                                                                 .SetOutputFormat(MediaFormat.Hash)
                                                                 .SetHashFormat("SHA512/224")
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            FileInfo fi = new FileInfo(output);

            Assert.Equal(fileExtension, fi.Extension);
            Assert.Equal(68L, fi.Length);

        }

        [RunnableInDebugOnly]
        public async Task GetScreenCaptureTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .GetScreenCapture(30)
                                                                 .SetInputTime(TimeSpan.FromSeconds(3))
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal("h264", resultFile.VideoStreams.First().Format);
            Assert.Equal(29.97, resultFile.VideoStreams.First().Framerate);
            Assert.Equal(TimeSpan.FromSeconds(3), resultFile.VideoStreams.First().Duration);
        }

        [Fact]
        public async Task SetVideoCodecTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.Mpeg4);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(videoStream)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal("mpeg4", resultFile.VideoStreams.First().Format);
        }

        [Fact]
        public async Task SetAudioCodecTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.Ac3);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(audioStream)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal("ac3", resultFile.AudioStreams.First().Format);
        }

        [Fact]
        public async Task SetInputTimeTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First();
            IVideoStream videoStream = info.VideoStreams.First();

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(videoStream)
                                                                 .AddStream(audioStream)
                                                                 .SetInputTime(TimeSpan.FromSeconds(5))
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal(TimeSpan.FromSeconds(5), resultFile.AudioStreams.First().Duration);
            Assert.Equal(TimeSpan.FromSeconds(5), resultFile.VideoStreams.First().Duration);
        }

        [Fact]
        public async Task SetOutputTimeTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First();
            IVideoStream videoStream = info.VideoStreams.First();

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(videoStream)
                                                                 .AddStream(audioStream)
                                                                 .SetOutputTime(TimeSpan.FromSeconds(5))
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal(TimeSpan.FromSeconds(5), resultFile.AudioStreams.First().Duration);
            Assert.Equal(TimeSpan.FromSeconds(5), resultFile.VideoStreams.First().Duration);
        }

        [Fact]
        public async Task SetAudioBitrateTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.Ac3);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(audioStream)
                                                                 .SetAudioBitrate(128000)
                                                                 .SetOutput(output)
                                                                 .Start();

            double lowerBound = 128000 * 0.95;
            double upperBound = 128000 * 1.05;
            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.InRange<double>(resultFile.AudioStreams.First().Bitrate, lowerBound, upperBound);
        }

        [Fact]
        public async Task SetLibH264VideoBitrateTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.Libx264);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(videoStream)
                                                                 .SetVideoBitrate(1500000)
                                                                 .SetOutput(output)
                                                                 .Start();

            double lowerBound = 1500000 * 0.95;
            double upperBound = 1500000 * 1.05;
            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.InRange<double>(resultFile.VideoStreams.First().Bitrate, lowerBound, upperBound);
        }

        [Fact]
        public async Task SetH264VideoBitrateTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.H264);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(videoStream)
                                                                 .SetVideoBitrate(1500000)
                                                                 .SetOutput(output)
                                                                 .Start();

            double lowerBound = 1500000 * 0.95;
            double upperBound = 1500000 * 1.05;
            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.InRange<double>(resultFile.VideoStreams.First().Bitrate, lowerBound, upperBound);
        }

        [Fact]
        public async Task SetNonH264VideoBitrateTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.Mpeg4);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(videoStream)
                                                                 .SetVideoBitrate(1500000)
                                                                 .SetOutput(output)
                                                                 .Start();

            double lowerBound = 1500000 * 0.95;
            double upperBound = 1500000 * 1.05;
            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.InRange<double>(resultFile.VideoStreams.First().Bitrate, lowerBound, upperBound);
        }

        [Theory]
        [InlineData(FileExtensions.Png)]
        [InlineData(FileExtensions.WebP)]
        [InlineData(FileExtensions.Jpg)]
        public async Task ExtractEveryNthFrameTest(string extension)
        {
            Guid fileGuid = Guid.NewGuid();
            Func<string, string> outputBuilder = (number) => { return Path.Combine(Path.GetTempPath(), fileGuid + number + extension); };
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First().SetCodec(VideoCodec.Png);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(videoStream)
                                                                 .ExtractEveryNthFrame(10, outputBuilder)
                                                                 .Start();

            Assert.True(conversionResult.Success);

            int outputFilesCount = Directory.EnumerateFiles(Path.GetTempPath())
                .Where(x => x.Contains(fileGuid.ToString()) && Path.GetExtension(x) == extension).Count();

            Assert.Equal(26, outputFilesCount);
        }

        [Theory]
        [InlineData(FileExtensions.Png)]
        [InlineData(FileExtensions.WebP)]
        [InlineData(FileExtensions.Jpg)]
        public async Task ExtractNthFrameTest(string extension)
        {
            Guid fileGuid = Guid.NewGuid();
            Func<string, string> outputBuilder = (number) => { return Path.Combine(Path.GetTempPath(), fileGuid + number + extension); };
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First().SetCodec(VideoCodec.Png);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(videoStream)
                                                                 .ExtractNthFrame(10, outputBuilder)
                                                                 .Start();

            Assert.True(conversionResult.Success);

            int outputFilesCount = Directory.EnumerateFiles(Path.GetTempPath())
                .Where(x => x.Contains(fileGuid.ToString()) && Path.GetExtension(x) == extension).Count();

            Assert.Equal(1, outputFilesCount);
        }

        [Fact]
        public async Task BuildVideoFromImagesTest()
        {
            List<string> files = Directory.EnumerateFiles(Resources.Images).ToList();
            InputBuilder builder = new InputBuilder();
            string preparedFilesDir = string.Empty;
            Func<string, string> inputBuilder = builder.PrepareInputFiles(files, out preparedFilesDir);
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .SetInputFrameRate(1)
                                                                 .BuildVideoFromImages(1, inputBuilder)
                                                                 .SetFrameRate(1)
                                                                 .SetOutputPixelFormat(PixelFormat.Yuv420P)
                                                                 .SetOutput(output)
                                                                 .Start();

            int preparedFilesCount = Directory.EnumerateFiles(preparedFilesDir).ToList().Count;

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal(builder.FileList.Count, preparedFilesCount);
            Assert.Equal(TimeSpan.FromSeconds(12), resultFile.VideoStreams.First().Duration);
            Assert.Equal(1, resultFile.VideoStreams.First().Framerate);
            Assert.Equal("yuv420p", resultFile.VideoStreams.First().PixelFormat);
        }

        [Fact]
        public async Task BuildVideoFromImagesListTest()
        {
            List<string> files = Directory.EnumerateFiles(Resources.Images).ToList();
            string preparedFilesDir = string.Empty;
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .SetInputFrameRate(1)
                                                                 .BuildVideoFromImages(files)
                                                                 .SetFrameRate(1)
                                                                 .SetOutputPixelFormat(PixelFormat.Yuv420P)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal(TimeSpan.FromSeconds(12), resultFile.VideoStreams.First().Duration);
            Assert.Equal(1, resultFile.VideoStreams.First().Framerate);
            Assert.Equal("yuv420p", resultFile.VideoStreams.First().PixelFormat);
        }

        [Fact]
        public async Task OverwriteFilesTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.Ac3);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(audioStream)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            Assert.Contains("-n ", conversionResult.Arguments);

            IConversionResult secondConversionResult = await Conversion.New()
                                                                 .AddStream(audioStream)
                                                                 .SetOverwriteOutput(true)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.True(secondConversionResult.Success);
            Assert.Contains(" -y ", secondConversionResult.Arguments);
            Assert.DoesNotContain(" -n ", secondConversionResult.Arguments);
        }

        [Fact]
        public async Task OverwriteFilesExceptionTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.Ac3);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(audioStream)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            Assert.Contains("-n ", conversionResult.Arguments);

            await Assert.ThrowsAsync<ConversionException>(() => Conversion.New()
                                                                          .AddStream(audioStream)
                                                                          .SetOutput(output)
                                                                          .Start());
        }

        [RunnableInDebugOnly]
        public async Task UseHardwareAcceleration()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);

            IConversionResult conversionResult = await Conversion.Convert(Resources.MkvWithAudio, output).UseHardwareAcceleration(HardwareAccelerator.auto, VideoCodec.H264_cuvid, VideoCodec.H264_nvenc, 0).Start();

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal("h264", resultFile.VideoStreams.First().Format);
        }


        [Theory]
        [InlineData("a16f0cb5c0354b6197e9f3bc3108c017")]
        public async Task MissingHardwareAccelerator(string hardwareAccelerator)
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);

            var exception = await Record.ExceptionAsync(async () =>
            {
                await Conversion.Convert(Resources.MkvWithAudio, output).UseHardwareAcceleration(hardwareAccelerator, VideoCodec.H264_cuvid.ToString(), VideoCodec.H264_nvenc.ToString()).Start();
            });

            Assert.NotNull(exception);
            Assert.IsType<ConversionException>(exception);
            Assert.IsType<HardwareAcceleratorNotFoundException>(exception.InnerException);
        }

        [Fact]
        public async Task UnknownDecoderException()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.Mpeg4);

            var exception = await Record.ExceptionAsync(async() => { 
                await Conversion.Convert(Resources.MkvWithAudio, output).UseHardwareAcceleration(HardwareAccelerator.auto, VideoCodec.H264_nvenc, VideoCodec.H264_cuvid).Start();
            });

            Assert.NotNull(exception);
            Assert.IsType<ConversionException>(exception);
            Assert.IsType<UnknownDecoderException>(exception.InnerException);
        }

        [Fact]
        public async Task RTSP_NotExistingStream_CancelledSucesfully()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.WebM);
            var cancellationToken = new CancellationTokenSource();
            var conversion = Conversion.New().AddStream(new WebStream(new Uri(@"rtsp://192.168.1.123:554/"), "M3U8", TimeSpan.FromMinutes(5))).SetOutput(output);
            var conversionTask = conversion.Start(cancellationToken.Token);
            cancellationToken.CancelAfter(2000);
            await Task.Delay(500);
            var ffmpegProcesses = System.Diagnostics.Process.GetProcessesByName("ffmpeg");
            Assert.Contains(ffmpegProcesses, _ => _.Id == conversion.FFmpegProcessId && !_.HasExited);

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await conversionTask);

            Assert.True(conversion.FFmpegProcessId > 0);
            ffmpegProcesses = System.Diagnostics.Process.GetProcessesByName("ffmpeg");
            Assert.DoesNotContain(ffmpegProcesses, _ => _.Id == conversion.FFmpegProcessId && !_.HasExited);
            
        }

        [Fact]
        public async Task Conversion_CancellationOccurs_ExeptionWasThrown()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.WebM);
            var cancellationTokenSource = new CancellationTokenSource();
            var conversion = await Conversion.ToWebM(Resources.Mp4WithAudio, output);
            var task = conversion
                    .SetPreset(ConversionPreset.UltraFast)
                    .Start(cancellationTokenSource.Token);

            cancellationTokenSource.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await task);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(8)]
        [InlineData(0)]
        public async Task UseMultithreadTest(int expectedThreadsCount)
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(info.VideoStreams.First())
                                                                 .UseMultiThread(expectedThreadsCount)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal("h264", resultFile.VideoStreams.First().Format);
            Assert.Contains($"-threads {expectedThreadsCount}", conversionResult.Arguments);
        }

        [Fact]
        public async Task UseMultithreadTest_WithoutThreadCount_AllThreads()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(info.VideoStreams.First())
                                                                 .UseMultiThread(true)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal("h264", resultFile.VideoStreams.First().Format);
            Assert.Contains($"-threads {Environment.ProcessorCount}", conversionResult.Arguments);
        }

        [Fact]
        public async Task UseMultithreadTest_WithoutMultithread_OneThreadOnly()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(info.VideoStreams.First())
                                                                 .UseMultiThread(false)
                                                                 .SetOutput(output)
                                                                 .Start()
                                                                 ;

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal("h264", resultFile.VideoStreams.First().Format);
            Assert.Contains($"-threads 1", conversionResult.Arguments);
        }

        [Fact]
        public async Task AddPreParameterTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(info.VideoStreams.First())
                                                                 .AddParameter("-re", ParameterPosition.PreInput)
                                                                 .SetOutput(output)
                                                                 .Start()
                                                                 ;

            Assert.StartsWith("-re", conversionResult.Arguments);
        }
    }
}


