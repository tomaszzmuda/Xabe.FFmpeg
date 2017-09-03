using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFMpeg.Enums;
using Xabe.FFMpeg.Exceptions;
using Xunit;

namespace Xabe.FFMpeg.Test
{
    public class VideoInfoTests

    {
        [Fact]
        public void MkvPropertiesTest()
        {
            IVideoInfo videoInfo = new VideoInfo(Resources.MkvWithAudio);

            Assert.True(File.Exists(videoInfo.FilePath));
            Assert.Equal(Extensions.Mkv, videoInfo.Extension);
            Assert.Equal("SampleVideo_360x240_1mb.mkv", Path.GetFileName(videoInfo.FilePath));

            Assert.Equal("aac", videoInfo.AudioFormat);
            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.AudioDuration);

            Assert.Equal(25, videoInfo.FrameRate);
            Assert.Equal(240, videoInfo.Height);
            Assert.Equal(320, videoInfo.Width);
            Assert.Equal("4:3", videoInfo.Ratio);
            Assert.Equal("h264", videoInfo.VideoFormat);
            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.VideoDuration);

            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.Duration);
            Assert.Equal(1055721, videoInfo.Size);
        }

        [Fact]
        public void PropertiesTest()
        {
            IVideoInfo videoInfo = new VideoInfo(Resources.Mp4WithAudio);

            Assert.True(File.Exists(videoInfo.FilePath));
            Assert.Equal(Extensions.Mp4, videoInfo.Extension);
            Assert.Equal("input.mp4", Path.GetFileName(videoInfo.FilePath));

            Assert.Equal("aac", videoInfo.AudioFormat);
            Assert.Equal(TimeSpan.FromSeconds(13), videoInfo.AudioDuration);

            Assert.Equal(25, videoInfo.FrameRate);
            Assert.Equal(720, videoInfo.Height);
            Assert.Equal(1280, videoInfo.Width);
            Assert.Equal("16:9", videoInfo.Ratio);
            Assert.Equal("h264", videoInfo.VideoFormat);
            Assert.Equal(TimeSpan.FromSeconds(13), videoInfo.VideoDuration);

            Assert.Equal(TimeSpan.FromSeconds(13), videoInfo.Duration);
            Assert.Equal(2107842, videoInfo.Size);
        }

        [Fact]
        public void AudioPopertiesTest()
        {
            IVideoInfo videoInfo = new VideoInfo(Resources.Mp3);

            Assert.True(File.Exists(videoInfo.FilePath));
            Assert.Equal(Extensions.Mp3, videoInfo.Extension);
            Assert.Equal("audio.mp3", Path.GetFileName(videoInfo.FilePath));

            Assert.Equal("mp3", videoInfo.AudioFormat);
            Assert.Equal(TimeSpan.FromSeconds(13), videoInfo.AudioDuration);

            Assert.Equal(0, videoInfo.FrameRate);
            Assert.Equal(0, videoInfo.Height);
            Assert.Equal(0, videoInfo.Width);
            Assert.Null(videoInfo.Ratio);
            Assert.Equal("none", videoInfo.VideoFormat);
            Assert.Equal(TimeSpan.FromSeconds(0), videoInfo.VideoDuration);

            Assert.Equal(TimeSpan.FromSeconds(13), videoInfo.Duration);
            Assert.Equal(216916, videoInfo.Size);
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
