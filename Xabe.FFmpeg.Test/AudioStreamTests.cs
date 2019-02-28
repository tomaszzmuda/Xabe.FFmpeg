using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;
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
            IMediaInfo inputFile = await MediaInfo.Get(Resources.Mp3).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp3);

            IConversionResult conversionResult = await Conversion.New()
                                                    .AddStream(inputFile.AudioStreams.First().ChangeSpeed(speed))
                                                    //.SetPreset(ConversionPreset.UltraFast)
                                                    .SetOutput(outputPath)
                                                    .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
            Assert.Equal(TimeSpan.FromSeconds(expectedDuration), mediaInfo.Duration);
            Assert.Equal(TimeSpan.FromSeconds(expectedAudioDuration), mediaInfo.AudioStreams.First().Duration);
            Assert.Equal("mp3", mediaInfo.AudioStreams.First().Format);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }

        [Fact]
        public async Task OnConversion_ExtractOnlyAudioStream_OnProgressFires()
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversion conversion = Conversion.New()
                                               .AddStream(inputFile.AudioStreams.First()
                                                        .SetSeek(TimeSpan.FromSeconds(2)))
                                               .SetOutput(outputPath);

            TimeSpan currentProgress;
            TimeSpan videoLength;
            conversion.OnProgress += (sender, e) =>
            {
                currentProgress = e.Duration;
                videoLength = e.TotalLength;
            };

            await conversion.Start().ConfigureAwait(false);

            Assert.True(currentProgress > TimeSpan.Zero);
            Assert.True(currentProgress <= videoLength);
            Assert.True(videoLength == TimeSpan.FromSeconds(7));
        }
    }
}


