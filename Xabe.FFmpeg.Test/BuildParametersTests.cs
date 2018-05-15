using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;
using Xabe.FFmpeg.Exceptions;
using Xabe.FFmpeg.Model;
using Xabe.FFmpeg.Streams;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class BuildParametersTests
    {
        [Fact]
        public async Task Test()
        {
            var cancellationToken = new CancellationTokenSource().Token;

            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mkv);
            var mediaInfo = await MediaInfo.Get(Resources.MkvWithAudio);

            var videoStream = mediaInfo.VideoStreams.First();
            var audioStream = mediaInfo.AudioStreams.First();
            ISubtitleStream subtitleStream = null;

            videoStream.SetWatermark(Resources.PngSample, Position.BottomRight);
            //videoStream.SetSize(new VideoSize(1920, 1080));
            //videoStream.Reverse();
            //videoStream.ChangeSpeed(2);

            var conversion = Conversion.New()
                                                       .SetOutput(outputPath)
                                                       .AddStream(videoStream)
                                                       .AddStream(audioStream)
                                                       .AddStream(subtitleStream)
                                                       .UseMultiThread(true)
                                                       .SetPreset(ConversionPreset.UltraFast);

            var result = await conversion.Start(cancellationToken);
            Assert.True(result.Success);
        }
    }
}


