using System.IO;
using Xunit;
using System.Threading.Tasks;
using NSubstitute;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using Xabe.FFmpeg.Downloader;

namespace Xabe.FFmpeg.Test
{
    public class DownloaderTests
    {
        [Fact]
        internal async Task FullProcessPassed()
        {
            var operatingSystemProvider = Substitute.For<IOperatingSystemProvider>();
            operatingSystemProvider.GetOperatingSystem().Returns(x => OperatingSystem.Linux64);
            FFmpegDownloader._linkProvider = new LinkProvider(operatingSystemProvider);
            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                FFmpeg.ExecutablesPath = Path.Combine(Path.GetTempPath(), System.Guid.NewGuid().ToString("N"));

                await FFmpegDownloader.GetLatestVersion();

                Assert.True(File.Exists(Path.Combine(FFmpeg.ExecutablesPath, "ffmpeg")));
                Assert.True(File.Exists(Path.Combine(FFmpeg.ExecutablesPath, "ffprobe")));
            }
            finally
            {
                FFmpeg.ExecutablesPath = ffmpegExecutablesPath;
            }
        }

        [Theory]
        [InlineData(OperatingSystem.Windows64)]
        [InlineData(OperatingSystem.Windows32)]
        [InlineData(OperatingSystem.Osx64)]
        [InlineData(OperatingSystem.Linux64)]
        [InlineData(OperatingSystem.LinuxArm64)]
        [InlineData(OperatingSystem.LinuxArmhf)]
        [InlineData(OperatingSystem.Linux32)]
        internal async Task DownloadLatestVersionTest(OperatingSystem os)
        {
            var operatingSystemProvider = Substitute.For<IOperatingSystemProvider>();
            operatingSystemProvider.GetOperatingSystem().Returns(x => os);

            var linkProvider = new LinkProvider(operatingSystemProvider);
            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try { 
            FFbinariesVersionInfo currentVersion = JsonConvert.DeserializeObject<FFbinariesVersionInfo>(File.ReadAllText(Resources.FFbinariesInfo));
            FFmpeg.ExecutablesPath = "assemblies";
            if(Directory.Exists("assemblies"))
            {
                Directory.Delete("assemblies", true);
            }

            FFmpegDownloader._linkProvider = linkProvider;
            await FFmpegDownloader.DownloadLatestVersion(currentVersion);

            if(os == OperatingSystem.Windows32 || os == OperatingSystem.Windows64)
            {
                Assert.True(File.Exists(Path.Combine("assemblies", "ffmpeg.exe")));
                Assert.True(File.Exists(Path.Combine("assemblies", "ffprobe.exe")));
            }
            else
            {
                Assert.True(File.Exists(Path.Combine("assemblies", "ffmpeg")));
                Assert.True(File.Exists(Path.Combine("assemblies", "ffprobe")));
            }
            }
            finally
            {
                FFmpeg.ExecutablesPath = ffmpegExecutablesPath;
            }
        }
    }
}
