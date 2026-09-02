using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Xunit;

namespace Xabe.FFmpeg.PackageContract.Test
{
    /// <summary>
    ///     The .nupkg layout and nuspec metadata for both packages. These are the artifacts NuGet.org
    ///     shows to users; anything not on the whitelist is a defect.
    /// </summary>
    [Collection("package-contract")]
    public partial class NupkgContractTests
    {
        private const string Copyright = "Copyright 2017-2026 (c) Xabe Tomasz Zmuda.";
        private const string ProjectUrl = "https://ffmpeg.xabe.net/index.html";
        private const string RepoUrl = "https://github.com/tomaszzmuda/Xabe.FFmpeg";
        private const string DeprecatedLicenseUrlMarker = "https://aka.ms/deprecateLicenseUrl";
        private const int MaxIconBytes = 100 * 1024;

        private static readonly Dictionary<string, (string Description, string[] Tags)> ExpectedById =
            new Dictionary<string, (string Description, string[] Tags)>
            {
                [PackArtifacts.CoreId] = (
                    "Fluent .NET Standard wrapper around the FFmpeg and FFprobe command-line tools for "
                    + "media conversion, probing and stream manipulation - no FFmpeg expertise required.",
                    new[] { "ffmpeg", "ffprobe", "media", "video", "audio", "conversion", "probe" }),
                [PackArtifacts.DownloaderId] = (
                    "Downloads the latest FFmpeg and FFprobe binaries for Windows, macOS, Linux or "
                    + "Android, so your .NET application can locate a working media toolkit at runtime.",
                    new[] { "ffmpeg", "ffprobe", "media", "video", "audio", "conversion", "probe", "downloader", "executables" })
            };

        private readonly PackArtifacts _artifacts;

        public NupkgContractTests(PackArtifacts artifacts)
        {
            _artifacts = artifacts;
        }

        [GeneratedRegex("^[0-9a-f]{40}$")]
        private static partial Regex CommitRegex();

        [Theory]
        [InlineData(PackArtifacts.CoreId)]
        [InlineData(PackArtifacts.DownloaderId)]
        public void NupkgContainsNothingBeyondTheWhitelistedLayout(string id)
        {
            string path = _artifacts.Nupkgs[id];
            string[] names = ZipHelper.Entries(path);
            Assert.True(names.Length > 0, path + " has no entries at all");
            Assert.True(names.Length == names.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
                "duplicate entry names differing only by case: " + string.Join(", ", names));

            var allowed = new HashSet<string>(
                new[]
                {
                    id + ".nuspec", "[Content_Types].xml", "_rels/.rels",
                    "README.md", "Xabe-License.txt", "xabe_icon.png",
                    "lib/netstandard2.0/" + id + ".dll", "lib/netstandard2.0/" + id + ".xml"
                });

            var offenders = names.Where(n =>
                    !allowed.Contains(n)
                    && !n.StartsWith("package/services/metadata/core-properties/", StringComparison.Ordinal))
                .ToArray();

            Assert.Empty(offenders);

            ZipHelper.ReadEntry(path, id + ".nuspec");
            ZipHelper.ReadEntry(path, "lib/netstandard2.0/" + id + ".dll");
            ZipHelper.ReadEntry(path, "lib/netstandard2.0/" + id + ".xml");
            ZipHelper.ReadEntry(path, "Xabe-License.txt");
            ZipHelper.ReadEntry(path, "README.md");
            ZipHelper.ReadEntry(path, "xabe_icon.png");

            long iconSize = ZipHelper.EntrySize(path, "xabe_icon.png");
            Assert.InRange(iconSize, 1, MaxIconBytes);
        }

        [Theory]
        [InlineData(PackArtifacts.CoreId)]
        [InlineData(PackArtifacts.DownloaderId)]
        public void NuspecPublishesTheCompleteModernMetadata(string id)
        {
            string path = _artifacts.Nupkgs[id];
            XmlDocument doc = XmlHelper.Load(ZipHelper.ReadEntry(path, id + ".nuspec"));
            XmlElement root = (XmlElement)doc.DocumentElement;
            Assert.NotNull(root);
            Assert.Equal("package", root.Name);
            string version = XmlHelper.GetString(root, "metadata/version");

            Assert.Equal(id, XmlHelper.GetString(root, "metadata/id"));
            Assert.Matches(@"^\d+\.\d+\.\d+(?:[-+][0-9A-Za-z.\-]+)*$", version);

            string expectedVersion = Environment.GetEnvironmentVariable("XABE_EXPECTED_PACKAGE_VERSION");
            if (!string.IsNullOrWhiteSpace(expectedVersion))
            {
                Assert.Equal(expectedVersion, version);
            }

            Assert.Equal(id, XmlHelper.GetString(root, "metadata/title"));
            Assert.Equal(ExpectedById[id].Description, XmlHelper.GetString(root, "metadata/description"));
            Assert.Equal(Copyright, XmlHelper.GetString(root, "metadata/copyright"));
            Assert.Equal(ProjectUrl, XmlHelper.GetString(root, "metadata/projectUrl"));
            Assert.Equal(
                new HashSet<string>(ExpectedById[id].Tags, StringComparer.OrdinalIgnoreCase),
                XmlHelper.GetString(root, "metadata/tags")
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase));

