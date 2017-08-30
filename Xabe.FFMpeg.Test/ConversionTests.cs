using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFMpeg.Enums;
using Xunit;

namespace Xabe.FFMpeg.Test
{
    public class ConversionTests
    {
        private static readonly FileInfo SampleMkvVideo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Resources", "SampleVideo_360x240_1mb.mkv"));
        private static readonly FileInfo SampleTsWithAudio = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Resources", "sample.ts"));

        [Fact]
        public void ChangeOutputFramesCountTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            bool conversionResult = new Conversion()
                .SetInput(SampleMkvVideo)
                .SetOutput(outputPath)
                .SetOutputFramesCount(50)
                .Start();
            var videoInfo = new VideoInfo(outputPath);

            Assert.Equal(TimeSpan.FromSeconds(2), videoInfo.Duration);
            Assert.Equal(50, videoInfo.Duration.TotalSeconds * videoInfo.FrameRate);
            Assert.True(conversionResult);
        }


        [Fact]
        public void ConcatConversionStatusTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".ts");
            IConversion conversion = new Conversion()
                .StreamCopy(Channel.Both)
                .SetBitstreamFilter(Channel.Audio, Filter.Aac_AdtstoAsc)
                .SetOutput(outputPath)
                .Concat(SampleTsWithAudio.FullName, SampleTsWithAudio.FullName);

            TimeSpan currentProgress;
            TimeSpan videoLength;

            conversion.OnProgress += (duration, length) =>
            {
                currentProgress = duration;
                videoLength = length;
            };
            bool conversionResult = conversion.Start();

            Assert.True(conversionResult);
            Assert.True(currentProgress > TimeSpan.Zero);
            Assert.True(new VideoInfo(outputPath).Duration == TimeSpan.FromSeconds(26));
        }

        [Fact]
        public void ConcatVideosTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Ts);
            bool conversionResult = new Conversion()
                .StreamCopy(Channel.Both)
                .SetBitstreamFilter(Channel.Audio, Filter.Aac_AdtstoAsc)
                .SetOutput(outputPath)
                .Concat(SampleTsWithAudio.FullName, SampleTsWithAudio.FullName)
                .Start();
            var videoInfo = new VideoInfo(outputPath);

            Assert.Equal(TimeSpan.FromSeconds(26), videoInfo.Duration);
            Assert.True(conversionResult);
        }

        [Fact]
        public void ConversionStatusTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".ts");
            IConversion conversion = new Conversion()
                .SetInput(SampleMkvVideo)
                .SetOutput(outputPath)
                .SetCodec(VideoCodec.MpegTs);

            TimeSpan currentProgress;
            TimeSpan videoLength;

            conversion.OnProgress += (duration, length) =>
            {
                currentProgress = duration;
                videoLength = length;
            };
            bool conversionResult = conversion.Start();

            Assert.True(conversionResult);
            Assert.True(currentProgress > TimeSpan.Zero);
            Assert.True(currentProgress <= videoLength);
            Assert.True(videoLength == TimeSpan.FromSeconds(9));
        }

        [Fact]
        public void DisableAudioChannelTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            bool conversionResult = new Conversion()
                .SetInput(SampleMkvVideo)
                .SetOutput(outputPath)
                .DisableChannel(Channel.Audio)
                .Start();
            var videoInfo = new VideoInfo(outputPath);

            Assert.Equal("none", videoInfo.AudioFormat);
            Assert.True(conversionResult);
        }

        [Fact]
        public void DisableVideoChannelTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            bool conversionResult = new Conversion()
                .SetInput(SampleMkvVideo)
                .SetOutput(outputPath)
                .DisableChannel(Channel.Video)
                .Start();
            var videoInfo = new VideoInfo(outputPath);

            Assert.Equal("none", videoInfo.VideoFormat);
            Assert.True(conversionResult);
        }

        [Fact]
        public void DisposeFFMpegProcessTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Ts);
            IConversion conversion = new Conversion();
            Task<bool> task = Task.Run(() =>
                conversion
                    .SetInput(SampleMkvVideo)
                    .SetScale(VideoSize.Uhd4320)
                    .SetVideo(VideoCodec.LibTheora, 2400)
                    .SetAudio(AudioCodec.LibVorbis, AudioQuality.Ultra)
                    .SetOutput(outputPath)
                    .SetSpeed(Speed.VerySlow)
                    .UseMultiThread(false)
                    .Start());

            while(!conversion.IsRunning)
            {
            }

            Assert.True(conversion.IsRunning);
            conversion.Dispose();
            Assert.False(conversion.IsRunning);
            Assert.False(task.Result);
        }

        [Fact]
        public void DoubleSlowVideoSpeedTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
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
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
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
            Assert.Throws<ArgumentException>(() =>
            {
                string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
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
        public void MinumumOptionsTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            bool conversionResult = new Conversion()
                .SetInput(SampleMkvVideo)
                .SetOutput(outputPath)
                .Start();
            var videoInfo = new VideoInfo(outputPath);

            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.Duration);
            Assert.True(conversionResult);
        }

        [Fact]
        public void ReverseTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
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
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            bool conversionResult = new Conversion()
                .SetInput(SampleMkvVideo)
                .SetSpeed(Speed.UltraFast)
                .UseMultiThread(true)
                .SetOutput(outputPath)
                .SetScale(VideoSize.Sqcif)
                .SetVideo(VideoCodec.LibX264, 2400)
                .SetAudio(AudioCodec.Aac, AudioQuality.Ultra)
                .Start();
            var videoInfo = new VideoInfo(outputPath);

            Assert.Equal(128, videoInfo.Width);
            Assert.Equal(96, videoInfo.Height);
            Assert.True(conversionResult);
        }

        [Fact]
        public void SizeTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            bool conversionResult = new Conversion()
                .SetInput(SampleMkvVideo)
                .SetOutput(outputPath)
                .SetSize(new Size(640, 480))
                .Start();
            var videoInfo = new VideoInfo(outputPath);

            Assert.Equal(640, videoInfo.Width);
            Assert.Equal(480, videoInfo.Height);
            Assert.True(conversionResult);
        }

        [Fact]
        public void VideoCodecTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".ts");
            bool conversionResult = new Conversion()
                .SetInput(SampleMkvVideo)
                .SetOutput(outputPath)
                .SetCodec(VideoCodec.MpegTs)
                .Start();
            var videoInfo = new VideoInfo(outputPath);

            Assert.Equal("mpeg2video", videoInfo.VideoFormat);
            Assert.True(conversionResult);
        }

        [Fact]
        public void PassArgumentsTest()
        {
            var inputFile = Path.Combine(Environment.CurrentDirectory, "Resources", "SampleVideo_360x240_1mb.mkv");
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            string arguments = $"-i \"{inputFile}\" \"{outputPath}\"";

            bool conversionResult = new Conversion().Start(arguments);

            Assert.Equal(TimeSpan.FromSeconds(9), new VideoInfo(outputPath).Duration);
            Assert.True(conversionResult);
        }

        [Theory]
        [InlineData(RotateDegrees.Clockwise)]
        [InlineData(RotateDegrees.Invert)]
        public void TransposeTest(RotateDegrees rotateDegrees)
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            bool conversionResult = new Conversion()
                .SetInput(SampleMkvVideo)
                .Rotate(rotateDegrees)
                .SetOutput(outputPath)
                .Start();
            var videoInfo = new VideoInfo(outputPath);

            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.Duration);
            Assert.True(conversionResult);
        }
    }
}
