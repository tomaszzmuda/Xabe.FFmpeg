using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFMpeg.Enums;
using Xunit;

namespace Xabe.FFMpeg.Test
{
    public class VideoInfoTests

    {
        private static readonly FileInfo SampleVideoWithAudio = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Resources", "input.mp4"));
        private static readonly FileInfo SampleAudio = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Resources", "audio.mp3"));
        private static readonly FileInfo SampleVideo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Resources", "mute.mp4"));
        private static readonly FileInfo SampleMkvVideo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Resources", "SampleVideo_360x240_1mb.mkv"));

        [Fact]
        public async Task AddAudio()
        {
            IVideoInfo videoInfo = new VideoInfo(SampleVideo);
            string output = Path.ChangeExtension(Path.GetTempFileName(), videoInfo.Extension);

            bool result = await videoInfo.AddAudio(SampleAudio, output);
            Assert.True(result);
            Assert.Equal(videoInfo.Duration, new VideoInfo(output).Duration);
        }

        [Fact]
        public async Task DisposeTest()
        {
            IVideoInfo videoInfo = new VideoInfo(SampleMkvVideo);
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Ts);

            Task<bool> result = videoInfo.ToMp4(output, Speed.VerySlow, null, AudioQuality.Ultra);
            while(!videoInfo.IsRunning)
            {
            }

            Assert.True(videoInfo.IsRunning);
            videoInfo.Dispose();
            Assert.False(videoInfo.IsRunning);
            Assert.False(await result);
        }

        [Fact]
        public async Task ExtractAudio()
        {
            FileInfo fileInfo = SampleVideoWithAudio;
            string output = Path.ChangeExtension(Path.GetTempFileName(), ".mp3");

            bool result = await new VideoInfo(fileInfo).ExtractAudio(output);

            Assert.True(result);
            Assert.Equal("none", new VideoInfo(output).VideoFormat);
        }

        [Fact]
        public async Task ExtractVideo()
        {
            FileInfo fileInfo = SampleVideoWithAudio;
            string output = Path.ChangeExtension(Path.GetTempFileName(), fileInfo.Extension);

            bool result = await new VideoInfo(fileInfo).ExtractVideo(output);
            Assert.True(result);
        }

        [Fact]
        public async Task JoinWith()
        {
            IVideoInfo videoInfo = new VideoInfo(SampleVideoWithAudio);
            string output = Path.ChangeExtension(Path.GetTempFileName(), videoInfo.Extension);

            bool result = await videoInfo.JoinWith(output, new VideoInfo(SampleVideo));

            Assert.True(result);
        }

