using System.IO;
using Xunit;
using System.Threading.Tasks;
using NSubstitute;
using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;

namespace Xabe.FFmpeg.ExecDownload.Tests
{
    public class LinkProviderTests
    {
        [Fact]
        internal async Task FullProcessPassed()
        {
            FFmpeg.ExecutablePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            await FFmpegDownloader.GetLatestVersion();

            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            { 
                Assert.True(File.Exists(Path.Combine(FFmpeg.ExecutablePath, "ffmpeg.exe")));
                Assert.True(File.Exists(Path.Combine(FFmpeg.ExecutablePath, "ffprobe.exe")));
            }
            else
            {
                Assert.True(File.Exists(Path.Combine(FFmpeg.ExecutablePath, "ffmpeg")));
                Assert.True(File.Exists(Path.Combine(FFmpeg.ExecutablePath, "ffprobe")));
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
        internal async Task LinkProviderTest(OperatingSystem os)
        {
            var operatingSystemProvider = Substitute.For<IOperatingSystemProvider>();
            operatingSystemProvider.GetOperatingSystem().Returns(x => os);

            var linkProvider = new LinkProvider(operatingSystemProvider);

            FFbinariesVersionInfo currentVersion = JsonConvert.DeserializeObject<FFbinariesVersionInfo>(File.ReadAllText("ffbinaries.json"));
            FFmpeg.ExecutablePath = "assemblies";
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
    }
}
