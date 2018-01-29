using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class NewConversionTests
    {
        [Theory]
        [InlineData(Position.UpperRight)]
        [InlineData(Position.BottomRight)]
        [InlineData(Position.Left)]
        [InlineData(Position.Right)]
        [InlineData(Position.Up)]
        [InlineData(Position.BottomLeft)]
        [InlineData(Position.UpperLeft)]
        [InlineData(Position.Center)]
        [InlineData(Position.Bottom)]
        public async Task WatermarkTest(Position position)
        {
            var inputFile = await MediaInfo.Get(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await Conversion.New()
                                                    .AddStream(inputFile.VideoStreams.First())
                                                    .SetWatermark(Resources.PngSample, position)
                                                    .SetOutput(outputPath)
                                                    .Start();

            Assert.True(conversionResult.Success);
            var mediaInfo = await MediaInfo.Get(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
        }
    }
}


