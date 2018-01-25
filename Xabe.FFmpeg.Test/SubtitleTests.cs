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

            IMediaInfo info = await MediaInfo.Get(Resources.SubtitleSrt);

            ISubtitleStream subtitleStream = info.SubtitleStreams.FirstOrDefault()
                                                 .SetFormat(new SubtitleFormat(format));

            IConversionResult result = await Conversion.New()
                                          .AddStream(subtitleStream)
                                          .SetOutput(outputPath)
                                          .Start();

            Assert.True(result.Success);
            IMediaInfo resultInfo = await MediaInfo.Get(outputPath);
            Assert.Equal(1, resultInfo.SubtitleStreams.Count());
            ISubtitleStream resultSteam = resultInfo.SubtitleStreams.First();
            Assert.Equal(expectedFormat, resultSteam.Format.ToLower());
        }
    }
}
