using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            IConversionResult result = await FFmpeg.Conversions.New()
                                                    .AddStream(inputFile.VideoStreams.First().Rotate(rotateDegrees))
                                                    .SetOutput(outputPath)
                                                    .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task ChangeFramerate()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            var videoStream = inputFile.VideoStreams.First();
            var originalFramerate = videoStream.Framerate;
            Assert.Equal(25, originalFramerate);
            videoStream.SetFramerate(24);

            IConversionResult result = await FFmpeg.Conversions.New()
                                                    .AddStream(videoStream)
                                                    .SetOutput(outputPath)
                                                    .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(24, mediaInfo.VideoStreams.First().Framerate);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task SetBitrate()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            var videoStream = inputFile.VideoStreams.First();
            var originalBitrate = videoStream.Bitrate;
            Assert.Equal(860233, originalBitrate);
            videoStream.SetBitrate(860237);

            IConversionResult result = await FFmpeg.Conversions.New()
                                                    .AddStream(videoStream)
                                                    .SetOutput(outputPath)
                                                    .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(561605, mediaInfo.VideoStreams.First().Bitrate);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task SetBitrate_WithMaxBitrateAndBuffer()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            var videoStream = inputFile.VideoStreams.First();
            var originalBitrate = videoStream.Bitrate;
            Assert.Equal(860233, originalBitrate);
            videoStream.SetBitrate(6000, 6000, 6000);

            IConversionResult result = await FFmpeg.Conversions.New()
                                                    .AddStream(videoStream)
                                                    .SetOutput(outputPath)
                                                    .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.InRange(mediaInfo.VideoStreams.First().Bitrate, 7000, 8000);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        // Check if Filter Flags do work. FFProbe does not support checking for Interlaced or Progressive,
        //  so there is no "real check" here
        [Fact]
        public async Task SetFlags()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            var videoStream = inputFile.VideoStreams.First();
            videoStream.SetFlags("ilme", "ildct");

            IConversionResult result = await FFmpeg.Conversions.New()
                                                    .AddStream(videoStream)
                                                    .SetOutput(outputPath)
                                                    .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
            Assert.Contains("-flags +ilme+ildct", result.Arguments);
        }

        // Check if Filter Flags do work. FFProbe does not support checking for Interlaced or Progressive,
        //  so there is no "real check" here
        [Fact]
        public async Task SetFlags_FlagsWithPlus_CorrectConversion()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            var videoStream = inputFile.VideoStreams.First();
            videoStream.SetFlags("ilme", "ildct");

            IConversionResult result = await FFmpeg.Conversions.New()
                                                    .AddStream(videoStream)
                                                    .SetOutput(outputPath)
                                                    .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
            Assert.Contains("-flags +ilme+ildct", result.Arguments);
        }

        // Check if Filter Flags do work. FFProbe does not support checking for Interlaced or Progressive,
        //  so there is no "real check" here
        [Fact]
        public async Task SetFlags_UseString_CorrectConversion()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            var videoStream = inputFile.VideoStreams.First();
            videoStream.SetFlags(Flag.ilme, Flag.ildct);

            IConversionResult result = await FFmpeg.Conversions.New()
                                                    .AddStream(videoStream)
                                                    .SetOutput(outputPath)
                                                    .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
            Assert.Contains("-flags +ilme+ildct", result.Arguments);
        }

        // Check if Filter Flags do work. FFProbe does not support checking for Interlaced or Progressive,
        //  so there is no "real check" here
        [Fact]
        public async Task SetFlags_ContatenatedFlags_CorrectConversion()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            var videoStream = inputFile.VideoStreams.First();
            videoStream.SetFlags("+ilme+ildct");

            IConversionResult result = await FFmpeg.Conversions.New()
                                                    .AddStream(videoStream)
                                                    .SetOutput(outputPath)
                                                    .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
            Assert.Contains("-flags +ilme+ildct", result.Arguments);
        }

        [Theory]
        [InlineData(9, 9, 1.0)]
        [InlineData(5, 5, 2.0)]
        [InlineData(19, 19, 0.5)]
        public async Task ChangeSpeedTest(int expectedDuration, int expectedVideoDuration, double speed)
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                    .AddStream(inputFile.VideoStreams.First().SetCodec(VideoCodec.h264).ChangeSpeed(speed))
                                                    .SetPreset(ConversionPreset.UltraFast)
                                                    .SetOutput(outputPath)
                                                    .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(expectedDuration), mediaInfo.Duration);
            Assert.Equal(TimeSpan.FromSeconds(expectedVideoDuration), mediaInfo.VideoStreams.First().Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Theory]
        [InlineData(2.5)]
        [InlineData(0.4)]
        public async Task ChangeMediaSpeedSTestArgumentOutOfRange(double multiplication)
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await FFmpeg.Conversions.New()
                                                                                              .AddStream(inputFile.VideoStreams.First()
                                                                                                                  .SetCodec(VideoCodec.h264)
                                                                                                                  .ChangeSpeed(multiplication))
                                                                                              .SetPreset(ConversionPreset.UltraFast)
                                                                                              .SetOutput(outputPath)
                                                                                              .Start());
        }

        [Fact]
        public async Task BurnSubtitlesTest()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
               .AddStream(inputFile.VideoStreams.First().AddSubtitles(Resources.SubtitleSrt))
               .SetOutput(outputPath)
               .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task BurnSubtitlesWithParametersTest()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversion conversion = FFmpeg.Conversions.New()
                                               .AddStream(inputFile.VideoStreams.First()
                                               .AddSubtitles(Resources.SubtitleSrt, VideoSize.Xga, "UTF-8", "Fontsize=20,PrimaryColour=&H00ffff&,MarginV=30"))
                                               .SetOutput(outputPath);
            IConversionResult conversionResult = await conversion.Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
            Assert.Contains($":charenc=UTF-8:force_style='Fontsize=20,PrimaryColour=&H00ffff&,MarginV=30':original_size=1024x768", conversion.Build());
        }

        [Fact]
        public async Task ChangeOutputFramesCountTest()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(inputFile.VideoStreams.First()
                                                                                                  .SetOutputFramesCount(50))
                                                                 .SetOutput(outputPath)
                                                                 .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(2), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
            Assert.Equal(50, mediaInfo.Duration.TotalSeconds * mediaInfo.VideoStreams.First().Framerate);
        }

        [Fact]
        public async Task IncompatibleParametersTest()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            await Assert.ThrowsAsync<ConversionException>(async () =>
            {
                try
                {
                    await FFmpeg.Conversions.New()
                                    .AddStream(inputFile.VideoStreams.First()
                                                                     .SetCodec(VideoCodec.h264)
                                                                     .Reverse()
                                                                     .CopyStream())
                                    .SetOutput(outputPath)
                                    .Start();
                }
                catch (ConversionException e)
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
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp4);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Gif);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                   .AddStream(inputFile.VideoStreams.First()
                                                                                    .SetLoop(1))
                                                   .SetOutput(outputPath)
                                                   .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(13), mediaInfo.Duration);
            Assert.Equal("gif", mediaInfo.VideoStreams.First().Codec);
            Assert.Equal("16:9", mediaInfo.VideoStreams.First().Ratio);
            Assert.Equal(25, mediaInfo.VideoStreams.First().Framerate);
            Assert.Equal(1280, mediaInfo.VideoStreams.First().Width);
            Assert.Equal(720, mediaInfo.VideoStreams.First().Height);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task ReverseTest()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                   .AddStream(inputFile.VideoStreams.First()
                                                                                    .SetCodec(VideoCodec.h264)
                                                                                    .Reverse())
                                                   .SetPreset(ConversionPreset.UltraFast)
                                                   .SetOutput(outputPath)
                                                   .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task SeekLengthTest()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversion conversion = FFmpeg.Conversions.New()
                                               .AddStream(inputFile.VideoStreams.First())
                                               .SetOutput(outputPath)
                                               .SetSeek(TimeSpan.FromSeconds(2));

            TimeSpan currentProgress = new TimeSpan();
            TimeSpan videoLength = new TimeSpan();
            conversion.OnProgress += (sender, e) =>
            {
                currentProgress = e.Duration;
                videoLength = e.TotalLength;
            };

            await conversion.Start();

            Assert.True(currentProgress > TimeSpan.Zero);
            Assert.True(currentProgress <= videoLength);
            Assert.True(videoLength == TimeSpan.FromSeconds(7));
        }

        [Fact]
        public async Task SimpleConversionTest()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(inputFile.VideoStreams.First())
                                                                 .SetOutput(outputPath)
                                                                 .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task X265Test()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(inputFile.VideoStreams.First()
                                                                                                  .SetCodec(VideoCodec.hevc))
               .SetOutput(outputPath)
               .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("hevc", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task SizeTest()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(inputFile.VideoStreams.First()
                                                                                                  .SetSize(640, 480))
                                                                 .SetOutput(outputPath)
                                                                 .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
            Assert.Equal(640, mediaInfo.VideoStreams.First().Width);
            Assert.Equal(480, mediaInfo.VideoStreams.First().Height);
        }

        [Fact]
        public async Task SetSize_UseEnum_ResultsAreCorrect()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(inputFile.VideoStreams.First()
                                                                                                  .SetSize(VideoSize.Sntsc))
                                                                 .SetOutput(outputPath)
                                                                 .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
            Assert.Equal(640, mediaInfo.VideoStreams.First().Width);
            Assert.Equal(480, mediaInfo.VideoStreams.First().Height);
        }

        [Fact]
        public async Task VideoCodecTest()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".ts");

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                                 .AddStream(inputFile.VideoStreams.First()
                                                                 .SetCodec(VideoCodec.mpeg2video))
                                                   .SetOutput(outputPath)
                                                   .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("mpeg2video", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task ExtractAdditionalValuesTest()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            Assert.Equal(1, inputFile.VideoStreams.First().Default.Value);
            Assert.Equal(0, inputFile.VideoStreams.First().Forced.Value);
        }

        [Fact]
        public async Task ChangeSpeed_CommaAsASeparator_CorrectResult()
        {
            CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("pl-PL");

            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                    .AddStream(inputFile.VideoStreams.First().SetCodec(VideoCodec.h264).ChangeSpeed(0.5))
                                                    .SetPreset(ConversionPreset.UltraFast)
                                                    .SetOutput(outputPath)
                                                    .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(19), mediaInfo.Duration);
            Assert.Equal(TimeSpan.FromSeconds(19), mediaInfo.VideoStreams.First().Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task SetBitstreamFilter_CorrectInput_CorrectResult()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp4);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                    .AddStream(inputFile.VideoStreams.First().SetBitstreamFilter(BitstreamFilter.h264_mp4toannexb))
                                                    .SetOutput(outputPath)
                                                    .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(13), mediaInfo.Duration);
            Assert.Equal(TimeSpan.FromSeconds(13), mediaInfo.VideoStreams.First().Duration);
            Assert.NotEmpty(mediaInfo.VideoStreams);
        }

        [Fact]
        public async Task SetBitstreamFilter_IncorrectFilter_ThrowConversionException()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp4);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            var exception = await Record.ExceptionAsync(async () => await FFmpeg.Conversions.New()
                                                    .AddStream(inputFile.VideoStreams.First().SetBitstreamFilter(BitstreamFilter.aac_adtstoasc))
                                                    .SetOutput(outputPath)
                                                    .Start()); ;

            Assert.NotNull(exception);
            Assert.IsType<ConversionException>(exception);
            Assert.IsType<InvalidBitstreamFilterException>(exception.InnerException);
        }

        [Fact]
        public async Task SetBitstreamFilter_CorrectInputAsString_CorrectResult()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp4);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                    .AddStream(inputFile.VideoStreams.First().SetBitstreamFilter("h264_mp4toannexb"))
                                                    .SetOutput(outputPath)
                                                    .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(13), mediaInfo.Duration);
            Assert.Equal(TimeSpan.FromSeconds(13), mediaInfo.VideoStreams.First().Duration);
            Assert.NotEmpty(mediaInfo.VideoStreams);
        }

        [Fact]
        public async Task SetBitstreamFilter_IncorrectFilterAsString_ThrowConversionException()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp4);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            var exception = await Record.ExceptionAsync(async () => await FFmpeg.Conversions.New()
                                                    .AddStream(inputFile.VideoStreams.First().SetBitstreamFilter("aac_adtstoasc"))
                                                    .SetOutput(outputPath)
                                                    .Start()); ;

            Assert.NotNull(exception);
            Assert.IsType<ConversionException>(exception);
            Assert.IsType<InvalidBitstreamFilterException>(exception.InnerException);
        }

        [Theory]
        [InlineData(VideoCodec._4xm, "4xm")]
        [InlineData(VideoCodec._8bps, "8bps")]
        [InlineData(VideoCodec._012v, "012v")]
        public async Task SetCodec_SpecialNames_EverythingIsCorrect(VideoCodec videoCodec, string expectedCodec)
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            var args = FFmpeg.Conversions.New()
               .AddStream(inputFile.VideoStreams.First()
                    .SetCodec(videoCodec))
               .SetOutput(outputPath)
               .Build();

            Assert.Contains($"-c:v {expectedCodec}", args);
        }

        [Fact]
        public async Task SetCodec_InvalidCodec_ThrowConversionException()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            var exception = await Record.ExceptionAsync(async () => await FFmpeg.Conversions.New()
                                .AddStream(inputFile.VideoStreams.First().SetCodec("notExisting"))
                                .SetOutput(outputPath)
                                .Start());

            Assert.NotNull(exception);
            Assert.IsType<ConversionException>(exception);
        }
    }
}
