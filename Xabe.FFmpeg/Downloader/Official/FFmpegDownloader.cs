using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xabe.FFmpeg.Downloader.Official
{
    internal class FFmpegDownloader : FFmpegDownloaderBase
    {
        internal LinkProvider LinkProvider { get; set; } = new LinkProvider();

        public override async Task GetLatestVersion()
        {
            var latestVersion = GetLatestVersionInfo();

            if (!CheckIfUpdateAvailable(latestVersion.Version) && !CheckIfFilesExist())
                return;

            await DownloadLatestVersion(latestVersion).ConfigureAwait(false);

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
            Links links = LinkProvider.GetLinks(latestFFmpegBinaries);

            var ffmpegZipDownloadTask = DownloadFile(links.FFmpegLink);
            var ffprobeZipDownloadTask = DownloadFile(links.FFprobeLink);

            var ffmpegZip = await ffmpegZipDownloadTask.ConfigureAwait(false);
            var ffprobeZip = await ffprobeZipDownloadTask.ConfigureAwait(false);

            Extract(ffmpegZip, FFmpeg.ExecutablesPath ?? ".");
            Extract(ffprobeZip, FFmpeg.ExecutablesPath ?? ".");

            if (Directory.Exists(Path.Combine(FFmpeg.ExecutablesPath ?? ".", "__MACOSX")))
                Directory.Delete(Path.Combine(FFmpeg.ExecutablesPath ?? ".", "__MACOSX"), true);
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
