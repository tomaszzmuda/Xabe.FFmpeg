using System;
using System.Threading.Tasks;

namespace Xabe.FFmpeg.Downloader
{
    public abstract class FFmpegDownloader
    {
        /// <summary>
        ///     Download latest FFmpeg version for current operating system to FFmpeg.ExecutablePath. If it is not set download to ".".
        /// <param id="version">Determine which version of FFmpeg should be downloaded</param>
        /// </summary>
        public static async Task GetLatestVersion(FFmpegVersion version)
        {
            await GetLatestVersion(version, null);
        }

        /// <summary>
        ///     Download latest FFmpeg version for current operating system to FFmpeg.ExecutablePath. If it is not set download to ".".
        /// <param id="version">Determine which version of FFmpeg should be downloaded</param>
        /// </summary>
        public static async Task GetLatestVersion(FFmpegVersion version, string path)
        {
            IFFmpegDownloader downloader;
            switch (version)
            {
                case FFmpegVersion.Official:
                    downloader = new OfficialFFmpegDownloader();
                    break;
                case FFmpegVersion.Full:
                    downloader = new FullFFmpegDownloader();
                    break;
                case FFmpegVersion.Shared:
                    downloader = new SharedFFmpegDownloader();
                    break;
                default:
                    throw new NotImplementedException();
            }
            await downloader.GetLatestVersion(path);
        }
    }
}
