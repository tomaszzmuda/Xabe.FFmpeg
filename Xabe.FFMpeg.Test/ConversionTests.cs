using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFMpeg.Enums;
using Xabe.FFMpeg.Exceptions;
using Xunit;

namespace Xabe.FFMpeg.Test
{
    public class ConversionTests
    {
        [Theory]
        [InlineData(RotateDegrees.Clockwise)]
        [InlineData(RotateDegrees.Invert)]
        public async Task TransposeTest(RotateDegrees rotateDegrees)
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            bool conversionResult = await new Conversion()
                .SetInput(Resources.MkvWithAudio)
                .Rotate(rotateDegrees)
                .SetOutput(outputPath)
                .Start();

            Assert.True(conversionResult);
            var videoInfo = new VideoInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.VideoProperties.Duration);
            Assert.Equal("h264", videoInfo.VideoProperties.VideoFormat);
            Assert.Equal("aac", videoInfo.VideoProperties.AudioFormat);
        }

        [Fact]
        public async Task ChangeOutputFramesCountTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            bool conversionResult = await new Conversion()
                .SetInput(Resources.MkvWithAudio)
                .SetOutput(outputPath)
                .SetOutputFramesCount(50)
                .Start();

            Assert.True(conversionResult);
            var videoInfo = new VideoInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(2), videoInfo.VideoProperties.Duration);
            Assert.Equal("h264", videoInfo.VideoProperties.VideoFormat);
            Assert.Equal("aac", videoInfo.VideoProperties.AudioFormat);
            Assert.Equal(50, videoInfo.VideoProperties.Duration.TotalSeconds * videoInfo.VideoProperties.FrameRate);
        }

        [Fact]
        public async Task ClearParametersTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            IConversion conversion = new Conversion()
                .SetInput(Resources.MkvWithAudio)
                .SetCodec(VideoCodec.LibVpx)
                .SetScale(VideoSize.Ega)
                .StreamCopy(Channel.Both)
                .Reverse(Channel.Both)
                .SetOutput(outputPath);

            conversion.Clear();

            bool conversionResult = await conversion.SetInput(Resources.MkvWithAudio)
                                                    .SetOutput(outputPath)
                                                    .Start();

            Assert.True(conversionResult);
            var videoInfo = new VideoInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.VideoProperties.Duration);
            Assert.Equal("h264", videoInfo.VideoProperties.VideoFormat);
            Assert.Equal("aac", videoInfo.VideoProperties.AudioFormat);
        }


        [Fact]
        public async Task ConcatConversionStatusTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".ts");
            IConversion conversion = new Conversion()
                .StreamCopy(Channel.Both)
                .SetBitstreamFilter(Channel.Audio, Filter.Aac_AdtstoAsc)
                .SetOutput(outputPath)
                .Concat(Resources.TsWithAudio.FullName, Resources.TsWithAudio.FullName);

            TimeSpan currentProgress;

            conversion.OnProgress += (duration, length) => { currentProgress = duration; };
            bool conversionResult = await conversion.Start();

            Assert.True(conversionResult);
            Assert.True(currentProgress > TimeSpan.Zero);
            Assert.True(new VideoInfo(outputPath).VideoProperties.Duration == TimeSpan.FromSeconds(26));
        }

        [Fact]
        public async Task ConcatVideosTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Ts);
            bool conversionResult = await new Conversion()
                .StreamCopy(Channel.Both)
                .SetBitstreamFilter(Channel.Audio, Filter.Aac_AdtstoAsc)
                .SetOutput(outputPath)
                .Concat(Resources.TsWithAudio.FullName, Resources.TsWithAudio.FullName)
                .Start();

            Assert.True(conversionResult);
            var videoInfo = new VideoInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(26), videoInfo.VideoProperties.Duration);
            Assert.Equal("h264", videoInfo.VideoProperties.VideoFormat);
            Assert.Equal("aac", videoInfo.VideoProperties.AudioFormat);
        }

        [Fact]
        public async Task ConversionStatusTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".ts");
            IConversion conversion = new Conversion()
                .SetInput(Resources.MkvWithAudio)
                .SetOutput(outputPath)
                .SetCodec(VideoCodec.MpegTs);

            TimeSpan currentProgress;
            TimeSpan videoLength;

            conversion.OnProgress += (duration, length) =>
            {
                currentProgress = duration;
                videoLength = length;
            };
            bool conversionResult = await conversion.Start();

            Assert.True(conversionResult);
            Assert.True(currentProgress > TimeSpan.Zero);
            Assert.True(currentProgress <= videoLength);
            Assert.True(videoLength == TimeSpan.FromSeconds(9));
        }

        [Fact]
        public async Task DisableAudioChannelTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            bool conversionResult = await new Conversion()
                .SetInput(Resources.MkvWithAudio)
                .SetOutput(outputPath)
                .DisableChannel(Channel.Audio)
                .Start();

            Assert.True(conversionResult);
            var videoInfo = new VideoInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.VideoProperties.Duration);
            Assert.Equal("h264", videoInfo.VideoProperties.VideoFormat);
            Assert.Null(videoInfo.VideoProperties.AudioFormat);
        }

        [Fact]
        public async Task DisableVideoChannelTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            bool conversionResult = await new Conversion()
                .SetInput(Resources.MkvWithAudio)
                .SetOutput(outputPath)
                .DisableChannel(Channel.Video)
                .Start();

            Assert.True(conversionResult);
            var videoInfo = new VideoInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.VideoProperties.Duration);
            Assert.Null(videoInfo.VideoProperties.VideoFormat);
            Assert.Equal("aac", videoInfo.VideoProperties.AudioFormat);
        }

        [Fact]
        public async Task DisposeFFMpegProcessTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Ts);
            IConversion conversion = new Conversion();
            Task<bool> result = conversion
                .SetInput(Resources.MkvWithAudio)
                .SetScale(VideoSize.Uhd4320)
                .SetVideo(VideoCodec.LibTheora, 2400)
                .SetAudio(AudioCodec.LibVorbis, AudioQuality.Ultra)
                .SetOutput(outputPath)
                .SetSpeed(Speed.VerySlow)
                .UseMultiThread(false)
                .Start();

            while(!conversion.IsRunning)
            {
            }

            Assert.True(conversion.IsRunning);
            conversion.Dispose();
            Assert.False(conversion.IsRunning);
            Assert.False(await result);
        }

        [Fact]
        public async Task DoubleSlowVideoSpeedTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            bool conversionResult = await new Conversion()
                .SetInput(Resources.MkvWithAudio)
                .SetSpeed(Speed.UltraFast)
                .UseMultiThread(true)
                .SetOutput(outputPath)
                .SetVideo(VideoCodec.LibX264, 2400)
                .SetAudio(AudioCodec.Aac, AudioQuality.Ultra)
                .ChangeVideoSpeed(0.5)
                .ChangeAudioSpeed(2)
                .Start();

            Assert.True(conversionResult);
            var videoInfo = new VideoInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(6), videoInfo.VideoProperties.Duration);
            Assert.Equal("h264", videoInfo.VideoProperties.VideoFormat);
            Assert.Equal("aac", videoInfo.VideoProperties.AudioFormat);
        }

        [Fact]
        public async Task DoubleVideoSpeedTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            bool conversionResult = await new Conversion()
                .SetInput(Resources.MkvWithAudio)
                .SetSpeed(Speed.UltraFast)
                .UseMultiThread(true)
                .SetOutput(outputPath)
                .SetVideo(VideoCodec.LibX264, 2400)
                .SetAudio(AudioCodec.Aac, AudioQuality.Ultra)
                .ChangeVideoSpeed(2)
                .ChangeAudioSpeed(0.5)
                .Start();

            Assert.True(conversionResult);
            var videoInfo = new VideoInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(24), videoInfo.VideoProperties.Duration);
            Assert.Equal("h264", videoInfo.VideoProperties.VideoFormat);
            Assert.Equal("aac", videoInfo.VideoProperties.AudioFormat);
        }

        [Fact]
        public async Task IncompatibleParametersTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            await Assert.ThrowsAsync<ConversionException>(async () =>
            {
                try
                {
                    await new Conversion()
                        .SetInput(Resources.MkvWithAudio)
                        .SetOutput(outputPath)
                        .SetVideo(VideoCodec.LibX264, 2400)
                        .SetAudio(AudioCodec.Aac, AudioQuality.Ultra)
                        .Reverse(Channel.Both)
                        .StreamCopy(Channel.Both)
                        .Start();
                }
                catch(ConversionException e)
                {
                    Assert.Equal(
                        $"-i \"{Resources.MkvWithAudio.FullName}\" -codec:v libx264 -b:v 2400k -codec:a aac -b:a 384k -strict experimental -c copy -vf reverse -af areverse \"{outputPath}\"",
                        e.InputParameters);
                    Assert.EndsWith(
                        "Filtergraph \'reverse\' was defined for video output stream 0:0 but codec copy was selected.\r\nFiltering and streamcopy cannot be used together.",
                        e.Message);
                    // ReSharper disable once PossibleIntendedRethrow
                    throw e;
                }
            });
        }

        [Fact]
        public async Task MinimumOptionsTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            bool conversionResult = await new Conversion()
                .SetInput(Resources.MkvWithAudio)
                .SetOutput(outputPath)
                .Start();

            Assert.True(conversionResult);
            var videoInfo = new VideoInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.VideoProperties.Duration);
            Assert.Equal("h264", videoInfo.VideoProperties.VideoFormat);
            Assert.Equal("aac", videoInfo.VideoProperties.AudioFormat);
        }

        [Fact]
        public async Task MultipleTaskTest()
        {
            string mp4Output = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            string tsOutput = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Ts);

            await Assert.ThrowsAsync<MultipleConversionException>(async () =>
            {
                try
                {
                    IConversion conversion = new Conversion();
                    Task<bool> toMp4 = conversion
                        .SetInput(Resources.MkvWithAudio)
                        .SetOutput(mp4Output)
                        .Start();

                    conversion.SetOutput(tsOutput);
                    await conversion.Start();
                    await toMp4;
                }
                catch(MultipleConversionException e)
                {
                    Assert.Equal(
                        $"-i \"{Resources.MkvWithAudio.FullName}\" \"{tsOutput}\"",
                        e.InputParameters);
                    Assert.Equal(
                        "Current FFMpeg process associated to this object is already in use. Please wait till the end of file conversion or create another VideoInfo/Conversion instance and run process.",
                        e.Message);
                    // ReSharper disable once PossibleIntendedRethrow
                    throw e;
                }
            });
        }

        [Fact]
        public async Task PassArgumentsTest()
        {
            string inputFile = Path.Combine(Environment.CurrentDirectory, "Resources", "SampleVideo_360x240_1mb.mkv");
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            string arguments = $"-i \"{inputFile}\" \"{outputPath}\"";

            bool conversionResult = await new Conversion().Start(arguments);

            Assert.True(conversionResult);
            var videoInfo = new VideoInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.VideoProperties.Duration);
            Assert.Equal("h264", videoInfo.VideoProperties.VideoFormat);
            Assert.Equal("aac", videoInfo.VideoProperties.AudioFormat);
        }

        [Fact]
        public async Task ReverseTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            bool conversionResult = await new Conversion()
                .SetInput(Resources.MkvWithAudio)
                .SetSpeed(Speed.UltraFast)
                .UseMultiThread(true)
                .SetOutput(outputPath)
                .SetVideo(VideoCodec.LibX264, 2400)
                .SetAudio(AudioCodec.Aac, AudioQuality.Ultra)
                .Reverse(Channel.Both)
                .Start();

            Assert.True(conversionResult);
            var videoInfo = new VideoInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.VideoProperties.Duration);
            Assert.Equal("h264", videoInfo.VideoProperties.VideoFormat);
            Assert.Equal("aac", videoInfo.VideoProperties.AudioFormat);
        }

        [Fact]
        public async Task ScaleTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            bool conversionResult = await new Conversion()
                .SetInput(Resources.MkvWithAudio)
                .SetSpeed(Speed.UltraFast)
                .UseMultiThread(true)
                .SetOutput(outputPath)
                .SetScale(VideoSize.Sqcif)
                .SetVideo(VideoCodec.LibX264, 2400)
                .SetAudio(AudioCodec.Aac, AudioQuality.Ultra)
                .Start();

            Assert.True(conversionResult);
            var videoInfo = new VideoInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.VideoProperties.Duration);
            Assert.Equal("h264", videoInfo.VideoProperties.VideoFormat);
            Assert.Equal("aac", videoInfo.VideoProperties.AudioFormat);
            Assert.Equal(128, videoInfo.VideoProperties.Width);
            Assert.Equal(96, videoInfo.VideoProperties.Height);
        }

        [Fact]
        public async Task SizeTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            bool conversionResult = await new Conversion()
                .SetInput(Resources.MkvWithAudio)
                .SetOutput(outputPath)
                .SetSize(new Size(640, 480))
                .Start();

            Assert.True(conversionResult);
            var videoInfo = new VideoInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.VideoProperties.Duration);
            Assert.Equal("h264", videoInfo.VideoProperties.VideoFormat);
            Assert.Equal("aac", videoInfo.VideoProperties.AudioFormat);
            Assert.Equal(640, videoInfo.VideoProperties.Width);
            Assert.Equal(480, videoInfo.VideoProperties.Height);
        }

        [Fact]
        public async Task VideoCodecTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".ts");
            bool conversionResult = await new Conversion()
                .SetInput(Resources.MkvWithAudio)
                .SetOutput(outputPath)
                .SetCodec(VideoCodec.MpegTs)
                .Start();

            Assert.True(conversionResult);
            var videoInfo = new VideoInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.VideoProperties.Duration);
            Assert.Equal("mpeg2video", videoInfo.VideoProperties.VideoFormat);
            Assert.Equal("mp2", videoInfo.VideoProperties.AudioFormat);
        }
    }
}
