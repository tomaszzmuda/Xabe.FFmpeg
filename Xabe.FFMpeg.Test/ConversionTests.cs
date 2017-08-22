using System;
using System.IO;
using Xabe.FFMpeg.Enums;
using Xunit;

namespace Xabe.FFMpeg.Test
{
    public class ConversionTests
    {
        private static readonly FileInfo SampleMkvVideo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Resources", "sampleMkv.mkv"));

        [Fact]
        public void DoubleVideoSpeedTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");
            bool conversionResult = new Conversion()
                .SetInput(SampleMkvVideo)
                .SetSpeed(Speed.UltraFast)
                .UseMultiThread(true)
                .SetOutput(outputPath)
                .SetScale(VideoSize.Original)
                .SetVideo(VideoCodec.LibX264, 2400)
                .SetAudio(AudioCodec.Aac, AudioQuality.Ultra)
                .ChangeVideoSpeed(2)
                .ChangeAudioSpeed(0.5)
                .Start();
            Assert.True(conversionResult);
        }

        [Fact]
        public void IncompatibleParametersTest()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");
                bool conversionResult = new Conversion()
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
                .SetScale(VideoSize.Original)
                .SetVideo(VideoCodec.LibX264, 2400)
                .SetAudio(AudioCodec.Aac, AudioQuality.Ultra)
                .Reverse(Channel.Both)
                .Start();
            Assert.True(conversionResult);
        }

        [Fact]
        public void Test()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");
            bool conversionResult = new Conversion()
                .SetInput(SampleMkvVideo)
                .SetSpeed(Speed.UltraFast)
                .UseMultiThread(true)
                .SetOutput(outputPath)
                .StreamCopy(Channel.Both)
                .SetScale(VideoSize.Original)
                .SetVideo(VideoCodec.LibX264, 2400)
                .SetAudio(AudioCodec.Aac, AudioQuality.Ultra)
                .Start();
            Assert.True(conversionResult);
        }
    }
}
