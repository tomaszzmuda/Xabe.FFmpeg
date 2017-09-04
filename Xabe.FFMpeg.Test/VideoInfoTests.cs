using System;
using System.IO;
using Xabe.FFMpeg.Enums;
using Xunit;

namespace Xabe.FFMpeg.Test
{
    public class VideoInfoTests

    {
        [Fact]
        public void AudioPopertiesTest()
        {
            IVideoInfo videoInfo = new VideoInfo(Resources.Mp3);

            Assert.True(File.Exists(videoInfo.FileInfo.FullName));
            Assert.Equal(Extensions.Mp3, videoInfo.FileInfo.Extension);
            Assert.Equal("audio.mp3", videoInfo.FileInfo.Name);

            Assert.Equal("mp3", videoInfo.VideoProperties.AudioFormat);
            Assert.Equal(TimeSpan.FromSeconds(13), videoInfo.VideoProperties.AudioDuration);

            Assert.Equal(0, videoInfo.VideoProperties.FrameRate);
            Assert.Equal(0, videoInfo.VideoProperties.Height);
            Assert.Equal(0, videoInfo.VideoProperties.Width);
            Assert.Null(videoInfo.VideoProperties.Ratio);
            Assert.Null(videoInfo.VideoProperties.VideoFormat);
            Assert.Equal(TimeSpan.FromSeconds(0), videoInfo.VideoProperties.VideoDuration);

            Assert.Equal(TimeSpan.FromSeconds(13), videoInfo.VideoProperties.Duration);
            Assert.Equal(216916, videoInfo.VideoProperties.Size);
        }

        [Fact]
        public void MkvPropertiesTest()
        {
            IVideoInfo videoInfo = new VideoInfo(Resources.MkvWithAudio);

            Assert.True(File.Exists(videoInfo.FileInfo.FullName));
            Assert.Equal(Extensions.Mkv, videoInfo.FileInfo.Extension);
            Assert.Equal("SampleVideo_360x240_1mb.mkv", videoInfo.FileInfo.Name);

            Assert.Equal("aac", videoInfo.VideoProperties.AudioFormat);
            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.VideoProperties.AudioDuration);

            Assert.Equal(25, videoInfo.VideoProperties.FrameRate);
            Assert.Equal(240, videoInfo.VideoProperties.Height);
            Assert.Equal(320, videoInfo.VideoProperties.Width);
            Assert.Equal("4:3", videoInfo.VideoProperties.Ratio);
            Assert.Equal("h264", videoInfo.VideoProperties.VideoFormat);
            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.VideoProperties.VideoDuration);

            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.VideoProperties.Duration);
            Assert.Equal(1055721, videoInfo.VideoProperties.Size);
        }

        [Fact]
        public void PropertiesTest()
        {
            IVideoInfo videoInfo = new VideoInfo(Resources.Mp4WithAudio);

            Assert.True(File.Exists(videoInfo.FileInfo.FullName));
            Assert.Equal(Extensions.Mp4, videoInfo.FileInfo.Extension);
            Assert.Equal("input.mp4", videoInfo.FileInfo.Name);

            Assert.Equal("aac", videoInfo.VideoProperties.AudioFormat);
            Assert.Equal(TimeSpan.FromSeconds(13), videoInfo.VideoProperties.AudioDuration);

            Assert.Equal(25, videoInfo.VideoProperties.FrameRate);
            Assert.Equal(720, videoInfo.VideoProperties.Height);
            Assert.Equal(1280, videoInfo.VideoProperties.Width);
            Assert.Equal("16:9", videoInfo.VideoProperties.Ratio);
            Assert.Equal("h264", videoInfo.VideoProperties.VideoFormat);
            Assert.Equal(TimeSpan.FromSeconds(13), videoInfo.VideoProperties.VideoDuration);

            Assert.Equal(TimeSpan.FromSeconds(13), videoInfo.VideoProperties.Duration);
            Assert.Equal(2107842, videoInfo.VideoProperties.Size);
        }

        [Fact]
        public void ToStringTest()
        {
            IVideoInfo videoInfo = new VideoInfo(Resources.Mp4WithAudio);
            string output = videoInfo.ToString();
            string expectedOutput =
                $"Video name: input.mp4{Environment.NewLine}Video extension : .mp4{Environment.NewLine}Video duration : 00:00:13{Environment.NewLine}Video format : h264{Environment.NewLine}Audio format : aac{Environment.NewLine}Audio duration : 00:00:13{Environment.NewLine}Aspect Ratio : 16:9{Environment.NewLine}Framerate : 16:9 fps{Environment.NewLine}Resolution : 1280 x 720{Environment.NewLine}Size : 2107842 b";
            Assert.EndsWith(expectedOutput, output);
        }
    }
}
