using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg.Exceptions;
using Xabe.FFmpeg.Test.Fixtures;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class ConversionTests : IClassFixture<StorageFixture>, IClassFixture<RtspServerFixture>
    {
        private readonly StorageFixture _storageFixture;
        private readonly RtspServerFixture _rtspServer;

        public ConversionTests(StorageFixture storageFixture, RtspServerFixture rtspServer)
        {
            _storageFixture = storageFixture;
            _rtspServer = rtspServer;
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
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
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
        public async Task SetOutputFormatTest()
        {
            string output = _storageFixture.GetTempFileName(FileExtensions.Ts);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
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
            string output = _storageFixture.GetTempFileName(FileExtensions.Ts);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);

            string args = FFmpeg.Conversions.New()
                                            .AddStream(videoStream)
                                            .SetOutputFormat(format)
                                            .SetOutput(output)
                                            .Build();

            Assert.Contains($"-f {expectedFormat}", args);
        }

        [Fact]
        public async Task SetOutputFormat_ValueAsString_CorrectParams()
        {
            string output = _storageFixture.GetTempFileName(FileExtensions.Ts);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);

            string args = FFmpeg.Conversions.New()
                                            .AddStream(videoStream)
                                            .SetOutputFormat("matroska")
                                            .SetOutput(output)
                                            .Build();

            Assert.Contains($"-f matroska", args);
        }

        [Fact]
        public async Task SetOutputFormat_NotExistingFormat_ThrowConversionException()
        {
            string output = _storageFixture.GetTempFileName(FileExtensions.Ts);
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
            string output = _storageFixture.GetTempFileName(FileExtensions.Ts);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);

            string args = FFmpeg.Conversions.New()
                                            .AddStream(videoStream)
                                            .SetInputFormat(format)
                                            .SetOutput(output)
                                            .Build();

            Assert.Contains($"-f {expectedFormat}", args);
        }

        [Fact]
        public async Task SetFormat_ValueAsString_CorrectParams()
        {
            string output = _storageFixture.GetTempFileName(FileExtensions.Ts);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);

            string args = FFmpeg.Conversions.New()
                                            .AddStream(videoStream)
                                            .SetInputFormat("matroska")
                                            .SetOutput(output)
                                            .Build();

            Assert.Contains($"-f matroska", args);
        }

        [Fact]
        public async Task SetFormat_NotExistingFormat_ThrowConversionException()
        {
            string output = _storageFixture.GetTempFileName(FileExtensions.Ts);
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
            string output = _storageFixture.GetTempFileName(FileExtensions.Avi);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
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
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
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
            string fileExtension = FileExtensions.Txt;
            if (hashFormat == Hash.SHA256)
                fileExtension = FileExtensions.Sha256;

            //string output = _storageFixture.GetTempFileName(fileExtension);
            string output = _storageFixture.GetTempFileName(fileExtension);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.copy);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.copy);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .AddStream(audioStream)
                                                                 .SetHashFormat(hashFormat)
                                                                 .SetOutput(output)
                                                                 .Start();


            FileInfo fi = new FileInfo(output);

            Assert.Equal(fileExtension, fi.Extension);
            Assert.Equal(expectedLength, fi.Length);
        }

        [Fact]
        public async Task SetHashFormat_HashInString_CorrectLenght()
        {
            string fileExtension = FileExtensions.Txt;

            string output = _storageFixture.GetTempFileName(fileExtension);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.copy);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.copy);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .AddStream(audioStream)
                                                                 .SetOutputFormat(Format.hash)
                                                                 .SetHashFormat("SHA512/224")
                                                                 .SetOutput(output)
                                                                 .Start();


            FileInfo fi = new FileInfo(output);

            Assert.Equal(fileExtension, fi.Extension);
            Assert.Equal(68L, fi.Length);

        }

        [RunnableInDebugOnly]
        public async Task GetScreenCaptureTest()
        {
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .GetScreenCapture(30, 10, 10, VideoSize.Qcif)
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

        [RunnableInDebugOnly]
        public async Task GetScreenCaptureTest2()
        {
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .GetScreenCapture(30, 10, 10, "176x144")
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
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .SetOutput(output)
                                                                 .Start();


            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal("mpeg4", resultFile.VideoStreams.First().Codec);
        }

        [Fact]
        public async Task SetAudioCodecTest()
        {
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.ac3);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(audioStream)
                                                                 .SetOutput(output)
                                                                 .Start();


            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal("ac3", resultFile.AudioStreams.First().Codec);
        }

        [Fact]
        public async Task SetInputTimeTest()
        {
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First();
            IVideoStream videoStream = info.VideoStreams.First();

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
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
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First();
            IVideoStream videoStream = info.VideoStreams.First();

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
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
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.ac3);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(audioStream)
                                                                 .SetAudioBitrate(128000)
                                                                 .SetOutput(output)
                                                                 .Start();

            double lowerBound = 128000 * 0.95;
            double upperBound = 128000 * 1.05;

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.InRange<double>(resultFile.AudioStreams.First().Bitrate, lowerBound, upperBound);
        }

        [Fact]
        public async Task SetLibH264VideoBitrateTest()
        {
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.libx264);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .SetVideoBitrate(1500000)
                                                                 .SetOutput(output)
                                                                 .Start();

            double lowerBound = 1500000 * 0.95;
            double upperBound = 1500000 * 1.05;
            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.InRange<double>(resultFile.VideoStreams.First().Bitrate, lowerBound, upperBound);
        }

        [Fact]
        public async Task SetH264VideoBitrateTest()
        {
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.h264);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .SetVideoBitrate(1500000)
                                                                 .SetOutput(output)
                                                                 .Start();

            double lowerBound = 1500000 * 0.95;
            double upperBound = 1500000 * 1.05;

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.InRange<double>(resultFile.VideoStreams.First().Bitrate, lowerBound, upperBound);
        }

        [Fact]
        public async Task SetNonH264VideoBitrateTest()
        {
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.mpeg4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .SetVideoBitrate(1500000)
                                                                 .SetOutput(output)
                                                                 .Start();

            double lowerBound = 1500000 * 0.95;
            double upperBound = 1500000 * 1.05;

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.InRange<double>(resultFile.VideoStreams.First().Bitrate, lowerBound, upperBound);
        }

        [Theory]
        [InlineData(FileExtensions.Png)]
        [InlineData(FileExtensions.WebP)]
        [InlineData(FileExtensions.Jpg)]
        public async Task ExtractEveryNthFrameTest(string extension)
        {
            var tempPath = _storageFixture.GetTempDirectory();
            Func<string, string> outputBuilder = (number) => { return Path.Combine(tempPath, number + extension); };
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First().SetCodec(VideoCodec.png);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .ExtractEveryNthFrame(10, outputBuilder)
                                                                 .Start();



            int outputFilesCount = Directory.EnumerateFiles(tempPath).Count();

            Assert.Equal(26, outputFilesCount);
        }

        [Theory]
        [InlineData(FileExtensions.Png)]
        [InlineData(FileExtensions.WebP)]
        [InlineData(FileExtensions.Jpg)]
        public async Task ExtractNthFrameTest(string extension)
        {
            var tempPath = _storageFixture.GetTempDirectory();
            Func<string, string> outputBuilder = (number) => { return Path.Combine(tempPath, number + extension); };
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First().SetCodec(VideoCodec.png);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .ExtractNthFrame(10, outputBuilder)
                                                                 .Start();



            int outputFilesCount = Directory.EnumerateFiles(tempPath).Count();

            Assert.Equal(1, outputFilesCount);
        }

        [Fact]
        public async Task BuildVideoFromImagesTest()
        {
            List<string> files = Directory.EnumerateFiles(Resources.Images).ToList();
            InputBuilder builder = new InputBuilder();
            string preparedFilesDir = string.Empty;
            Func<string, string> inputBuilder = builder.PrepareInputFiles(files, out preparedFilesDir);
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .SetInputFrameRate(1)
                                                                 .BuildVideoFromImages(1, inputBuilder)
                                                                 .SetFrameRate(1)
                                                                 .SetPixelFormat(PixelFormat.yuv420p)
                                                                 .SetOutput(output)
                                                                 .Start();

            int preparedFilesCount = Directory.EnumerateFiles(preparedFilesDir).ToList().Count;


            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(builder.FileList.Count, preparedFilesCount);
            Assert.Equal(TimeSpan.FromSeconds(12), resultFile.VideoStreams.First().Duration);
            Assert.Equal(1, resultFile.VideoStreams.First().Framerate);
            Assert.Equal("yuv420p", resultFile.VideoStreams.First().PixelFormat);
        }

        [Fact]
        public async Task BuildVideoFromImagesAndAudioTest()
        {
            List<string> files = Directory.EnumerateFiles(Resources.Images).ToList();
            InputBuilder builder = new InputBuilder();
            string preparedFilesDir = string.Empty;
            IMediaInfo audioInfo = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IAudioStream audioStream = audioInfo.AudioStreams.First();
            Func<string, string> inputBuilder = builder.PrepareInputFiles(files, out preparedFilesDir);


            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .SetInputFrameRate(1)
                                                                 .BuildVideoFromImages(1, inputBuilder)
                                                                 .SetFrameRate(1)
                                                                 .SetPixelFormat(PixelFormat.yuv420p)
                                                                 .AddStream(audioStream)
                                                                 .SetOutput(output)
                                                                 .Start();

            int preparedFilesCount = Directory.EnumerateFiles(preparedFilesDir).ToList().Count;


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
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);

            string args = FFmpeg.Conversions.New()
                                    .SetPixelFormat(pixelFormat)
                                    .SetOutput(output)
                                    .Build();

            Assert.Contains($"-pix_fmt {expectedPixelFormat}", args);
        }

        [Fact]
        public void SetPixelFormat_DataFromString_CorrectArgs()
        {
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);

            string args = FFmpeg.Conversions.New()
                                    .SetPixelFormat("testFormat")
                                    .SetOutput(output)
                                    .Build();

            Assert.Contains($"-pix_fmt testFormat", args);
        }

        [Fact]
        public async Task SetPixelFormat_NotExistingFormat_ThrowConversionException()
        {
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);

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
            List<string> files = Directory.EnumerateFiles(Resources.Images).ToList();
            string preparedFilesDir = string.Empty;
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
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
            List<string> files = Directory.EnumerateFiles(Resources.Images).ToList();
            string preparedFilesDir = string.Empty;
            IMediaInfo audioInfo = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IAudioStream audioStream = audioInfo.AudioStreams.First();
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
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
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
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
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
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
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            IConversionResult conversionResult = await (await FFmpeg.Conversions.FromSnippet.Convert(Resources.MkvWithAudio, output)).UseHardwareAcceleration(HardwareAccelerator.auto, VideoCodec.h264_cuvid, VideoCodec.h264_nvenc, 0).Start();


            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal("h264", resultFile.VideoStreams.First().Codec);
        }


        [Theory]
        [InlineData("a16f0cb5c0354b6197e9f3bc3108c017")]
        public async Task MissingHardwareAccelerator(string hardwareAccelerator)
        {
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
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
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
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
            string output = _storageFixture.GetTempFileName(FileExtensions.WebM);

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
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
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
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(info.VideoStreams.First())
                                                                 .UseMultiThread(true)
                                                                 .SetOutput(output)
                                                                 .Start();


            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            Assert.Equal("h264", resultFile.VideoStreams.First().Codec);
            Assert.Contains($"-threads {Environment.ProcessorCount}", conversionResult.Arguments);
        }

        [Fact]
        public async Task UseMultithreadTest_WithoutMultithread_OneThreadOnly()
        {
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
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
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
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

                string output = _storageFixture.GetTempFileName(FileExtensions.Ts);
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
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.SloMoMp4);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.h264);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(videoStream)
                                                                 .SetFrameRate(videoStream.Framerate)
                                                                 .SetOutput(output)
                                                                 .Start();


            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);
            IVideoStream resultVideoStream = resultFile.VideoStreams?.First();

            Assert.Equal(".mp4", Path.GetExtension(resultFile.Path));
            Assert.Equal(116.244, resultVideoStream.Framerate);
            Assert.Equal(3, resultVideoStream.Duration.Seconds);
        }

        [Theory]
        [InlineData(VideoSyncMethod.cfr)]
        [InlineData(VideoSyncMethod.drop)]
        [InlineData(VideoSyncMethod.passthrough)]
        [InlineData(VideoSyncMethod.vfr)]
        public async Task AddVsync_CorrectValues_VsyncMethodIsSet(VideoSyncMethod vsyncMethod)
        {
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
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
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
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
            string output = "rtsp://127.0.0.1:8554/newFile";

            // Act
            (await FFmpeg.Conversions.FromSnippet.SendToRtspServer(Resources.Mp4, new Uri(output))).Start();
            //Give it some time to warm up
            await Task.Delay(2000);

            // Assert
            IMediaInfo info = await FFmpeg.GetMediaInfo(output);
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
    }
}


