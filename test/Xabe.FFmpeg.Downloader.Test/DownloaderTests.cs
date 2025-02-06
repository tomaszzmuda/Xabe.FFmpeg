using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NSubstitute;
using Xabe.FFmpeg.Downloader.Android;
using Xabe.FFmpeg.Test.Common;
using Xabe.FFmpeg.Test.Common.Fixtures;
using Xunit;

namespace Xabe.FFmpeg.Downloader.Test
{
    public class DownloaderTests : IClassFixture<StorageFixture>
    {
        private readonly StorageFixture _storageFixture;

        private readonly JsonSerializerOptions _defaultSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            WriteIndented = true
        };

        public DownloaderTests(StorageFixture storageFixture)
        {
            _storageFixture = storageFixture;
        }

        [Fact]
        internal async Task FullProcessPassed()
        {
            const OperatingSystem OS = OperatingSystem.Linux64;

            var operatingSystemProvider = Substitute.For<IOperatingSystemProvider>();
            operatingSystemProvider.GetOperatingSystem().Returns(x => OS);
            var downloader = new OfficialFFmpegDownloader(operatingSystemProvider);

            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());

                var ffmpegPath = downloader.ComputeFileDestinationPath("ffmpeg", OS, FFmpeg.ExecutablesPath);
                var ffprobePath = downloader.ComputeFileDestinationPath("ffprobe", OS, FFmpeg.ExecutablesPath);

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
        internal async Task FullProcessPassedWithRetries()
        {
            const OperatingSystem OS = OperatingSystem.Linux64;

            var operatingSystemProvider = Substitute.For<IOperatingSystemProvider>();
            operatingSystemProvider.GetOperatingSystem().Returns(x => OS);
            var downloader = new OfficialFFmpegDownloader(operatingSystemProvider);

            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());

                var ffmpegPath = downloader.ComputeFileDestinationPath("ffmpeg", OS, FFmpeg.ExecutablesPath);
                var ffprobePath = downloader.ComputeFileDestinationPath("ffprobe", OS, FFmpeg.ExecutablesPath);

