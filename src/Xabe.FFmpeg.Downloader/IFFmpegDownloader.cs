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
        Task GetLatestVersion(string path, IProgress<(long, long)> progress = null);
    }
}
