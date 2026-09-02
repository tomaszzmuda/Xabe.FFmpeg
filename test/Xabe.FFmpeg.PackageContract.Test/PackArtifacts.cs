using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Xabe.FFmpeg.PackageContract.Test
{
    /// <summary>
    ///     Packed artifacts (.nupkg + .snupkg) for both packages, resolved once per test run.
    ///     Uses XABE_PACK_OUTPUT_DIR when provided (CI packs once for everyone); otherwise
    ///     packs the src projects itself into a scratch directory.
    /// </summary>
    [CollectionDefinition("package-contract")]
    public class PackageContractSuite : ICollectionFixture<PackArtifacts>
    {
    }

    public sealed class PackArtifacts
    {
        public const string CoreId = "Xabe.FFmpeg";
        public const string DownloaderId = "Xabe.FFmpeg.Downloader";

        public PackArtifacts()
        {
            RepoRoot = FindRepoRoot(AppContext.BaseDirectory);
            string configuredDir = Environment.GetEnvironmentVariable("XABE_PACK_OUTPUT_DIR");

            if (string.IsNullOrWhiteSpace(configuredDir))
            {
                SelfPackDir = Path.Combine(GetScratchRoot(), "packages");
                Directory.CreateDirectory(SelfPackDir);
                PackInto(SelfPackDir);
                PackageDir = SelfPackDir;
            }
            else
            {
                PackageDir = configuredDir;
            }

            foreach (string id in new[] { CoreId, DownloaderId })
            {
                Nupkgs[id] = LocateArtifact(id, "nupkg");
                Snupkgs[id] = LocateArtifact(id, "snupkg");
            }

            AssertUtil.AreEqual(
                GetVersion(Nupkgs[CoreId], CoreId), GetVersion(Nupkgs[DownloaderId], DownloaderId),
                "Xabe.FFmpeg and Xabe.FFmpeg.Downloader must always share the same package version");
        }

        public string PackageDir { get; }
        public string SelfPackDir { get; }
        public string RepoRoot { get; }
        public IDictionary<string, string> Nupkgs { get; } = new Dictionary<string, string>();
        public IDictionary<string, string> Snupkgs { get; } = new Dictionary<string, string>();

        private string LocateArtifact(string id, string extension)
        {
            string prefix = id + ".";
            string hit = Directory.GetFiles(PackageDir, prefix + "*")
                .FirstOrDefault(candidate =>
                {
                    string name = Path.GetFileName(candidate);
                    if (!name.EndsWith("." + extension, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }

                    string afterId = Path.GetFileNameWithoutExtension(candidate)
                        .Substring(prefix.Length);
                    return afterId.Length > 0 && char.IsDigit(afterId[0]);
                });

            Assert.True(hit != null, "expected a " + id + ".<version>." + extension + " in " + PackageDir);
            return hit;
        }

        public static string GetVersion(string artifactPath, string id)
        {
            string stem = Path.GetFileNameWithoutExtension(artifactPath);
            string prefix = id + ".";
            Assert.True(stem.StartsWith(prefix, StringComparison.Ordinal),
                "artifact " + artifactPath + " does not belong to package " + id);
            return stem.Substring(prefix.Length);
        }

        public static string GetScratchRoot()
        {
            string configured = Environment.GetEnvironmentVariable("XABE_SMOKE_SCRATCH");
            string root = string.IsNullOrWhiteSpace(configured)
                ? Path.Combine(Path.GetTempPath(), "xab-package-contract-" + Environment.ProcessId)
                : configured;
            Directory.CreateDirectory(root);
            return root;
        }

        public static string FindRepoRoot(string start)
        {
            DirectoryInfo dir = new DirectoryInfo(start);
            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "Xabe.FFmpeg.sln")))
                {
                    return dir.FullName;
                }

                dir = dir.Parent;
            }

            Assert.True(false, "could not locate Xabe.FFmpeg.sln walking up from " + start);
            return null;
        }

        private void PackInto(string dir)
        {
            foreach (string project in new[] { "src/Xabe.FFmpeg/Xabe.FFmpeg.csproj", "src/Xabe.FFmpeg.Downloader/Xabe.FFmpeg.Downloader.csproj" })
            {
                (int exit, string output) = ProcessUtil.Run(
                    "dotnet",
                    new[] { "pack", Path.Combine(RepoRoot, project), "-c", "Release",
                        "-p:ContinuousIntegrationBuild=true", "-o", dir },
                    TimeSpan.FromMinutes(20),
                    "pack " + project,
                    psi => psi.WorkingDirectory = RepoRoot);

                Assert.True(exit == 0 && output.Contains("Successfully created package"),
                    "dotnet pack failed for " + project + " (exit " + exit + "):\n" + ProcessUtil.Tail(output));
            }
        }
    }
}
