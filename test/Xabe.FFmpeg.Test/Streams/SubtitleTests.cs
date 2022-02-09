using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Test.Common.Fixtures;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class SubtitleTests : IClassFixture<StorageFixture>
    {
        private readonly StorageFixture _storageFixture;

        public SubtitleTests(StorageFixture storageFixture)
        {
            _storageFixture = storageFixture;
        }

        [Theory]
        [InlineData(Format.ass, ".ass", "ass")]
        [InlineData(Format.webvtt, ".vtt", "webvtt")]
        [InlineData(Format.srt, ".srt", "subrip")]
        public async Task ConvertTest(Format format, string extension, string expectedFormat)
        {
            var outputPath = _storageFixture.GetTempFileName(extension);

            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.SubtitleSrt);

            ISubtitleStream subtitleStream = info.SubtitleStreams.FirstOrDefault();

            IConversionResult result = await FFmpeg.Conversions.New()
                                          .AddStream(subtitleStream)
                                          .SetOutput(outputPath)
                                          .SetOutputFormat(format)
                                          .Start();

            IMediaInfo resultInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Single(resultInfo.SubtitleStreams);
            ISubtitleStream resultSteam = resultInfo.SubtitleStreams.First();
            Assert.Equal(expectedFormat, resultSteam.Codec.ToLower());
        }

        [Theory]
        [InlineData(".ass", "ass", false)]
        [InlineData(".vtt", "webvtt", false)]
        [InlineData(".srt", "subrip", false)]
        public async Task ExtractSubtitles(string extension, string expectedFormat, bool checkOutputLanguage)
        {
            var outputPath = _storageFixture.GetTempFileName(extension);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MultipleStream);

            ISubtitleStream subtitleStream = info.SubtitleStreams.FirstOrDefault(x => x.Language == "spa");
            Assert.NotNull(subtitleStream);

            IConversionResult result = await FFmpeg.Conversions.New()
                                                       .AddStream(subtitleStream)
                                                       .SetOutput(outputPath)
                                                       .Start();

            IMediaInfo resultInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Empty(resultInfo.VideoStreams);
            Assert.Empty(resultInfo.AudioStreams);
            Assert.Single(resultInfo.SubtitleStreams);
            Assert.Equal(expectedFormat, resultInfo.SubtitleStreams.First().Codec);
            if (checkOutputLanguage)
            {
                Assert.Equal("spa", resultInfo.SubtitleStreams.First().Language);
            }

            Assert.Equal(0, resultInfo.SubtitleStreams.First().Default.Value);
            Assert.Equal(0, resultInfo.SubtitleStreams.First().Forced.Value);
        }
    }
}
