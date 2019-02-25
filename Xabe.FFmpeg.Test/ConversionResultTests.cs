using System;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFmpeg.Exceptions;
using Xabe.FFmpeg.Model;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class ConversionResultTests
    {
        [Fact]
        public async Task ConversionResultTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");

            IConversionResult result = await Conversion.ToMp4(Resources.Mp4WithAudio, outputPath)
                                             .SetPreset(Enums.ConversionPreset.UltraFast)
                                             .Start().ConfigureAwait(false);

            Assert.True(result.Success);
            Assert.NotNull(result.MediaInfo.Value);
            Assert.True(result.StartTime != DateTime.MinValue);
            Assert.True(result.EndTime != DateTime.MinValue);
            Assert.True(result.Duration > TimeSpan.FromMilliseconds(1));
        }

        [Fact]
        public async Task ConversionWithWrongInputTest2()
        {
            await Assert.ThrowsAsync<InvalidInputException>(async () => await MediaInfo.Get("Z:\\test.mp4").ConfigureAwait(false)).ConfigureAwait(false);
        }
    }
}
