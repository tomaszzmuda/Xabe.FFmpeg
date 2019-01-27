using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xabe.FFmpeg.Downloader
{
    internal class FFmpegDownloader
    {
        internal static LinkProvider _linkProvider = new LinkProvider();
        private static readonly OperatingSystem _os = new OperatingSystemProvider().GetOperatingSystem();

        private static string FfmpegDestinationPath => ComputeFileDestinationPath("ffmpeg", _os);
        private static string FfprobeDestinationPath => ComputeFileDestinationPath("ffprobe", _os);

        internal async static Task GetLatestVersion()
        {
            var latestVersion = GetLatestVersionInfo();

            if(!CheckIfUpdateAvailable(latestVersion.Version) && !CheckIfFilesExist())
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
            Extract(ffmpegZip, FFmpeg.ExecutablesPath ?? ".");
            Extract(ffprobeZip, FFmpeg.ExecutablesPath ?? ".");

            if(Directory.Exists(Path.Combine(FFmpeg.ExecutablesPath ?? ".", "__MACOSX")))
                Directory.Delete(Path.Combine(FFmpeg.ExecutablesPath ?? ".", "__MACOSX"), true);
        }

        private static void Extract(string ffMpegZipPath, string destinationDir)
        {
            using(ZipArchive zipArchive = ZipFile.OpenRead(ffMpegZipPath))
            {
                if(!Directory.Exists(destinationDir))
                    Directory.CreateDirectory(destinationDir);

                foreach(ZipArchiveEntry zipEntry in zipArchive.Entries)
                {
                    string destinationPath = Path.Combine(destinationDir, zipEntry.FullName);

                    // Archived empty directories have empty Names
                    if(zipEntry.Name == string.Empty)
                    {
                        Directory.CreateDirectory(destinationPath);
                        continue;
                    }

                    zipEntry.ExtractToFile(destinationPath, overwrite: true);
                }
            }

            File.Delete(ffMpegZipPath);
        }

        private static bool CheckIfFilesExist()
        {
            return !File.Exists(FfmpegDestinationPath) && !File.Exists(FfprobeDestinationPath);
        }

        internal static string ComputeFileDestinationPath(string filename, OperatingSystem os)
        {
            string path = Path.Combine(FFmpeg.ExecutablesPath ?? ".", filename);
            
            if(os == OperatingSystem.Windows32 || os == OperatingSystem.Windows64)
                path += ".exe";

            return path;
        }

        private static bool CheckIfUpdateAvailable(string latestVersion)
        {
            var versionPath = Path.Combine(FFmpeg.ExecutablesPath ?? ".", "version.json");
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

        internal static void SaveVersion(FFbinariesVersionInfo latestVersion)
        {
            var versionPath = Path.Combine(FFmpeg.ExecutablesPath ?? ".", "version.json");
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
