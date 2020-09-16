using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg.Extensions;

namespace Xabe.FFmpeg.Downloader
{
    internal abstract class FFmpegDownloaderBase : IFFmpegDownloader
    {
        protected IOperatingSystemProvider _operatingSystemProvider;
        protected IOperatingSystemArchitectureProvider _operatingSystemArchitectureProvider;

        protected FFmpegDownloaderBase(IOperatingSystemProvider operatingSystemProvider)
        {
            _operatingSystemProvider = operatingSystemProvider;
        }

        protected FFmpegDownloaderBase(IOperatingSystemArchitectureProvider operatingSystemArchitectureProvider)
        {
            _operatingSystemArchitectureProvider = operatingSystemArchitectureProvider;
        }

        protected FFmpegDownloaderBase()
        {
            _operatingSystemProvider = new OperatingSystemProvider();
            _operatingSystemArchitectureProvider = new OperatingSystemArchitectureProvider();
        }

        public abstract Task GetLatestVersion(string path, IProgress<ProgressInfo> progress = null);

        protected bool CheckIfFilesExist(string path)
        {
            if (_operatingSystemProvider != null)
                return !File.Exists(ComputeFileDestinationPath("ffmpeg", _operatingSystemProvider.GetOperatingSystem(), path)) || !File.Exists(ComputeFileDestinationPath("ffprobe", _operatingSystemProvider.GetOperatingSystem(), path));
            else if (_operatingSystemArchitectureProvider != null)
                return !File.Exists(ComputeFileDestinationPath("ffmpeg", _operatingSystemArchitectureProvider.GetArchitecture(), path)) || !File.Exists(ComputeFileDestinationPath("ffprobe", _operatingSystemArchitectureProvider.GetArchitecture(), path));
            else
                return false;
        }

        internal string ComputeFileDestinationPath(string filename, OperatingSystem os, string destinationPath)
        {
            string path = Path.Combine(destinationPath ?? ".", filename);

            if (os == OperatingSystem.Windows32 || os == OperatingSystem.Windows64)
                path += ".exe";

            return path;
        }

        internal string ComputeFileDestinationPath(string filename, OperatingSystemArchitecture arch, string destinationPath)
        {
            return Path.Combine(destinationPath ?? ".", filename);
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

        protected async Task<string> DownloadFile(string url, IProgress<ProgressInfo> progress)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(5);

                // Create a file stream to store the downloaded data.
                // This really can be any type of writeable stream.
                using (var file = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    // Use the custom extension method below to download the data.
                    // The passed progress-instance will receive the download status updates.
                    await client.DownloadAsync(url, file, progress, CancellationToken.None);
                }
            }

            return tempPath;
        }
    }
}
