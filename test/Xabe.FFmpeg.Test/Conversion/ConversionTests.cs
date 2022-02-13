using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg.Exceptions;
using Xabe.FFmpeg.Test.Common.Fixtures;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class ConversionTests : IClassFixture<StorageFixture>, IClassFixture<RtspServerFixture>
    {
        private readonly StorageFixture _storageFixture;

        public ConversionTests(StorageFixture storageFixture)
        {
            _storageFixture = storageFixture;
        }

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
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            var outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            IVideoStream stream = inputFile.VideoStreams.First()
                                  .SetWatermark(Resources.PngSample, position);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .SetPreset(ConversionPreset.UltraFast)
                                                                 .AddStream(stream)
                                                                 .SetOutput(outputPath)
                                                                 .Start();

            Assert.Contains("overlay", conversionResult.Arguments);
            Assert.Contains(Resources.PngSample, conversionResult.Arguments);
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(9, mediaInfo.Duration.Seconds);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task PipedOutputTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Ts);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);

            IConversion conversion = FFmpeg.Conversions.New()
                                                    .AddStream(videoStream)
                                                    .SetOutputFormat(Format.mpegts)
                                                    .PipeOutput();
            using var fs = new FileStream(output, FileMode.OpenOrCreate);
            conversion.OnVideoDataReceived += (sender, args) =>
            {
                fs.Write(args.Data, 0, args.Data.Length);
            };

            await conversion.Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(".ts", Path.GetExtension(resultFile.Path));
        }

        [Fact]
        public async Task SetOutputFormatTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Ts);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);
            _ = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .SetOutputFormat(Format.mpegts)
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(".ts", Path.GetExtension(resultFile.Path));
        }

        [Theory]
        [InlineData(Format._3dostr, "3dostr")]
        [InlineData(Format._3g2, "3g2")]
        [InlineData(Format._3gp, "3gp")]
        [InlineData(Format._4xm, "4xm")]
        [InlineData(Format.matroska, "matroska")]
        public async Task SetOutputFormat_ValuesFromEnum_CorrectParams(Format format, string expectedFormat)
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Ts);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);

            var args = FFmpeg.Conversions.New()
                                            .AddStream(videoStream)
                                            .SetOutputFormat(format)
                                            .SetOutput(output)
                                            .Build();

            Assert.Contains($"-f {expectedFormat}", args);
        }

        [Fact]
        public async Task SetOutputFormat_ValueAsString_CorrectParams()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Ts);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);

            var args = FFmpeg.Conversions.New()
                                            .AddStream(videoStream)
                                            .SetOutputFormat("matroska")
                                            .SetOutput(output)
                                            .Build();

            Assert.Contains($"-f matroska", args);
        }

        [Fact]
        public async Task SetOutputFormat_NotExistingFormat_ThrowConversionException()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Ts);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);

            var exception = await Record.ExceptionAsync(async () => await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .SetOutputFormat("notExisting")
                                                                 .SetOutput(output)
                                                                 .Start());
            Assert.NotNull(exception);
            Assert.IsType<ConversionException>(exception);
        }

        [Theory]
        [InlineData(Format._3dostr, "3dostr")]
        [InlineData(Format._3g2, "3g2")]
        [InlineData(Format._3gp, "3gp")]
        [InlineData(Format._4xm, "4xm")]
        [InlineData(Format.matroska, "matroska")]
        public async Task SetFormat_ValuesFromEnum_CorrectParams(Format format, string expectedFormat)
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Ts);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);

            var args = FFmpeg.Conversions.New()
                                            .AddStream(videoStream)
                                            .SetInputFormat(format)
                                            .SetOutput(output)
                                            .Build();

            Assert.Contains($"-f {expectedFormat}", args);
        }

        [Fact]
        public async Task SetFormat_ValueAsString_CorrectParams()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Ts);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);

            var args = FFmpeg.Conversions.New()
                                            .AddStream(videoStream)
                                            .SetInputFormat("matroska")
                                            .SetOutput(output)
                                            .Build();

            Assert.Contains($"-f matroska", args);
        }

        [Fact]
        public async Task SetFormat_NotExistingFormat_ThrowConversionException()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Ts);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);

            var exception = await Record.ExceptionAsync(async () => await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .SetInputFormat("notExisting")
                                                                 .SetOutput(output)
                                                                 .Start());
            Assert.NotNull(exception);
            Assert.IsType<ConversionException>(exception);
        }

        [Fact]
        public async Task SetInputAndOutputFormatTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Avi);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);
            _ = await FFmpeg.Conversions.New()
                                                                 .SetInputFormat(Format.matroska)
                                                                 .AddStream(videoStream)
                                                                 .SetOutputFormat(Format.avi)
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal("mpeg4", resultFile.VideoStreams.First().Codec);
            Assert.Equal(".avi", Path.GetExtension(resultFile.Path));
        }

        [Fact]
        public async Task SetOutputPixelFormatTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);
            _ = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .SetPixelFormat(PixelFormat.yuv420p)
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal("yuv420p", resultFile.VideoStreams.First().PixelFormat);
        }

        [Theory]
        [InlineData(Hash.MD5, 37L)]
        [InlineData(Hash.murmur3, 41L)]
        [InlineData(Hash.RIPEMD128, 43L)]
        [InlineData(Hash.RIPEMD160, 51L)]
        [InlineData(Hash.RIPEMD256, 75L)]
        [InlineData(Hash.RIPEMD320, 91L)]
        [InlineData(Hash.SHA160, 48L)]
        [InlineData(Hash.SHA224, 64L)]
        [InlineData(Hash.SHA256, 72L)]
        [InlineData(Hash.SHA512_224, 68L)]
        [InlineData(Hash.SHA512_256, 76L)]
        [InlineData(Hash.SHA384, 104L)]
        [InlineData(Hash.SHA512, 136L)]
        [InlineData(Hash.CRC32, 15L)]
        [InlineData(Hash.adler32, 17L)]
        public async Task SetHashFormatTest(Hash hashFormat, long expectedLength)
        {
            var fileExtension = FileExtensions.Txt;
            if (hashFormat == Hash.SHA256)
            {
                fileExtension = FileExtensions.Sha256;
            }

            //string output = _storageFixture.GetTempFileName(fileExtension);
            var output = _storageFixture.GetTempFileName(fileExtension);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.copy);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.copy);
            _ = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .AddStream(audioStream)
                                                                 .SetHashFormat(hashFormat)
                                                                 .SetOutput(output)
                                                                 .Start();

            var fi = new FileInfo(output);

            Assert.Equal(fileExtension, fi.Extension);
            Assert.Equal(expectedLength, fi.Length);
        }

        [Fact]
        public async Task SetHashFormat_HashInString_CorrectLenght()
        {
            var fileExtension = FileExtensions.Txt;

            var output = _storageFixture.GetTempFileName(fileExtension);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.copy);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.copy);
            _ = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .AddStream(audioStream)
                                                                 .SetOutputFormat(Format.hash)
                                                                 .SetHashFormat("SHA512/224")
                                                                 .SetOutput(output)
                                                                 .Start();

            var fi = new FileInfo(output);

            Assert.Equal(fileExtension, fi.Extension);
            Assert.Equal(68L, fi.Length);

        }

        [RunnableInDebugOnly]
        public async Task GetScreenCaptureTest_UseVideoSize_EverythingIsCorrect()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = await FFmpeg.Conversions.New()
                                                                 .AddDesktopStream(VideoSize.Qcif, 29.833, 10, 10)
                                                                 .SetInputTime(TimeSpan.FromSeconds(3))
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal("h264", resultFile.VideoStreams.First().Codec);
            Assert.Equal(29, (int)resultFile.VideoStreams.First().Framerate);
            Assert.Equal(3, resultFile.VideoStreams.First().Duration.Seconds);
            Assert.Equal(176, resultFile.VideoStreams.First().Width);
            Assert.Equal(144, resultFile.VideoStreams.First().Height);
        }

        [RunnableInDebugOnly]
        public async Task GetScreenCaptureTest_UseVideoSizeAsString_EverythingIsCorrect()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = await FFmpeg.Conversions.New()
                                                                 .AddDesktopStream("176x144", 30, 10, 10)
                                                                 .SetInputTime(TimeSpan.FromSeconds(3))
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal("h264", resultFile.VideoStreams.First().Codec);
            Assert.Equal(30, resultFile.VideoStreams.First().Framerate);
            Assert.Equal(3, resultFile.VideoStreams.First().Duration.Seconds);
            Assert.Equal(176, resultFile.VideoStreams.First().Width);
            Assert.Equal(144, resultFile.VideoStreams.First().Height);
        }

        [Fact]
        public async Task SetVideoCodecTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);
            _ = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal("mpeg4", resultFile.VideoStreams.First().Codec);
        }

        [Fact]
        public async Task SetAudioCodecTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.ac3);
            _ = await FFmpeg.Conversions.New()
                                                                 .AddStream(audioStream)
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal("ac3", resultFile.AudioStreams.First().Codec);
        }

        [Fact]
        public async Task SetInputTimeTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First();
            IVideoStream videoStream = info.VideoStreams.First();
            _ = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .AddStream(audioStream)
                                                                 .SetInputTime(TimeSpan.FromSeconds(5))
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(5, resultFile.AudioStreams.First().Duration.Seconds);
            Assert.Equal(5, resultFile.VideoStreams.First().Duration.Seconds);
        }

        [Fact]
        public async Task SetOutputTimeTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First();
            IVideoStream videoStream = info.VideoStreams.First();
            _ = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .AddStream(audioStream)
                                                                 .SetOutputTime(TimeSpan.FromSeconds(5))
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(5), resultFile.AudioStreams.First().Duration);
            Assert.Equal(TimeSpan.FromSeconds(5), resultFile.VideoStreams.First().Duration);
        }

        [Fact]
        public async Task SetAudioBitrateTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.ac3);
            _ = await FFmpeg.Conversions.New()
                                                                 .AddStream(audioStream)
                                                                 .SetAudioBitrate(128000)
                                                                 .SetOutput(output)
                                                                 .Start();

            var lowerBound = 128000 * 0.95;
            var upperBound = 128000 * 1.05;

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.InRange(resultFile.AudioStreams.First().Bitrate, lowerBound, upperBound);
        }

        [Fact]
        public async Task SetLibH264VideoBitrateTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.libx264);
            _ = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .SetVideoBitrate(1500000)
                                                                 .SetOutput(output)
                                                                 .Start();

            var lowerBound = 1500000 * 0.95;
            var upperBound = 1500000 * 1.05;
            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.InRange(resultFile.VideoStreams.First().Bitrate, lowerBound, upperBound);
        }

        [Fact]
        public async Task SetH264VideoBitrateTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.h264);
            _ = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .SetVideoBitrate(1500000)
                                                                 .SetOutput(output)
                                                                 .Start();

            var lowerBound = 1500000 * 0.95;
            var upperBound = 1500000 * 1.05;

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.InRange(resultFile.VideoStreams.First().Bitrate, lowerBound, upperBound);
        }

        [Fact]
        public async Task SetNonH264VideoBitrateTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);
            _ = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .SetVideoBitrate(1500000)
                                                                 .SetOutput(output)
                                                                 .Start();

            var lowerBound = 1500000 * 0.95;
            var upperBound = 1500000 * 1.05;

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.InRange(resultFile.VideoStreams.First().Bitrate, lowerBound, upperBound);
        }

        [Theory]
        [InlineData(FileExtensions.Png)]
        [InlineData(FileExtensions.WebP)]
        [InlineData(FileExtensions.Jpg)]
        public async Task ExtractEveryNthFrameTest(string extension)
        {
            var tempPath = _storageFixture.GetTempDirectory();
            string OutputBuilder(string number)
            {
                return Path.Combine(tempPath, number + extension);
            }

            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First().SetCodec(VideoCodec.png);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .ExtractEveryNthFrame(10, OutputBuilder)
                                                                 .Start();

            var outputFilesCount = Directory.EnumerateFiles(tempPath).Count();

            Assert.Equal(26, outputFilesCount);
        }

        [Theory]
        [InlineData(FileExtensions.Png)]
        [InlineData(FileExtensions.WebP)]
        [InlineData(FileExtensions.Jpg)]
        public async Task ExtractNthFrameTest(string extension)
        {
            var tempPath = _storageFixture.GetTempDirectory();
            string OutputBuilder(string number)
            {
                return Path.Combine(tempPath, number + extension);
            }

            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First().SetCodec(VideoCodec.png);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .ExtractNthFrame(10, OutputBuilder)
                                                                 .Start();

            var outputFilesCount = Directory.EnumerateFiles(tempPath).Count();

            Assert.Equal(1, outputFilesCount);
        }

        [Fact]
        public async Task BuildVideoFromImagesTest()
        {
            var files = Directory.EnumerateFiles(Resources.Images).ToList();
            var builder = new InputBuilder();
            Func<string, string> inputBuilder = builder.PrepareInputFiles(files, out var preparedFilesDir);
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = await FFmpeg.Conversions.New()
                                                                 .SetInputFrameRate(1)
                                                                 .BuildVideoFromImages(1, inputBuilder)
                                                                 .SetFrameRate(1)
                                                                 .SetPixelFormat(PixelFormat.yuv420p)
                                                                 .SetOutput(output)
                                                                 .Start();

            var preparedFilesCount = Directory.EnumerateFiles(preparedFilesDir).ToList().Count;

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(builder.FileList.Count, preparedFilesCount);
            Assert.Equal(TimeSpan.FromSeconds(12), resultFile.VideoStreams.First().Duration);
            Assert.Equal(1, resultFile.VideoStreams.First().Framerate);
            Assert.Equal("yuv420p", resultFile.VideoStreams.First().PixelFormat);
        }

        [Fact]
        public async Task BuildVideoFromImagesAndAudioTest()
        {
            var files = Directory.EnumerateFiles(Resources.Images).ToList();
            var builder = new InputBuilder();
            IMediaInfo audioInfo = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IAudioStream audioStream = audioInfo.AudioStreams.First();
            Func<string, string> inputBuilder = builder.PrepareInputFiles(files, out var preparedFilesDir);

            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = await FFmpeg.Conversions.New()
                                                                 .SetInputFrameRate(1)
                                                                 .BuildVideoFromImages(1, inputBuilder)
                                                                 .SetFrameRate(1)
                                                                 .SetPixelFormat(PixelFormat.yuv420p)
                                                                 .AddStream(audioStream)
                                                                 .SetOutput(output)
                                                                 .Start();

            var preparedFilesCount = Directory.EnumerateFiles(preparedFilesDir).ToList().Count;

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(builder.FileList.Count, preparedFilesCount);
            Assert.Equal(TimeSpan.FromSeconds(12), resultFile.VideoStreams.First().Duration);
            Assert.Equal(1, resultFile.VideoStreams.First().Framerate);
            Assert.Equal("yuv420p", resultFile.VideoStreams.First().PixelFormat);
            Assert.Single(resultFile.AudioStreams);
        }

        [Theory]
        [InlineData(PixelFormat._0bgr, "0bgr")]
        [InlineData(PixelFormat._0rgb, "0rgb")]
        [InlineData(PixelFormat.yuv410p, "yuv410p")]
        public void SetPixelFormat_DataFromEnum_CorrectArgs(PixelFormat pixelFormat, string expectedPixelFormat)
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);

            var args = FFmpeg.Conversions.New()
                                    .SetPixelFormat(pixelFormat)
                                    .SetOutput(output)
                                    .Build();

            Assert.Contains($"-pix_fmt {expectedPixelFormat}", args);
        }

        [Fact]
        public void SetPixelFormat_DataFromString_CorrectArgs()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);

            var args = FFmpeg.Conversions.New()
                                    .SetPixelFormat("testFormat")
                                    .SetOutput(output)
                                    .Build();

            Assert.Contains($"-pix_fmt testFormat", args);
        }

        [Fact]
        public async Task SetPixelFormat_NotExistingFormat_ThrowConversionException()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);

            var exception = await Record.ExceptionAsync(async () => await FFmpeg.Conversions.New()
                                    .SetPixelFormat("notExistingFormat")
                                    .SetOutput(output)
                                    .Start());

            Assert.NotNull(exception);
            Assert.IsType<ConversionException>(exception);
        }

        [Fact]
        public async Task BuildVideoFromImagesListTest()
        {
            var files = Directory.EnumerateFiles(Resources.Images).ToList();
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = await FFmpeg.Conversions.New()
                                                                 .SetInputFrameRate(1)
                                                                 .BuildVideoFromImages(files)
                                                                 .SetFrameRate(1)
                                                                 .SetPixelFormat(PixelFormat.yuv420p)
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(12), resultFile.VideoStreams.First().Duration);
            Assert.Equal(1, resultFile.VideoStreams.First().Framerate);
            Assert.Equal("yuv420p", resultFile.VideoStreams.First().PixelFormat);
        }

        [Fact]
        public async Task BuildVideoFromImagesListAndAudioTest()
        {
            var files = Directory.EnumerateFiles(Resources.Images).ToList();
            IMediaInfo audioInfo = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IAudioStream audioStream = audioInfo.AudioStreams.First();
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = await FFmpeg.Conversions.New()
                                                                 .SetInputFrameRate(1)
                                                                 .BuildVideoFromImages(files)
                                                                 .SetFrameRate(1)
                                                                 .SetPixelFormat(PixelFormat.yuv420p)
                                                                 .AddStream(audioStream)
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(12), resultFile.VideoStreams.First().Duration);
            Assert.Equal(1, resultFile.VideoStreams.First().Framerate);
            Assert.Equal("yuv420p", resultFile.VideoStreams.First().PixelFormat);
            Assert.Single(resultFile.AudioStreams);
        }

        [Fact]
        public async Task OverwriteFilesTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.ac3);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(audioStream)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.Contains("-n ", conversionResult.Arguments);

            IConversionResult secondConversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(audioStream)
                                                                 .SetOverwriteOutput(true)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.Contains(" -y ", secondConversionResult.Arguments);
            Assert.DoesNotContain(" -n ", secondConversionResult.Arguments);
        }

        [Fact]
        public async Task OverwriteFilesExceptionTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.ac3);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(audioStream)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.Contains("-n ", conversionResult.Arguments);

            await Assert.ThrowsAsync<ConversionException>(() => FFmpeg.Conversions.New()
                                                                          .AddStream(audioStream)
                                                                          .SetOutput(output)
                                                                          .Start());
        }

        [RunnableInDebugOnly]
        public async Task UseHardwareAcceleration()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = await (await FFmpeg.Conversions.FromSnippet.Convert(Resources.MkvWithAudio, output)).UseHardwareAcceleration(HardwareAccelerator.auto, VideoCodec.h264_cuvid, VideoCodec.h264_nvenc, 0).Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal("h264", resultFile.VideoStreams.First().Codec);
        }

        [Theory]
        [InlineData("a16f0cb5c0354b6197e9f3bc3108c017")]
        public async Task MissingHardwareAccelerator(string hardwareAccelerator)
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            var exception = await Record.ExceptionAsync(async () =>
            {
                await (await FFmpeg.Conversions.FromSnippet.Convert(Resources.MkvWithAudio, output)).UseHardwareAcceleration(hardwareAccelerator, "h264_cuvid", "h264_nvenc").Start();
            });

            Assert.NotNull(exception);
            Assert.IsType<ConversionException>(exception);
            Assert.IsType<HardwareAcceleratorNotFoundException>(exception.InnerException);
        }

        [Fact]
        public async Task UnknownDecoderException()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);

            var exception = await Record.ExceptionAsync(async () =>
            {
                await (await FFmpeg.Conversions.FromSnippet.Convert(Resources.MkvWithAudio, output)).UseHardwareAcceleration(HardwareAccelerator.auto, VideoCodec.h264_nvenc, VideoCodec.h264_cuvid).Start();
            });

            Assert.NotNull(exception);
            Assert.IsType<ConversionException>(exception);
            Assert.IsType<UnknownDecoderException>(exception.InnerException);
        }

        [Fact]
        public async Task Conversion_CancellationOccurs_ExeptionWasThrown()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.WebM);

            var cancellationTokenSource = new CancellationTokenSource();
            var conversion = Conversion.ToWebM(Resources.Mp4WithAudio, output);
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
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(info.VideoStreams.First())
                                                                 .UseMultiThread(expectedThreadsCount)
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal("h264", resultFile.VideoStreams.First().Codec);
            Assert.Contains($"-threads {expectedThreadsCount}", conversionResult.Arguments);
        }

        [Fact]
        public async Task UseMultithreadTest_WithoutThreadCount_AllThreads()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(info.VideoStreams.First())
                                                                 .UseMultiThread(true)
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal("h264", resultFile.VideoStreams.First().Codec);
            Assert.Contains($"-threads {Math.Min(Environment.ProcessorCount, 16)}", conversionResult.Arguments);
        }

        [Fact]
        public async Task UseMultithreadTest_WithoutMultithread_OneThreadOnly()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(info.VideoStreams.First())
                                                                 .UseMultiThread(false)
                                                                 .SetOutput(output)
                                                                 .Start()
                                                                 ;

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal("h264", resultFile.VideoStreams.First().Codec);
            Assert.Contains($"-threads 1", conversionResult.Arguments);
        }

        [Fact]
        public async Task AddPreParameterTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(info.VideoStreams.First())
                                                                 .AddParameter("-re", ParameterPosition.PreInput)
                                                                 .SetOutput(output)
                                                                 .Start()
                                                                 ;

            Assert.StartsWith("-re", conversionResult.Arguments);
        }

        [Fact]
        public async Task TryConvertMedia_NoFilesInFFmpegDirectory_ThrowFFmpegNotFoundException()
        {
            var path = FFmpeg.ExecutablesPath;

            try
            {
                FFmpeg.SetExecutablesPath(_storageFixture.TempDirPath);

                var output = _storageFixture.GetTempFileName(FileExtensions.Ts);
                var exception = await Record.ExceptionAsync(async () => await FFmpeg.GetMediaInfo(Resources.MkvWithAudio));

                Assert.NotNull(exception);
                Assert.IsType<FFmpegNotFoundException>(exception);
            }
            finally
            {
                FFmpeg.SetExecutablesPath(path);
            }
        }

        [Fact]
        public async Task ConvertSloMoTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.SloMoMp4);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.h264);
            _ = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .SetFrameRate(videoStream.Framerate)
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            IVideoStream resultVideoStream = resultFile.VideoStreams?.First();

            Assert.Equal(".mp4", Path.GetExtension(resultFile.Path));

            // It does not has to be the same
            Assert.Equal(116, (int)resultVideoStream.Framerate);
            Assert.Equal(3, resultVideoStream.Duration.Seconds);
        }

        [Theory]
        [InlineData(VideoSyncMethod.cfr)]
        [InlineData(VideoSyncMethod.drop)]
        [InlineData(VideoSyncMethod.passthrough)]
        [InlineData(VideoSyncMethod.vfr)]
        public async Task AddVsync_CorrectValues_VsyncMethodIsSet(VideoSyncMethod vsyncMethod)
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(info.VideoStreams.First().SetCodec(VideoCodec.copy))
                                                                 .SetVideoSyncMethod(vsyncMethod)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.Contains($"-vsync {vsyncMethod}", conversionResult.Arguments);
        }

        [Fact]
        public async Task AddVsync_AutoMethod_VsyncMethodIsSetCorrectly()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(info.VideoStreams.First())
                                                                 .SetVideoSyncMethod(VideoSyncMethod.auto)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.Contains($"-vsync -1", conversionResult.Arguments);
        }

        [Fact]
        public async Task SendToRtspServer_MinimumConfiguration_FileIsBeingStreamed()
        {
            // Arrange
            var output = "rtsp://127.0.0.1:8554/newFile";

            // Act
            _ = (await FFmpeg.Conversions.FromSnippet.SendToRtspServer(Resources.Mp4, new Uri(output))).Start();
            await Task.Delay(2000);

            // Assert
            var info = await MediaInfo.Get(output);

            Assert.Single(info.Streams);
        }

        [RunnableInDebugOnly]
        public async Task GetAvailableDevices_SomeDevicesAreConnected_ReturnAllDevices()
        {
            // Arrange
            var devices = await FFmpeg.GetAvailableDevices();

            // Assert
            Assert.Equal(2, devices.Count());
            Assert.Single(devices.Where(x => x.Name == "Logitech HD Webcam C270"));
        }

        [RunnableInDebugOnly]
        public async Task SendDesktopToRtspServer_MinimumConfiguration_DesktopIsBeingStreamed()
        {
            // Arrange
            var output = "rtsp://127.0.0.1:8554/desktop";

            // Act
            _ = (await FFmpeg.Conversions.FromSnippet.SendDesktopToRtspServer(new Uri(output))).Start();
            //Give it some time to warm up
            await Task.Delay(2000);

            // Assert
            IMediaInfo info = await FFmpeg.GetMediaInfo(output);
            Assert.Single(info.Streams);
        }

        [RunnableInDebugOnly]
        public async Task GetScreenCaptureTest_UseNewAddDesktopStream_EverythingIsCorrect()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = await FFmpeg.Conversions.New()
                                                                 .AddDesktopStream(VideoSize.Cga, 29.833, 0, 0)
                                                                 .SetInputTime(TimeSpan.FromSeconds(3))
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal("h264", resultFile.VideoStreams.First().Codec);
            Assert.Equal(3, resultFile.VideoStreams.First().Duration.Seconds);
            Assert.Equal(320, resultFile.VideoStreams.First().Width);
            Assert.Equal(200, resultFile.VideoStreams.First().Height);
            Assert.Equal(29.833, resultFile.VideoStreams.First().Framerate);
        }

        [Fact]
        public async Task Conversion_MilisecondsInTimeSpan_WorksCorrectly()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First();
            IVideoStream videoStream = info.VideoStreams.First();
            _ = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .AddStream(audioStream)
                                                                 .SetInputTime(TimeSpan.FromMilliseconds(1500))
                                                                 .SetOutputTime(TimeSpan.FromMilliseconds(1500))
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(1500, resultFile.AudioStreams.First().Duration.TotalMilliseconds);
            Assert.Equal(1500, resultFile.AudioStreams.First().Duration.TotalMilliseconds);
        }

        [Fact]
        public async Task Conversion_SpacesInOutputPath_WorksCorrectly()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            var nameWithSpaces = new FileInfo(output).Name.Replace("-", " ");
            output = output.Replace(nameWithSpaces.Replace(" ", "-"), nameWithSpaces);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            _ = await FFmpeg.Conversions.New()
                                                                 .AddStream(info.VideoStreams.First())
                                                                 .AddParameter("-re", ParameterPosition.PreInput)
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo outputMediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.NotNull(outputMediaInfo.Streams);
        }

        [Theory]
        [InlineData("'")]
        [InlineData("\"")]
        public async Task Conversion_OutputPathEscaped_WorksCorrectly(string escapeCharacter)
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            var nameWithSpaces = new FileInfo(output).Name.Replace("-", " ");
            output = output.Replace(nameWithSpaces.Replace(" ", "-"), nameWithSpaces);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            _ = await FFmpeg.Conversions.New()
                                                                 .AddStream(info.VideoStreams.First())
                                                                 .AddParameter("-re", ParameterPosition.PreInput)
                                                                 .SetOutput($"{escapeCharacter}{output}{escapeCharacter}")
                                                                 .Start();

            IMediaInfo outputMediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.NotNull(outputMediaInfo.Streams);
        }

        [Theory]
        [InlineData("Crime d'Amour.mp4")]
        public async Task Conversion_SpecialCharactersInName_WorksCorrectly(string outputFileName)
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            var name = new FileInfo(output).Name;
            output = output.Replace(name, outputFileName);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(info.VideoStreams.First())
                                                                 .AddParameter("-re", ParameterPosition.PreInput)
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo outputMediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.NotNull(outputMediaInfo.Streams);
            Assert.Contains("Crime d'Amour", conversionResult.Arguments);
        }

        [Fact]
        public async Task ExtractEveryNthFrame_OutputDirectoryNotExists_OutputDirectoryIsCreated()
        {
            var tempPath = Path.Combine(_storageFixture.GetTempDirectory(), Guid.NewGuid().ToString());
            string OutputBuilder(string number)
            {
                return Path.Combine(tempPath, number + FileExtensions.Png);
            }

            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First().SetCodec(VideoCodec.png);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .ExtractEveryNthFrame(10, OutputBuilder)
                                                                 .Start();

            var outputFilesCount = Directory.EnumerateFiles(tempPath).Count();

            Assert.Equal(26, outputFilesCount);
        }

        [Fact]
        public async Task Conversion_OutputDirectoryNotExists_OutputDirectoryIsCreated()
        {
            var tempPath = _storageFixture.GetTempDirectory();
            var output = Path.Combine(tempPath, Guid.NewGuid().ToString(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.Mp4);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.h264);
            _ = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .SetFrameRate(videoStream.Framerate)
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.NotEmpty(resultFile.Streams);
        }

        [Fact]
        public async Task Conversion_DoubleNestedNotExistingDirectory_OutputDirectoryIsCreated()
        {
            var tempPath = _storageFixture.GetTempDirectory();
            var output = Path.Combine(tempPath, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.Mp4);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.h264);
            _ = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .SetFrameRate(videoStream.Framerate)
                                                                 .SetOutput(output)
                                                                 .Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.NotEmpty(resultFile.Streams);
        }

        [Fact]
        public async Task Conversion_RunItSecondTime_ItWorks()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.ac3);

            var conversion = FFmpeg.Conversions.New()
                                                .AddStream(audioStream)
                                                .SetOutput(output);
            await conversion.Start();

            var secondOutput = _storageFixture.GetTempFileName(FileExtensions.Mkv);
            var exception = await Record.ExceptionAsync(async () => await conversion.SetOutput(secondOutput).Start());

            Assert.Null(exception);
        }

        [Fact]
        public async Task Conversion_EverythingIsPassedAsAdditionalParameter_EverythingWorks()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddParameter($"-ss 00:00:01 -t 00:00:03 -i {Resources.Mp4} -ss 00:00:05 -t 00:00:03 -i {Resources.Mp4}", ParameterPosition.PreInput)
                                                                 .SetOutput(output)
                                                                 .Start();

            var mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(2, conversionResult.Arguments.Split(" ").Where(x => x == "-ss").Count());
            Assert.Equal(2, conversionResult.Arguments.Split(" ").Where(x => x == "-t").Count());
        }

        [Fact]
        public async Task Conversion_EverythingIsPassedAsAdditionalParameters_EverythingWorks()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddParameter($"-ss 00:00:01", ParameterPosition.PreInput)
                                                                 .AddParameter($"-t 00:00:03", ParameterPosition.PreInput)
                                                                 .AddParameter($"-i {Resources.Mp4}", ParameterPosition.PreInput)
                                                                 .AddParameter($"-ss 00:00:05", ParameterPosition.PreInput)
                                                                 .AddParameter($"-t 00:00:03", ParameterPosition.PreInput)
                                                                 .AddParameter($"-i {Resources.Mp4}", ParameterPosition.PreInput)
                                                                 .SetOutput(output)
                                                                 .Start();

            var mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(2, conversionResult.Arguments.Split(" ").Where(x => x == "-ss").Count());
            Assert.Equal(2, conversionResult.Arguments.Split(" ").Where(x => x == "-t").Count());
        }
    }
}
