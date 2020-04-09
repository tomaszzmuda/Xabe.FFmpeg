namespace Xabe.FFmpeg.Downloader
{
    internal class LinkProvider
    {
        private IOperatingSystemProvider _operatingSystemProvider;

        internal LinkProvider()
        {
            _operatingSystemProvider = new OperatingSystemProvider();
        }

        internal LinkProvider(IOperatingSystemProvider operatingSystemProvider)
        {
            _operatingSystemProvider = operatingSystemProvider;
        }

        internal Links GetLinks(FFbinariesVersionInfo version)
        {
            var links = new Links();
            switch (_operatingSystemProvider.GetOperatingSystem())
            {
                case OperatingSystem.Windows64:
                    links.FFmpegLink = version.BinariesUrl.Windows64.Ffmpeg;
                    links.FFprobeLink = version.BinariesUrl.Windows64.Ffprobe;
                    break;
                case OperatingSystem.Windows32:
                    links.FFmpegLink = version.BinariesUrl.Windows32.Ffmpeg;
                    links.FFprobeLink = version.BinariesUrl.Windows32.Ffprobe;
                    break;
                case OperatingSystem.Osx64:
                    links.FFmpegLink = version.BinariesUrl.Osx64.Ffmpeg;
                    links.FFprobeLink = version.BinariesUrl.Osx64.Ffprobe;
                    break;
                case OperatingSystem.Linux64:
                    links.FFmpegLink = version.BinariesUrl.Linux64.Ffmpeg;
                    links.FFprobeLink = version.BinariesUrl.Linux64.Ffprobe;
                    break;
                case OperatingSystem.Linux32:
                    links.FFmpegLink = version.BinariesUrl.Linux32.Ffmpeg;
                    links.FFprobeLink = version.BinariesUrl.Linux32.Ffprobe;
                    break;
                case OperatingSystem.LinuxArmhf:
                    links.FFmpegLink = version.BinariesUrl.LinuxArmhf.Ffmpeg;
                    links.FFprobeLink = version.BinariesUrl.LinuxArmhf.Ffprobe;
                    break;
                case OperatingSystem.LinuxArm64:
                    links.FFmpegLink = version.BinariesUrl.LinuxArm64.Ffmpeg;
                    links.FFprobeLink = version.BinariesUrl.LinuxArm64.Ffprobe;
                    break;
            }

            return links;
        }
    }
}
