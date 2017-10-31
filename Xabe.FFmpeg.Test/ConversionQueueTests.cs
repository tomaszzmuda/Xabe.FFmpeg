using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Xabe.FFmpeg.Enums;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class ConversionQueueTests

    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void QueueTest(bool parallel)
        {
            var queue = new ConversionQueue(parallel);
            var outputs = new List<string>();
            var conversions = new List<IConversion>();

            for (var i = 0; i < 2; i++)
            {
                string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Mp4);
                outputs.Add(output);
                IConversion conversion = ConversionHelper.ToTs(Resources.Mp4, output);
                conversions.Add(conversion);
                queue.Add(conversion);
            }

            queue.Start();
            var resetEvent = new AutoResetEvent(false);
            queue.OnConverted += (number, count, conversion) => Queue_OnConverted(number, count, conversion, resetEvent);
            queue.OnException += (number, count, conversion) =>
            {
                resetEvent.Set();
                throw new Exception();
            };
            resetEvent.WaitOne();
        }

        private void Queue_OnConverted(int conversionNumber, int totalConversionsCount, IConversion currentConversion, AutoResetEvent resetEvent)
        {
            var mediaInfo = new MediaInfo(currentConversion.OutputFilePath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Properties.Duration);
            Assert.Equal("h264", mediaInfo.Properties.VideoFormat);
            Assert.Equal("aac", mediaInfo.Properties.AudioFormat);
            if(conversionNumber == totalConversionsCount)
            {
                resetEvent.Set();
            }
        }
    }
}
