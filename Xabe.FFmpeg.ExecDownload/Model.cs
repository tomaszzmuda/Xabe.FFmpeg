using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xabe.FFmpeg.ExecDownload
{
    internal class BaseBinaries
    {
        [JsonProperty(PropertyName = "ffmpeg")]
        public string Ffmpeg { get; set; }

        [JsonProperty(PropertyName = "ffplay")]
        public string Ffplay { get; set; }

        [JsonProperty(PropertyName = "ffprobe")]
        public string Ffprobe { get; set; }
    }

    internal class Windows32 : BaseBinaries
    {
    }

    internal class Windows64 : BaseBinaries
    {
    }

    internal class Linux32 : BaseBinaries
    {
    }

    internal class Linux64 : BaseBinaries
    {
    }

    internal class LinuxArmhf : BaseBinaries
    {
    }

    internal class LinuxArmel : BaseBinaries
    {
    }

    internal class LinuxArm64 : BaseBinaries
    {
    }

    internal class Osx64
    {
        [JsonProperty(PropertyName = "ffmpeg")]
        public string Ffmpeg { get; set; }
        [JsonProperty(PropertyName = "ffplay")]
        public string Ffplay { get; set; }
        [JsonProperty(PropertyName = "ffprobe")]
        public string Ffprobe { get; set; }
    }

    internal class Bin
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

    internal class Links
    {
        public string FFmpegLink { get; set; }
        public string FFprobeLink { get; set; }
    }

    internal class DownloadedVersion
    {
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }
    }

    internal class FFbinariesVersionInfo
    {
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "bin")]
        public Bin BinariesUrl { get; set; }
    }
}
