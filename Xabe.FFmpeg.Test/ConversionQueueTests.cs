using System;
using System.IO;
using System.Threading;
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
        public void QueueTest(bool parallel)
        {
            var queue = new ConversionQueue(parallel);

            for(var i = 0; i < 2; i++)
            {
                string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Mp4);
                IConversion conversion = ConversionHelper.ToTs(Resources.Mp4, output);
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
            Assert.True(resetEvent.WaitOne(2000));
        }

        private void Queue_OnConverted(int conversionNumber, int totalConversionsCount, IConversion currentConversion, AutoResetEvent resetEvent)
        {
            var mediaInfo = new MediaInfo(currentConversion.OutputFilePath);
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Properties.Duration);
            Assert.Equal("h264", mediaInfo.Properties.VideoFormat);
            Assert.Equal("aac", mediaInfo.Properties.AudioFormat);
            if(conversionNumber == totalConversionsCount)
                resetEvent.Set();
        }

        [Fact]
        public void QueueExceptionTest()
        {
            var queue = new ConversionQueue();
            var exceptionOccures = false;

            for(var i = 0; i < 2; i++)
            {
                string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Mp4);
                File.Create(output);
                IConversion conversion = ConversionHelper.ToMp4(Resources.MkvWithAudio, output);
                queue.Add(conversion);
            }

            var resetEvent = new AutoResetEvent(false);
            queue.OnException += (number, count, conversion) =>
            {
                exceptionOccures = true;
                resetEvent.Set();
            };
            queue.Start();
            Assert.True(resetEvent.WaitOne(2000));
            Assert.True(exceptionOccures);
        }
    }
}
