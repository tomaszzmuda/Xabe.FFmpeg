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
        public const int DEFAULT_MAX_RETRIES = 6;

        private readonly TimeSpan _initialDelay = TimeSpan.FromSeconds(1);
        private readonly TimeSpan _maxDelay = TimeSpan.FromMinutes(2);
        private const double DELAY_MULTIPLIER = 2.0;

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

        public abstract Task GetLatestVersion(string path, IProgress<ProgressInfo> progress = null, int retries = DEFAULT_MAX_RETRIES);

        protected bool CheckIfFilesExist(string path)
        {
            if (_operatingSystemProvider != null)
            {
                return !File.Exists(ComputeFileDestinationPath("ffmpeg", _operatingSystemProvider.GetOperatingSystem(), path)) || !File.Exists(ComputeFileDestinationPath("ffprobe", _operatingSystemProvider.GetOperatingSystem(), path));
            }
            else if (_operatingSystemArchitectureProvider != null)
            {
                return !File.Exists(ComputeFileDestinationPath("ffmpeg", path)) || !File.Exists(ComputeFileDestinationPath("ffprobe", path));
            }
            else
            {
                return false;
            }
        }

        internal string ComputeFileDestinationPath(string filename, OperatingSystem os, string destinationPath)
        {
            var path = Path.Combine(destinationPath ?? ".", filename);

            if (os == OperatingSystem.Windows32 || os == OperatingSystem.Windows64)
            {
                path += ".exe";
            }

            return path;
        }

        internal string ComputeFileDestinationPath(string filename, string destinationPath)
        {
            return Path.Combine(destinationPath ?? ".", filename);
        }

        protected virtual void Extract(string ffMpegZipPath, string destinationDir)
        {
            using (ZipArchive zipArchive = ZipFile.OpenRead(ffMpegZipPath))
            {
                if (!Directory.Exists(destinationDir))
                {
                    Directory.CreateDirectory(destinationDir);
                }

                foreach (ZipArchiveEntry zipEntry in zipArchive.Entries)
                {
                    var destinationPath = Path.Combine(destinationDir, zipEntry.FullName);

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

        protected async Task<string> DownloadFile(string url, IProgress<ProgressInfo> progress, int retries)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var tryCount = 0;
            TimeSpan retryDelay = _initialDelay;

            using (var client = new HttpClient() { Timeout = Timeout.InfiniteTimeSpan })
            {
                do
                {
                    // Add an exponential delay between subsequent retries
                    await Task.Delay(retryDelay);
                    retryDelay = TimeSpan.FromSeconds(Math.Min(_maxDelay.TotalSeconds, retryDelay.TotalSeconds * DELAY_MULTIPLIER));

                    // Create a file stream to store the downloaded data.
                    // This really can be any type of writeable stream.
                    using (var file = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        try
                        {
                            // Use the custom extension method below to download the data.
                            // The passed progress-instance will receive the download status updates.
                            await client.DownloadAsync(url, file, progress, CancellationToken.None);
                            break;
                        }
                        catch (HttpRequestException) { /* continue to next attempt */ }
                        catch (IOException) { /* continue to next attempt */ }
                    }
                }

                while (++tryCount <= retries);
            }

            return tempPath;
        }
    }
}
