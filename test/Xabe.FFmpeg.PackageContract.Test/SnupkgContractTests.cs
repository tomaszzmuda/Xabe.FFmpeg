using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using Xunit;

namespace Xabe.FFmpeg.PackageContract.Test
{
    /// <summary>
    ///     The .snupkg artifacts: symbol-only content, and source links that resolve to the exact
    ///     public commit the nuspec advertises.
    /// </summary>
    [Collection("package-contract")]
    public partial class SnupkgContractTests
    {
        private readonly PackArtifacts _artifacts;

        public SnupkgContractTests(PackArtifacts artifacts)
        {
            _artifacts = artifacts;
        }

        [Theory]
        [InlineData(PackArtifacts.CoreId)]
        [InlineData(PackArtifacts.DownloaderId)]
        public void SnupkgContainsOnlyThePortableSymbolAndManifest(string id)
        {
            string path = _artifacts.Snupkgs[id];
            string[] names = ZipHelper.Entries(path);
            Assert.True(names.Length > 0, path + " has no entries at all");
            Assert.True(names.Length == names.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
                "duplicate entry names differing only by case: " + string.Join(", ", names));

            var allowed = new HashSet<string>(
                new[]
                {
                    id + ".nuspec", "[Content_Types].xml", "_rels/.rels",
                    "lib/netstandard2.0/" + id + ".pdb"
                });

            var offenders = names.Where(n =>
                    !allowed.Contains(n)
                    && !n.StartsWith("package/services/metadata/core-properties/", StringComparison.Ordinal))
                .ToArray();

            Assert.Empty(offenders);

            byte[] pdb = ZipHelper.ReadEntry(path, "lib/netstandard2.0/" + id + ".pdb");
            Assert.True(pdb.Length > 4, path + ": pdb is suspiciously small");
            Assert.Equal((byte)0x42, pdb[0]);
            Assert.Equal((byte)0x53, pdb[1]);
            Assert.Equal((byte)0x4A, pdb[2]);
            Assert.Equal((byte)0x42, pdb[3]);

            XmlElement snuRoot = (XmlElement)XmlHelper.Load(ZipHelper.ReadEntry(path, id + ".nuspec")).DocumentElement;
            XmlElement nupkgRoot = (XmlElement)XmlHelper.Load(ZipHelper.ReadEntry(_artifacts.Nupkgs[id], id + ".nuspec")).DocumentElement;
            AssertUtil.AreEqual(XmlHelper.GetString(nupkgRoot, "metadata/version"), XmlHelper.GetString(snuRoot, "metadata/version"),
                id + " snupkg/nupkg version mismatch");

            // The SDK omits <packageTypes> from generated snupkgs; the symbol-only nature is
            // pinned by the entry whitelist above (a dll sneaking in would fail it).
        }

        [Theory]
        [InlineData(PackArtifacts.CoreId)]
        [InlineData(PackArtifacts.DownloaderId)]
        public void SourceLinksPointAtTheExactPublicCommitAnnouncedInTheNuspec(string id)
        {
            string path = _artifacts.Snupkgs[id];
            byte[] pdb = ZipHelper.ReadEntry(path, "lib/netstandard2.0/" + id + ".pdb");

            string nuspecCommit = XmlHelper.GetAttribute(
                XmlHelper.Load(ZipHelper.ReadEntry(_artifacts.Nupkgs[id], id + ".nuspec")).DocumentElement,
                "metadata/repository", "commit");

            List<string> documents = ExtractSourceDocuments(pdb);
            Assert.True(documents.Count >= 3,
                "expected source documents inside " + path + " but found " + documents.Count);

            // Debuggers resolve documents through the SourceLink mapping blob; without it source
            // linking is decoration.
            List<KeyValuePair<string, string>> mapping = ExtractSourceLinkMapping(pdb);
            Assert.NotEmpty(mapping);

            var commits = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var orgs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string document in documents)
            {
                if (TryRecordFromUrl(document, commits, orgs))
                {
                    continue;
                }

                Assert.True(TryRecordViaMapping(document, mapping, commits, orgs),
                    "source document is neither a public raw URL nor resolvable through the "
                    + "SourceLink mapping: " + document);
            }

            Assert.Single(commits);
            AssertUtil.AreEqual(nuspecCommit, commits.First(),
                "the embedded sources must be retrievable at the commit the package announces");

