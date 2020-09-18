using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xabe.FFmpeg.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<bool> DownloadAsync(this HttpClient client, string requestUri, Stream destination, IProgress<ProgressInfo> progress = null, CancellationToken cancellationToken = default)
        {
            // Get the http headers first to examine the content length
            using (var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead))
            {
                var contentLength = response.Content.Headers.ContentLength;

                if (!response.IsSuccessStatusCode)
                    return false;

                using (var download = await response.Content.ReadAsStreamAsync())
                {
                    // Ignore progress reporting when no progress reporter was 
                    // passed or when the content length is unknown
                    if (progress == null || !contentLength.HasValue)
                    {
                        await download.CopyToAsync(destination);
                        return false;
                    }

                    var relativeProgress = new Progress<ProgressInfo>(totalBytes => progress.Report(totalBytes));
                    // Use extension method to report progress while downloading
                    await download.CopyToAsync(destination, contentLength.Value, 81920, relativeProgress, cancellationToken);
                    progress.Report(new ProgressInfo(1L, 1L));

                    return true;
                }
            }
        }
    }
}
