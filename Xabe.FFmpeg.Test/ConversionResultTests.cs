using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFmpeg.Exceptions;
using Xabe.FFmpeg.Model;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class ConversionResultTests
    {
        [Theory]
        [InlineData(ProcessPriorityClass.High)]
        [InlineData(ProcessPriorityClass.BelowNormal)]
        public async Task ConversionResultTestWithPriority(ProcessPriorityClass priority)
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");


            IConversionResult result = await Conversion.ToMp4(Resources.Mp4WithAudio, outputPath)
                                             .SetPreset(Enums.ConversionPreset.UltraFast)
                                             .SetPriority(FFmpegProcessPriority.FromPriorityClass(priority))
                                             .Start().ConfigureAwait(false);

            Assert.True(result.Success);
            Assert.NotNull(result.MediaInfo.Value);
            Assert.True(result.StartTime != DateTime.MinValue);
            Assert.True(result.EndTime != DateTime.MinValue);
            Assert.True(result.Duration > TimeSpan.FromMilliseconds(1));
        }

        [Fact]
        public async Task ConversionResultTestWithDefaultPriority()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");


            IConversionResult result = await Conversion.ToMp4(Resources.Mp4WithAudio, outputPath)
                                                       .SetPreset(Enums.ConversionPreset.UltraFast)
                                                       .SetPriority(FFmpegProcessPriority.Default())
                                                       .Start().ConfigureAwait(false);

            Assert.True(result.Success);
            Assert.NotNull(result.MediaInfo.Value);
            Assert.True(result.StartTime != DateTime.MinValue);
            Assert.True(result.EndTime != DateTime.MinValue);
            Assert.True(result.Duration > TimeSpan.FromMilliseconds(1));
        }

        [Fact]
        public async Task ConversionResultTestWithCurrentProcessPriority()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");


            IConversionResult result = await Conversion.ToMp4(Resources.Mp4WithAudio, outputPath)
                                                       .SetPreset(Enums.ConversionPreset.UltraFast)
                                                       .SetPriority(FFmpegProcessPriority.FromCurrentProcess())
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
