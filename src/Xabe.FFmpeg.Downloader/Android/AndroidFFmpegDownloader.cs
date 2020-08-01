using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Xabe.FFmpeg.Downloader.Android
{
    internal class AndroidFFmpegDownloader : FFmpegDownloaderBase
    {
        private OperatingSystemArchitectureProvider OperatingSystemArchProvider { get; set; }

        public AndroidFFmpegDownloader()
        {
            OperatingSystemArchProvider = new OperatingSystemArchitectureProvider();
        }

        private string GenerateLink(OperatingSystemArchitecture arch)
        {
            if(arch == OperatingSystemArchitecture.Arm64)
            {
                return "https://xabe.net/ffmpeg/versions/ffmpeg-android-arm64.zip";
            }

            if (arch == OperatingSystemArchitecture.Arm)
            {
                return "https://xabe.net/ffmpeg/versions/ffmpeg-android-arm.zip";
            }

            if (arch == OperatingSystemArchitecture.X86)
            {
                return "https://xabe.net/ffmpeg/versions/ffmpeg-android-x86.zip";
            }

            if (arch == OperatingSystemArchitecture.X64)
            {
                return "https://xabe.net/ffmpeg/versions/ffmpeg-android-x86_64.zip";
            }

            return string.Empty;
        }

        public override async Task GetLatestVersion(string path)
        {
            OperatingSystemArchitecture arch = OperatingSystemArchProvider.GetArchitecture();

            await GetLatestVersionForArchitecture(path, arch);
        }

        protected async Task GetLatestVersionForArchitecture(string path, OperatingSystemArchitecture arch)
        {
            if (!CheckIfFilesExist(path))
            {
                return;
            }

            string link = GenerateLink(arch);
            var fullPackZip = await DownloadFile(link, arch);

            Extract(fullPackZip, path ?? ".");
        }

        protected async Task<string> DownloadFile(string url, OperatingSystemArchitecture arch)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(15);
                using (var result = await client.GetAsync(url))
                {
                    if (!result.IsSuccessStatusCode)
                        return null;
                    var readedData = await result.Content.ReadAsByteArrayAsync();
                    if (readedData == null)
                        return null;
                    File.WriteAllBytes(tempPath, readedData);
                }
            }

            return tempPath;
        }
    }
}
