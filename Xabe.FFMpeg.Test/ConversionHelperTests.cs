using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFMpeg.Enums;
using Xabe.FFMpeg.Exceptions;
using Xunit;

namespace Xabe.FFMpeg.Test
{
    public class ConversionHelperTests

    {
        [Fact]
        public async Task AddAudio()
        {
            IVideoInfo videoInfo = new VideoInfo(Resources.Mp4);
            string output = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);

            bool result = await ConversionHelper.AddAudio(Resources.Mp4.FullName, Resources.Mp3.FullName, output).Start();
            Assert.True(result);
            var outputInfo = new VideoInfo(output);
            Assert.Equal(videoInfo.Duration, outputInfo.Duration);
            //Assert.NotEqual(videoInfo.AudioFormat, "none");
        }

        [Fact]
        public async Task ExtractAudio()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp3);
            bool result = await ConversionHelper.ExtractAudio(Resources.Mp4WithAudio.FullName, output).Start();

            Assert.True(result);
            Assert.Equal("none", new VideoInfo(output).VideoFormat);
        }

        [Fact]
        public async Task ExtractVideo()
        {
            FileInfo fileInfo = Resources.Mp4WithAudio;
            string output = Path.ChangeExtension(Path.GetTempFileName(), fileInfo.Extension);

            bool result = await ConversionHelper.ExtractVideo(fileInfo.FullName, output).Start();
            Assert.True(result);
        }

        [Fact]
        public async Task JoinWith()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);

            bool result = await ConversionHelper.JoinWith(output, Resources.Mp4.FullName, Resources.Mp4WithAudio.FullName);

            Assert.True(result);
        }

        [Fact]
        public async Task SnapshotTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Png);
            bool result = await ConversionHelper.Snapshot(Resources.Mp4WithAudio.FullName, output).Start();

            Assert.True(result);
            Assert.True(File.Exists(output));
            Image snapshot = Image.FromFile(output);
            Assert.Equal(snapshot.Width, snapshot.Width);
        }

        [Fact]
        public async Task ToMp4Test()
        {
            IVideoInfo videoInfo = new VideoInfo(Resources.MkvWithAudio);
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Mp4);

            await ConversionHelper.ToMp4(Resources.MkvWithAudio.FullName, output).Start();

            Assert.True(File.Exists(output));
            Assert.Equal(TimeSpan.FromSeconds(9), new VideoInfo(output).Duration);
        }

        [Fact]
        public async Task ToOgvTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Ogv);

            await ConversionHelper.ToOgv(Resources.MkvWithAudio.FullName, output).Start();

            Assert.True(File.Exists(output));
            Assert.Equal(TimeSpan.FromSeconds(9), new VideoInfo(output).Duration);
        }

        [Fact]
        public void ToStringTest()
        {
            IVideoInfo videoInfo = new VideoInfo(Resources.Mp4WithAudio);
            string output = videoInfo.ToString();
            Assert.EndsWith(
                $"Video Name: input.mp4{Environment.NewLine}Video Extension : .mp4{Environment.NewLine}Video duration : 00:00:13{Environment.NewLine}Audio format : aac{Environment.NewLine}Video format : h264{Environment.NewLine}Aspect Ratio : 16:9{Environment.NewLine}Framerate : 25fps{Environment.NewLine}Resolution : 1280x720{Environment.NewLine}Size : 1,95 MB",
                output);
        }

        [Fact]
        public async Task ToTsTest()
        {
            IVideoInfo videoInfo = new VideoInfo(Resources.Mp4WithAudio);
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Ts);

            await ConversionHelper.ToTs(Resources.Mp4WithAudio.FullName, output).Start();

            Assert.True(File.Exists(output));
            Assert.Equal(TimeSpan.FromSeconds(13), new VideoInfo(output).Duration);
        }

        [Fact]
        public async Task ToWebMTest()
        {
            IVideoInfo videoInfo = new VideoInfo(Resources.Mp4WithAudio);
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.WebM);

            await ConversionHelper.ToWebM(Resources.Mp4WithAudio.FullName, output).Start();

            Assert.True(File.Exists(output));
            Assert.Equal(TimeSpan.FromSeconds(13), new VideoInfo(output).Duration);
        }
    }
}
