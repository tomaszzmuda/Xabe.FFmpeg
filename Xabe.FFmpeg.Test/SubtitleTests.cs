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
        [InlineData("Ass")]
        [InlineData("WebVtt", Skip = "Bad output format, why?")]
        [InlineData("Srt", Skip = "Bad output format, why?")]
        public async Task ConvertTest(string format)
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), format);

            IMediaInfo info = await MediaInfo.Get(Resources.SubtitleSrt);

            ISubtitleStream subtitleStream = info.SubtitleStreams.FirstOrDefault()
                                           .SetFormat(new SubtitleFormat(format));

            bool result = await Conversion.New()
                .AddStream(subtitleStream)
                .AddStream(info.AudioStreams.ToArray())
                .SetOutput(outputPath).Start();

            Assert.True(result);
            IMediaInfo resultInfo = await MediaInfo.Get(outputPath);
            Assert.Equal(1, resultInfo.SubtitleStreams.Count());
            ISubtitleStream resultSteam = resultInfo.SubtitleStreams.First();
            Assert.Equal(format.ToLower(), resultSteam.Format.ToLower());
        }
    }
}
