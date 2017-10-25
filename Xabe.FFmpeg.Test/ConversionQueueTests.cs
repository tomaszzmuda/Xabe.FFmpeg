using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class ConversionQueueTests

    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task QueueTest(bool parallel)
        {
            var queue = new ConversionQueue(parallel);
            var outputs = new List<string>();
            var conversions = new List<IConversion>();

            for(var i = 0; i < 2; i++)
            {
                string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Mp4);
                outputs.Add(output);
                IConversion conversion = ConversionHelper.ToMp4(Resources.MkvWithAudio, output, multithread:!parallel);
                conversions.Add(conversion);
                queue.Add(conversion);
            }

            await queue.Start();

            foreach(string output in outputs)
            {
                var mediaInfo = new MediaInfo(output);
                Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Properties.Duration);
                Assert.Equal("h264", mediaInfo.Properties.VideoFormat);
                Assert.Equal("aac", mediaInfo.Properties.AudioFormat);
            }
        }
    }
}
