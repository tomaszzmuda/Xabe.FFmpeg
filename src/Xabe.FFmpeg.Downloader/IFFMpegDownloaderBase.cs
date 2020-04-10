using System.Threading.Tasks;

namespace Xabe.FFmpeg.Downloader
{
    /// <summary>Downloads the Latest Version of FFMpeg</summary>
    public interface IFFMpegDownloaderBase
    {
        /// <summary>Do the download and install FFMpeg</summary>
        Task GetLatestVersion();
    }
}
