using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class ConversionToFormatTests

    {
        [Theory]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        public async Task ToGifTest(int loopCount, int delay)
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Gif);

            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.ToGif(Resources.Mp4, output, loopCount, delay))
                                             .SetPreset(ConversionPreset.UltraFast)
                                             .Start();

            IMediaInfo mediaInfo = await MediaInfo.Get(output);
            Assert.Equal(TimeSpan.FromSeconds(13), mediaInfo.Duration);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Empty(mediaInfo.AudioStreams);
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.Equal("gif", videoStream.Codec);
            Assert.Equal("16:9", videoStream.Ratio);
            Assert.Equal(25, videoStream.Framerate);
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
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);

            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.ToMp4(Resources.MkvWithAudio, output))
                                          .Start();


            IMediaInfo mediaInfo = await MediaInfo.Get(output);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
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
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Ogv);

            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.ToOgv(Resources.MkvWithAudio, output))
                                             .Start();


            IMediaInfo mediaInfo = await MediaInfo.Get(output);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
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
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Ts);

            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.ToTs(Resources.Mp4WithAudio, output))
                                             .Start();


            IMediaInfo mediaInfo = await MediaInfo.Get(output);
            Assert.Equal(TimeSpan.FromSeconds(13), mediaInfo.Duration);
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
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.WebM);

            IConversionResult result = await (await Conversion.ToWebM(Resources.Mp4WithAudio, output))
                                             .SetPreset(ConversionPreset.UltraFast)
                                             .Start();


            IMediaInfo mediaInfo = await MediaInfo.Get(output);
            Assert.Equal(TimeSpan.FromSeconds(13), mediaInfo.Duration);
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
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);

            IConversionResult result = await (await Conversion.Convert(Resources.MkvWithAudio, output)).Start();


            IMediaInfo mediaInfo = await MediaInfo.Get(output);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
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
