using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Handling async code synchronously
    /// </summary>
    internal static class AsyncHelper
    {
        /// <summary>
        /// Execute's an async Task method which has a void return value synchronously
        /// </summary>
        /// <param name="task">Task method to execute</param>
        public static void RunSync(Func<Task> task)
        {
            task().GetAwaiter().GetResult();
            return;
            var oldContext = SynchronizationContext.Current;
            using (var synch = new ExclusiveSynchronizationContext())
            {
                SynchronizationContext.SetSynchronizationContext(synch);
                synch.Post(_ =>
                {
                    try
                    {
                        task().GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        synch.InnerException = e;
                        throw;
                    }
                    finally
                    {
                        synch.EndMessageLoop();
                    }
                }, null);
                synch.BeginMessageLoop();

                SynchronizationContext.SetSynchronizationContext(oldContext);
            }
        }

        /// <summary>
        /// Execute's an async Task method which has a T return type synchronously
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="task">Task method to execute</param>
        /// <returns></returns>
        public static T RunSync<T>(Func<Task<T>> task)
        {
            return task().GetAwaiter().GetResult();
            var oldContext = SynchronizationContext.Current;
            using (var synch = new ExclusiveSynchronizationContext())
            {
                SynchronizationContext.SetSynchronizationContext(synch);
                T ret = default(T);
                synch.Post(_ =>
                {
                    try
                    {
                        ret = task().GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        synch.InnerException = e;
                        throw;
                    }
                    finally
                    {
                        synch.EndMessageLoop();
                    }
                }, null);
                synch.BeginMessageLoop();
                SynchronizationContext.SetSynchronizationContext(oldContext);
                return ret;
            }
        }

        private class ExclusiveSynchronizationContext : SynchronizationContext, IDisposable
        {
            private bool _done;
            public Exception InnerException { get; set; }
            private readonly AutoResetEvent _workItemsWaiting = new AutoResetEvent(false);
            private readonly Queue<Tuple<SendOrPostCallback, object>> _items =
                new Queue<Tuple<SendOrPostCallback, object>>();

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("We cannot send to our same thread");
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                lock (_items)
                {
                    _items.Enqueue(Tuple.Create(d, state));
                }
                _workItemsWaiting.Set();
            }

            public void EndMessageLoop()
            {
                Post(_ => _done = true, null);
            }

            public void BeginMessageLoop()
            {
                while (!_done)
                {
                    Tuple<SendOrPostCallback, object> task = null;
                    lock (_items)
                    {
                        if (_items.Count > 0)
                        {
                            task = _items.Dequeue();
                        }
                    }
                    if (task != null)
                    {
                        task.Item1(task.Item2);
                        if (InnerException != null) // the method threw an exeption
                        {
                            throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
                        }
                    }
                    else
                    {
                        _workItemsWaiting.WaitOne();
                    }
                }
            }

            public override SynchronizationContext CreateCopy()
            {
                return this;
            }

            #region IDisposable
            private bool _isDisposed = false;
            ~ExclusiveSynchronizationContext()
            {
                this.Dispose(false);
            }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (_isDisposed)
                {
                    return;
                }

                if (disposing)
                {
                    _workItemsWaiting?.Dispose();
                }

                _isDisposed = true;
            }
            #endregion
        }
    }
}
