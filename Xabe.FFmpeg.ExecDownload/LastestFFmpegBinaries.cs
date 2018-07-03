using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xabe.FFmpeg.ExecDownload
{
    public class BaseBinaries
    {
        [JsonProperty(PropertyName = "ffmpeg")]
        public string Ffmpeg { get; set; }

        [JsonProperty(PropertyName = "ffplay")]
        public string Ffplay { get; set; }

        [JsonProperty(PropertyName = "ffprobe")]
        public string Ffprobe { get; set; }
    }

    public class Windows32 : BaseBinaries
    {
    }

    public class Windows64 : BaseBinaries
    {
    }

    public class Linux32 : BaseBinaries
    {
    }

    public class Linux64 : BaseBinaries
    {
    }

    public class LinuxArmhf : BaseBinaries
    {
    }

    public class LinuxArmel : BaseBinaries
    {
    }

    public class LinuxArm64 : BaseBinaries
    {
    }

    public class Osx64
    {
        [JsonProperty(PropertyName = "ffmpeg")]
        public string Ffmpeg { get; set; }
        [JsonProperty(PropertyName = "ffplay")]
        public string Ffplay { get; set; }
        [JsonProperty(PropertyName = "ffprobe")]
        public string Ffprobe { get; set; }
    }

    public class Bin
    {
        [JsonProperty(PropertyName = "windows-32")]
        public Windows32 Windows32 { get; set; }

        [JsonProperty(PropertyName = "windows-64")]
        public Windows64 Windows64 { get; set; }

        [JsonProperty(PropertyName = "linux-32")]
        public Linux32 Linux32 { get; set; }

        [JsonProperty(PropertyName = "linux-64")]
        public Linux64 Linux64 { get; set; }

        [JsonProperty(PropertyName = "linux-armhf")]
        public LinuxArmhf LinuxArmhf { get; set; }

        [JsonProperty(PropertyName = "linux-armel")]
        public LinuxArmel LinuxArmel { get; set; }

        [JsonProperty(PropertyName = "linux-arm64")]
        public LinuxArm64 LinuxArm64 { get; set; }

        [JsonProperty(PropertyName = "osx-64")]
        public Osx64 Osx64 { get; set; }
    }

    public class DownloadedVersion
    {
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }
    }

    public class LastestFFmpegBinaries
    {
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "permalink")]
        public string Permalink { get; set; }

        [JsonProperty(PropertyName = "bin")]
        public Bin BinariesUrl { get; set; }

        /// <summary>
        /// Acquires the lastests binaries.
        /// </summary>
        public static void AcquireLastestsBinaries()
        {
            // Download lastest version of FFMpeg && FFProbe from "http://ffbinaries.com/api/v1/version/latest"
            using(var wc = new WebClient())
            {
                var json = wc.DownloadString("http://ffbinaries.com/api/v1/version/latest");
                var obj = JsonConvert.DeserializeObject<LastestFFmpegBinaries>(json);
                if(obj == null)
                    return;

                if(!UpgradeIfNecessary(new Version(obj.Version)))
                    return;

                // Download Files for os version
                DownloadApplications(obj);
            }
        }

        /// <summary>
        /// Downloads the applications.
        /// </summary>
        /// <param name="obj">The object.</param>
        private static void DownloadApplications(LastestFFmpegBinaries obj)
        {
            var ffMpegZipPath = Path.Combine(Path.GetTempPath(), "FFMpeg.zip");
            var ffProbeZipPath = Path.Combine(Path.GetTempPath(), "FFProbe.zip");
            string urlFfmpeg = string.Empty, urlFfprobe = string.Empty;
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if(RuntimeInformation.OSArchitecture == Architecture.X64)
                {
                    urlFfmpeg = obj.BinariesUrl.Windows64.Ffmpeg;
                    urlFfprobe = obj.BinariesUrl.Windows64.Ffprobe;
                }
                else if(RuntimeInformation.OSArchitecture == Architecture.X64)
                {
                    urlFfmpeg = obj.BinariesUrl.Windows32.Ffmpeg;
                    urlFfprobe = obj.BinariesUrl.Windows32.Ffprobe;
                }
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                urlFfmpeg = obj.BinariesUrl.Osx64.Ffmpeg;
                urlFfprobe = obj.BinariesUrl.Osx64.Ffprobe;
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                switch(RuntimeInformation.OSArchitecture)
                {
                    case Architecture.X64:
                        urlFfmpeg = obj.BinariesUrl.Linux64.Ffmpeg;
                        urlFfprobe = obj.BinariesUrl.Linux64.Ffprobe;
                        break;
                    case Architecture.X86:
                        urlFfmpeg = obj.BinariesUrl.Linux32.Ffmpeg;
                        urlFfprobe = obj.BinariesUrl.Linux32.Ffprobe;
                        break;
                    case Architecture.Arm:
                        urlFfmpeg = obj.BinariesUrl.LinuxArmhf.Ffmpeg;
                        urlFfprobe = obj.BinariesUrl.LinuxArmhf.Ffprobe;
                        break;
                    case Architecture.Arm64:
                        urlFfmpeg = obj.BinariesUrl.LinuxArm64.Ffmpeg;
                        urlFfprobe = obj.BinariesUrl.LinuxArm64.Ffprobe;
                        break;
                }

                // TODO : How to distinct Tizen / Raspberry architecture
                // Linux (Armet) (Tizen)
                // Linux (LinuxArmhf) (for glibc based OS) -> Raspberry Pi
            }

            if(!string.IsNullOrEmpty(urlFfmpeg) &&
                DownloadFile(urlFfmpeg, ffMpegZipPath).Result)
            {
                ZipFile.ExtractToDirectory(ffMpegZipPath, FFbase.FFmpegDir);
                File.Delete(ffMpegZipPath);
            }

            if(!string.IsNullOrEmpty(urlFfprobe) &&
                DownloadFile(urlFfprobe, ffProbeZipPath).Result)
            {
                ZipFile.ExtractToDirectory(ffProbeZipPath, FFbase.FFmpegDir);
                File.Delete(ffProbeZipPath);
            }

            // Clean Folder
            if(Directory.Exists(Path.Combine(FFbase.FFmpegDir, "__MACOSX")))
                Directory.Delete(Path.Combine(FFbase.FFmpegDir, "__MACOSX"), true);

            // Save current version
            SaveVersion(obj);
        }

        /// <summary>
        /// Upgrades if necessary.
        /// </summary>
        /// <param name="remoteVersion">The remote version.</param>
        /// <returns></returns>
        private static bool UpgradeIfNecessary(Version remoteVersion)
        {
            var versionPath = Path.Combine(FFbase.FFmpegDir, "version.json");
            if(!File.Exists(versionPath))
                return true;

            var obj = JsonConvert.DeserializeObject<LastestFFmpegBinaries>(File.ReadAllText(versionPath));
            if(obj != null)
            {
                if(remoteVersion > new Version(obj.Version))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Saves the version.
        /// </summary>
        /// <param name="obj">The object.</param>
        private static void SaveVersion(LastestFFmpegBinaries obj)
        {
            var versionPath = Path.Combine(FFbase.FFmpegDir, "version.json");
            File.WriteAllText(versionPath, JsonConvert.SerializeObject(new DownloadedVersion()
            {
                Version = obj.Version
            }, Formatting.Indented));
        }

        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private static async Task<bool> DownloadFile(string url, string path)
        {
            using(var client = new HttpClient())
            {
                using(var result = await client.GetAsync(url))
                {
                    if(!result.IsSuccessStatusCode)
                        return false;
                    var readedData = await result.Content.ReadAsByteArrayAsync();
                    if(readedData == null)
                        return false;
                    File.WriteAllBytes(path, readedData);
                    return true;
                }
            }
        }
    }


}
