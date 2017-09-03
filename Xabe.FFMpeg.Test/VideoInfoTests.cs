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
            Assert.Equal(2, videoInfo.AudioSize);

            Assert.Equal(25, videoInfo.FrameRate);
            Assert.Equal(240, videoInfo.Height);
            Assert.Equal(320, videoInfo.Width);
            Assert.Equal("4:3", videoInfo.Ratio);
            Assert.Equal("h264", videoInfo.VideoFormat);
            Assert.Equal(1, videoInfo.VideoSize);
            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.VideoDuration);

            Assert.Equal(TimeSpan.FromSeconds(9), videoInfo.Duration);
            Assert.Equal(0, videoInfo.Size);
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
            Assert.Equal(5158365, videoInfo.AudioSize);

            Assert.Equal(25, videoInfo.FrameRate);
            Assert.Equal(720, videoInfo.Height);
            Assert.Equal(1280, videoInfo.Width);
            Assert.Equal("16:9", videoInfo.Ratio);
            Assert.Equal("h264", videoInfo.VideoFormat);
            Assert.Equal(11218883, videoInfo.VideoSize);
            Assert.Equal(TimeSpan.FromSeconds(13), videoInfo.VideoDuration);

            Assert.Equal(TimeSpan.FromSeconds(13), videoInfo.Duration);
            Assert.Equal(16377248, videoInfo.Size);
        }

        [Fact]
        public void ToStringTest()
        {
            IVideoInfo videoInfo = new VideoInfo(Resources.Mp4WithAudio);
            string output = videoInfo.ToString();
            string expectedOutput =
                $"Video name: input.mp4{Environment.NewLine}Video extension : .mp4{Environment.NewLine}Video duration : 00:00:13{Environment.NewLine}Video format : h264{Environment.NewLine}Video size : 11218883 b{Environment.NewLine}Audio format : aac{Environment.NewLine}Audio size : 5158365 b{Environment.NewLine}Audio duration : 00:00:13{Environment.NewLine}Aspect Ratio : 16:9{Environment.NewLine}Framerate : 16:9 fps{Environment.NewLine}Resolution : 1280 x 720{Environment.NewLine}Size : 16377248 b";
            Assert.EndsWith(expectedOutput, output);
        }
    }
}
