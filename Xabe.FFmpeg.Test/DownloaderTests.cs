using System;
using System.IO;
using Xunit;
using System.Threading.Tasks;
using NSubstitute;
using Newtonsoft.Json;
using Xabe.FFmpeg.Downloader;
using OperatingSystem = Xabe.FFmpeg.Downloader.OperatingSystem;

namespace Xabe.FFmpeg.Test
{
    public class DownloaderTests
    {
        [Fact]
        internal async Task FullProcessPassed()
        {
            const OperatingSystem os = OperatingSystem.Linux64;

            var operatingSystemProvider = Substitute.For<IOperatingSystemProvider>();
            operatingSystemProvider.GetOperatingSystem().Returns(x => os);
            FFmpegDownloader._linkProvider = new LinkProvider(operatingSystemProvider);
            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                FFmpeg.ExecutablesPath = Path.Combine(Path.GetTempPath(), System.Guid.NewGuid().ToString("N"));

                string ffmpegPath = FFmpegDownloader.ComputeFileDestinationPath("ffmpeg", os);
                string ffprobePath = FFmpegDownloader.ComputeFileDestinationPath("ffprobe", os);

                // 1- First download

                await FFmpegDownloader.GetLatestVersion();

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 2- Check updates (same version)

                await FFmpegDownloader.GetLatestVersion();

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 3- Check updates (outdated version)

                var fFbinariesVersionInfo = new FFbinariesVersionInfo
                {
                    Version = new Version().ToString() // "0.0"
                };
                FFmpegDownloader.SaveVersion(fFbinariesVersionInfo);

                await FFmpegDownloader.GetLatestVersion();

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 4- Missing ffmpeg

                File.Delete(ffmpegPath);

                await FFmpegDownloader.GetLatestVersion();

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 5- Missing ffprobe

                File.Delete(ffprobePath);

                await FFmpegDownloader.GetLatestVersion();

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));
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

            try
            {
                FFbinariesVersionInfo currentVersion = JsonConvert.DeserializeObject<FFbinariesVersionInfo>(File.ReadAllText(Resources.FFbinariesInfo));
                FFmpeg.ExecutablesPath = "assemblies";
                if (Directory.Exists("assemblies"))
                {
                    Directory.Delete("assemblies", true);
                }

                FFmpegDownloader._linkProvider = linkProvider;
                await FFmpegDownloader.DownloadLatestVersion(currentVersion);

                Assert.True(File.Exists(FFmpegDownloader.ComputeFileDestinationPath("ffmpeg", os)));
                Assert.True(File.Exists(FFmpegDownloader.ComputeFileDestinationPath("ffprobe", os)));
            }
            finally
            {
                FFmpeg.ExecutablesPath = ffmpegExecutablesPath;
            }
        }
    }
}