            // licensing: acceptance kept from the historical contract, modern file license, and the
            // legacy URL slot is either absent or the SDK's deprecation marker and nothing else
            Assert.Equal("true", XmlHelper.GetString(root, "metadata/requireLicenseAcceptance"));
            Assert.Equal("file", XmlHelper.GetAttribute(root, "metadata/license", "type"));
            Assert.Equal("Xabe-License.txt", XmlHelper.GetString(root, "metadata/license"));
            string licenseUrl = XmlHelper.GetString(root, "metadata/licenseUrl");
            Assert.True(
                string.IsNullOrEmpty(licenseUrl) || licenseUrl == DeprecatedLicenseUrlMarker,
                "licenseUrl must be absent or the SDK deprecation marker, was: " + licenseUrl);

            // identity: embedded icon and per-package readme, no legacy slots
            Assert.Equal("xabe_icon.png", XmlHelper.GetString(root, "metadata/icon"));
            Assert.Null(XmlHelper.GetString(root, "metadata/iconUrl"));
            Assert.Equal("README.md", XmlHelper.GetString(root, "metadata/readme"));

            // release notes: deep link to the public release for this exact version
            Assert.Equal(
                RepoUrl + "/releases/tag/v" + version,
                XmlHelper.GetString(root, "metadata/releaseNotes"));

            // repository: modern triplet, real commit
            Assert.Equal(RepoUrl, XmlHelper.GetAttribute(root, "metadata/repository", "url"));
            Assert.Equal("git", XmlHelper.GetAttribute(root, "metadata/repository", "type"));
            string commit = XmlHelper.GetAttribute(root, "metadata/repository", "commit");
            Assert.Matches("^[0-9a-f]{40}$", commit);
            string ciHead = Environment.GetEnvironmentVariable("GITHUB_SHA");
            if (!string.IsNullOrWhiteSpace(ciHead) && CommitRegex().IsMatch(ciHead))
            {
                Assert.Equal(ciHead.ToLowerInvariant(), commit);
            }

            // dependency closure is EXACT: System.Text.Json is declared while the shipped assemblies
            // reference it (surfacing a formerly-hidden reference; XAB-1375 removes the use entirely and
            // then shrinks this expected set). MinVer never appears. The downloader additionally pins
            // the exact same core version. Deps nest under the target-framework group element.
            var declaredDependencies = new List<string>();
            XmlNodeList dependencyNodes = root.SelectNodes("metadata/dependencies/group/dependency");
            if (dependencyNodes != null)
            {
                foreach (XmlElement dependency in dependencyNodes)
                {
                    AssertUtil.AreEqual("Build,Analyzers", dependency.GetAttribute("exclude"),
                        "unexpected exclude on dependency " + dependency.GetAttribute("id"));
                    declaredDependencies.Add(dependency.GetAttribute("id") + "@" + dependency.GetAttribute("version"));
                }
            }

            string[] expectedDependencies = id == PackArtifacts.CoreId
                ? new[] { "System.Text.Json@9.0.0" }
                : new[] { PackArtifacts.CoreId + "@" + version, "System.Text.Json@9.0.0" };
            Assert.Equal(
                expectedDependencies.OrderBy(d => d, StringComparer.OrdinalIgnoreCase).ToArray(),
                declaredDependencies.OrderBy(d => d, StringComparer.OrdinalIgnoreCase).ToArray());

            // no empty metadata placeholders
            foreach (string slot in new[] { "description", "tags", "copyright", "projectUrl", "releaseNotes" })
            {
                Assert.False(string.IsNullOrWhiteSpace(XmlHelper.GetString(root, "metadata/" + slot)),
                    "empty metadata element: " + slot);
            }
        }
    }
}