        [Fact]
        public void MkvPropertiesTest()
        {
            IVideoInfo videoInfo = new VideoInfo(SampleMkvVideo);

            Assert.Equal("aac", videoInfo.AudioFormat);
            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.Duration);
            Assert.True(File.Exists(videoInfo.FilePath));
            Assert.Equal(".mkv", videoInfo.Extension);
            Assert.Equal(25, videoInfo.FrameRate);
            Assert.Equal(240, videoInfo.Height);
            Assert.Equal(320, videoInfo.Width);
            Assert.Equal("SampleVideo_360x240_1mb.mkv", Path.GetFileName(videoInfo.FilePath));
            Assert.False(videoInfo.IsRunning);
            Assert.Equal("4:3", videoInfo.Ratio);
        }

        [Fact]
        public void PropertiesTest()
        {
            IVideoInfo videoInfo = new VideoInfo(SampleVideoWithAudio);

            Assert.Equal("aac", videoInfo.AudioFormat);
            Assert.Equal(TimeSpan.FromSeconds(13), videoInfo.Duration);
            Assert.True(File.Exists(videoInfo.FilePath));
            Assert.Equal(".mp4", videoInfo.Extension);
            Assert.Equal(25, videoInfo.FrameRate);
            Assert.Equal(720, videoInfo.Height);
            Assert.Equal(1280, videoInfo.Width);
            Assert.Equal("input.mp4", Path.GetFileName(videoInfo.FilePath));
            Assert.False(videoInfo.IsRunning);
            Assert.Equal("16:9", videoInfo.Ratio);
        }

        [Fact]
        public async Task Snapshot()
        {
            IVideoInfo videoInfo = new VideoInfo(SampleVideoWithAudio);
            Image snapshot = await videoInfo.Snapshot();

            Assert.Equal(snapshot.Width, videoInfo.Width);
        }

        [Fact]
        public async Task SnapshotWithOutput()
        {
            IVideoInfo videoInfo = new VideoInfo(SampleVideoWithAudio);
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Png);
            Image snapshot = await videoInfo.Snapshot(output);

            Assert.Equal(snapshot.Width, videoInfo.Width);
            Assert.True(File.Exists(output));
        }

        [Fact]
        public async Task ToMp4Test()
        {
            IVideoInfo videoInfo = new VideoInfo(SampleMkvVideo);
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Mp4);

            await videoInfo.ToMp4(output);

            Assert.True(File.Exists(output));
            Assert.Equal(TimeSpan.FromSeconds(9), new VideoInfo(output).Duration);
        }

        [Fact]
        public async Task OnProgressChangedTest()
        {
            IVideoInfo videoInfo = new VideoInfo(SampleMkvVideo);
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Mp4);

            TimeSpan currentProgress;
            TimeSpan videoLength;

            videoInfo.OnConversionProgress += (duration, length) =>
            {
                currentProgress = duration;
                videoLength = length;
            };
            bool conversionResult = await videoInfo.ToTs(output);

            Assert.True(conversionResult);
            Assert.True(currentProgress > TimeSpan.Zero);
            Assert.True(currentProgress <= videoLength);
            Assert.True(videoLength == TimeSpan.FromSeconds(9));
        }

        [Fact]
        public async Task ToOgvTest()
        {
            IVideoInfo videoInfo = new VideoInfo(SampleVideoWithAudio);
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Ogv);

            await videoInfo.ToOgv(output);

            Assert.True(File.Exists(output));
            Assert.Equal(TimeSpan.FromSeconds(13), new VideoInfo(output).Duration);
        }

        [Fact]
        public async Task MultipleTaskTest()
        {
            IVideoInfo videoInfo = new VideoInfo(SampleVideoWithAudio);
            string ogvOutput = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Ogv);
            string tsOutput = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Ts);

            Task<bool> ogvResult = videoInfo.ToOgv(ogvOutput);
            Task<bool> mp4Result = videoInfo.ToTs(tsOutput);

            Assert.True(await ogvResult);
            Assert.True(await mp4Result);
            Assert.True(File.Exists(ogvOutput));
            Assert.True(File.Exists(tsOutput));
            Assert.Equal(TimeSpan.FromSeconds(13), new VideoInfo(ogvOutput).Duration);
            Assert.Equal(TimeSpan.FromSeconds(13), new VideoInfo(tsOutput).Duration);
        }

        [Fact]
        public void ToStringTest()
        {
            IVideoInfo videoInfo = new VideoInfo(SampleVideoWithAudio);
            string output = videoInfo.ToString();
            Assert.EndsWith(
                $"Video Name: input.mp4{Environment.NewLine}Video Extension : .mp4{Environment.NewLine}Video duration : 00:00:13{Environment.NewLine}Audio format : aac{Environment.NewLine}Video format : h264{Environment.NewLine}Aspect Ratio : 16:9{Environment.NewLine}Framerate : 25fps{Environment.NewLine}Resolution : 1280x720{Environment.NewLine}Size : 2 MB",
                output);
        }

        [Fact]
        public async Task ToTsTest()
        {
            IVideoInfo videoInfo = new VideoInfo(SampleVideoWithAudio);
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Ts);

            await videoInfo.ToTs(output);

            Assert.True(File.Exists(output));
            Assert.Equal(TimeSpan.FromSeconds(13), new VideoInfo(output).Duration);
        }

        [Fact]
        public async Task ToWebMTest()
        {
            IVideoInfo videoInfo = new VideoInfo(SampleVideoWithAudio);
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.WebM);

            await videoInfo.ToWebM(output);

            Assert.True(File.Exists(output));
            Assert.Equal(TimeSpan.FromSeconds(13), new VideoInfo(output).Duration);
        }
    }
}
