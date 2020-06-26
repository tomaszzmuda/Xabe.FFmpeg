using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class VideoSnippetsTests
    {
        public static IEnumerable<object[]> JoinFiles => new[]
        {
            new object[] {Resources.MkvWithAudio, Resources.Mp4WithAudio, 23, 1280, 720, "16:9"},
            new object[] {Resources.MkvWithAudio, Resources.MkvWithAudio, 19, 320, 240, "4:3"},
            new object[] {Resources.MkvWithAudio, Resources.Mp4, 23, 1280, 720, "16:9" }
        };

        [Theory]
        [MemberData(nameof(JoinFiles))]
        public async Task Concatenate_Test(string firstFile, string secondFile, int duration, int width, int height, string ratio)
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.Concatenate(output, firstFile, secondFile)).Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(duration), mediaInfo.Duration);
            Assert.Single(mediaInfo.VideoStreams);
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.Equal(width, videoStream.Width);
            Assert.Equal(height, videoStream.Height);
            Assert.Contains($"-aspect {ratio}", result.Arguments);
        }

        [Fact]
        public async Task ChangeSizeTest()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mkv);
            string input = Resources.MkvWithAudio;

            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.ChangeSize(input, output, 640, 360))
                                             .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Single(mediaInfo.VideoStreams);
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.Equal(640, videoStream.Width);
            Assert.Equal(360, videoStream.Height);
        }

        [Fact]
        public async Task ExtractVideo()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), Path.GetExtension(Resources.Mp4WithAudio));

            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.ExtractVideo(Resources.Mp4WithAudio, output))
                                             .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Empty(mediaInfo.AudioStreams);
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.Equal("h264", videoStream.Codec);
        }

        [Fact]
        public async Task SnapshotInvalidArgumentTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Png);
            await Assert.ThrowsAsync<ArgumentException>(async () => await (await FFmpeg.Conversions.FromSnippet.Snapshot(Resources.Mp4WithAudio, output, TimeSpan.FromSeconds(999)))
                                                                                    .Start());
        }

        [Theory]
        [InlineData(FileExtensions.Png, 1825653)]
        [InlineData(FileExtensions.Jpg, 84461)]
        public async Task SnapshotTest(string extension, long expectedLength)
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + extension);
            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.Snapshot(Resources.Mp4WithAudio, output, TimeSpan.FromSeconds(0)))
                                             .Start();


            Assert.True(File.Exists(output));
            Assert.Equal(expectedLength, (await File.ReadAllBytesAsync(output)).LongLength);
        }

        [Fact]
        public async Task SplitVideoTest()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.Split(Resources.Mp4WithAudio, output, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(8)))
                                             .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("aac", audioStream.Codec);
            Assert.Equal("h264", videoStream.Codec);
            Assert.Equal(TimeSpan.FromSeconds(8), audioStream.Duration);
            Assert.Equal(TimeSpan.FromSeconds(8), videoStream.Duration);
            Assert.Equal(TimeSpan.FromSeconds(8), mediaInfo.Duration);
        }

        [Fact]
        public async Task WatermarkTest()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.SetWatermark(Resources.Mp4WithAudio, output, Resources.PngSample, Position.Center))
                                             .Start();


            Assert.Contains("overlay=", result.Arguments);
            Assert.Contains(Resources.Mp4WithAudio, result.Arguments);
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("aac", audioStream.Codec);
            Assert.Equal("h264", videoStream.Codec);
        }

        [Theory]
        [InlineData("https://bitdash-a.akamaihd.net/content/MI201109210084_1/m3u8s/f08e80da-bf1d-4e3d-8899-f0f6155f6efa.m3u8", true)]
        [InlineData("http://www.bitdash-a.akamaihd.net/content/MI201109210084_1/m3u8s/f08e80da-bf1d-4e3d-8899-f0f6155f6efa.m3u8", false)]
        public async Task SaveM3U8Stream(string input, bool success)
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), "mkv");

            var exception = await Record.ExceptionAsync(async() => await (await FFmpeg.Conversions.FromSnippet.SaveM3U8Stream(new Uri(input), output, TimeSpan.FromSeconds(1)))
                                                                    .Start());

            Assert.Equal(success, exception == null);
        }

        [Fact]
        public async Task BasicConversion_InputFileWithSubtitles_SkipSubtitles()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);

            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.Convert(Resources.MkvWithSubtitles, output)).Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("h264", videoStream.Codec);
            Assert.Equal("aac", audioStream.Codec);
            Assert.Empty(mediaInfo.SubtitleStreams);
            Assert.Equal(25, videoStream.Framerate);
        }

        [Fact]
        public async Task BasicConversion_SloMoVideo_CorrectFramerate()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);

            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.Convert(Resources.SloMoMp4, output)).Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(3), mediaInfo.Duration);
            Assert.Single(mediaInfo.VideoStreams);
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.Equal("h264", videoStream.Codec);
            Assert.Empty(mediaInfo.SubtitleStreams);
            Assert.Equal(116.244, videoStream.Framerate);
        }

        [Fact]
        public async Task BasicConversion_InputFileWithMultipleStreams_CorrectResult()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);

            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.Convert(Resources.MultipleStream, output)).Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(46), mediaInfo.Duration);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Equal(2, mediaInfo.AudioStreams.Count());
            Assert.Empty(mediaInfo.SubtitleStreams);
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.Equal(24, videoStream.Framerate);
        }

        [Fact(Skip = "The RTSP stream is not valid anymore")]
        public async Task Rtsp_GotTwoStreams_SaveEverything()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            var mediaInfo = await FFmpeg.GetMediaInfo("rtsp://wowzaec2demo.streamlock.net/vod/mp4:BigBuckBunny_115k.mov");

            var conversionResult = await FFmpeg.Conversions.New().AddStream(mediaInfo.Streams).SetInputTime(TimeSpan.FromSeconds(3)).SetOutput(output).Start();

            IMediaInfo result = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(9 * 60 + 56), mediaInfo.Duration);
            Assert.Single(result.VideoStreams);
            Assert.Single(result.AudioStreams);
            Assert.Empty(result.SubtitleStreams);
            Assert.Equal("h264", result.VideoStreams.First().Codec);
            Assert.Equal("aac", result.AudioStreams.First().Codec);
        }
    }
}
