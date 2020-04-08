using System;
using System.Globalization;
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

        [Theory]
        [InlineData(192000)]
        [InlineData(32000)]
        public async Task ChangeBitrate(int expectedBitrate)
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.Mp3).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp3);

            var audioStream = inputFile.AudioStreams.First();
            var bitrate = audioStream.Bitrate;
            audioStream.ChangeBitrate(expectedBitrate);

            IConversionResult conversionResult = await Conversion.New()
                                                    .AddStream(audioStream)
                                                    .SetOutput(outputPath)
                                                    .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);

            Assert.Equal(expectedBitrate, mediaInfo.AudioStreams.First().Bitrate);
            Assert.Equal("mp3", mediaInfo.AudioStreams.First().Format);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }

        [Fact]
        public async Task ChangeChannels()
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.Mp3).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp3);

            var audioStream = inputFile.AudioStreams.First();
            var channels = audioStream.Channels;
            Assert.Equal(2, channels);
            audioStream.SetChannels(1);

            IConversionResult conversionResult = await Conversion.New()
                                                    .AddStream(audioStream)
                                                    .SetOutput(outputPath)
                                                    .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);

            Assert.Equal(1, mediaInfo.AudioStreams.First().Channels);
            Assert.Equal("mp3", mediaInfo.AudioStreams.First().Format);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }

        [Fact]
        public async Task ChangeSamplerate()
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.Mp3).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp3);

            var audioStream = inputFile.AudioStreams.First();
            var sampleRate = audioStream.SampleRate;
            Assert.Equal(48000, sampleRate);
            audioStream.SetSampleRate(44100);

            IConversionResult conversionResult = await Conversion.New()
                                                    .AddStream(audioStream)
                                                    .SetOutput(outputPath)
                                                    .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);

            Assert.Equal(44100, mediaInfo.AudioStreams.First().SampleRate);
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

            TimeSpan currentProgress = new TimeSpan();
            TimeSpan videoLength = new TimeSpan();
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

        [Fact]
        public async Task ExtractAdditionalValuesTest()
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio).ConfigureAwait(false);

            Assert.Equal(1, inputFile.AudioStreams.First().Default.Value);
            Assert.Equal(0, inputFile.AudioStreams.First().Forced.Value);
            Assert.Null(inputFile.AudioStreams.First().Language);
        }

        [Fact]
        public async Task ChangeSpeed_CommaAsASeparator_CorrectResult()
        {
            CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("pl-PL");

            IMediaInfo inputFile = await MediaInfo.Get(Resources.Mp3).ConfigureAwait(false);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp3);

            IConversionResult conversionResult = await Conversion.New()
                                                    .AddStream(inputFile.AudioStreams.First().ChangeSpeed(0.5))
                                                    .SetOutput(outputPath)
                                                    .Start().ConfigureAwait(false);

            Assert.True(conversionResult.Success);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath).ConfigureAwait(false);
            Assert.Equal(TimeSpan.FromSeconds(27), mediaInfo.Duration);
            Assert.Equal(TimeSpan.FromSeconds(27), mediaInfo.AudioStreams.First().Duration);
            Assert.Equal("mp3", mediaInfo.AudioStreams.First().Format);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }
    }
}


