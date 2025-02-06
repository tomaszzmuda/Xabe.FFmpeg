﻿using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Exceptions;
using Xabe.FFmpeg.Test.Common.Fixtures;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class AudioStreamTests(StorageFixture storageFixture) : IClassFixture<StorageFixture>
    {
        [Theory]
        [InlineData(13, 13, 1.0)]
        [InlineData(6, 6, 2.0)]
        [InlineData(27, 27, 0.5)]
        public async Task ChangeSpeedTest(int expectedDuration, int expectedAudioDuration, double speed)
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp3);
            var outputPath = storageFixture.GetTempFileName(FileExtensions.Mp3);
            _ = await FFmpeg.Conversions.New()
                                                    .AddStream(inputFile.AudioStreams.First().ChangeSpeed(speed))
                                                    .SetOutput(outputPath)
                                                    .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(expectedDuration, mediaInfo.Duration.Seconds);
            Assert.Equal(expectedAudioDuration, mediaInfo.AudioStreams.First().Duration.Seconds);
            Assert.Equal("mp3", mediaInfo.AudioStreams.First().Codec);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }

        [Theory]
        [InlineData(192000)]
        [InlineData(32000)]
        public async Task SetBitrate(int expectedBitrate)
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp3);
            var outputPath = storageFixture.GetTempFileName(FileExtensions.Mp3);

            var audioStream = inputFile.AudioStreams.First();
            audioStream.SetBitrate(expectedBitrate);
            _ = await FFmpeg.Conversions.New()
                                                    .AddStream(audioStream)
                                                    .SetOutput(outputPath)
                                                    .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);

            Assert.Equal(expectedBitrate, mediaInfo.AudioStreams.First().Bitrate);
            Assert.Equal("mp3", mediaInfo.AudioStreams.First().Codec);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }

        [Fact]
        public async Task SetBitrate_WithMaximumBitrate()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp3);
            var outputPath = storageFixture.GetTempFileName(FileExtensions.Mp3);

            var audioStream = inputFile.AudioStreams.First();
            audioStream.SetBitrate(32000, 32000, 8000);
            _ = await FFmpeg.Conversions.New()
                                                    .AddStream(audioStream)
                                                    .SetOutput(outputPath)
                                                    .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);

            Assert.Equal(32000, mediaInfo.AudioStreams.First().Bitrate);
            Assert.Equal("mp3", mediaInfo.AudioStreams.First().Codec);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }

        [Fact]
        public async Task ChangeChannels()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp3);
            var outputPath = storageFixture.GetTempFileName(FileExtensions.Mp3);

            var audioStream = inputFile.AudioStreams.First();
            var channels = audioStream.Channels;
            Assert.Equal(2, channels);
            audioStream.SetChannels(1);
            _ = await FFmpeg.Conversions.New()
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
            var outputPath = storageFixture.GetTempFileName(FileExtensions.Mp3);

            var audioStream = inputFile.AudioStreams.First();
            var sampleRate = audioStream.SampleRate;
            Assert.Equal(48000, sampleRate);
            audioStream.SetSampleRate(44100);
            _ = await FFmpeg.Conversions.New()
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
            var outputPath = storageFixture.GetTempFileName(FileExtensions.Mp4);

            IConversion conversion = FFmpeg.Conversions.New()
                                               .AddStream(inputFile.AudioStreams.First()
                                                        .SetSeek(TimeSpan.FromSeconds(2)))
                                               .SetOutput(outputPath);

            var currentProgress = new TimeSpan();
            var videoLength = new TimeSpan();
            conversion.OnProgress += (sender, e) =>
            {
                currentProgress = e.Duration;
                videoLength = e.TotalLength;
            };

            await conversion.Start();

            Assert.True(currentProgress > TimeSpan.Zero);
            Assert.True(currentProgress <= videoLength);
            Assert.Equal(7, videoLength.TotalSeconds);
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
            var outputPath = storageFixture.GetTempFileName(FileExtensions.Mp3);
            _ = await FFmpeg.Conversions.New()
                                                    .AddStream(inputFile.AudioStreams.First().ChangeSpeed(0.5))
                                                    .SetOutput(outputPath)
                                                    .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(27, mediaInfo.Duration.Seconds);
            Assert.Equal(27, mediaInfo.AudioStreams.First().Duration.Seconds);
            Assert.Equal("mp3", mediaInfo.AudioStreams.First().Codec);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }

        [Fact]
        public async Task SetBitstreamFilter_CorrectInput_CorrectResult()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            var outputPath = storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = await FFmpeg.Conversions.New()
                                                    .AddStream(inputFile.AudioStreams.First().SetBitstreamFilter(BitstreamFilter.aac_adtstoasc))
                                                    .SetOutput(outputPath)
                                                    .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(9, mediaInfo.Duration.Seconds);
            Assert.Equal(9, mediaInfo.AudioStreams.First().Duration.Seconds);
            Assert.Equal("aac", mediaInfo.AudioStreams.First().Codec);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }

        [Fact]
        public async Task SetBitstreamFilter_IncorrectFilter_ThrowConversionException()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            var outputPath = storageFixture.GetTempFileName(FileExtensions.Mp4);

            var exception = await Record.ExceptionAsync(async () => await FFmpeg.Conversions.New()
                                                    .AddStream(inputFile.AudioStreams.First().SetBitstreamFilter(BitstreamFilter.h264_mp4toannexb))
                                                    .SetOutput(outputPath)
                                                    .Start());

            Assert.NotNull(exception);
            Assert.IsType<ConversionException>(exception);
            Assert.IsType<InvalidBitstreamFilterException>(exception.InnerException);
        }

        [Fact]
        public async Task SetBitstreamFilter_CorrectInputAsString_CorrectResult()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            var outputPath = storageFixture.GetTempFileName(FileExtensions.Mp4);
            _ = await FFmpeg.Conversions.New()
                                                    .AddStream(inputFile.AudioStreams.First().SetBitstreamFilter("aac_adtstoasc"))
                                                    .SetOutput(outputPath)
                                                    .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(9, mediaInfo.Duration.Seconds);
            Assert.Equal(9, mediaInfo.AudioStreams.First().Duration.Seconds);
            Assert.Equal("aac", mediaInfo.AudioStreams.First().Codec);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }

        [Fact]
        public async Task SetBitstreamFilter_IncorrectFilterAsString_ThrowConversionException()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            var outputPath = storageFixture.GetTempFileName(FileExtensions.Mp4);

            var exception = await Record.ExceptionAsync(async () => await FFmpeg.Conversions.New()
                                                    .AddStream(inputFile.AudioStreams.First().SetBitstreamFilter("h264_mp4toannexb"))
                                                    .SetOutput(outputPath)
                                                    .Start());

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
            var outputPath = storageFixture.GetTempFileName(FileExtensions.Mp4);

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
            var outputPath = storageFixture.GetTempFileName(FileExtensions.Mp4);

            var audioStream = inputFile.AudioStreams.First();
            audioStream.SetCodec("mp3");
            _ = await FFmpeg.Conversions.New()
                                .AddStream(audioStream)
                                .SetOutput(outputPath)
                                .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.Equal(13, mediaInfo.Duration.Seconds);
            Assert.Equal(13, mediaInfo.AudioStreams.First().Duration.Seconds);
            Assert.Equal("mp3", mediaInfo.AudioStreams.First().Codec);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }

        [Fact]
        public async Task ChangeCodec_IncorrectCodec_NotFound()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp4WithAudio);
            var outputPath = storageFixture.GetTempFileName(FileExtensions.Mp4);

            var audioStream = inputFile.AudioStreams.First();
            audioStream.SetCodec("notExisting");

            var exception = await Record.ExceptionAsync(async () => await FFmpeg.Conversions.New()
                                .AddStream(audioStream)
                                .SetOutput(outputPath)
                                .Start());

            Assert.NotNull(exception);
            Assert.IsType<ConversionException>(exception);
        }

        [Fact]
        public async Task CopyStream_CorrectFFmpegArguments()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp4WithAudio);
            var outputPath = storageFixture.GetTempFileName(FileExtensions.Mp4);

            var audioStream = inputFile.AudioStreams.First();
            audioStream.SetCodec(AudioCodec.comfortnoise);
            audioStream.CopyStream();

            var result = await FFmpeg.Conversions.New()
                                .AddStream(audioStream)
                                .SetOutput(outputPath)
                                .Start();
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);

            Assert.Contains($"-c:a copy", result.Arguments);
            Assert.Equal(inputFile.AudioStreams.First().Codec, mediaInfo.AudioStreams.First().Codec);
        }

        [Fact]
        public async Task SetInputFormat_ChangeIfFormatIsApplied()
        {
            IMediaInfo inputFile = await FFmpeg.GetMediaInfo(Resources.Mp3);
            var outputPath = storageFixture.GetTempFileName(FileExtensions.Mp3);

            var audioStream = inputFile.AudioStreams.First();
            audioStream.SetInputFormat(Format.mp3);

            IConversionResult result = await FFmpeg.Conversions.New()
                                                    .AddStream(audioStream)
                                                    .SetOutput(outputPath)
                                                    .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(outputPath);

            Assert.Contains($"-f mp3 -i", result.Arguments);
            Assert.NotEmpty(mediaInfo.AudioStreams);
        }
    }
}

