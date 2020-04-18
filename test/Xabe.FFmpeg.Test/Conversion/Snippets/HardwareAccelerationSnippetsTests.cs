using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class HardwareAcceleration
    {
        [RunnableInDebugOnly]
        public async Task ConversionWithHardware()
        {
            string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);

            IConversionResult result = await Conversion.ConvertWithHardware(Resources.MkvWithAudio, output, HardwareAccelerator.cuvid, VideoCodec.h264_cuvid, VideoCodec.h264_nvenc).Start();

            var mediaInfo = await MediaInfo.Get(output);
            Assert.Equal(TimeSpan.FromSeconds(10), mediaInfo.Duration);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("h264", videoStream.Codec);
            Assert.Equal("aac", audioStream.Codec);
        }
    }
}
