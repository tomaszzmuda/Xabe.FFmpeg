using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xabe.FFmpeg.Downloader
{
    internal abstract class FFmpegDownloaderBase : IFFMpegDownloader
    {
        protected readonly OperatingSystem s_os = new OperatingSystemProvider().GetOperatingSystem();
        private string FfmpegDestinationPath => ComputeFileDestinationPath("ffmpeg", s_os);
        private string FfprobeDestinationPath => ComputeFileDestinationPath("ffprobe", s_os);

        public abstract Task GetLatestVersion();

        protected bool CheckIfFilesExist()
        {
            return !File.Exists(FfmpegDestinationPath) || !File.Exists(FfprobeDestinationPath);
        }

        internal string ComputeFileDestinationPath(string filename, OperatingSystem os)
        {
            string path = Path.Combine(FFmpeg.ExecutablesPath ?? ".", filename);

            if (os == OperatingSystem.Windows32 || os == OperatingSystem.Windows64)
                path += ".exe";

            return path;
        }

        protected virtual void Extract(string ffMpegZipPath, string destinationDir)
        {
            using (ZipArchive zipArchive = ZipFile.OpenRead(ffMpegZipPath))
            {
                if (!Directory.Exists(destinationDir))
                    Directory.CreateDirectory(destinationDir);

                foreach (ZipArchiveEntry zipEntry in zipArchive.Entries)
                {
                    string destinationPath = Path.Combine(destinationDir, zipEntry.FullName);

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

        protected async Task<string> DownloadFile(string url)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            using (var client = new HttpClient())
            {
                using (var result = await client.GetAsync(url).ConfigureAwait(false))
                {
                    if (!result.IsSuccessStatusCode)
                        return null;
                    var readedData = await result.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                    if (readedData == null)
                        return null;
                    File.WriteAllBytes(tempPath, readedData);
                }
            }

            return tempPath;
        }
    }

}