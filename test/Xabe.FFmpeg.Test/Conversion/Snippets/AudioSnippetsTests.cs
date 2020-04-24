using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class AudioSnippetsTests
    {
        [Fact]
        public async Task AddAudio()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);

            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.AddAudio(Resources.Mp4, Resources.Mp3, output))
                                             .Start();

            IMediaInfo mediaInfo = await MediaInfo.Get(output);
            Assert.Single(mediaInfo.AudioStreams);
            Assert.Equal("aac", mediaInfo.AudioStreams.First()
                                         .Codec);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Equal(TimeSpan.FromSeconds(13), mediaInfo.Duration);
        }

        [Fact]
        public async Task ExtractAudio()
        {
            string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp3);
            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.ExtractAudio(Resources.Mp4WithAudio, output))
                                             .Start();


            IMediaInfo mediaInfo = await MediaInfo.Get(output);
            Assert.Empty(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            Assert.NotNull(audioStream);
            Assert.Equal("mp3", audioStream.Codec);
            Assert.Equal(TimeSpan.FromSeconds(13), audioStream.Duration);
            Assert.Equal(320000, audioStream.Bitrate);
        }
    }
}
