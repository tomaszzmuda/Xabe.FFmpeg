using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Test.Fixtures;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class AudioSnippetsTests : IClassFixture<StorageFixture>
    {
        private readonly StorageFixture _storageFixture;

        public AudioSnippetsTests(StorageFixture storageFixture)
        {
            _storageFixture = storageFixture;
        }

        [Fact]
        public async Task AddAudio()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.AddAudio(Resources.Mp4, Resources.Mp3, output))
                                             .Start();

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Single(mediaInfo.AudioStreams);
            Assert.Equal("aac", mediaInfo.AudioStreams.First()
                                         .Codec);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Equal(13, mediaInfo.Duration.Seconds);
        }

        [Fact]
        public async Task ExtractAudio()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp3);
            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.ExtractAudio(Resources.Mp4WithAudio, output))
                                             .Start();


            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Empty(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            Assert.NotNull(audioStream);
            Assert.Equal("mp3", audioStream.Codec);
            Assert.Equal(13, audioStream.Duration.Seconds);
            Assert.Equal(320000, audioStream.Bitrate);
        }

        [Theory]
        [InlineData(VideoSize.Hd1080, PixelFormat.yuv420p, VisualisationMode.bar, AmplitudeScale.lin, FrequencyScale.log)]
        [InlineData(VideoSize.Hd1080, PixelFormat.yuv420p, VisualisationMode.bar, AmplitudeScale.log, FrequencyScale.lin)]
        [InlineData(VideoSize.Hd1080, PixelFormat.yuv420p, VisualisationMode.bar, AmplitudeScale.sqrt, FrequencyScale.rlog)]
        [InlineData(VideoSize.Hd1080, PixelFormat.yuv420p, VisualisationMode.bar, AmplitudeScale.cbrt, FrequencyScale.log)]
        [InlineData(VideoSize.Hd1080, PixelFormat.yuv420p, VisualisationMode.dot, AmplitudeScale.lin, FrequencyScale.log)]
        [InlineData(VideoSize.Hd1080, PixelFormat.yuv420p, VisualisationMode.dot, AmplitudeScale.log, FrequencyScale.lin)]
        [InlineData(VideoSize.Hd1080, PixelFormat.yuv420p, VisualisationMode.dot, AmplitudeScale.sqrt, FrequencyScale.rlog)]
        [InlineData(VideoSize.Hd1080, PixelFormat.yuv420p, VisualisationMode.dot, AmplitudeScale.cbrt, FrequencyScale.log)]
        [InlineData(VideoSize.Hd1080, PixelFormat.yuv420p, VisualisationMode.line, AmplitudeScale.lin, FrequencyScale.log)]
        [InlineData(VideoSize.Hd1080, PixelFormat.yuv420p, VisualisationMode.line, AmplitudeScale.log, FrequencyScale.lin)]
        [InlineData(VideoSize.Hd1080, PixelFormat.yuv420p, VisualisationMode.line, AmplitudeScale.sqrt, FrequencyScale.rlog)]
        [InlineData(VideoSize.Hd1080, PixelFormat.yuv420p, VisualisationMode.line, AmplitudeScale.cbrt, FrequencyScale.log)]
        public async Task VisualiseAudioTest(VideoSize size, PixelFormat pixelFormat, VisualisationMode mode, AmplitudeScale amplitudeScale, FrequencyScale frequencyScale)
        {
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);
            IMediaInfo info = await FFmpeg.GetMediaInfo(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.aac);

            IConversionResult conversionResult = await (await FFmpeg.Conversions.FromSnippet.VisualiseAudio(Resources.Mp4WithAudio, output, size, pixelFormat, mode, amplitudeScale, frequencyScale))
                .Start();

            IMediaInfo resultFile = await FFmpeg.GetMediaInfo(output);

            // The resulting streams are 4 seconds longer than the original
            Assert.Equal((audioStream.Duration + TimeSpan.FromSeconds(4)).Seconds, resultFile.VideoStreams.First().Duration.Seconds);
            Assert.Equal((audioStream.Duration + TimeSpan.FromSeconds(4)).Seconds, resultFile.AudioStreams.First().Duration.Seconds);
            Assert.Equal(1920, resultFile.VideoStreams.First().Width);
            Assert.Equal(1080, resultFile.VideoStreams.First().Height);
        }
    }
}
