using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Xabe.FFmpeg.Extensions
{
    /// <summary>
    ///     Download helpers for HttpClient
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        ///     Downloads the resource at the URI into the destination, reporting progress when the content length is known
        /// </summary>
        /// <param name="client">HTTP client</param>
        /// <param name="requestUri">URI to download</param>
        /// <param name="destination">Destination stream</param>
        /// <param name="progress">Progress reporter</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination, IProgress<ProgressInfo> progress = null, CancellationToken cancellationToken = default)
        {
            // Get the http headers first to examine the content length
            using (var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                var contentLength = response.Content.Headers.ContentLength;
                using (var download = await response.Content.ReadAsStreamAsync())
                {
                    // Ignore progress reporting when no progress reporter was 
                    // passed or when the content length is unknown
                    if (progress == null || !contentLength.HasValue)
                    {
                        await download.CopyToAsync(destination);
                        return;
                    }

                    var relativeProgress = new Progress<ProgressInfo>(totalBytes => progress.Report(totalBytes));
                    // Use extension method to report progress while downloading
                    await download.CopyToAsync(destination, contentLength.Value, 81920, relativeProgress, cancellationToken);
                    progress.Report(new ProgressInfo(1L, 1L));
                }
            }
        }
    }
}
