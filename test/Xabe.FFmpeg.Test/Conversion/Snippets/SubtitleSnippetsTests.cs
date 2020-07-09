using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Streams.SubtitleStream;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class SubtitleSnippetsTests
    {
        [Theory]
        [InlineData(SubtitleCodec.webvtt)]
        [InlineData(SubtitleCodec.subrip)]
        [InlineData(SubtitleCodec.copy)]
        public async Task AddSubtitleTest(SubtitleCodec subtitleCodec)
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mkv);
            string input = Resources.MkvWithAudio;

            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.AddSubtitle(input, output, Resources.SubtitleSrt, subtitleCodec))
                                             .Start();

            IMediaInfo outputInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(3071), outputInfo.Duration);
            Assert.Single(outputInfo.SubtitleStreams);
            Assert.Single(outputInfo.VideoStreams);
            Assert.Single(outputInfo.AudioStreams);

            if (subtitleCodec.ToString() == "copy")
                Assert.Equal("subrip", outputInfo.SubtitleStreams.First().Codec);
            else
                Assert.Equal(subtitleCodec.ToString(), outputInfo.SubtitleStreams.First().Codec);
        }

        [Theory]
        [InlineData(SubtitleCodec.webvtt)]
        [InlineData(SubtitleCodec.subrip)]
        [InlineData(SubtitleCodec.copy)]
        public async Task AddSubtitleWithLanguageTest(SubtitleCodec subtitleCodec)
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mkv);
            string input = Resources.MkvWithAudio;

            var language = "pol";
            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.AddSubtitle(input, output, Resources.SubtitleSrt, subtitleCodec, language))
                                             .SetPreset(ConversionPreset.UltraFast)
                                             .Start();


            IMediaInfo outputInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(3071), outputInfo.Duration);
            Assert.Single(outputInfo.SubtitleStreams);
            Assert.Single(outputInfo.VideoStreams);
            Assert.Single(outputInfo.AudioStreams);
            Assert.Equal(language, outputInfo.SubtitleStreams.First().Language);

            if(subtitleCodec.ToString() == "copy")
                Assert.Equal("subrip", outputInfo.SubtitleStreams.First().Codec);
            else 
                Assert.Equal(subtitleCodec.ToString(), outputInfo.SubtitleStreams.First().Codec);
        }

        [Fact]
        public async Task BurnSubtitleTest()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            string input = Resources.Mp4;

            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.BurnSubtitle(input, output, Resources.SubtitleSrt))
                                             .SetPreset(ConversionPreset.UltraFast)
                                             .Start();

            IMediaInfo outputInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(13), outputInfo.Duration);
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
        }
    }
}
