using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Exceptions;
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
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp3);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp3);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                    .AddStream(inputFile.AudioStreams.First().ChangeSpeed(speed))
                                                    //.SetPreset(ConversionPreset.UltraFast)
                                                    .SetOutput(outputPath)
                                                    .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(expectedDuration), mediaInfo.Duration);
            Assert.Equal(TimeSpan.FromSeconds(expectedAudioDuration), mediaInfo.AudioStreams.First().Duration);
            Assert.Equal("mp3", mediaInfo.AudioStreams.First().Codec);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }

        [Theory]
        [InlineData(192000)]
        [InlineData(32000)]
        public async Task SetBitrate(int expectedBitrate)
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp3);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp3);

            var audioStream = inputFile.AudioStreams.First();
            var bitrate = audioStream.Bitrate;
            audioStream.SetBitrate(expectedBitrate);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                    .AddStream(audioStream)
                                                    .SetOutput(outputPath)
                                                    .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);

            Assert.Equal(expectedBitrate, mediaInfo.AudioStreams.First().Bitrate);
            Assert.Equal("mp3", mediaInfo.AudioStreams.First().Codec);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }

        [Fact]
        public async Task ChangeChannels()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp3);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp3);

            var audioStream = inputFile.AudioStreams.First();
            var channels = audioStream.Channels;
            Assert.Equal(2, channels);
            audioStream.SetChannels(1);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                    .AddStream(audioStream)
                                                    .SetOutput(outputPath)
                                                    .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);

            Assert.Equal(1, mediaInfo.AudioStreams.First().Channels);
            Assert.Equal("mp3", mediaInfo.AudioStreams.First().Codec);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }

        [Fact]
        public async Task ChangeSamplerate()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp3);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp3);

            var audioStream = inputFile.AudioStreams.First();
            var sampleRate = audioStream.SampleRate;
            Assert.Equal(48000, sampleRate);
            audioStream.SetSampleRate(44100);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                    .AddStream(audioStream)
                                                    .SetOutput(outputPath)
                                                    .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);

            Assert.Equal(44100, mediaInfo.AudioStreams.First().SampleRate);
            Assert.Equal("mp3", mediaInfo.AudioStreams.First().Codec);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }


        [Fact]
        public async Task OnConversion_ExtractOnlyAudioStream_OnProgressFires()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversion conversion = FFmpeg.Conversions.New()
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

            await conversion.Start();

            Assert.True(currentProgress > TimeSpan.Zero);
            Assert.True(currentProgress <= videoLength);
            Assert.True(videoLength == TimeSpan.FromSeconds(7));
        }

        [Fact]
        public async Task ExtractAdditionalValuesTest()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);

            Assert.Equal(1, inputFile.AudioStreams.First().Default.Value);
            Assert.Equal(0, inputFile.AudioStreams.First().Forced.Value);
            Assert.Null(inputFile.AudioStreams.First().Language);
        }

        [Fact]
        public async Task ChangeSpeed_CommaAsASeparator_CorrectResult()
        {
            CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("pl-PL");

            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp3);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp3);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                    .AddStream(inputFile.AudioStreams.First().ChangeSpeed(0.5))
                                                    .SetOutput(outputPath)
                                                    .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(27), mediaInfo.Duration);
            Assert.Equal(TimeSpan.FromSeconds(27), mediaInfo.AudioStreams.First().Duration);
            Assert.Equal("mp3", mediaInfo.AudioStreams.First().Codec);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }

        [Fact]
        public async Task SetBitstreamFilter_CorrectInput_CorrectResult()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                    .AddStream(inputFile.AudioStreams.First().SetBitstreamFilter(BitstreamFilter.aac_adtstoasc))
                                                    .SetOutput(outputPath)
                                                    .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.AudioStreams.First().Duration);
            Assert.Equal("aac", mediaInfo.AudioStreams.First().Codec);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }

        [Fact]
        public async Task SetBitstreamFilter_IncorrectFilter_ThrowConversionException()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            var exception = await Record.ExceptionAsync(async () => await FFmpeg.Conversions.New()
                                                    .AddStream(inputFile.AudioStreams.First().SetBitstreamFilter(BitstreamFilter.h264_mp4toannexb))
                                                    .SetOutput(outputPath)
                                                    .Start()); ;

            Assert.NotNull(exception);
            Assert.IsType<ConversionException>(exception);
            Assert.IsType<InvalidBitstreamFilterException>(exception.InnerException);
        }

        [Fact]
        public async Task SetBitstreamFilter_CorrectInputAsString_CorrectResult()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                                    .AddStream(inputFile.AudioStreams.First().SetBitstreamFilter("aac_adtstoasc"))
                                                    .SetOutput(outputPath)
                                                    .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.AudioStreams.First().Duration);
            Assert.Equal("aac", mediaInfo.AudioStreams.First().Codec);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }

        [Fact]
        public async Task SetBitstreamFilter_IncorrectFilterAsString_ThrowConversionException()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            var exception = await Record.ExceptionAsync(async () => await FFmpeg.Conversions.New()
                                                    .AddStream(inputFile.AudioStreams.First().SetBitstreamFilter("h264_mp4toannexb"))
                                                    .SetOutput(outputPath)
                                                    .Start()); ;

            Assert.NotNull(exception);
            Assert.IsType<ConversionException>(exception);
            Assert.IsType<InvalidBitstreamFilterException>(exception.InnerException);
        }

        [Theory]
        [InlineData(AudioCodec.mp2, "mp2")]
        [InlineData(AudioCodec._4gv, "4gv")]
        [InlineData(AudioCodec._8svx_exp, "8svx_exp")]
        [InlineData(AudioCodec._8svx_fib, "8svx_fib")]
        public async Task ChangeCodec_EnumValue_EverythingMapsCorrectly(AudioCodec audioCodec, string expectedCodec)
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp4WithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            var audioStream = inputFile.AudioStreams.First();
            audioStream.SetCodec(audioCodec);

            var args = FFmpeg.Conversions.New()
                                .AddStream(audioStream)
                                .SetOutput(outputPath)
                                .Build();

            Assert.Contains($"-c:a {expectedCodec}", args);
        }

        [Fact]
        public async Task ChangeCodec_StringValue_CorrectResult()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp4WithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            var audioStream = inputFile.AudioStreams.First();
            audioStream.SetCodec("mp3");

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                                .AddStream(audioStream)
                                .SetOutput(outputPath)
                                .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(13), mediaInfo.Duration);
            Assert.Equal(TimeSpan.FromSeconds(13), mediaInfo.AudioStreams.First().Duration);
            Assert.Equal("mp3", mediaInfo.AudioStreams.First().Codec);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }

        [Fact]
        public async Task ChangeCodec_IncorrectCodec_NotFound()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp4WithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            var audioStream = inputFile.AudioStreams.First();
            audioStream.SetCodec("notExisting");

            var exception = await Record.ExceptionAsync(async() => await FFmpeg.Conversions.New()
                                .AddStream(audioStream)
                                .SetOutput(outputPath)
                                .Start());

            Assert.NotNull(exception);
            Assert.IsType<ConversionException>(exception);
        }
    }
}


