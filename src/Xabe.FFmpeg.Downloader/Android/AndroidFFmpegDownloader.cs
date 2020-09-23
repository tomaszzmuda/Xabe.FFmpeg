using System;
using System.Threading.Tasks;

namespace Xabe.FFmpeg.Downloader.Android
{
    internal class AndroidFFmpegDownloader : FFmpegDownloaderBase
    {
        public AndroidFFmpegDownloader() : base()
        {

        }

        public AndroidFFmpegDownloader(IOperatingSystemArchitectureProvider archProvider) : base(archProvider)
        {

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

        public override async Task GetLatestVersion(string path, IProgress<ProgressInfo> progress = null, int retries = DefaultMaxRetries)
        {
            OperatingSystemArchitecture arch = _operatingSystemArchitectureProvider.GetArchitecture();

            await GetLatestVersionForArchitecture(path, arch, progress, retries);
        }

        protected async Task GetLatestVersionForArchitecture(string path, OperatingSystemArchitecture arch, IProgress<ProgressInfo> progress = null, int retries = DefaultMaxRetries)
        {
            if (!CheckIfFilesExist(path))
            {
                return;
            }

            string link = GenerateLink(arch);
            var fullPackZip = await DownloadFile(link, progress, retries);

            Extract(fullPackZip, path ?? ".");
        }
    }
}
