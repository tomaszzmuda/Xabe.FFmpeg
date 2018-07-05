using System.Linq;
using System.IO;
using Xunit;
using System.Threading.Tasks;
using static Xabe.FFmpeg.ExecDownload.Tests.OperatingSystemProvider;
using NSubstitute;
using Newtonsoft.Json;

namespace Xabe.FFmpeg.ExecDownload.Tests
{
    public class LinkProviderTests
    {
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
            FFbase.FFmpegDir = "assemblies";
            if(Directory.Exists("assemblies"))
            {
                Directory.Delete("assemblies", true);
            }

            LastestFFmpegBinaries2._linkProvider = linkProvider;
            await LastestFFmpegBinaries2.DownloadLatestVersion(currentVersion);

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

        [Fact]
        public async Task TestDownloader()
        {
            await new LastestFFmpegBinaries2(new LinkProvider()).GetLatestVersion();
            //Assert.True(Directory.GetFiles(".").Any(x => x.Equals("ffmpeg") || x.Equals("ffmpeg.exe")));
            //Assert.True(Directory.GetFiles(".").Any(x => x.Equals("ffprobe") || x.Equals("ffprobe.exe")));
        }
    }
}
