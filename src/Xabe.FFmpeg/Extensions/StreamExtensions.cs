using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Xabe.FFmpeg.Extensions
{
    /// <summary>
    ///     Helpers for copying streams with progress reporting
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        ///     Copies the source stream into the destination in bounded chunks, reporting progress
        /// </summary>
        /// <param name="source">Readable source stream</param>
        /// <param name="destination">Writable destination stream</param>
        /// <param name="contentLength">Expected content length in bytes</param>
        /// <param name="bufferSize">Chunk size in bytes</param>
        /// <param name="progress">Progress reporter</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task CopyToAsync(this Stream source, Stream destination, long contentLength, int bufferSize, IProgress<ProgressInfo> progress = null, CancellationToken cancellationToken = default)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!source.CanRead)
            {
                throw new ArgumentException("Has to be readable", nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (!destination.CanWrite)
            {
                throw new ArgumentException("Has to be writable", nameof(destination));
            }

            if (bufferSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            }

            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                progress?.Report(new ProgressInfo(totalBytesRead, contentLength));
            }
        }
    }
}
