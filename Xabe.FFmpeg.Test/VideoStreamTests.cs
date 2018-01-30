using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;
using Xabe.FFmpeg.Exceptions;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class VideoStreamTests
    {
        [Theory]
        [InlineData(RotateDegrees.Clockwise)]
        [InlineData(RotateDegrees.Invert)]
        public async Task TransposeTest(RotateDegrees rotateDegrees)
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            var inputFile = await MediaInfo.Get(Resources.MkvWithAudio);

            IConversionResult result = await Conversion.New()
                                                    .AddStream(inputFile.VideoStreams.First().Rotate(rotateDegrees))
                                                    .SetOutput(outputPath)
                                                    .Start();

            Assert.True(result.Success);
            var mediaInfo = await MediaInfo.Get(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Theory]
        [InlineData(9, 9, 1.0)]
        [InlineData(5, 5, 2.0)]
        [InlineData(19, 19, 0.5)]
        public async Task ChangeSpeedTest(int expectedDuration, int expectedVideoDuration, double speed)
        {
            var inputFile = await MediaInfo.Get(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await Conversion.New()
                                                    .AddStream(inputFile.VideoStreams.First().SetCodec(VideoCodec.h264).ChangeSpeed(speed))
                                                    .SetSpeed(ConversionSpeed.UltraFast)
                                                    .UseMultiThread(true)
                                                    .SetOutput(outputPath)
                                                    .Start();

            Assert.True(conversionResult.Success);
            var mediaInfo = await MediaInfo.Get(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(expectedDuration), mediaInfo.Duration);
            Assert.Equal(TimeSpan.FromSeconds(expectedVideoDuration), mediaInfo.VideoStreams.First().Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Theory]
        [InlineData(2.5)]
        [InlineData(0.4)]
        public async Task ChangeMediaSpeedSTestArgumentOutOfRange(double multiplication)
        {
            var inputFile = await MediaInfo.Get(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await Conversion.New()
                                                                                              .AddStream(inputFile.VideoStreams.First()
                                                                                                                  .SetCodec(VideoCodec.h264, 2400)
                                                                                                                  .ChangeSpeed(multiplication))
                                                                                              .SetSpeed(ConversionSpeed.UltraFast)
                                                                                              .UseMultiThread(true)
                                                                                              .SetOutput(outputPath)
                                                                                              .Start());
        }

        [Fact]
        public async Task BurnSubtitlesTest()
        {
            var inputFile = await MediaInfo.Get(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await Conversion.New()
               .AddStream(inputFile.VideoStreams.First().AddSubtitles(Resources.SubtitleSrt))
               .SetOutput(outputPath)
               .Start();

            Assert.True(conversionResult.Success);
            var mediaInfo = await MediaInfo.Get(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task BurnSubtitlesWithParametersTest()
        {
            var inputFile = await MediaInfo.Get(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            string subtitle = Resources.SubtitleSrt.Replace("\\", "\\\\")
                                                    .Replace(":", "\\:");

            IConversion conversion = Conversion.New()
                                               .AddStream(inputFile.VideoStreams.First()
                                               .AddSubtitles(Resources.SubtitleSrt, "UTF-8", "Fontsize=20,PrimaryColour=&H00ffff&,MarginV=30", new VideoSize(1024, 768)))
                                               .SetOutput(outputPath);
            IConversionResult conversionResult = await conversion.Start();

            Assert.True(conversionResult.Success);
            var mediaInfo = await MediaInfo.Get(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
            Assert.Contains($":charenc=UTF-8:force_style='Fontsize=20,PrimaryColour=&H00ffff&,MarginV=30':original_size=1024x768", conversion.Build());
        }

        [Fact]
        public async Task ChangeOutputFramesCountTest()
        {
            var inputFile = await MediaInfo.Get(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(inputFile.VideoStreams.First()
                                                                                                  .SetOutputFramesCount(50))
                                                                 .SetOutput(outputPath)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            var mediaInfo = await MediaInfo.Get(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(2), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
            Assert.Equal(50, mediaInfo.Duration.TotalSeconds * mediaInfo.VideoStreams.First().FrameRate);
        }

        [Fact]
        public async Task IncompatibleParametersTest()
        {
            var inputFile = await MediaInfo.Get(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            await Assert.ThrowsAsync<ConversionException>(async () =>
            {
                try
                {
                    await Conversion.New()
                                    .AddStream(inputFile.VideoStreams.First()
                                                                     .SetCodec(VideoCodec.h264, 2400)
                                                                     .Reverse()
                                                                     .CopyStream())
                                    .SetOutput(outputPath)
                                    .Start();
                }
                catch(ConversionException e)
                {
                    Assert.Contains("-c:v copy", e.InputParameters);
                    Assert.Contains("-vf reverse", e.InputParameters);
                    Assert.EndsWith(
                        $"Filtergraph \'reverse\' was defined for video output stream 0:0 but codec copy was selected.{Environment.NewLine}Filtering and streamcopy cannot be used together.",
                        e.Message);
                    throw;
                }
            });
        }

        [Fact]
        public async Task LoopTest()
        {
            var inputFile = await MediaInfo.Get(Resources.Mp4);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Gif);

            IConversionResult conversionResult = await Conversion.New()
                                                   .AddStream(inputFile.VideoStreams.First()
                                                                                    .SetLoop(1))
                                                   .SetOutput(outputPath)
                                                   .Start();

            Assert.True(conversionResult.Success);
            var mediaInfo = await MediaInfo.Get(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(0), mediaInfo.Duration);
            Assert.Equal("gif", mediaInfo.VideoStreams.First().Format);
            Assert.Equal("16:9", mediaInfo.VideoStreams.First().Ratio);
            Assert.Equal(25, mediaInfo.VideoStreams.First().FrameRate);
            Assert.Equal(1280, mediaInfo.VideoStreams.First().Width);
            Assert.Equal(720, mediaInfo.VideoStreams.First().Height);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task ReverseTest()
        {
            var inputFile = await MediaInfo.Get(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await Conversion.New()
                                                   .AddStream(inputFile.VideoStreams.First()
                                                                                    .SetCodec(VideoCodec.h264, 2400)
                                                                                    .Reverse())
                                                   .SetSpeed(ConversionSpeed.UltraFast)
                                                   .UseMultiThread(true)
                                                   .SetOutput(outputPath)
                                                   .Start();

            Assert.True(conversionResult.Success);
            var mediaInfo = await MediaInfo.Get(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        //    [Fact]
        //    public async Task ScaleTest()
        //    {
        //        string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
        //         IConversionResult conversionResult = await Conversion.New()
        //                                                .SetInput(Resources.MkvWithAudio)
        //                                                .SetSpeed(Speed.UltraFast)
        //                                                .UseMultiThread(true)
        //                                                .SetOutput(outputPath)
        //                                                .SetScale(VideoSize.Sqcif)
        //                                                .SetCodec(VideoCodec.h264, 2400)
        //                                                .SetCodec(AudioCodec.aac, AudioQuality.Ultra)
        //                                                .Start();

        //        Assert.True(conversionResult);
        //        var mediaInfo = await MediaInfo.Get(outputPath);
        //        Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
        //        Assert.Equal("h264", mediaInfo.MediaFormat);
        //        Assert.Equal("aac", mediaInfo.AudioFormat);
        //        Assert.Equal(128, mediaInfo.Width);
        //        Assert.Equal(96, mediaInfo.Height);
        //    }

        //    [Fact]
        //    public async Task SeekLengthTest()
        //    {
        //        string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
        //        IConversion conversion = Conversion.New()
        //                                           .SetInput(Resources.MkvWithAudio)
        //                                           .SetSeek(TimeSpan.FromSeconds(2))
        //                                           .SetOutput(outputPath);


        //        TimeSpan currentProgress;
        //        TimeSpan videoLength;
        //        conversion.OnProgress += (sender, e) =>
        //        {
        //            currentProgress = e.Duration;
        //            videoLength = e.TotalLength;
        //        };

        //        await conversion.Start();

        //        Assert.True(currentProgress > TimeSpan.Zero);
        //        Assert.True(currentProgress <= videoLength);
        //        Assert.True(videoLength == TimeSpan.FromSeconds(7));
        //    }

        //    [Fact]
        //    public async Task SimpleConversionTest()
        //    {
        //        string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
        //         IConversionResult conversionResult = await Conversion.New()
        //                                                .SetInput(Resources.MkvWithAudio)
        //                                                .SetOutput(outputPath)
        //                                                .Start();

        //        Assert.True(conversionResult);
        //        var mediaInfo = await MediaInfo.Get(outputPath);
        //        Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
        //        Assert.Equal("h264", mediaInfo.MediaFormat);
        //        Assert.Equal("aac", mediaInfo.AudioFormat);
        //    }

        //    [Fact]
        //    public async Task X265Test()
        //    {
        //        string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
        //         IConversionResult conversionResult = await Conversion.New()
        //            .SetInput(Resources.MkvWithAudio)
        //            .SetCodec(VideoCodec.hevc)
        //            .SetOutput(outputPath)
        //            .Start();

        //        Assert.True(conversionResult);
        //        var mediaInfo = await MediaInfo.Get(outputPath);
        //        Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
        //        Assert.Equal("hevc", mediaInfo.MediaFormat);
        //        Assert.Equal("aac", mediaInfo.AudioFormat);
        //    }

        //    [Fact]
        //    public async Task SizeTest()
        //    {
        //        string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
        //         IConversionResult conversionResult = await Conversion.New()
        //                                                .SetInput(Resources.MkvWithAudio)
        //                                                .SetOutput(outputPath)
        //                                                .SetSize(new VideoSize(640, 480))
        //                                                .Start();

        //        Assert.True(conversionResult);
        //        var mediaInfo = await MediaInfo.Get(outputPath);
        //        Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
        //        Assert.Equal("h264", mediaInfo.MediaFormat);
        //        Assert.Equal("aac", mediaInfo.AudioFormat);
        //        Assert.Equal(640, mediaInfo.Width);
        //        Assert.Equal(480, mediaInfo.Height);
        //    }

        //    [Fact]
        //    public async Task SetNullSizeTest()
        //    {
        //        string inputPath = Resources.MkvWithAudio;
        //        var inputMediaInfo = await MediaInfo.Get(inputPath);
        //        string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
        //         IConversionResult conversionResult = await Conversion.New()
        //                                                .SetInput(inputPath)
        //                                                .SetOutput(outputPath)
        //                                                .SetSize(null)
        //                                                .Start();

        //        Assert.True(conversionResult);
        //        var mediaInfo = await MediaInfo.Get(outputPath);
        //        Assert.Equal(inputMediaInfo.Width, mediaInfo.Width);
        //        Assert.Equal(inputMediaInfo.Height, mediaInfo.Height);
        //    }

        //    [Fact]
        //    public async Task VideoCodecTest()
        //    {
        //        string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".ts");
        //         IConversionResult conversionResult = await Conversion.New()
        //                                                .SetInput(Resources.MkvWithAudio)
        //                                                .SetOutput(outputPath)
        //                                                .SetFormat(MediaFormat.mpegts)
        //                                                .Start();

        //        Assert.True(conversionResult);
        //        var mediaInfo = await MediaInfo.Get(outputPath);
        //        Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
        //        Assert.Equal("mpeg2video", mediaInfo.MediaFormat);
        //        Assert.Equal("mp2", mediaInfo.AudioFormat);
        //    }
    }
}