            string expectedOrg = Environment.GetEnvironmentVariable("XABE_SOURCELINK_EXPECTED_ORG");
            if (!string.IsNullOrWhiteSpace(expectedOrg))
            {
                Assert.All(orgs, o => Assert.Equal(expectedOrg, o));
            }
        }

        [GeneratedRegex("^https://raw\\.githubusercontent\\.com/(?<org>[^/]+)/Xabe\\.FFmpeg/(?<commit>[0-9a-f]{40})(?:(/.*)?)$")]
        private static partial Regex RawSourceUrl();

        private static bool TryRecordFromUrl(string url, HashSet<string> commits, HashSet<string> orgs)
        {
            Match match = RawSourceUrl().Match(url);
            if (!match.Success)
            {
                return false;
            }

            commits.Add(match.Groups["commit"].Value);
            orgs.Add(match.Groups["org"].Value);
            return true;
        }

        private static bool TryRecordViaMapping(
            string document,
            List<KeyValuePair<string, string>> mapping,
            HashSet<string> commits,
            HashSet<string> orgs)
        {
            foreach (KeyValuePair<string, string> entry in mapping)
            {
                string sourceAnchor = entry.Key.EndsWith('*')
                    ? entry.Key.Substring(0, entry.Key.Length - 1)
                    : entry.Key;
                if (!document.StartsWith(sourceAnchor, StringComparison.Ordinal))
                {
                    continue;
                }

                int starIndex = entry.Value.LastIndexOf('*');
                if (starIndex < 0)
                {
                    continue;
                }

                string resolved = string.Concat(
                    entry.Value.AsSpan(0, starIndex),
                    document.AsSpan(sourceAnchor.Length));
                Match match = RawSourceUrl().Match(resolved);
                if (!match.Success)
                {
                    continue;
                }

                commits.Add(match.Groups["commit"].Value);
                orgs.Add(match.Groups["org"].Value);
                return true;
            }

            return false;
        }

        private static List<string> ExtractSourceDocuments(byte[] pdb)
        {
            using var stream = new MemoryStream(pdb, writable: false);
            using var provider = MetadataReaderProvider.FromPortablePdbStream(stream);
            MetadataReader reader = provider.GetMetadataReader();

            var documents = new List<string>();
            foreach (DocumentHandle handle in reader.Documents)
            {
                string name = new string(reader.GetString(reader.GetDocument(handle).Name));
                if (!string.IsNullOrEmpty(name))
                {
                    documents.Add(name);
                }
            }

            return documents;
        }

        private static List<KeyValuePair<string, string>> ExtractSourceLinkMapping(byte[] pdb)
        {
            int index = IndexOf(pdb, "\"documents\":{");
            Assert.True(index >= 0, "no SourceLink mapping blob found in the portable PDB");
            string json = BalancedJson(pdb, index + "\"documents\":".Length);
            JsonElement root = JsonDocument.Parse(json).RootElement;

            var mapping = new List<KeyValuePair<string, string>>();
            foreach (JsonProperty property in root.EnumerateObject())
            {
                mapping.Add(new KeyValuePair<string, string>(property.Name, property.Value.GetString()));
            }

            return mapping;
        }

        private static int IndexOf(byte[] haystack, string needle)
        {
            byte[] needleBytes = System.Text.Encoding.ASCII.GetBytes(needle);
            for (int i = 0; i + needleBytes.Length <= haystack.Length; i++)
            {
                bool hit = true;
                for (int j = 0; j < needleBytes.Length; j++)
                {
                    if (haystack[i + j] != needleBytes[j])
                    {
                        hit = false;
                        break;
                    }
                }

                if (hit)
                {
                    return i;
                }
            }

            return -1;
        }

        private static string BalancedJson(byte[] haystack, int start)
        {
            int depth = 0;
            for (int i = start; i < haystack.Length; i++)
            {
                char c = (char)haystack[i];
                if (c == '{')
                {
                    depth++;
                }
                else if (c == '}')
                {
                    depth--;
                    if (depth == 0)
                    {
                        return System.Text.Encoding.UTF8.GetString(haystack, start, i - start + 1);
                    }
                }
            }

            throw new InvalidDataException("unbalanced SourceLink blob");
        }
    }
}
