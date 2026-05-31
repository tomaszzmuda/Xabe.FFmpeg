using System;
using System.Threading.Tasks;

namespace Xabe.FFmpeg.Downloader
{
    /// <summary>Downloads the Latest Version of FFmpeg</summary>
    public interface IFFmpegDownloader
    {
        /// <summary>
        ///     Do the download and install FFmpeg with progress reporting
        /// </summary>
        /// <param name="path">FFmpeg executables destination directory</param>
        /// <param name="progress">Progress of download</param>
        /// <param name="retries">Amount of times to retry downloading in the event of a failure</param>
        Task GetLatestVersion(string path, IProgress<ProgressInfo> progress = null, int retries = FFmpegDownloaderBase.DEFAULT_MAX_RETRIES);
    }
}
