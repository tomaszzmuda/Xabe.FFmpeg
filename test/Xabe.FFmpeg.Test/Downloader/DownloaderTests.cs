using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NSubstitute;
using Xabe.FFmpeg.Downloader;
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
            OfficialFFmpegDownloader downloader = new OfficialFFmpegDownloader(operatingSystemProvider);

            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            { 
                FFmpeg.SetExecutablesPath(Path.Combine(Path.GetTempPath(), System.Guid.NewGuid().ToString("N")));

                string ffmpegPath = downloader.ComputeFileDestinationPath("ffmpeg", os, FFmpeg.ExecutablesPath);
                string ffprobePath = downloader.ComputeFileDestinationPath("ffprobe", os, FFmpeg.ExecutablesPath);

                // 1- First download

                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 2- Check updates (same version)

                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 3- Check updates (outdated version)

                var fFbinariesVersionInfo = new FFbinariesVersionInfo
                {
                    Version = new Version().ToString() // "0.0"
                };
                downloader.SaveVersion(fFbinariesVersionInfo, FFmpeg.ExecutablesPath);

                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 4- Missing ffmpeg

                File.Delete(ffmpegPath);

                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 5- Missing ffprobe

                File.Delete(ffprobePath);

                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));
            }
            finally
            {
                FFmpeg.SetExecutablesPath(ffmpegExecutablesPath);
            }
        }

        [Fact]
        internal async Task FullProcessPassedWithProgress()
        {
            const OperatingSystem os = OperatingSystem.Linux64;

            var operatingSystemProvider = Substitute.For<IOperatingSystemProvider>();
            operatingSystemProvider.GetOperatingSystem().Returns(x => os);
            OfficialFFmpegDownloader downloader = new OfficialFFmpegDownloader(operatingSystemProvider);

            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                FFmpeg.SetExecutablesPath(Path.Combine(Path.GetTempPath(), System.Guid.NewGuid().ToString("N")));

                string ffmpegPath = downloader.ComputeFileDestinationPath("ffmpeg", os, FFmpeg.ExecutablesPath);
                string ffprobePath = downloader.ComputeFileDestinationPath("ffprobe", os, FFmpeg.ExecutablesPath);
                IProgress<float> progress;

                // 1- First download
                progress = new Progress<float>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 2- Check updates (same version)

                progress = new Progress<float>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 3- Check updates (outdated version)

                var fFbinariesVersionInfo = new FFbinariesVersionInfo
                {
                    Version = new Version().ToString() // "0.0"
                };
                downloader.SaveVersion(fFbinariesVersionInfo, FFmpeg.ExecutablesPath);

                progress = new Progress<float>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 4- Missing ffmpeg

                File.Delete(ffmpegPath);

                progress = new Progress<float>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 5- Missing ffprobe

                File.Delete(ffprobePath);

                progress = new Progress<float>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));
            }
            finally
            {
                FFmpeg.SetExecutablesPath(ffmpegExecutablesPath);
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
                FFmpeg.SetExecutablesPath("assemblies");
                if (Directory.Exists("assemblies"))
                {
                    Directory.Delete("assemblies", true);
                }
                OfficialFFmpegDownloader downloader = new OfficialFFmpegDownloader(operatingSystemProvider);
                await downloader.DownloadLatestVersion(currentVersion, FFmpeg.ExecutablesPath);

                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffmpeg", os, FFmpeg.ExecutablesPath)));
                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffprobe", os, FFmpeg.ExecutablesPath)));
            }
            finally
            {
                FFmpeg.SetExecutablesPath(ffmpegExecutablesPath);
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
        internal async Task DownloadLatestVersionWithProgressTest(OperatingSystem os)
        {
            var operatingSystemProvider = Substitute.For<IOperatingSystemProvider>();
            operatingSystemProvider.GetOperatingSystem().Returns(x => os);

            var linkProvider = new LinkProvider(operatingSystemProvider);
            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                FFbinariesVersionInfo currentVersion = JsonConvert.DeserializeObject<FFbinariesVersionInfo>(File.ReadAllText(Resources.FFbinariesInfo));
                FFmpeg.SetExecutablesPath("assemblies");
                if (Directory.Exists("assemblies"))
                {
                    Directory.Delete("assemblies", true);
                }
                OfficialFFmpegDownloader downloader = new OfficialFFmpegDownloader(operatingSystemProvider);
                IProgress<float> progress = new Progress<float>();
                await downloader.DownloadLatestVersion(currentVersion, FFmpeg.ExecutablesPath, progress);

                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffmpeg", os, FFmpeg.ExecutablesPath)));
                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffprobe", os, FFmpeg.ExecutablesPath)));
            }
            finally
            {
                FFmpeg.SetExecutablesPath(ffmpegExecutablesPath);
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
                FFmpeg.SetExecutablesPath("assemblies");
                if (Directory.Exists("assemblies"))
                {
                    Directory.Delete("assemblies", true);
                }
                FullFFmpegDownloader downloader = new FullFFmpegDownloader(operatingSystemProvider);

                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath);

                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffmpeg", os, FFmpeg.ExecutablesPath)));
                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffprobe", os, FFmpeg.ExecutablesPath)));
            }
            finally
            {
                FFmpeg.SetExecutablesPath(ffmpegExecutablesPath);
            }
        }

        [Theory]
        [InlineData(OperatingSystem.Windows64)]
        [InlineData(OperatingSystem.Windows32)]
        [InlineData(OperatingSystem.Osx64)]
        internal async Task DownloadLatestFullVersionWithProgressTest(OperatingSystem os)
        {
            var operatingSystemProvider = Substitute.For<IOperatingSystemProvider>();
            operatingSystemProvider.GetOperatingSystem().Returns(x => os);

            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                FFmpeg.SetExecutablesPath("assemblies");
                if (Directory.Exists("assemblies"))
                {
                    Directory.Delete("assemblies", true);
                }
                FullFFmpegDownloader downloader = new FullFFmpegDownloader(operatingSystemProvider);
                IProgress<float> progress = new Progress<float>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress);

                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffmpeg", os, FFmpeg.ExecutablesPath)));
                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffprobe", os, FFmpeg.ExecutablesPath)));
            }
            finally
            {
                FFmpeg.SetExecutablesPath(ffmpegExecutablesPath);
            }
        }


        public static IEnumerable<object[]> FFmpegDownloaders
        {
            get
            {
                yield return new object[] { new OfficialFFmpegDownloader() };
                yield return new object[] { new FullFFmpegDownloader() };
                yield return new object[] { new SharedFFmpegDownloader() };
            }
        }

        [Theory]
        [MemberData(nameof(FFmpegDownloaders))]
        internal async Task DownloadLatestVersion_NoOperatingSystemProviderIsSpecified_UseDefaultOne(IFFmpegDownloader downloader)
        {
            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                FFmpeg.SetExecutablesPath("assemblies");
                if (Directory.Exists("assemblies"))
                {
                    Directory.Delete("assemblies", true);
                }
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath);
            }
            finally
            {
                FFmpeg.SetExecutablesPath(ffmpegExecutablesPath);
            }
        }

        [Theory]
        [MemberData(nameof(FFmpegDownloaders))]
        internal async Task DownloadLatestVersion_NoOperatingSystemProviderIsSpecified_UseDefaultOne_WithProgress(IFFmpegDownloader downloader)
        {
            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                FFmpeg.SetExecutablesPath("assemblies");
                if (Directory.Exists("assemblies"))
                {
                    Directory.Delete("assemblies", true);
                }

                IProgress<float> progress = new Progress<float>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress);
            }
            finally
            {
                FFmpeg.SetExecutablesPath(ffmpegExecutablesPath);
            }
        }
    }
}
