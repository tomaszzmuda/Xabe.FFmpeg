using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFmpeg.Exceptions;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class ConversionResultTests
    {
        [Theory]
        [InlineData(null)]
        //[InlineData(ProcessPriorityClass.High)]
        [InlineData(ProcessPriorityClass.BelowNormal)]
        public async Task ConversionResultTest(ProcessPriorityClass? priority)
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");

            IConversionResult result = await Conversion.ToMp4(Resources.Mp4WithAudio, outputPath)
                                             .SetPreset(ConversionPreset.UltraFast)
                                             .SetPriority(priority)
                                             .Start();

            Assert.True(result.Success);
            Assert.NotNull(result.MediaInfo.Value);
            Assert.True(result.StartTime != DateTime.MinValue);
            Assert.True(result.EndTime != DateTime.MinValue);
            Assert.True(result.Duration > TimeSpan.FromMilliseconds(1));
        }

        [Fact]
        public async Task ConversionWithWrongInputTest2()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => await MediaInfo.Get("Z:\\test.mp4"));
        }
    }
}
