using System;
using System.Threading.Tasks;
using Xabe.FFmpeg.Downloader.Android;

namespace Xabe.FFmpeg.Downloader
{
    public abstract class FFmpegDownloader
    {
        /// <summary>
        ///     Download latest FFmpeg version for current operating system to "." directory.
        /// <param name="version">Determine which version of FFmpeg should be downloaded</param>
        /// <param name="progress">Progress of download</param>
        /// </summary>
        public static async Task GetLatestVersion(FFmpegVersion version, IProgress<ProgressInfo> progress = null)
        {
            await GetLatestVersion(version, null, progress);
        }

        /// <summary>
        ///     Download latest FFmpeg version for current operating system to "." directory. Works best with FFmpeg.ExecutablesPath as path
        /// <param name="version">Determine which version of FFmpeg should be downloaded</param>
        /// <param name="path">FFmpeg executables destination directory</param>
        /// <param name="progress">Progress of download</param>
        /// </summary>
        public static async Task GetLatestVersion(FFmpegVersion version, string path, IProgress<ProgressInfo> progress = null)
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
                case FFmpegVersion.Android:
                    downloader = new AndroidFFmpegDownloader();
                    break;
                default:
                    throw new NotImplementedException();
            }
            await downloader.GetLatestVersion(path, progress);
        }
    }
}
