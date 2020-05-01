using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class SubtitleSnippetsTests
    {
        [Fact]
        public async Task AddSubtitleTest()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mkv);
            string input = Resources.MkvWithAudio;

            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.AddSubtitle(input, output, Resources.SubtitleSrt))
                                             .Start();

            IMediaInfo outputInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(3071), outputInfo.Duration);
            Assert.Single(outputInfo.SubtitleStreams);
            Assert.Single(outputInfo.VideoStreams);
            Assert.Single(outputInfo.AudioStreams);
        }

        [Fact]
        public async Task AddSubtitleWithLanguageTest()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mkv);
            string input = Resources.MkvWithAudio;

            var language = "pol";
            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.AddSubtitle(input, output, Resources.SubtitleSrt, language))
                                             .SetPreset(ConversionPreset.UltraFast)
                                             .Start();


            IMediaInfo outputInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(3071), outputInfo.Duration);
            Assert.Single(outputInfo.SubtitleStreams);
            Assert.Single(outputInfo.VideoStreams);
            Assert.Single(outputInfo.AudioStreams);
            Assert.Equal(language, outputInfo.SubtitleStreams.First()
                                             .Language);
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
