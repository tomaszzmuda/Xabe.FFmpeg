using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Xabe.FFmpeg.Downloader
{
    internal class OfficialFFmpegDownloader : FFmpegDownloaderBase
    {
        private readonly LinkProvider _linkProvider;

        internal OfficialFFmpegDownloader() : base()
        {
            _linkProvider = new LinkProvider(_operatingSystemProvider);
        }

        internal OfficialFFmpegDownloader(IOperatingSystemProvider operatingSystemProvider) : base(operatingSystemProvider)
        {
            _linkProvider = new LinkProvider(operatingSystemProvider);
        }

        public override async Task GetLatestVersion(string path, IProgress<ProgressInfo> progress = null, int retries = DEFAULT_MAX_RETRIES)
        {
            var latestVersion = GetLatestVersionInfo();

            if (!CheckIfUpdateAvailable(latestVersion.Version, path) && !CheckIfFilesExist(path))
            {
                return;
            }

            await DownloadLatestVersion(latestVersion, path, progress, retries);

            SaveVersion(latestVersion, path);
        }

        internal FFbinariesVersionInfo GetLatestVersionInfo()
        {
            using (var wc = new WebClient())
            {
                var json = wc.DownloadString("https://ffbinaries.com/api/v1/version/latest");
                return JsonDocument.Map(json, "ffbinaries manifest", FFbinariesVersionInfo.FromManifest);
            }
        }

        internal async Task DownloadLatestVersion(FFbinariesVersionInfo latestFFmpegBinaries, string path, IProgress<ProgressInfo> progress = null, int retries = DEFAULT_MAX_RETRIES)
        {
            Links links = _linkProvider.GetLinks(latestFFmpegBinaries);

            var ffmpegZipDownloadTask = DownloadFile(links.FFmpegLink, progress, retries);
            var ffprobeZipDownloadTask = DownloadFile(links.FFprobeLink, progress, retries);

            var ffmpegZip = await ffmpegZipDownloadTask;
            var ffprobeZip = await ffprobeZipDownloadTask;

            Extract(ffmpegZip, path ?? ".");
            Extract(ffprobeZip, path ?? ".");

            File.Delete(ffmpegZip);
            File.Delete(ffprobeZip);
        }

        private bool CheckIfUpdateAvailable(string latestVersion, string path)
        {
            var versionPath = Path.Combine(path ?? ".", "version.json");
            if (!File.Exists(versionPath))
            {
                return true;
            }

            var savedVersion = JsonDocument.Map(File.ReadAllText(versionPath), "version.json", FFbinariesVersionInfo.ReadSavedVersion);
            if (savedVersion != null && new Version(latestVersion) <= new Version(savedVersion))
            {
                return false;
            }

            return true;
        }

        internal void SaveVersion(FFbinariesVersionInfo latestVersion, string path)
        {
            var versionPath = Path.Combine(path ?? ".", "version.json");

            File.WriteAllText(versionPath, FFbinariesVersionInfo.RenderSavedVersion(latestVersion.Version));
        }
    }
}
