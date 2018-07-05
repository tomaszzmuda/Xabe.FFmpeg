using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xabe.FFmpeg.ExecDownload.Tests
{
    public class LastestFFmpegBinaries2
    {
        internal static LinkProvider _linkProvider = new LinkProvider();

        public LastestFFmpegBinaries2(LinkProvider linkProvider)
        {
            _linkProvider = linkProvider;
        }

        /// <summary>
        /// Acquires the latests binaries.
        /// </summary>
        public async Task GetLatestVersion()
        {
            var latestVersion = GetLatestVersionInfo();

            if(!CheckIfUpdateAvaiable(latestVersion.Version))
                return;

            // Download Files for os version
            await DownloadLatestVersion(latestVersion);

            // Save current version
            SaveVersion(latestVersion);
        }

        internal static FFbinariesVersionInfo GetLatestVersionInfo()
        {
            using(var wc = new WebClient())
            {
                var json = wc.DownloadString("http://ffbinaries.com/api/v1/version/latest");
                return JsonConvert.DeserializeObject<FFbinariesVersionInfo>(json);
            }
        }

        /// <summary>
        /// Downloads the applications.
        /// </summary>
        /// <param name="latestFFmpegBinaries">The object.</param>
        internal async static Task DownloadLatestVersion(FFbinariesVersionInfo latestFFmpegBinaries)
        {
            var ffProbeZipPath = Path.Combine(Path.GetTempPath(), "FFprobe.zip");

            Links links = _linkProvider.GetLinks(latestFFmpegBinaries);

            var ffmpegZip = await DownloadFile(links.FFmpegLink);
            var ffprobeZip = await DownloadFile(links.FFprobeLink);
            Extract(ffmpegZip, FFbase.FFmpegDir ?? ".");
            Extract(ffprobeZip, FFbase.FFmpegDir ?? ".");

            // Clean Folder
            if(Directory.Exists(Path.Combine(FFbase.FFmpegDir ?? ".", "__MACOSX")))
                Directory.Delete(Path.Combine(FFbase.FFmpegDir ?? ".", "__MACOSX"), true);
        }



        private static void Extract(string ffMpegZipPath, string destinationDir)
        {
                ZipFile.ExtractToDirectory(ffMpegZipPath, destinationDir);
                File.Delete(ffMpegZipPath);
        }

        /// <summary>
        /// Upgrades if necessary.
        /// </summary>
        /// <param name="remoteVersion">The remote version.</param>
        /// <returns></returns>
        private bool CheckIfUpdateAvaiable(string latestVersion)
        {
            var versionPath = Path.Combine(FFbase.FFmpegDir ?? ".", "version.json");
            if(!File.Exists(versionPath))
                return true;

            FFbinariesVersionInfo currentVersion = JsonConvert.DeserializeObject<FFbinariesVersionInfo>(File.ReadAllText(versionPath));
            if(currentVersion != null)
            {
                if(new Version(latestVersion) > new Version(currentVersion.Version))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Saves the version.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void SaveVersion(FFbinariesVersionInfo latestVersion)
        {
            var versionPath = Path.Combine(FFbase.FFmpegDir ?? ".", "version.json");
            File.WriteAllText(versionPath, JsonConvert.SerializeObject(new DownloadedVersion()
            {
                Version = latestVersion.Version
            }, Formatting.Indented));
        }

        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private async static Task<string> DownloadFile(string url)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            using(var client = new HttpClient())
            {
                using(var result = await client.GetAsync(url))
                {
                    if(!result.IsSuccessStatusCode)
                        return null;
                    var readedData = await result.Content.ReadAsByteArrayAsync();
                    if(readedData == null)
                        return null;
                    File.WriteAllBytes(tempPath, readedData);
                }
            }

            return tempPath;
        }
    }


}
