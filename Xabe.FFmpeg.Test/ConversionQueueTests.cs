using System;
using System.IO;
using System.Linq;
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
                string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
                IConversion conversion = Conversion.ToTs(Resources.Mp4, output);
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
            Assert.True(resetEvent.WaitOne(60000));
        }

        private void Queue_OnConverted(int conversionNumber, int totalConversionsCount, IConversion currentConversion, AutoResetEvent resetEvent)
        {
            IMediaInfo mediaInfo = MediaInfo.Get(currentConversion.OutputFilePath)
                                            .Result;
            Assert.Equal(TimeSpan.FromSeconds(9), mediaInfo.Duration);
            Assert.Equal(1, mediaInfo.VideoStreams.Count());
            Assert.Equal(1, mediaInfo.AudioStreams.Count());
            Assert.Equal("h264", mediaInfo.VideoStreams.First()
                                          .Format);
            Assert.Equal("aac", mediaInfo.AudioStreams.First()
                                         .Format);
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
                string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
                File.Create(output);
                IConversion conversion = Conversion.ToMp4(Resources.MkvWithAudio, output);
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

            for(var i = 0; i < 2; i++)
            {
                string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
                File.Create(output);
                IConversion conversion = Conversion.ToMp4(Resources.MkvWithAudio, output);
                queue.Add(conversion);
            }

            var resetEvent = new AutoResetEvent(false);
            queue.OnException += (number, count, conversion) =>
            {
                totalItemsCount = count;
                currentItemNumber = number;
                if(number == count)
                    resetEvent.Set();
            };
            queue.Start();
            Assert.True(resetEvent.WaitOne(10000));
            Assert.Equal(totalItemsCount, currentItemNumber);
        }
    }
}
