using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Create queue for conversions
    /// </summary>
    public class ConversionQueue
    {
        private readonly bool _parallel;
        private readonly ConcurrentQueue<IConversion> _queue = new ConcurrentQueue<IConversion>();
        private readonly List<Task> _tasks = new List<Task>();

        /// <inheritdoc />
        public ConversionQueue(bool parallel = false)
        {
            _parallel = parallel;
        }

        private async Task Worker()
        {
            while(_queue.TryDequeue(out IConversion conversion))
                await conversion.Start();
        }

        /// <summary>
        ///     Start conversions
        /// </summary>
        public async Task Start()
        {
            if (!_parallel)
                _tasks.Add(Worker());
            else
                for (var i = 0; i < Environment.ProcessorCount; i++)
                    _tasks.Add(Worker());

            await Task.WhenAll(_tasks);
        }

        /// <summary>
        ///     Add conversion to queue
        /// </summary>
        /// <param name="conversion">Defined conversion</param>
        public void Add(IConversion conversion)
        {
            _queue.Enqueue(conversion);
        }
    }
}
