using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Gif);

            bool result = await Conversion.ToGif(Resources.Mp4, output, loopCount, delay).Execute();

            Assert.True(result);
            var mediaInfo = await MediaInfo.Get(output);
            Assert.Equal(TimeSpan.FromSeconds(0), mediaInfo.Duration);
            Assert.Equal(1, mediaInfo.VideoStreams.Count());
            Assert.Equal(0, mediaInfo.AudioStreams.Count());
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.Equal("gif", videoStream.Format);
            Assert.Equal("16:9", videoStream.Ratio);
            Assert.Equal(25, videoStream.FrameRate);
            Assert.Equal(1280, videoStream.Width);
            Assert.Equal(720, videoStream.Height);
        }

        [Fact]
        public async Task AddAudio()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            bool result = await Conversion.AddAudio(Resources.Mp4, Resources.Mp3, output).Execute();

            Assert.True(result);
            IMediaInfo mediaInfo = await MediaInfo.Get(output);
            Assert.Equal(1, mediaInfo.AudioStreams.Count());
            Assert.Equal("aac", mediaInfo.AudioStreams.First().Format);
            Assert.Equal(1, mediaInfo.VideoStreams.Count());
            Assert.Equal(TimeSpan.FromSeconds(13), mediaInfo.Duration);
        }

        [Fact]
        public async Task AddSubtitleTest()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mkv);
            string input = Resources.MkvWithAudio;

            bool result = await ConversionHelper.AddSubtitle(input, output, Resources.SubtitleSrt, "pol")
                                                .Start();

            Assert.True(result);
            var outputInfo = await MediaInfo.Get(output);
            Assert.Equal(TimeSpan.FromSeconds(3071), outputInfo.Duration);
        }

        [Fact]
        public async Task BurnSubtitleTest()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            string input = Resources.Mp4;

            bool result = await ConversionHelper.BurnSubtitle(input, output, Resources.SubtitleSrt)
                                                .Start();

            Assert.True(result);
            var outputInfo = await MediaInfo.Get(output);
            Assert.Equal(TimeSpan.FromSeconds(13), outputInfo.Duration);
        }

        [Fact]
        public async Task ChangeSizeTest()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mkv);
            string input = Resources.MkvWithAudio;

            bool result = await Conversion.ChangeSize(input, output, new VideoSize(640, 360)).Execute();

            Assert.True(result);
            IMediaInfo mediaInfo = await MediaInfo.Get(output);
            Assert.Equal(1, mediaInfo.VideoStreams.Count());
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);            
            Assert.Equal(640, videoStream.Width);
            Assert.Equal(360, videoStream.Height);
        }

        [Fact]
        public async Task ExtractAudio()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp3);
            bool result = await Conversion.ExtractAudio(Resources.Mp4WithAudio, output).Execute();

            Assert.True(result);
            var mediaInfo = await MediaInfo.Get(output);
            Assert.Equal(0, mediaInfo.VideoStreams.Count());
            Assert.Equal(1, mediaInfo.AudioStreams.Count());
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            Assert.NotNull(audioStream);
            Assert.Equal("mp3", audioStream.Format);
        }

        [Fact]
        public async Task ExtractVideo()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), Path.GetExtension(Resources.Mp4WithAudio));

            bool result = await Conversion.ExtractVideo(Resources.Mp4WithAudio, output).Execute();

            Assert.True(result);
            var mediaInfo = await MediaInfo.Get(output);
            Assert.Equal(1, mediaInfo.VideoStreams.Count());
            Assert.Equal(0, mediaInfo.AudioStreams.Count());
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.Equal("h264", videoStream.Format);
        }

        public static IEnumerable<object[]> JoinFiles => new[]
        {
            new object[] {Resources.MkvWithAudio, Resources.Mp4WithAudio, 23, 1280, 720},
            new object[] {Resources.MkvWithAudio, Resources.MkvWithAudio, 19, 320, 240},
            new object[] {Resources.MkvWithAudio, Resources.Mp4, 23, 1280, 720 }
        };

        [Theory]
        [MemberData(nameof(JoinFiles))]
        public async Task JoinWith(string firstFile, string secondFile, int duration, int width, int height)
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            bool result = await ConversionHelper.Concatenate(output, firstFile, secondFile);

            Assert.True(result);
            var mediaInfo = await MediaInfo.Get(output);
            Assert.Equal(TimeSpan.FromSeconds(duration), mediaInfo.Duration);
            Assert.Equal(1, mediaInfo.VideoStreams.Count());
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.Equal(width, videoStream.Width);
            Assert.Equal(height, videoStream.Width);
        }

        [Fact]
        public async Task SnapshotTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Png);
            bool result = await Conversion.Snapshot(Resources.Mp4WithAudio, output, TimeSpan.FromSeconds(0)).Execute();

            Assert.True(result);
            Assert.True(File.Exists(output));
            Assert.Equal(1825653, (await File.ReadAllBytesAsync(output)).LongLength);
        }

        [Fact]
        public async Task SnapshotInvalidArgumentTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Png);
            await Assert.ThrowsAsync<ArgumentException>(async () => await Conversion.Snapshot(Resources.Mp4WithAudio, output, TimeSpan.FromSeconds(999)).Execute());
        }

        [Fact]
        public async Task SplitVideoTest()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            bool result = await Conversion.Split(Resources.Mp4WithAudio, output, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(8)).Execute();

            Assert.True(result);
            var mediaInfo = await MediaInfo.Get(output);
            Assert.Equal(1, mediaInfo.VideoStreams.Count());
            Assert.Equal(1, mediaInfo.AudioStreams.Count());
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("aac", audioStream.Format);
            Assert.Equal("h264", videoStream.Format);
            Assert.Equal(TimeSpan.FromSeconds(8), audioStream.Duration);
            Assert.Equal(TimeSpan.FromSeconds(8), videoStream.Duration);
            Assert.Equal(TimeSpan.FromSeconds(8), mediaInfo.Duration);
        }

        [Fact]
        public async Task ToMp4Test()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);

            bool result = await Conversion.ToMp4(Resources.MkvWithAudio, output)
                                          .Execute();

            Assert.True(result);
            var mediaInfo = await MediaInfo.Get(output);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal(1, mediaInfo.VideoStreams.Count());
            Assert.Equal(1, mediaInfo.AudioStreams.Count());
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("h264", videoStream.Format);
            Assert.Equal("aac", audioStream.Format);
        }

        [Fact]
        public async Task ToOgvTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Ogv);

            bool result = await Conversion.ToOgv(Resources.MkvWithAudio, output).Execute();

            Assert.True(result);
            var mediaInfo = await MediaInfo.Get(output);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal(1, mediaInfo.VideoStreams.Count());
            Assert.Equal(1, mediaInfo.AudioStreams.Count());
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("theora", videoStream.Format);
            Assert.Equal("vorbis", audioStream.Format);
        }

        [Fact]
        public async Task ToTsTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Ts);

            bool result = await Conversion.ToTs(Resources.Mp4WithAudio, output).Execute();

            Assert.True(result);
            var mediaInfo = await MediaInfo.Get(output);
            Assert.Equal(TimeSpan.FromSeconds(13), mediaInfo.Duration);
            Assert.Equal(1, mediaInfo.VideoStreams.Count());
            Assert.Equal(1, mediaInfo.AudioStreams.Count());
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("h264", videoStream.Format);
            Assert.Equal("aac", audioStream.Format);
        }

        [Fact]
        public async Task ToWebMTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.WebM);

            bool result = await Conversion.ToWebM(Resources.Mp4WithAudio, output).Execute();

            Assert.True(result);
            var mediaInfo = await MediaInfo.Get(output);
            Assert.Equal(TimeSpan.FromSeconds(13), mediaInfo.Duration);
            Assert.Equal(1, mediaInfo.VideoStreams.Count());
            Assert.Equal(1, mediaInfo.AudioStreams.Count());
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("vp8", videoStream.Format);
            Assert.Equal("vorbis", audioStream.Format);
        }

        [Fact]
        public async Task WatermarkTest()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp3);
            bool result = await Conversion.SetWatermark(Resources.Mp4WithAudio, output, Resources.PngSample, Position.Center).Execute();

            Assert.True(result);
            var mediaInfo = await MediaInfo.Get(output);
            Assert.Equal(1, mediaInfo.VideoStreams.Count());
            Assert.Equal(1, mediaInfo.AudioStreams.Count());
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("mp3", audioStream.Format);
            Assert.Equal("png", videoStream.Format);
        }
    }
}