                // 1- First download

                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, null, 3);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 2- Check updates (same version)

                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, null, 3);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 3- Check updates (outdated version)

                var fFbinariesVersionInfo = new FFbinariesVersionInfo
                {
                    Version = new Version().ToString() // "0.0"
                };
                downloader.SaveVersion(fFbinariesVersionInfo, FFmpeg.ExecutablesPath);

                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, null, 3);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 4- Missing ffmpeg

                File.Delete(ffmpegPath);

                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, null, 3);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 5- Missing ffprobe

                File.Delete(ffprobePath);

                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, null, 3);

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
            const OperatingSystem OS = OperatingSystem.Linux64;

            var operatingSystemProvider = Substitute.For<IOperatingSystemProvider>();
            operatingSystemProvider.GetOperatingSystem().Returns(x => OS);
            var downloader = new OfficialFFmpegDownloader(operatingSystemProvider);

            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());

                var ffmpegPath = downloader.ComputeFileDestinationPath("ffmpeg", OS, FFmpeg.ExecutablesPath);
                var ffprobePath = downloader.ComputeFileDestinationPath("ffprobe", OS, FFmpeg.ExecutablesPath);
                IProgress<ProgressInfo> progress;

                // 1- First download
                progress = new Progress<ProgressInfo>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 2- Check updates (same version)

                progress = new Progress<ProgressInfo>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 3- Check updates (outdated version)

                var fFbinariesVersionInfo = new FFbinariesVersionInfo
                {
                    Version = new Version().ToString() // "0.0"
                };
                downloader.SaveVersion(fFbinariesVersionInfo, FFmpeg.ExecutablesPath);

                progress = new Progress<ProgressInfo>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 4- Missing ffmpeg

                File.Delete(ffmpegPath);

                progress = new Progress<ProgressInfo>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 5- Missing ffprobe

                File.Delete(ffprobePath);

                progress = new Progress<ProgressInfo>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));
            }
            finally
            {
                FFmpeg.SetExecutablesPath(ffmpegExecutablesPath);
            }
        }

        [Fact]
        internal async Task FullProcessPassedWithProgressAndRetries()
        {
            const OperatingSystem OS = OperatingSystem.Linux64;

            var operatingSystemProvider = Substitute.For<IOperatingSystemProvider>();
            operatingSystemProvider.GetOperatingSystem().Returns(x => OS);
            var downloader = new OfficialFFmpegDownloader(operatingSystemProvider);

            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());

                var ffmpegPath = downloader.ComputeFileDestinationPath("ffmpeg", OS, FFmpeg.ExecutablesPath);
                var ffprobePath = downloader.ComputeFileDestinationPath("ffprobe", OS, FFmpeg.ExecutablesPath);
                IProgress<ProgressInfo> progress;

                // 1- First download
                progress = new Progress<ProgressInfo>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress, 3);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 2- Check updates (same version)

                progress = new Progress<ProgressInfo>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress, 3);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 3- Check updates (outdated version)

                var fFbinariesVersionInfo = new FFbinariesVersionInfo
                {
                    Version = new Version().ToString() // "0.0"
                };
                downloader.SaveVersion(fFbinariesVersionInfo, FFmpeg.ExecutablesPath);

                progress = new Progress<ProgressInfo>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress, 3);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 4- Missing ffmpeg

                File.Delete(ffmpegPath);

                progress = new Progress<ProgressInfo>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress, 3);

                Assert.True(File.Exists(ffmpegPath));
                Assert.True(File.Exists(ffprobePath));

                // 5- Missing ffprobe

                File.Delete(ffprobePath);

                progress = new Progress<ProgressInfo>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress, 3);

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
                var currentVersion = JsonSerializer.Deserialize<FFbinariesVersionInfo>(await File.ReadAllTextAsync(Resources.FFbinariesInfo), _defaultSerializerOptions);
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());
                var downloader = new OfficialFFmpegDownloader(operatingSystemProvider);
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
        internal async Task DownloadLatestVersionWithRetriesTest(OperatingSystem os)
        {
            var operatingSystemProvider = Substitute.For<IOperatingSystemProvider>();
            operatingSystemProvider.GetOperatingSystem().Returns(x => os);

            var linkProvider = new LinkProvider(operatingSystemProvider);
            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                var currentVersion = JsonSerializer.Deserialize<FFbinariesVersionInfo>(await File.ReadAllTextAsync(Resources.FFbinariesInfo), _defaultSerializerOptions);
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());
                var downloader = new OfficialFFmpegDownloader(operatingSystemProvider);
                await downloader.DownloadLatestVersion(currentVersion, FFmpeg.ExecutablesPath, null, 3);

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
                var currentVersion = JsonSerializer.Deserialize<FFbinariesVersionInfo>(await File.ReadAllTextAsync(Resources.FFbinariesInfo), _defaultSerializerOptions);
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());
                var downloader = new OfficialFFmpegDownloader(operatingSystemProvider);
                IProgress<ProgressInfo> progress = new Progress<ProgressInfo>();
                await downloader.DownloadLatestVersion(currentVersion, FFmpeg.ExecutablesPath, progress, 0);

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
        internal async Task DownloadLatestVersionWithProgressAndRetriesTest(OperatingSystem os)
        {
            var operatingSystemProvider = Substitute.For<IOperatingSystemProvider>();
            operatingSystemProvider.GetOperatingSystem().Returns(x => os);

            var linkProvider = new LinkProvider(operatingSystemProvider);
            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                var currentVersion = JsonSerializer.Deserialize<FFbinariesVersionInfo>(await File.ReadAllTextAsync(Resources.FFbinariesInfo), _defaultSerializerOptions);
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());
                var downloader = new OfficialFFmpegDownloader(operatingSystemProvider);
                IProgress<ProgressInfo> progress = new Progress<ProgressInfo>();
                await downloader.DownloadLatestVersion(currentVersion, FFmpeg.ExecutablesPath, progress, 3);

                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffmpeg", os, FFmpeg.ExecutablesPath)));
                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffprobe", os, FFmpeg.ExecutablesPath)));
            }
            finally
            {
                FFmpeg.SetExecutablesPath(ffmpegExecutablesPath);
            }
        }

        [Theory]
        [InlineData(OperatingSystemArchitecture.Arm)]
        [InlineData(OperatingSystemArchitecture.Arm64)]
        [InlineData(OperatingSystemArchitecture.X86)]
        [InlineData(OperatingSystemArchitecture.X64)]
        internal async Task DownloadLatestAndroidVersionTest(OperatingSystemArchitecture arch)
        {
            var operatingSystemArchProvider = Substitute.For<IOperatingSystemArchitectureProvider>();
            operatingSystemArchProvider.GetArchitecture().Returns(x => arch);

            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                var currentVersion = JsonSerializer.Deserialize<FFbinariesVersionInfo>(await File.ReadAllTextAsync(Resources.FFbinariesInfo), _defaultSerializerOptions);
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());
                var downloader = new AndroidFFmpegDownloader(operatingSystemArchProvider);
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath);

                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffmpeg", FFmpeg.ExecutablesPath)));
                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffprobe", FFmpeg.ExecutablesPath)));
            }
            finally
            {
                FFmpeg.SetExecutablesPath(ffmpegExecutablesPath);
            }
        }

        [Theory]
        [InlineData(OperatingSystemArchitecture.Arm)]
        [InlineData(OperatingSystemArchitecture.Arm64)]
        [InlineData(OperatingSystemArchitecture.X86)]
        [InlineData(OperatingSystemArchitecture.X64)]
        internal async Task DownloadLatestAndroidVersionWithRetriesTest(OperatingSystemArchitecture arch)
        {
            var operatingSystemArchProvider = Substitute.For<IOperatingSystemArchitectureProvider>();
            operatingSystemArchProvider.GetArchitecture().Returns(x => arch);

            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                var currentVersion = JsonSerializer.Deserialize<FFbinariesVersionInfo>(await File.ReadAllTextAsync(Resources.FFbinariesInfo), _defaultSerializerOptions);
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());
                var downloader = new AndroidFFmpegDownloader(operatingSystemArchProvider);
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, null, 3);

                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffmpeg", FFmpeg.ExecutablesPath)));
                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffprobe", FFmpeg.ExecutablesPath)));
            }
            finally
            {
                FFmpeg.SetExecutablesPath(ffmpegExecutablesPath);
            }
        }

        [Theory]
        [InlineData(OperatingSystemArchitecture.Arm)]
        [InlineData(OperatingSystemArchitecture.Arm64)]
        [InlineData(OperatingSystemArchitecture.X86)]
        [InlineData(OperatingSystemArchitecture.X64)]
        internal async Task DownloadLatestAndroidVersionWithProgressTest(OperatingSystemArchitecture arch)
        {
            var operatingSystemArchProvider = Substitute.For<IOperatingSystemArchitectureProvider>();
            operatingSystemArchProvider.GetArchitecture().Returns(x => arch);

            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                var currentVersion = JsonSerializer.Deserialize<FFbinariesVersionInfo>(await File.ReadAllTextAsync(Resources.FFbinariesInfo), _defaultSerializerOptions);
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());
                var downloader = new AndroidFFmpegDownloader(operatingSystemArchProvider);
                IProgress<ProgressInfo> progress = new Progress<ProgressInfo>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress);

                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffmpeg", FFmpeg.ExecutablesPath)));
                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffprobe", FFmpeg.ExecutablesPath)));
            }
            finally
            {
                FFmpeg.SetExecutablesPath(ffmpegExecutablesPath);
            }
        }

        [Theory]
        [InlineData(OperatingSystemArchitecture.Arm)]
        [InlineData(OperatingSystemArchitecture.Arm64)]
        [InlineData(OperatingSystemArchitecture.X86)]
        [InlineData(OperatingSystemArchitecture.X64)]
        internal async Task DownloadLatestAndroidVersionWithProgressAndRetriesTest(OperatingSystemArchitecture arch)
        {
            var operatingSystemArchProvider = Substitute.For<IOperatingSystemArchitectureProvider>();
            operatingSystemArchProvider.GetArchitecture().Returns(x => arch);

            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                var currentVersion = JsonSerializer.Deserialize<FFbinariesVersionInfo>(await File.ReadAllTextAsync(Resources.FFbinariesInfo), _defaultSerializerOptions);
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());
                var downloader = new AndroidFFmpegDownloader(operatingSystemArchProvider);
                IProgress<ProgressInfo> progress = new Progress<ProgressInfo>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress, 3);

                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffmpeg", FFmpeg.ExecutablesPath)));
                Assert.True(File.Exists(downloader.ComputeFileDestinationPath("ffprobe", FFmpeg.ExecutablesPath)));
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
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());
                var downloader = new FullFFmpegDownloader(operatingSystemProvider);

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
        internal async Task DownloadLatestFullVersionWithRetriesTest(OperatingSystem os)
        {
            var operatingSystemProvider = Substitute.For<IOperatingSystemProvider>();
            operatingSystemProvider.GetOperatingSystem().Returns(x => os);

            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());
                var downloader = new FullFFmpegDownloader(operatingSystemProvider);

                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, null, 3);

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
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());
                var downloader = new FullFFmpegDownloader(operatingSystemProvider);
                IProgress<ProgressInfo> progress = new Progress<ProgressInfo>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress);

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
        internal async Task DownloadLatestFullVersionWithProgressAndRetriesTest(OperatingSystem os)
        {
            var operatingSystemProvider = Substitute.For<IOperatingSystemProvider>();
            operatingSystemProvider.GetOperatingSystem().Returns(x => os);

            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());
                var downloader = new FullFFmpegDownloader(operatingSystemProvider);
                IProgress<ProgressInfo> progress = new Progress<ProgressInfo>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress, 3);

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
                var operatingSystemProviderMock = Substitute.For<IOperatingSystemProvider>();
                operatingSystemProviderMock.GetOperatingSystem().Returns(x => OperatingSystem.Windows64);

                var operatingSystemArchitectureProviderMock = Substitute.For<IOperatingSystemArchitectureProvider>();
                operatingSystemArchitectureProviderMock.GetArchitecture().Returns(x => OperatingSystemArchitecture.X86);

                yield return new object[] { new OfficialFFmpegDownloader(operatingSystemProviderMock) };
                yield return new object[] { new FullFFmpegDownloader(operatingSystemProviderMock) };
                yield return new object[] { new SharedFFmpegDownloader(operatingSystemProviderMock) };
                yield return new object[] { new AndroidFFmpegDownloader(operatingSystemArchitectureProviderMock) };
            }
        }

        [Theory]
        [MemberData(nameof(FFmpegDownloaders))]
        internal async Task DownloadLatestVersion_NoOperatingSystemProviderIsSpecified_UseDefaultOne(IFFmpegDownloader downloader)
        {
            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath);
            }
            finally
            {
                FFmpeg.SetExecutablesPath(ffmpegExecutablesPath);
            }
        }

        [Theory]
        [MemberData(nameof(FFmpegDownloaders))]
        internal async Task DownloadLatestVersion_NoOperatingSystemProviderIsSpecified_UseDefaultOne_WithRetries(IFFmpegDownloader downloader)
        {
            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, null, 3);
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
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());
                IProgress<ProgressInfo> progress = new Progress<ProgressInfo>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress);
            }
            finally
            {
                FFmpeg.SetExecutablesPath(ffmpegExecutablesPath);
            }
        }

        [Theory]
        [MemberData(nameof(FFmpegDownloaders))]
        internal async Task DownloadLatestVersion_NoOperatingSystemProviderIsSpecified_UseDefaultOne_WithProgressAndRetries(IFFmpegDownloader downloader)
        {
            var ffmpegExecutablesPath = FFmpeg.ExecutablesPath;

            try
            {
                FFmpeg.SetExecutablesPath(_storageFixture.GetTempDirectory());
                IProgress<ProgressInfo> progress = new Progress<ProgressInfo>();
                await downloader.GetLatestVersion(FFmpeg.ExecutablesPath, progress, 3);
            }
            finally
            {
                FFmpeg.SetExecutablesPath(ffmpegExecutablesPath);
            }
        }
    }
}
