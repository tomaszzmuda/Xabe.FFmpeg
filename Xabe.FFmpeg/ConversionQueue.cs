using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Create queue for conversions
    /// </summary>
    public class ConversionQueue : IDisposable
    {
        private readonly bool _parallel;
        private readonly ConcurrentBag<IConversion> _list = new ConcurrentBag<IConversion>();
        private readonly ManualResetEvent _start = new ManualResetEvent(false);
        private long _number;
        private long _totalItems;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public delegate void ConversionQueueEventHandler(int conversionNumber, int totalConversionsCount, IConversion currentConversion);

        public event ConversionQueueEventHandler OnConverted;
        public event ConversionQueueEventHandler OnException;

        /// <inheritdoc />
        public ConversionQueue(bool parallel = false)
        {
            _parallel = parallel;

            if (!_parallel)
                Task.Run(() => Worker(cancellationTokenSource.Token));
            else
                for (var i = 0; i < Environment.ProcessorCount; i++)
                    Task.Run(() => Worker(cancellationTokenSource.Token));
        }

        private bool GetNext(out IConversion conversion)
        {
            return _list.TryTake(out conversion);
        }

        private async Task Worker(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                IConversion conversion = null;
                try
                {
                    _start.WaitOne();
                    if(!GetNext(out conversion))
                        continue;
                    await conversion.Start();
                    Interlocked.Increment(ref _number);
                    OnConverted((int) Interlocked.Read(ref _number), (int) Interlocked.Read(ref _totalItems), conversion);
                }
                catch(Exception)
                {
                    OnException((int)Interlocked.Read(ref _number), (int)Interlocked.Read(ref _totalItems), conversion);
                }
            }
        }

        public void Start()
        {
            _start.Set();
        }

        public void Pause()
        {
            _start.Reset();
        }

        /// <summary>
        ///     Add conversion to queue
        /// </summary>
        /// <param name="conversion">Defined conversion</param>
        public void Add(IConversion conversion)
        {
            Interlocked.Increment(ref _totalItems);
            _list.Add(conversion);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _start?.Dispose();
        }
    }
}
