using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Xabe.FFmpeg.Downloader
{
    internal class SharedFFmpegDownloader : FFmpegDownloaderBase
    {
        internal SharedFFmpegDownloader() : base()
        {
        }

        internal SharedFFmpegDownloader(IOperatingSystemProvider operatingSystemProvider) : base(operatingSystemProvider)
        {
        }

        private string GenerateLink()
        {
            switch (_operatingSystemProvider.GetOperatingSystem())
            {
                case OperatingSystem.Windows64:
                    return "https://xabe.net/ffmpeg/versions/ffmpeg-latest-win64-shared.zip";
                case OperatingSystem.Windows32:
                    return "https://xabe.net/ffmpeg/versions/ffmpeg-latest-win32-shared.zip";
                case OperatingSystem.Osx64:
                    return "https://xabe.net/ffmpeg/versions/ffmpeg-latest-macos64-shared.zip";
                default:
                    throw new NotSupportedException($"The automated download of the full Shared FFmpeg package is not supported for the current Operation System: {_operatingSystemProvider.GetOperatingSystem()}.");
            }
        }

        public override async Task GetLatestVersion(string path, IProgress<ProgressInfo> progress = null, int retries = DEFAULT_MAX_RETRIES)
        {
            if (!CheckIfFilesExist(path))
            {
                return;
            }

            var link = GenerateLink();
            var fullPackZip = await DownloadFile(link, progress, retries);

            Extract(fullPackZip, path ?? ".");
        }

        /// <summary>Extract only binary files from the zip archive</summary>
        protected override void Extract(string ffMpegZipPath, string destinationDir)
        {
            using (ZipArchive zipArchive = ZipFile.OpenRead(ffMpegZipPath))
            {
                if (!Directory.Exists(destinationDir))
                {
                    Directory.CreateDirectory(destinationDir);
                }

                foreach (ZipArchiveEntry zipEntry in zipArchive.Entries.Where(item => item.FullName.Contains("bin")))
                {
                    var destinationPath = Path.Combine(destinationDir, zipEntry.Name);

                    // Archived empty directories have empty Names
                    if (zipEntry.Name == string.Empty)
                    {
                        Directory.CreateDirectory(destinationPath);
                        continue;
                    }

                    zipEntry.ExtractToFile(destinationPath, overwrite: true);
                }
            }

            File.Delete(ffMpegZipPath);
        }
    }
}
