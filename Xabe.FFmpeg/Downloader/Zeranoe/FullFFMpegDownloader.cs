using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xabe.FFmpeg.Downloader.Zeranoe
{
    ///<summary>Download a Fullbuild from zeranoe.com for Windows or macOS</summary>
    internal class FullFFmpegDownloader : FFmpegDownloaderBase
    {
        internal FullFFmpegDownloader(IOperatingSystemProvider operatingSystemProvider = default) : base(operatingSystemProvider)
        {
        }

        private string GenerateLink()
        {
            switch (OperatingSystem)
            {
                case OperatingSystem.Windows64:
                    return "https://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-latest-win64-static.zip";
                case OperatingSystem.Windows32:
                    return "https://ffmpeg.zeranoe.com/builds/win32/static/ffmpeg-latest-win32-static.zip";
                case OperatingSystem.Osx64:
                    return "https://ffmpeg.zeranoe.com/builds/macos64/static/ffmpeg-latest-macos64-static.zip";
                default:
                    throw new NotSupportedException($"The automated download of the full FFMpeg package is not supported for the current Operation System: {OperatingSystem}.");
            }
        }

        public override async Task GetLatestVersion()
        {
            if (!CheckIfFilesExist())
            {
                return;
            }

            string link = GenerateLink();
            var fullPackZip = await DownloadFile(link).ConfigureAwait(false);

            Extract(fullPackZip, FFmpeg.ExecutablesPath ?? ".");
        }

        /// <summary>Extract only binary files from the zip archive</summary>
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

                    zipEntry.ExtractToFile(destinationPath, overwrite: true);
                }
            }

            File.Delete(ffMpegZipPath);
        }
    }
}
