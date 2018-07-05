using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xabe.FFmpeg.ExecDownload.Tests
{
    public class FFmpegDownloader
    {
        internal static LinkProvider _linkProvider = new LinkProvider();

        internal FFmpegDownloader(LinkProvider linkProvider)
        {
            _linkProvider = linkProvider;
        }

        /// <summary>
        ///     Download latest FFmpeg version for current operating system to FFbase.FFmpegDir. If it is not set download to ".".
        /// </summary>
        public async static Task GetLatestVersion()
        {
            var latestVersion = GetLatestVersionInfo();

            if(!CheckIfUpdateAvaiable(latestVersion.Version))
                return;

            await DownloadLatestVersion(latestVersion);

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

        internal async static Task DownloadLatestVersion(FFbinariesVersionInfo latestFFmpegBinaries)
        {
            var ffProbeZipPath = Path.Combine(Path.GetTempPath(), "FFprobe.zip");

            Links links = _linkProvider.GetLinks(latestFFmpegBinaries);

            var ffmpegZip = await DownloadFile(links.FFmpegLink);
            var ffprobeZip = await DownloadFile(links.FFprobeLink);
            Extract(ffmpegZip, FFbase.FFmpegDir ?? ".");
            Extract(ffprobeZip, FFbase.FFmpegDir ?? ".");

            if(Directory.Exists(Path.Combine(FFbase.FFmpegDir ?? ".", "__MACOSX")))
                Directory.Delete(Path.Combine(FFbase.FFmpegDir ?? ".", "__MACOSX"), true);
        }



        private static void Extract(string ffMpegZipPath, string destinationDir)
        {
            ZipFile.ExtractToDirectory(ffMpegZipPath, destinationDir);
            File.Delete(ffMpegZipPath);
        }

        private static bool CheckIfUpdateAvaiable(string latestVersion)
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

        private static void SaveVersion(FFbinariesVersionInfo latestVersion)
        {
            var versionPath = Path.Combine(FFbase.FFmpegDir ?? ".", "version.json");
            File.WriteAllText(versionPath, JsonConvert.SerializeObject(new DownloadedVersion()
            {
                Version = latestVersion.Version
            }, Formatting.Indented));
        }

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
