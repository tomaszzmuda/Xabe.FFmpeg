using System.Threading.Tasks;

namespace Xabe.FFmpeg.Downloader
{
    /// <summary>Downloads the Latest Version of FFMpeg</summary>
    public interface IFFmpegDownloader
    {
        /// <summary>Do the download and install FFMpeg</summary>
        Task GetLatestVersion();
    }
}
