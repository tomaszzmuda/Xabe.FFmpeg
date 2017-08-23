using System;
using System.IO;
using Xabe.FFMpeg.Enums;
using Xunit;

namespace Xabe.FFMpeg.Test
{
    public class ConversionTests
    {
        private static readonly FileInfo SampleMkvVideo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Resources", "SampleVideo_360x240_1mb.mkv"));

        [Fact]
        public void DoubleSlowVideoSpeedTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");
            bool conversionResult = new Conversion()
                .SetInput(SampleMkvVideo)
                .SetSpeed(Speed.UltraFast)
                .UseMultiThread(true)
                .SetOutput(outputPath)
                .SetVideo(VideoCodec.LibX264, 2400)
                .SetAudio(AudioCodec.Aac, AudioQuality.Ultra)
                .ChangeVideoSpeed(0.5)
                .ChangeAudioSpeed(2)
                .Start();
            var videoInfo = new VideoInfo(outputPath);

            Assert.Equal(TimeSpan.FromSeconds(5), videoInfo.Duration);
            Assert.True(conversionResult);
        }

        [Fact]
        public void DoubleVideoSpeedTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");
            bool conversionResult = new Conversion()
                .SetInput(SampleMkvVideo)
                .SetSpeed(Speed.UltraFast)
                .UseMultiThread(true)
                .SetOutput(outputPath)
                .SetVideo(VideoCodec.LibX264, 2400)
                .SetAudio(AudioCodec.Aac, AudioQuality.Ultra)
                .ChangeVideoSpeed(2)
                .ChangeAudioSpeed(0.5)
                .Start();
            var videoInfo = new VideoInfo(outputPath);

            Assert.Equal(TimeSpan.FromSeconds(19), videoInfo.Duration);
            Assert.True(conversionResult);
        }

        [Fact]
        public void IncompatibleParametersTest()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");
                new Conversion()
                    .SetInput(SampleMkvVideo)
                    .SetOutput(outputPath)
                    .SetVideo(VideoCodec.LibX264, 2400)
                    .SetAudio(AudioCodec.Aac, AudioQuality.Ultra)
                    .Reverse(Channel.Both)
                    .StreamCopy(Channel.Both)
                    .Start();
            });
        }

        [Fact]
        public void ReverseTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");
            bool conversionResult = new Conversion()
                .SetInput(SampleMkvVideo)
                .SetSpeed(Speed.UltraFast)
                .UseMultiThread(true)
                .SetOutput(outputPath)
                .SetVideo(VideoCodec.LibX264, 2400)
                .SetAudio(AudioCodec.Aac, AudioQuality.Ultra)
                .Reverse(Channel.Both)
                .Start();
            var videoInfo = new VideoInfo(outputPath);

            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.Duration);
            Assert.True(conversionResult);
        }

        [Fact]
        public void ScaleTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");
            bool conversionResult = new Conversion()
                .SetInput(SampleMkvVideo)
                .SetSpeed(Speed.UltraFast)
                .UseMultiThread(true)
                .SetOutput(outputPath)
                .SetScale(VideoSize.sqcif)
                .SetVideo(VideoCodec.LibX264, 2400)
                .SetAudio(AudioCodec.Aac, AudioQuality.Ultra)
                .Start();
            var videoInfo = new VideoInfo(outputPath);

            Assert.Equal(128, videoInfo.Width);
            Assert.Equal(96, videoInfo.Height);
            Assert.True(conversionResult);
        }

        [Fact]
        public void MinumumOptionsTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");
            bool conversionResult = new Conversion()
                .SetInput(SampleMkvVideo)
                .SetOutput(outputPath)
                .Start();
            var videoInfo = new VideoInfo(outputPath);

            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.Duration);
            Assert.True(conversionResult);
        }
    }
}
