using System.Text.Json.Serialization;

namespace Xabe.FFmpeg.Downloader
{
    internal class BaseBinaries
    {
        [JsonPropertyName("ffmpeg")]
        public string Ffmpeg { get; set; }

        [JsonPropertyName("ffplay")]
        public string Ffplay { get; set; }

        [JsonPropertyName("ffprobe")]
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
        [JsonPropertyName("ffmpeg")]
        public string Ffmpeg { get; set; }
        [JsonPropertyName("ffplay")]
        public string Ffplay { get; set; }
        [JsonPropertyName("ffprobe")]
        public string Ffprobe { get; set; }
    }

    internal class Bin
    {
        [JsonPropertyName("windows-32")]
        public Windows32 Windows32 { get; set; }

        [JsonPropertyName("windows-64")]
        public Windows64 Windows64 { get; set; }

        [JsonPropertyName("linux-32")]
        public Linux32 Linux32 { get; set; }

        [JsonPropertyName("linux-64")]
        public Linux64 Linux64 { get; set; }

        [JsonPropertyName("linux-armhf")]
        public LinuxArmhf LinuxArmhf { get; set; }

        [JsonPropertyName("linux-armel")]
        public LinuxArmel LinuxArmel { get; set; }

        [JsonPropertyName("linux-arm64")]
        public LinuxArm64 LinuxArm64 { get; set; }

        [JsonPropertyName("osx-64")]
        public Osx64 Osx64 { get; set; }
    }

    internal class Links
    {
        public string FFmpegLink { get; set; }
        public string FFprobeLink { get; set; }
    }

    internal class DownloadedVersion
    {
        public string Version { get; set; }
    }

    internal class FFbinariesVersionInfo
    {
        public string Version { get; set; }

        [JsonPropertyName("bin")]
        public Bin BinariesUrl { get; set; }
    }
}
