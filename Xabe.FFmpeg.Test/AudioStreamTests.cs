using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;
using Xabe.FFmpeg.Exceptions;
using Xabe.FFmpeg.Model;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class AudioStreamTests
    {
        [Theory]
        [InlineData(13, 13, 1.0)]
        [InlineData(6, 6, 2.0)]
        [InlineData(27, 27, 0.5)]
        public async Task ChangeSpeedTest(int expectedDuration, int expectedAudioDuration, double speed)
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.Mp3);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp3);

            IConversionResult conversionResult = await Conversion.New()
                                                    .AddStream(inputFile.AudioStreams.First().ChangeSpeed(speed))
                                                    //.SetPreset(ConversionPreset.UltraFast)
                                                    .SetOutput(outputPath)
                                                    .Start();

            Assert.True(conversionResult.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(expectedDuration), mediaInfo.Duration);
            Assert.Equal(TimeSpan.FromSeconds(expectedAudioDuration), mediaInfo.AudioStreams.First().Duration);
            Assert.Equal("mp3", mediaInfo.AudioStreams.First().Format);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }
    }
}


