using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg.Extensions;

namespace Xabe.FFmpeg.Downloader
{
    internal abstract class FFmpegDownloaderBase : IFFmpegDownloader
    {
        public const int DefaultMaxRetries = 6;
        
        private readonly TimeSpan InitialDelay = TimeSpan.FromSeconds(1);
        private readonly TimeSpan MaxDelay = TimeSpan.FromMinutes(2);
        private const double DelayMultiplier = 2.0;

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

        public abstract Task GetLatestVersion(string path, IProgress<ProgressInfo> progress = null, int retries = DefaultMaxRetries);

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

        protected virtual void Extract(string ffMpegZipPath, string destinationDir) => Extract(ffMpegZipPath, destinationDir, _ => true, zipEntry => zipEntry.FullName);

        internal void Extract(string ffMpegZipPath, string destinationDir, Func<ZipArchiveEntry, bool> filter, Func<ZipArchiveEntry, string> getName)
        { 
            destinationDir = Path.GetFullPath(destinationDir);

            using (ZipArchive zipArchive = ZipFile.OpenRead(ffMpegZipPath))
            {
                if (!Directory.Exists(destinationDir))
                    Directory.CreateDirectory(destinationDir);

                foreach (ZipArchiveEntry zipEntry in zipArchive.Entries.Where(filter))
                {
                    // As recomended by the docs(https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchiveentry.fullname?view=net-6.0)
                    // We need to check that the target path is contained within the destination path, otherwise a malicious zip file
                    // could overwrite other files in the system. We start by getting the full path to ensure that any relative segments are removed.

                    string destinationPath = Path.GetFullPath(Path.Combine(destinationDir, getName(zipEntry)));

                    // Ordinal match is safest, as case-sensitive volumes can be mounted within volumes that are case-insensitive.
                    if (destinationPath.StartsWith(destinationDir, StringComparison.Ordinal))
                    {
                        // Archived empty directories have empty Names
                        if (string.IsNullOrEmpty(zipEntry.Name))
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
            }

            File.Delete(ffMpegZipPath);
        }

        protected async Task<string> DownloadFile(string url, IProgress<ProgressInfo> progress, int retries)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            int tryCount = 0;
            TimeSpan retryDelay = InitialDelay;

            using (var client = new HttpClient() { Timeout = Timeout.InfiniteTimeSpan })
            {
                do
                {
                    // Add an exponential delay between subsequent retries
                    await Task.Delay(retryDelay);
                    retryDelay = TimeSpan.FromSeconds(Math.Min(MaxDelay.TotalSeconds, retryDelay.TotalSeconds * DelayMultiplier));

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
