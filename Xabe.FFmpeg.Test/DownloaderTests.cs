using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NSubstitute;
using Xabe.FFmpeg.Downloader;
using Xabe.FFmpeg.Downloader.Official;
using Xabe.FFmpeg.Downloader.Zeranoe;
using Xunit;
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
            FFmpegDownloader downloader = new FFmpegDownloader
            {
                LinkProvider = new LinkProvider(operatingSystemProvider)
            };
            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                FFmpeg.ExecutablesPath = Path.Combine(Path.GetTempPath(), System.Guid.NewGuid().ToString("N"));

                string ffmpegPath = downloader.ComputeFileDestinationPath("ffmpeg", os);
                string ffprobePath = downloader.ComputeFileDestinationPath("ffprobe", os);

                // 1- First download

                await downloader.GetLatestVersion().ConfigureAwait(false);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 2- Check updates (same version)

                await downloader.GetLatestVersion().ConfigureAwait(false);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 3- Check updates (outdated version)

                var fFbinariesVersionInfo = new FFbinariesVersionInfo
                {
                    Version = new Version().ToString() // "0.0"
                };
                downloader.SaveVersion(fFbinariesVersionInfo);

                await downloader.GetLatestVersion().ConfigureAwait(false);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 4- Missing ffmpeg

                File.Delete(ffmpegPath);

                await downloader.GetLatestVersion().ConfigureAwait(false);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 5- Missing ffprobe

                File.Delete(ffprobePath);

                await downloader.GetLatestVersion().ConfigureAwait(false);

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
                FFmpegDownloader downloader = new FFmpegDownloader();
                downloader.LinkProvider = linkProvider;
                await downloader.DownloadLatestVersion(currentVersion).ConfigureAwait(false);

                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffmpeg", os)));
                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffprobe", os)));
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
        internal async Task DownloadLatestFullVersionTest(OperatingSystem os)
        {
            var operatingSystemProvider = Substitute.For<IOperatingSystemProvider>();
            operatingSystemProvider.GetOperatingSystem().Returns(x => os);

            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                FFmpeg.ExecutablesPath = "assemblies";
                if (Directory.Exists("assemblies"))
                {
                    Directory.Delete("assemblies", true);
                }
                FullFFmpegDownloader downloader = new FullFFmpegDownloader();

                await downloader.GetLatestVersion().ConfigureAwait(false);

                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffmpeg", os)));
                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffprobe", os)));
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
        internal async Task DownloadLatestSharedVersionTest(OperatingSystem os)
        {
            var operatingSystemProvider = Substitute.For<IOperatingSystemProvider>();
            operatingSystemProvider.GetOperatingSystem().Returns(x => os);

            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                FFmpeg.ExecutablesPath = "assemblies";
                if (Directory.Exists("assemblies"))
                {
                    Directory.Delete("assemblies", true);
                }
                SharedFFMpegDownloader downloader = new SharedFFMpegDownloader();
                await downloader.GetLatestVersion().ConfigureAwait(false);

                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffmpeg", os)));
                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffprobe", os)));
            }
            finally
            {
                FFmpeg.ExecutablesPath = ffmpegExecutablesPath;
            }
        }
    }
}
