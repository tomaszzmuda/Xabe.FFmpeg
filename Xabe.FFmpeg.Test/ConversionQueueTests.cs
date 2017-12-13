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
        private readonly AutoResetEvent _resetEvent = new AutoResetEvent(false);

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
            queue.OnConverted += (number, count, conversion) => Queue_OnConverted(number, count, conversion, _resetEvent);
            queue.OnException += (number, count, conversion) =>
            {
                _resetEvent.Set();
                throw new Exception();
            };
            Assert.True(_resetEvent.WaitOne(60000));
        }

        private void Queue_OnConverted(int conversionNumber, int totalConversionsCount, IConversion currentConversion, AutoResetEvent resetEvent)
        {
            var mediaInfo = MediaInfo.Get(currentConversion.OutputFilePath).Result;
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

        [Fact]
        public void QueueNumberIncrementExceptionTest()
        {
            var queue = new ConversionQueue();
            var currentItemNumber = 0;
            var totalItemsCount = 0;

            for (var i = 0; i < 2; i++)
            {
                string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Extensions.Mp4);
                File.Create(output);
                IConversion conversion = ConversionHelper.ToMp4(Resources.MkvWithAudio, output);
                queue.Add(conversion);
            }

            var resetEvent = new AutoResetEvent(false);
            queue.OnException += (number, count, conversion) =>
            {
                totalItemsCount = count;
                currentItemNumber = number;
                if (number == count) resetEvent.Set();
            };
            queue.Start();
            Assert.True(resetEvent.WaitOne(10000));
            Assert.Equal(totalItemsCount, currentItemNumber);
        }
    }
}
