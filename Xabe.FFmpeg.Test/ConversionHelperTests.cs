using System;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class ConversionHelperTests

    {
        [Theory]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        public async Task ToGifTest(int loopCount, int delay)
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Gif);

            bool result = await ConversionHelper.ToGif(Resources.Mp4.FullName, output, loopCount, delay)
                                                .Start();

            Assert.True(result);
            var mediaInfo = new MediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(0), mediaInfo.Properties.Duration);
            Assert.Equal("gif", mediaInfo.Properties.VideoFormat);
            Assert.Null(mediaInfo.Properties.AudioFormat);
            Assert.Equal("16:9", mediaInfo.Properties.Ratio);
            Assert.Equal(25, mediaInfo.Properties.FrameRate);
            Assert.Equal(1280, mediaInfo.Properties.Width);
            Assert.Equal(720, mediaInfo.Properties.Height);
        }

        [Fact]
        public async Task AddAudio()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);

            bool result = await ConversionHelper.AddAudio(Resources.Mp4.FullName, Resources.Mp3.FullName, output)
                                                .Start();

            Assert.True(result);
            var outputInfo = new MediaInfo(output);
            Assert.Equal("aac", outputInfo.Properties.AudioFormat);
            Assert.Equal(TimeSpan.FromSeconds(13), outputInfo.Properties.Duration);
        }

        [Fact]
        public async Task ChangeSizeTest()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mkv);
            string input = Resources.MkvWithAudio.FullName;

            bool result = await ConversionHelper.ChangeSize(input, output, new VideoSize(640, 360))
                                                .Start();

            Assert.True(result);
            var outputInfo = new MediaInfo(output);
            Assert.Equal(640, outputInfo.Properties.Width);
            Assert.Equal(360, outputInfo.Properties.Height);
        }

        [Fact]
        public async Task ExtractAudio()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp3);
            bool result = await ConversionHelper.ExtractAudio(Resources.Mp4WithAudio.FullName, output)
                                                .Start();

            Assert.True(result);
            var outputInfo = new MediaInfo(output);
            Assert.Equal("mp3", outputInfo.Properties.AudioFormat);
            Assert.Null(outputInfo.Properties.VideoFormat);
        }

        [Fact]
        public async Task ExtractVideo()
        {
            FileInfo fileInfo = Resources.Mp4WithAudio;
            string output = Path.ChangeExtension(Path.GetTempFileName(), fileInfo.Extension);

            bool result = await ConversionHelper.ExtractVideo(fileInfo.FullName, output)
                                                .Start();

            Assert.True(result);
            var outputInfo = new MediaInfo(output);
            Assert.Equal("h264", outputInfo.Properties.VideoFormat);
            Assert.Null(outputInfo.Properties.AudioFormat);
        }

        [Fact]
        public async Task JoinWith()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);

            bool result = await ConversionHelper.Concatenate(output, Resources.MkvWithAudio.FullName, Resources.Mp4WithAudio.FullName);

            Assert.True(result);
            var outputInfo = new MediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(23), outputInfo.Properties.Duration);
            Assert.Equal("h264", outputInfo.Properties.VideoFormat);
            Assert.Equal("aac", outputInfo.Properties.AudioFormat);
        }

        [Fact]
        public async Task SnapshotTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Png);
            bool result = await ConversionHelper.Snapshot(Resources.Mp4WithAudio.FullName, output)
                                                .Start();

            Assert.True(result);
            Assert.True(File.Exists(output));
            Assert.Equal(1890492, (await File.ReadAllBytesAsync(output)).LongLength);
        }

        [Fact]
        public async Task SplitVideoTest()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
            bool result = await ConversionHelper.Split(Resources.Mp4WithAudio.FullName, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(8), output)
                                                .Start();

            Assert.True(result);
            var outputInfo = new MediaInfo(output);
            Assert.Equal("aac", outputInfo.Properties.AudioFormat);
            Assert.Equal("h264", outputInfo.Properties.VideoFormat);
            Assert.Equal(TimeSpan.FromSeconds(8), outputInfo.Properties.AudioDuration);
            Assert.Equal(TimeSpan.FromSeconds(8), outputInfo.Properties.VideoDuration);
            Assert.Equal(TimeSpan.FromSeconds(8), outputInfo.Properties.Duration);
        }

        [Fact]
        public async Task ToMp4Test()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Mp4);

            bool result = await ConversionHelper.ToMp4(Resources.MkvWithAudio.FullName, output)
                                                .Start();

            Assert.True(result);
            var outputInfo = new MediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(9), outputInfo.Properties.Duration);
            Assert.Equal("h264", outputInfo.Properties.VideoFormat);
            Assert.Equal("aac", outputInfo.Properties.AudioFormat);
        }

        [Fact]
        public async Task ToOgvTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Ogv);

            bool result = await ConversionHelper.ToOgv(Resources.MkvWithAudio.FullName, output)
                                                .Start();

            Assert.True(result);
            var outputInfo = new MediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(9), outputInfo.Properties.Duration);
            Assert.Equal("theora", outputInfo.Properties.VideoFormat);
            Assert.Equal("vorbis", outputInfo.Properties.AudioFormat);
        }

        [Fact]
        public async Task ToTsTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Ts);

            bool result = await ConversionHelper.ToTs(Resources.Mp4WithAudio.FullName, output)
                                                .Start();

            Assert.True(result);
            var outputInfo = new MediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(13), outputInfo.Properties.Duration);
            Assert.Equal("h264", outputInfo.Properties.VideoFormat);
            Assert.Equal("aac", outputInfo.Properties.AudioFormat);
        }

        [Fact]
        public async Task ToWebMTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.WebM);

            bool result = await ConversionHelper.ToWebM(Resources.Mp4WithAudio.FullName, output)
                                                .Start();

            Assert.True(result);
            var outputInfo = new MediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(13), outputInfo.Properties.Duration);
            Assert.Equal("vp8", outputInfo.Properties.VideoFormat);
            Assert.Equal("vorbis", outputInfo.Properties.AudioFormat);
        }

        [Fact]
        public async Task WatermarkTest()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp3);
            bool result = await ConversionHelper.SetWatermark(Resources.Mp4WithAudio.FullName, Resources.PngSample.FullName, Position.Center, output)
                                                .Start();

            Assert.True(result);
            var outputInfo = new MediaInfo(output);
            Assert.Equal("mp3", outputInfo.Properties.AudioFormat);
            Assert.Equal("png", outputInfo.Properties.VideoFormat);
        }
    }
}
