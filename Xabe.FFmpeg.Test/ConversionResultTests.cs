using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class ConversionResultTests
    {
        [Fact]
        public async Task ConversionResultTest()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");

            IConversionResult result = await (await Conversion.ToMp4(Resources.Mp4WithAudio, outputPath))
                                             .UseMultiThread(true)
                                             .SetSpeed(Enums.ConversionSpeed.UltraFast)
                                             .Start();

            Assert.True(result.Success);
            Assert.NotNull(result.MediaInfo.Value);
            Assert.True(result.StartTime != DateTime.MinValue);
            Assert.True(result.EndTime != DateTime.MinValue);
            Assert.True(result.Duration > TimeSpan.FromMilliseconds(1));
        }
    }
}
