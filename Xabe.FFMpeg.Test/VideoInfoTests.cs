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
        private static readonly FileInfo SampleMkvVideo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Resources", "sampleMkv.mkv"));


        [Fact]
        public void ToTs()
        {
            FileInfo fileInfo = SampleVideoWithAudio;
            var output = Path.ChangeExtension(Path.GetTempFileName(), ".ts");

            VideoInfo outputVideo = new VideoInfo(fileInfo).ToTs(output);
            Assert.True(File.Exists(outputVideo.FilePath));
            Assert.Equal(TimeSpan.FromSeconds(13), outputVideo.Duration);
        }

        [Fact]
        public void ToWebM()
        {
            FileInfo fileInfo = SampleVideoWithAudio;
            var output = Path.ChangeExtension(Path.GetTempFileName(), ".webm");

            VideoInfo outputVideo = new VideoInfo(fileInfo).ToWebM(output);
            Assert.True(File.Exists(outputVideo.FilePath));
            Assert.Equal(TimeSpan.FromSeconds(13), outputVideo.Duration);
        }

        [Fact]
        public void ToOGV()
        {
            FileInfo fileInfo = SampleVideoWithAudio;
            var output = Path.ChangeExtension(Path.GetTempFileName(), ".ogv");

            VideoInfo outputVideo = new VideoInfo(fileInfo).ToOgv(output);
            Assert.True(File.Exists(outputVideo.FilePath));
            Assert.Equal(TimeSpan.FromSeconds(13), outputVideo.Duration);
        }

        [Fact]
        public void ToMp4()
        {
            FileInfo fileInfo = SampleMkvVideo;
            var output = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");

            VideoInfo outputVideo = new VideoInfo(fileInfo).ToMp4(output);
            Assert.True(File.Exists(outputVideo.FilePath));
            Assert.Equal(TimeSpan.FromSeconds(30), outputVideo.Duration);
        }

        [Fact]
        public void AddAudio()
        {
            var videoInfo = new VideoInfo(SampleVideo);
            var output = Path.ChangeExtension(Path.GetTempFileName(), videoInfo.Extension);

            bool result = videoInfo.AddAudio(SampleAudio, output);
            Assert.True(result);
            Assert.Equal(videoInfo.Duration, new VideoInfo(output).Duration);
        }

        [Fact]
        public void ExtractAudio()
        {
            FileInfo fileInfo = SampleVideoWithAudio;
            var output = Path.ChangeExtension(Path.GetTempFileName(), ".mp3");

            bool result = new VideoInfo(fileInfo).ExtractAudio(output);
            Assert.True(result);
        }

        [Fact]
        public void ExtractVideo()
        {
            FileInfo fileInfo = SampleVideoWithAudio;
            var output = Path.ChangeExtension(Path.GetTempFileName(), fileInfo.Extension);

            bool result = new VideoInfo(fileInfo).ExtractVideo(output);
            Assert.True(result);
        }

        [Fact]
        public void JoinWith()
        {
            var videoInfo = new VideoInfo(SampleVideoWithAudio);
            var output = Path.ChangeExtension(Path.GetTempFileName(), videoInfo.Extension);

            bool result = videoInfo.JoinWith(output, new VideoInfo(SampleVideo));

            Assert.True(result);
        }

        [Fact]
        public void MkvPropertiesTest()
        {
            var videoInfo = new VideoInfo(SampleMkvVideo);

            Assert.Equal("none", videoInfo.AudioFormat);
            Assert.Equal(TimeSpan.FromSeconds(30), videoInfo.Duration);
            Assert.True(File.Exists(videoInfo.FilePath));
            Assert.Equal(".mkv", videoInfo.Extension);
            Assert.Equal(29.97, videoInfo.FrameRate);
            Assert.Equal(1080, videoInfo.Height);
            Assert.Equal(1920, videoInfo.Width);
            Assert.Equal("sampleMkv.mkv", Path.GetFileName(videoInfo.FilePath));
            Assert.False(videoInfo.IsRunning);
            Assert.Equal("16:9", videoInfo.Ratio);
        }

        [Fact]
        public void PropertiesTest()
        {
            var videoInfo = new VideoInfo(SampleVideoWithAudio);

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
            var videoInfo = new VideoInfo(SampleVideoWithAudio);
            Image snapshot = videoInfo.Snapshot();

            Assert.Equal(snapshot.Width, videoInfo.Width);
        }

        [Fact]
        public void SnapshotWithOutput()
        {
            var videoInfo = new VideoInfo(SampleVideoWithAudio);
            var output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Png); 
            Image snapshot = videoInfo.Snapshot(output);

            Assert.Equal(snapshot.Width, videoInfo.Width);
            Assert.True(File.Exists(output));
        }
    }
}
