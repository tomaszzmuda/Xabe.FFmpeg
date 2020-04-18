using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Exceptions;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class ConversionResultTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData(ProcessPriorityClass.BelowNormal)]
        public async Task ConversionResultTest(ProcessPriorityClass? priority)
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");

            IConversionResult result = await Conversion.ToMp4(Resources.FlvWithAudio, outputPath)
                                             .SetPreset(ConversionPreset.UltraFast)
                                             .SetPriority(priority)
                                             .Start();


            var mediaInfo = await MediaInfo.Get(outputPath);
            Assert.NotNull(mediaInfo);
            Assert.True(result.StartTime != DateTime.MinValue);
            Assert.True(result.EndTime != DateTime.MinValue);
            Assert.Equal(TimeSpan.FromSeconds(5), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Codec);
        }

        [Fact]
        public async Task ConversionWithWrongInputTest2()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => await MediaInfo.Get("Z:\\test.mp4"));
        }
    }
}
