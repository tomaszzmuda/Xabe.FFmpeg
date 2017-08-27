using System;
using System.Drawing;
using System.IO;
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
        public void AddAudio()
        {
            IVideoInfo videoInfo = new VideoInfo(SampleVideo);
            string output = Path.ChangeExtension(Path.GetTempFileName(), videoInfo.Extension);

            bool result = videoInfo.AddAudio(SampleAudio, output);
            Assert.True(result);
            Assert.Equal(videoInfo.Duration, new VideoInfo(output).Duration);
        }

        [Fact]
        public void ExtractAudio()
        {
            FileInfo fileInfo = SampleVideoWithAudio;
            string output = Path.ChangeExtension(Path.GetTempFileName(), ".mp3");

            bool result = new VideoInfo(fileInfo).ExtractAudio(output);
            Assert.True(result);
        }

        [Fact]
        public void ExtractVideo()
        {
            FileInfo fileInfo = SampleVideoWithAudio;
            string output = Path.ChangeExtension(Path.GetTempFileName(), fileInfo.Extension);

            bool result = new VideoInfo(fileInfo).ExtractVideo(output);
            Assert.True(result);
        }

        [Fact]
        public void JoinWith()
        {
            IVideoInfo videoInfo = new VideoInfo(SampleVideoWithAudio);
            string output = Path.ChangeExtension(Path.GetTempFileName(), videoInfo.Extension);

            bool result = videoInfo.JoinWith(output, new VideoInfo(SampleVideo));

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
        public void Snapshot()
        {
            IVideoInfo videoInfo = new VideoInfo(SampleVideoWithAudio);
            Image snapshot = videoInfo.Snapshot();

            Assert.Equal(snapshot.Width, videoInfo.Width);
        }

        [Fact]
        public void SnapshotWithOutput()
        {
            IVideoInfo videoInfo = new VideoInfo(SampleVideoWithAudio);
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Png);
            Image snapshot = videoInfo.Snapshot(output);

            Assert.Equal(snapshot.Width, videoInfo.Width);
            Assert.True(File.Exists(output));
        }
    }
}
