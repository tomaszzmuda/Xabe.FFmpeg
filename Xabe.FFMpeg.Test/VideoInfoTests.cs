using System;
using System.Drawing;
using System.IO;
using Xabe.FFMpeg.Enums;
using Xunit;

namespace Xabe.FFMpeg.Test
{
    public class VideoInfoTests
    {
        public static readonly FileInfo SampleVideoWithAudio = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Resources", "input.mp4"));
        public static readonly FileInfo SampleAudio = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Resources", "audio.mp3"));
        public static readonly FileInfo SampleVideo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Resources", "mute.mp4"));

        [Theory]
        //[InlineData(VideoType.Ogv, ".ogv")] Commented due to lack encoder in CI environment
        [InlineData(VideoType.Ts, ".ts")]
        [InlineData(VideoType.Mp4, ".mp4")]
        public void Convert(VideoType videoType, string extension)
        {
            FileInfo fileInfo = SampleVideoWithAudio;
            var output = new FileInfo(Path.ChangeExtension(Path.GetTempFileName(), extension));
            output.Delete();

            VideoInfo outputVideo = new VideoInfo(fileInfo).ConvertTo(videoType, output, Speed.UltraFast);
            Assert.True(outputVideo.Exists);
            Assert.Equal(TimeSpan.FromSeconds(13), outputVideo.Duration);
        }

        [Fact]
        public void AddAudio()
        {
            var videoInfo = new VideoInfo(SampleVideo);
            var output = new FileInfo(Path.ChangeExtension(Path.GetTempFileName(), videoInfo.Extension));

            bool result = videoInfo.AddAudio(SampleAudio, output);
            Assert.True(result);
            Assert.Equal(videoInfo.Duration, new VideoInfo(output).Duration);
        }

        [Fact]
        public void ExtractAudio()
        {
            FileInfo fileInfo = SampleVideoWithAudio;
            var output = new FileInfo(Path.ChangeExtension(Path.GetTempFileName(), ".mp3"));

            bool result = new VideoInfo(fileInfo).ExtractAudio(output);
            Assert.True(result);
        }

        [Fact]
        public void ExtractVideo()
        {
            FileInfo fileInfo = SampleVideoWithAudio;
            var output = new FileInfo(Path.ChangeExtension(Path.GetTempFileName(), fileInfo.Extension));

            bool result = new VideoInfo(fileInfo).ExtractVideo(output);
            Assert.True(result);
        }

        [Fact]
        public void JoinWith()
        {
            var videoInfo = new VideoInfo(SampleVideoWithAudio);
            var output = new FileInfo(Path.ChangeExtension(Path.GetTempFileName(), videoInfo.Extension));

            bool result = videoInfo.JoinWith(output, new VideoInfo(SampleVideo));

            Assert.True(result);
        }

        [Fact]
        public void PropertiesTest()
        {
            var videoInfo = new VideoInfo(SampleVideoWithAudio);

            Assert.Equal("aac", videoInfo.AudioFormat);
            Assert.Equal(TimeSpan.FromSeconds(13), videoInfo.Duration);
            Assert.True(videoInfo.Exists);
            Assert.Equal(".mp4", videoInfo.Extension);
            Assert.Equal(25, videoInfo.FrameRate);
            Assert.Equal(720, videoInfo.Height);
            Assert.Equal(1280, videoInfo.Width);
            Assert.Equal("input.mp4", videoInfo.Name);
            Assert.False(videoInfo.IsReadOnly);
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
            var output = new FileInfo(Path.GetTempFileName());
            Image snapshot = videoInfo.Snapshot(output);

            Assert.Equal(snapshot.Width, videoInfo.Width);
            Assert.True(output.Exists);
        }
    }
}
