using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Xabe.FFmpeg.Downloader
{
    internal class FullFFmpegDownloader : FFmpegDownloaderBase
    {
        internal FullFFmpegDownloader() : base()
        {

        }

        internal FullFFmpegDownloader(IOperatingSystemProvider operatingSystemProvider) : base(operatingSystemProvider)
        {
        }

        private string GenerateLink()
        {
            switch (_operatingSystemProvider.GetOperatingSystem())
            {
                case OperatingSystem.Windows64:
                    return "https://xabe.net/ffmpeg/versions/ffmpeg-latest-win64-static.zip";
                case OperatingSystem.Windows32:
                    return "https://xabe.net/ffmpeg/versions/ffmpeg-latest-win32-static.zip";
                case OperatingSystem.Osx64:
                    return "https://xabe.net/ffmpeg/versions/ffmpeg-latest-macos64-static.zip";
                default:
                    throw new NotSupportedException($"The automated download of the full FFmpeg package is not supported for the current Operation System: {_operatingSystemProvider.GetOperatingSystem()}.");
            }
        }

        public override async Task GetLatestVersion(string path, IProgress<ProgressInfo> progress = null, int retries = DefaultMaxRetries)
        {
            if (!CheckIfFilesExist(path))
            {
                return;
            }

            string link = GenerateLink();
            var fullPackZip = await DownloadFile(link, progress, retries);

            Extract(fullPackZip, path ?? ".");
        }

        protected override void Extract(string ffMpegZipPath, string destinationDir)
        {
            using (ZipArchive zipArchive = ZipFile.OpenRead(ffMpegZipPath))
            {
                if (!Directory.Exists(destinationDir))
                    Directory.CreateDirectory(destinationDir);

                foreach (ZipArchiveEntry zipEntry in zipArchive.Entries.Where(item => item.FullName.Contains("bin")))
                {
                    string destinationPath = Path.Combine(destinationDir, zipEntry.Name);

                    // Archived empty directories have empty Names
                    if (zipEntry.Name == string.Empty)
                    {
                        Directory.CreateDirectory(destinationPath);
                        continue;
                    }

                    var directoryPath = Path.GetDirectoryName(destinationPath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    zipEntry.ExtractToFile(destinationPath, overwrite: true);
                }
            }

            File.Delete(ffMpegZipPath);
        }
    }
}
