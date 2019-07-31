using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;
using Xabe.FFmpeg.Exceptions;
using Xabe.FFmpeg.Model;
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
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);

            IConversionResult result = await Conversion.New()
                                                    .AddStream(inputFile.VideoStreams.First().Rotate(rotateDegrees))
                                                    .SetOutput(outputPath)
                                                    .Start().ConfigureAwait(false);

            Assert.True(result.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task ChangeFramerate()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);

            var videoStream = inputFile.VideoStreams.First();
            var originalFramerate = videoStream.FrameRate;
            Assert.Equal(25, originalFramerate);
            videoStream.SetFramerate(24);

            IConversionResult result = await Conversion.New()
                                                    .AddStream(videoStream)
                                                    .SetOutput(outputPath)
                                                    .Start().ConfigureAwait(false);

            Assert.True(result.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
            Assert.Equal(24, mediaInfo.VideoStreams.First().FrameRate);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task ChangeBitrate()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);

            var videoStream = inputFile.VideoStreams.First();
            var originalBitrate = videoStream.Bitrate;
            Assert.Equal(860233, originalBitrate);
            videoStream.SetBitrate(6000);

            IConversionResult result = await Conversion.New()
                                                    .AddStream(videoStream)
                                                    .SetOutput(outputPath)
                                                    .Start().ConfigureAwait(false);

            Assert.True(result.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
            Assert.True(mediaInfo.VideoStreams.First().Bitrate < 10000);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        // Check if Filter Flags do work. FFProbe does not support checking for Interlaced or Progressive,
        //  so there is no "real check" here
        [Fact]
        public async Task SetFlags()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);

            var videoStream = inputFile.VideoStreams.First();
            videoStream.SetFlags(Flags.Interlaced);

            IConversionResult result = await Conversion.New()
                                                    .AddStream(videoStream)
                                                    .SetOutput(outputPath)
                                                    .Start().ConfigureAwait(false);

            Assert.True(result.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Theory]
        [InlineData(9, 9, 1.0)]
        [InlineData(5, 5, 2.0)]
        [InlineData(19, 19, 0.5)]
        public async Task ChangeSpeedTest(int expectedDuration, int expectedVideoDuration, double speed)
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await Conversion.New()
                                                    .AddStream(inputFile.VideoStreams.First().SetCodec(VideoCodec.H264).ChangeSpeed(speed))
                                                    .SetPreset(ConversionPreset.UltraFast)
                                                    .SetOutput(outputPath)
                                                    .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
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
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await Conversion.New()
                                                                                              .AddStream(inputFile.VideoStreams.First()
                                                                                                                  .SetCodec(VideoCodec.H264)
                                                                                                                  .ChangeSpeed(multiplication))
                                                                                              .SetPreset(ConversionPreset.UltraFast)
                                                                                              .SetOutput(outputPath)
                                                                                              .Start().ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task BurnSubtitlesTest()
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await Conversion.New()
               .AddStream(inputFile.VideoStreams.First().AddSubtitles(Resources.SubtitleSrt))
               .SetOutput(outputPath)
               .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task BurnSubtitlesWithParametersTest()
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversion conversion = Conversion.New()
                                               .AddStream(inputFile.VideoStreams.First()
                                               .AddSubtitles(Resources.SubtitleSrt, "UTF-8", "Fontsize=20,PrimaryColour=&H00ffff&,MarginV=30", new VideoSize(1024, 768)))
                                               .SetOutput(outputPath);
            IConversionResult conversionResult = await conversion.Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
            Assert.Contains($":charenc=UTF-8:force_style='Fontsize=20,PrimaryColour=&H00ffff&,MarginV=30':original_size=1024x768", conversion.Build());
        }

        [Fact]
        public async Task ChangeOutputFramesCountTest()
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(inputFile.VideoStreams.First()
                                                                                                  .SetOutputFramesCount(50))
                                                                 .SetOutput(outputPath)
                                                                 .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
            Assert.Equal(TimeSpan.FromSeconds(2), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
            Assert.Equal(50, mediaInfo.Duration.TotalSeconds * mediaInfo.VideoStreams.First().FrameRate);
        }

        [Fact]
        public async Task IncompatibleParametersTest()
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            await Assert.ThrowsAsync<ConversionException>(async () =>
            {
                try
                {
                    await Conversion.New()
                                    .AddStream(inputFile.VideoStreams.First()
                                                                     .SetCodec(VideoCodec.H264)
                                                                     .Reverse()
                                                                     .CopyStream())
                                    .SetOutput(outputPath)
                                    .Start().ConfigureAwait(false);
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
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task LoopTest()
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.Mp4).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Gif);

            IConversionResult conversionResult = await Conversion.New()
                                                   .AddStream(inputFile.VideoStreams.First()
                                                                                    .SetLoop(1))
                                                   .SetOutput(outputPath)
                                                   .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
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
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await Conversion.New()
                                                   .AddStream(inputFile.VideoStreams.First()
                                                                                    .SetCodec(VideoCodec.H264)
                                                                                    .Reverse())
                                                   .SetPreset(ConversionPreset.UltraFast)
                                                   .SetOutput(outputPath)
                                                   .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task ScaleTest()
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await Conversion.New()
                                                   .AddStream(inputFile.VideoStreams.First()
                                                                                     .SetScale(VideoSize.Sqcif)
                                                                                     .SetCodec(VideoCodec.H264))
                                                   .SetPreset(ConversionPreset.UltraFast)
                                                   .SetOutput(outputPath)
                                                   .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.Equal(128, mediaInfo.VideoStreams.First().Width);
            Assert.Equal(96, mediaInfo.VideoStreams.First().Height);
        }

        [Fact]
        public async Task SeekLengthTest()
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversion conversion = Conversion.New()
                                               .AddStream(inputFile.VideoStreams.First())
                                               .SetOutput(outputPath)
                                               .SetSeek(TimeSpan.FromSeconds(2));

            TimeSpan currentProgress;
            TimeSpan videoLength;
            conversion.OnProgress += (sender, e) =>
            {
                currentProgress = e.Duration;
                videoLength = e.TotalLength;
            };

            await conversion.Start().ConfigureAwait(false);

            Assert.True(currentProgress > TimeSpan.Zero);
            Assert.True(currentProgress <= videoLength);
            Assert.True(videoLength == TimeSpan.FromSeconds(7));
        }

        [Fact]
        public async Task SimpleConversionTest()
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(inputFile.VideoStreams.First())
                                                                 .SetOutput(outputPath)
                                                                 .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task X265Test()
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(inputFile.VideoStreams.First()
                                                                                                  .SetCodec(VideoCodec.Hevc))
               .SetOutput(outputPath)
               .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("hevc", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task SizeTest()
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(inputFile.VideoStreams.First()
                                                                                                  .SetSize(new VideoSize(640, 480)))
                                                                 .SetOutput(outputPath)
                                                                 .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
            Assert.Equal(640, mediaInfo.VideoStreams.First().Width);
            Assert.Equal(480, mediaInfo.VideoStreams.First().Height);
        }

        [Fact]
        public async Task SetNullSizeTest()
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);

            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(inputFile.VideoStreams.First()
                                                                                                  .SetSize(null))
                                                   .SetOutput(outputPath)
                                                   .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
            Assert.Equal(inputFile.VideoStreams.First().Width, mediaInfo.VideoStreams.First().Width);
            Assert.Equal(inputFile.VideoStreams.First().Height, mediaInfo.VideoStreams.First().Height);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task VideoCodecTest()
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".ts");

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(inputFile.VideoStreams.First()
                                                                 .SetCodec(new VideoCodec("mpeg2video")))
                                                   .SetOutput(outputPath)
                                                   .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("mpeg2video", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task ExtractAdditionalValuesTest()
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);

            Assert.Equal(1, inputFile.VideoStreams.First().Default.Value);
            Assert.Equal(0, inputFile.VideoStreams.First().Forced.Value);
        }
    }
}
