using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        public override async Task GetLatestVersion()
        {
            var latestVersion = GetLatestVersionInfo();

            if (!CheckIfUpdateAvailable(latestVersion.Version) && !CheckIfFilesExist())
                return;

            await DownloadLatestVersion(latestVersion);

            SaveVersion(latestVersion);
        }

        internal FFbinariesVersionInfo GetLatestVersionInfo()
        {
            using (var wc = new WebClient())
            {
                var json = wc.DownloadString("http://ffbinaries.com/api/v1/version/latest");
                return JsonConvert.DeserializeObject<FFbinariesVersionInfo>(json);
            }
        }

        internal async Task DownloadLatestVersion(FFbinariesVersionInfo latestFFmpegBinaries)
        {
            Links links = _linkProvider.GetLinks(latestFFmpegBinaries);

            var ffmpegZipDownloadTask = DownloadFile(links.FFmpegLink);
            var ffprobeZipDownloadTask = DownloadFile(links.FFprobeLink);

            var ffmpegZip = await ffmpegZipDownloadTask;
            var ffprobeZip = await ffprobeZipDownloadTask;

            Extract(ffmpegZip, FFmpeg.ExecutablesPath ?? ".");
            Extract(ffprobeZip, FFmpeg.ExecutablesPath ?? ".");

            File.Delete(ffmpegZip);
            File.Delete(ffprobeZip);
        }

        private bool CheckIfUpdateAvailable(string latestVersion)
        {
            var versionPath = Path.Combine(FFmpeg.ExecutablesPath ?? ".", "version.json");
            if (!File.Exists(versionPath))
                return true;

            FFbinariesVersionInfo currentVersion = JsonConvert.DeserializeObject<FFbinariesVersionInfo>(File.ReadAllText(versionPath));
            if (currentVersion != null)
            {
                if (new Version(latestVersion) > new Version(currentVersion.Version))
                    return true;
            }

            return false;
        }

        internal void SaveVersion(FFbinariesVersionInfo latestVersion)
        {
            var versionPath = Path.Combine(FFmpeg.ExecutablesPath ?? ".", "version.json");
            File.WriteAllText(versionPath, JsonConvert.SerializeObject(new DownloadedVersion()
            {
                Version = latestVersion.Version
            }, Formatting.Indented));
        }
    }
}
