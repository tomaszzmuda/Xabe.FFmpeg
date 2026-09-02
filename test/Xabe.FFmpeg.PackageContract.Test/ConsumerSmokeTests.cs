using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Xabe.FFmpeg.PackageContract.Test
{
    /// <summary>
    ///     Near-fresh-consumer proofs, executed only in CI/release pipelines
    ///     (XABE_SMOKE_ENABLE=true): a brand-new project restores the packages from a local feed,
    ///     compiles, runs a probe and a conversion against the pinned FFmpeg binary, and proves the
    ///     loaded assemblies come byte-for-byte from the produced packages.
    /// </summary>
    [Collection("package-contract")]
    public class ConsumerSmokeTests
    {
        private static readonly string[] BuildArgs =
        {
            "build", "-c", "Release", "--nologo", "-v", "minimal"
        };

        private static readonly string[] CoreSmokeMarkers =
        {
            "PROBE-OK", "CONVERT-OK", "ASM-SHA256"
        };

        private static readonly string[] DownloaderSmokeMarkers =
        {
            "DOWNLOAD-OK", "DISCOVER-OK", "CORE-ASM-SHA256"
        };

        private readonly string _scratch;
        private readonly string _feed;
        private readonly string _mediaFile;
        private readonly string _coreVersion;
        private readonly string _downloaderVersion;
        private readonly byte[] _coreDllBytes;

        public ConsumerSmokeTests(PackArtifacts artifacts)
        {
            _scratch = Path.Combine(PackArtifacts.GetScratchRoot(), "consumers");
            _feed = artifacts.PackageDir;
            _mediaFile = Path.Combine(
                artifacts.RepoRoot, "test", "Xabe.FFmpeg.Test.Common", "Resources", "SampleVideo_360x240_1mb.mkv");
            _coreVersion = PackArtifacts.GetVersion(artifacts.Nupkgs[PackArtifacts.CoreId], PackArtifacts.CoreId);
            _downloaderVersion = PackArtifacts.GetVersion(artifacts.Nupkgs[PackArtifacts.DownloaderId], PackArtifacts.DownloaderId);
            _coreDllBytes = ZipHelper.ReadEntry(artifacts.Nupkgs[PackArtifacts.CoreId], "lib/netstandard2.0/Xabe.FFmpeg.dll");
        }

        private bool Enabled => string.Equals(
            Environment.GetEnvironmentVariable("XABE_SMOKE_ENABLE"), "true", StringComparison.OrdinalIgnoreCase);

        [Fact]
        public void FreshCoreConsumerBuildsRunsAndProvesTheProducedPackage()
        {
            if (!Enabled)
            {
                return; // smoke evidence is collected in CI/release pipelines
            }

            EnsureMediaAvailable();

            string projectDir = Path.Combine(_scratch, "core");
            Scaffold(projectDir, "XabSmokeCore", PackArtifacts.CoreId, _coreVersion, _feed,
                ConsumerScaffolding.CoreProgram);

            string[] buildArgs = InsertProjectAfter(BuildArgs, "XabSmokeCore.csproj");
            (int exit, string output) = ProcessUtil.Run(
                "dotnet",
                buildArgs,
                TimeSpan.FromMinutes(15),
                "build fresh Xabe.FFmpeg consumer",
                psi =>
                {
                    psi.WorkingDirectory = projectDir;
                    psi.Environment["NUGET_PACKAGES"] = Path.Combine(_scratch, "core-pkgs");
                });

            Assert.True(exit == 0, "fresh consumer restore/build failed:\n" + ProcessUtil.Tail(output));

            string consumerDll = Path.Combine(projectDir, "bin", "Release", "net8.0", "XabSmokeCore.dll");
            Assert.True(File.Exists(consumerDll), "built consumer output missing at " + consumerDll);

            (exit, output) = ProcessUtil.Run(
                "dotnet",
                new[] { consumerDll, _mediaFile },
                TimeSpan.FromMinutes(15),
                "run fresh Xabe.FFmpeg consumer",
                psi => ConfigureExecutables(psi));

            AssertProvedConsumed(exit, output, CoreSmokeMarkers, "ASM-LOCATION");
        }

        [Fact]
        public void FreshDownloaderConsumerDownloadsPinnedBinariesAndResolvesCoreFromTheFeed()
        {
            if (!Enabled)
            {
                return; // smoke evidence is collected in CI/release pipelines
            }

            EnsureMediaAvailable();

            string projectDir = Path.Combine(_scratch, "downloader");
            Scaffold(projectDir, "XabSmokeDownloader", PackArtifacts.DownloaderId, _downloaderVersion, _feed,
                ConsumerScaffolding.DownloaderProgram);

            string[] buildArgs = InsertProjectAfter(BuildArgs, "XabSmokeDownloader.csproj");
            (int exit, string output) = ProcessUtil.Run(
                "dotnet",
                buildArgs,
                TimeSpan.FromMinutes(15),
                "build fresh Xabe.FFmpeg.Downloader consumer",
                psi =>
                {
                    psi.WorkingDirectory = projectDir;
                    psi.Environment["NUGET_PACKAGES"] = Path.Combine(_scratch, "downloader-pkgs");
                });

            Assert.True(exit == 0, "fresh downloader consumer restore/build failed:\n" + ProcessUtil.Tail(output));

            string consumerDll = Path.Combine(projectDir, "bin", "Release", "net8.0", "XabSmokeDownloader.dll");
            Assert.True(File.Exists(consumerDll), "built consumer output missing at " + consumerDll);

            string downloadDir = Path.Combine(_scratch, "downloader-binaries");
            (exit, output) = ProcessUtil.Run(
                "dotnet",
                new[] { consumerDll, downloadDir, _mediaFile },
                TimeSpan.FromMinutes(30),
                "run fresh Xabe.FFmpeg.Downloader consumer",
                psi => ConfigureExecutables(psi));

            AssertProvedConsumed(exit, output, DownloaderSmokeMarkers, "CORE-ASM-LOCATION");
        }

        private void EnsureMediaAvailable()
        {
            Assert.True(File.Exists(_mediaFile), "smoke media sample missing: " + _mediaFile);
        }

        private static string[] InsertProjectAfter(string[] verbFirst, string project)
        {
            var result = new string[verbFirst.Length + 1];
            result[0] = verbFirst[0];
            result[1] = project;
            Array.Copy(verbFirst, 1, result, 2, verbFirst.Length - 1);
            return result;
        }

        private static void Scaffold(
            string projectDir,
            string assemblyName,
            string packageName,
            string packageVersion,
            string localFeed,
            string programSource)
        {
            Directory.CreateDirectory(projectDir);
            File.WriteAllText(
                Path.Combine(projectDir, assemblyName + ".csproj"),
                ConsumerScaffolding.Csproj(assemblyName, packageName, packageVersion));
            File.WriteAllText(
                Path.Combine(projectDir, "nuget.config"),
                ConsumerScaffolding.NuGetConfig(localFeed));
            File.WriteAllText(Path.Combine(projectDir, "Program.cs"), programSource);
        }

        private static void ConfigureExecutables(System.Diagnostics.ProcessStartInfo psi)
        {
            string ffmpegDir = Environment.GetEnvironmentVariable("XABE_FFMPEG_BIN_DIR");
            if (!string.IsNullOrWhiteSpace(ffmpegDir))
            {
                psi.Environment["SMOKE_FFMPEG_DIR"] = ffmpegDir;
            }
        }

        private void AssertProvedConsumed(int exit, string output, string[] markers, string locationMarker)
        {
            Assert.True(exit == 0, "consumer run failed:\n" + ProcessUtil.Tail(output));

            string[] lines = output.Split('\n');
            foreach (string marker in markers)
            {
                string line = lines.FirstOrDefault(l => l.TrimStart().StartsWith(marker, StringComparison.Ordinal));
                Assert.True(line != null, "smoke marker '" + marker + "' missing. Output:\n" + ProcessUtil.Tail(output));
            }

            string hashLine = lines.First(l => l.TrimStart().StartsWith(markers.Last(), StringComparison.Ordinal));
            string observed = ValueOf(hashLine, markers.Last());
            AssertUtil.AreEqual(
                HashUtil.Sha256Hex(_coreDllBytes), observed,
                "the consumer did not load the exact assembly shipped in the produced package");

            // byte-identity (asserted above against the nupkg contents) is the proof of provenance;
            // the reported location only has to be a real file the runtime actually loaded
            string locationLine = lines.First(l => l.TrimStart().StartsWith(locationMarker, StringComparison.Ordinal));
            string location = locationLine.Substring(LocationPrefixLength(locationLine, locationMarker)).Trim();
            Assert.True(File.Exists(location),
                "the consumer-reported assembly location does not exist: " + location);
        }

        private static int LocationPrefixLength(string line, string marker)
        {
            return line.IndexOf(marker, StringComparison.Ordinal) + marker.Length;
        }

        private static string ValueOf(string line, string marker)
        {
            return line.Substring(LocationPrefixLength(line, marker)).Trim();
        }
    }
}
