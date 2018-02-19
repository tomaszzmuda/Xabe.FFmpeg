using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;
using Xabe.FFmpeg.Model;
using Xabe.FFmpeg.Streams;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class NewConversionTests
    {
        [Theory]
        [InlineData(Position.UpperRight)]
        [InlineData(Position.BottomRight)]
        [InlineData(Position.Left)]
        [InlineData(Position.Right)]
        [InlineData(Position.Up)]
        [InlineData(Position.BottomLeft)]
        [InlineData(Position.UpperLeft)]
        [InlineData(Position.Center)]
        [InlineData(Position.Bottom)]
        public async Task WatermarkTest(Position position)
        {
            IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio);
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
            IVideoStream stream = inputFile.VideoStreams.First()
                                  .SetWatermark(Resources.PngSample, position);

            IConversionResult conversionResult = await Conversion.New()
                                                    .AddStream(stream)
                                                    .SetOutput(outputPath)
                                                    .Start();

            Assert.True(conversionResult.Success);
            Assert.Contains("overlay", conversionResult.Arguments);
            Assert.Contains(Resources.PngSample, conversionResult.Arguments);
            IMediaInfo mediaInfo = await MediaInfo.Get(outputPath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal("h264", mediaInfo.VideoStreams.First().Format);
            Assert.False(mediaInfo.AudioStreams.Any());
        }

        [Fact]
        public async Task SetVideoCodecTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.Mpeg4);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(videoStream)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal("mpeg4", resultFile.VideoStreams.First().Format);
        }

        [Fact]
        public async Task SetAudioCodecTest()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
            IMediaInfo info = await MediaInfo.Get(Resources.MkvWithAudio);
            IAudioStream audioStream = info.AudioStreams.First()?.SetCodec(AudioCodec.Ac3);

            IConversionResult conversionResult = await Conversion.New()
                                                                 .AddStream(audioStream)
                                                                 .SetOutput(output)
                                                                 .Start();

            Assert.True(conversionResult.Success);
            IMediaInfo resultFile = conversionResult.MediaInfo.Value;
            Assert.Equal("ac3", resultFile.AudioStreams.First().Format);
        }
    }
}


