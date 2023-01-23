using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Test.Common.Fixtures;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class ConversionToFormatTests : IClassFixture<StorageFixture>
    {
        private readonly StorageFixture _storageFixture;

        public ConversionToFormatTests(StorageFixture storageFixture)
        {
            _storageFixture = storageFixture;
        }

        [Theory]
        [InlineData(1, 0, 25)]
        [InlineData(1, 1, 24.889)]
        public async Task ToGifTest(int loopCount, int delay, double framerate)
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Gif);
            _ = await (await FFmpeg.Conversions.FromSnippet.ToGif(Resources.Mp4, output, loopCount, delay))
                                             .SetPreset(ConversionPreset.UltraFast)
                                             .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(13, mediaInfo.Duration.Seconds);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Empty(mediaInfo.AudioStreams);
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.Equal("gif", videoStream.Codec);
            Assert.Equal("16:9", videoStream.Ratio);
            Assert.Equal(framerate, videoStream.Framerate);
            Assert.Equal(1280, videoStream.Width);
            Assert.Equal(720, videoStream.Height);
        }

        public static IEnumerable<object[]> JoinFiles => new[]
        {
            new object[] {Resources.MkvWithAudio, Resources.Mp4WithAudio, 23, 1280, 720, "16:9"},
            new object[] {Resources.MkvWithAudio, Resources.MkvWithAudio, 19, 320, 240, "4:3"},
            new object[] {Resources.MkvWithAudio, Resources.Mp4, 23, 1280, 720, "16:9" }
        };

        [Fact]
        public async Task ToMp4Test()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = await (await FFmpeg.Conversions.FromSnippet.ToMp4(Resources.MkvWithAudio, output))
                                          .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(9, mediaInfo.Duration.Seconds);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("h264", videoStream.Codec);
            Assert.Equal("aac", audioStream.Codec);
        }

        [Fact]
        public async Task ToOgvTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Ogv);
            _ = await (await FFmpeg.Conversions.FromSnippet.ToOgv(Resources.MkvWithAudio, output))
                                             .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(9, mediaInfo.Duration.Seconds);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("theora", videoStream.Codec);
            Assert.Equal("vorbis", audioStream.Codec);
        }

        [Fact]
        public async Task ToTsTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Ts);
            _ = await (await FFmpeg.Conversions.FromSnippet.ToTs(Resources.Mp4WithAudio, output))
                                             .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(13, mediaInfo.Duration.Seconds);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("mpeg2video", videoStream.Codec);
            Assert.Equal("mp2", audioStream.Codec);
        }

        [Fact]
        public async Task ToWebMTest()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.WebM);
            _ = await (await FFmpeg.Conversions.FromSnippet.ToWebM(Resources.Mp4WithAudio, output))
                                             .SetPreset(ConversionPreset.UltraFast)
                                             .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(13, mediaInfo.Duration.Seconds);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("vp8", videoStream.Codec);
            Assert.Equal("vorbis", audioStream.Codec);
        }

        [Fact]
        public async Task ConversionWithoutSpecificFormat()
        {
            var output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = (await Conversion.ConvertAsync(Resources.MkvWithAudio, output)).Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(9, mediaInfo.Duration.Seconds);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("h264", videoStream.Codec);
            Assert.Equal("aac", audioStream.Codec);
        }
    }
}
