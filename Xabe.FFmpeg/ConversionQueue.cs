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
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        ///     Information about queue's media status
        /// </summary>
        /// <param name="currentItemNumber">Auto incremented id of conversion</param>
        /// <param name="totalItemsCount">Count of conversion items</param>
        /// <param name="conversion">Conversion that was just finished</param>
        public delegate void ConversionQueueEventHandler(int currentItemNumber, int totalItemsCount, IConversion conversion);

        /// <summary>
        ///     Occurs when conversion in queue finished
        /// </summary>
        public event ConversionQueueEventHandler OnConverted;

        /// <summary>
        ///     Occurs when appers any exceptions during conversion
        /// </summary>
        public event ConversionQueueEventHandler OnException;

        /// <summary>
        ///     Queue for conversions.
        /// </summary>
        /// <param name="parallel">If set, queue create multiple workers based on CPU cores. It's best to set if files are small (less than few MB).</param>
        public ConversionQueue(bool parallel = false)
        {
            _parallel = parallel;

            if (!_parallel)
                Task.Run(() => Worker(_cancellationTokenSource.Token));
            else
                for (var i = 0; i < Environment.ProcessorCount; i++)
                    Task.Run(() => Worker(_cancellationTokenSource.Token));
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
                    await conversion.Start(token);
                    Interlocked.Increment(ref _number);
                    OnConverted?.Invoke((int) Interlocked.Read(ref _number), (int) Interlocked.Read(ref _totalItems), conversion);
                }
                catch(Exception)
                {
                    OnException?.Invoke((int)Interlocked.Read(ref _number), (int)Interlocked.Read(ref _totalItems), conversion);
                }
            }
        }

        /// <summary>
        ///     Start converting media in queue
        /// </summary>
        public void Start(CancellationTokenSource cancellationTokenSource = null)
        {
            _cancellationTokenSource = cancellationTokenSource ?? new CancellationTokenSource();
            _start.Set();
        }

        /// <summary>
        ///     Pause converting media in queue
        /// </summary>
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
            _cancellationTokenSource.Cancel();
        }
    }
}
