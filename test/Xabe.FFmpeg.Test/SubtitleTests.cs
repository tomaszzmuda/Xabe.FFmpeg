using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class SubtitleTests
    {
        [Theory]
        [InlineData("Ass", "ass", "ass")]
        [InlineData("WebVTT", "vtt", "webvtt")]
        [InlineData("Srt", "srt", "subrip")]
        public async Task ConvertTest(string format, string extension, string expectedFormat)
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), extension);

            IMediaInfo info = await MediaInfo.Get(Resources.SubtitleSrt).ConfigureAwait(false);

            ISubtitleStream subtitleStream = info.SubtitleStreams.FirstOrDefault()
                                                 .SetFormat(new SubtitleFormat(format));

            IConversionResult result = await Conversion.New()
                                          .AddStream(subtitleStream)
                                          .SetOutput(outputPath)
                                          .Start().ConfigureAwait(false);

            Assert.True(result.Success);
            IMediaInfo resultInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
            Assert.Single(resultInfo.SubtitleStreams);
            ISubtitleStream resultSteam = resultInfo.SubtitleStreams.First();
            Assert.Equal(expectedFormat, resultSteam.Format.ToLower());
        }

        [Theory]
        [InlineData("ass", "ass", false)]
        [InlineData("vtt", "webvtt", false)]
        [InlineData("srt", "subrip", false)]
        public async Task ExtractSubtitles(string format, string expectedFormat, bool checkOutputLanguage)
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), format);
            IMediaInfo info = await MediaInfo.Get(Resources.MultipleStream).ConfigureAwait(false);

            ISubtitleStream subtitleStream = info.SubtitleStreams.FirstOrDefault(x => x.Language == "spa");
            Assert.NotNull(subtitleStream);

            IConversionResult result = await Conversion.New()
                                                       .AddStream(subtitleStream)
                                                       .SetOutput(outputPath)
                                                       .Start().ConfigureAwait(false);

            Assert.True(result.Success);
            IMediaInfo resultInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
            Assert.Empty(resultInfo.VideoStreams);
            Assert.Empty(resultInfo.AudioStreams);
            Assert.Single(resultInfo.SubtitleStreams);
            Assert.Equal(expectedFormat, resultInfo.SubtitleStreams.First().Format);
            if (checkOutputLanguage)
            {
                Assert.Equal("spa", resultInfo.SubtitleStreams.First().Language);
            }
            Assert.Equal(0, resultInfo.SubtitleStreams.First().Default.Value);
            Assert.Equal(0, resultInfo.SubtitleStreams.First().Forced.Value);
        }
    }
}
