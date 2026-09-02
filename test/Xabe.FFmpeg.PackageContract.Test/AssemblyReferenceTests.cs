using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Xunit;

namespace Xabe.FFmpeg.PackageContract.Test
{
    /// <summary>
    ///     The shipped assemblies must not carry build-only (PrivateAssets=all) references: if a
    ///     library ships an assembly reference, consumers pay for that runtime dependency whether
    ///     they know it or not.
    /// </summary>
    [Collection("package-contract")]
    public class AssemblyReferenceTests
    {
        private readonly PackArtifacts _artifacts;

        public AssemblyReferenceTests(PackArtifacts artifacts)
        {
            _artifacts = artifacts;
        }

        [Theory]
        [InlineData(PackArtifacts.CoreId)]
        [InlineData(PackArtifacts.DownloaderId)]
        public void ShippedAssembliesDoNotLeakBuildOnlyReferences(string id)
        {
            byte[] dll = ZipHelper.ReadEntry(_artifacts.Nupkgs[id], "lib/netstandard2.0/" + id + ".dll");
            string[] references = ReadAssemblyReferences(dll, id);

            Assert.NotEmpty(references);
            Assert.False(
                references.Any(r => string.Equals(r, "MinVer", StringComparison.OrdinalIgnoreCase)),
                "MinVer is a pack-time-only reference and must never ship in the assembly: " +
                string.Join(",", references));

            bool strict = string.Equals(
                Environment.GetEnvironmentVariable("XABE_REQUIRE_NO_JSON_RUNTIME_DEPS"),
                "true",
                StringComparison.OrdinalIgnoreCase);

            if (strict)
            {
                Assert.False(
                    references.Any(r => string.Equals(r, "System.Text.Json", StringComparison.OrdinalIgnoreCase)),
                    "System.Text.Json must no longer leak into consumers once XAB-1375 lands: " +
                    string.Join(",", references));
            }
        }

        [Fact]
        public void ShippedLibrariesStillTargetNetStandard20()
        {
            foreach (string id in new[] { PackArtifacts.CoreId, PackArtifacts.DownloaderId })
            {
                byte[] dll = ZipHelper.ReadEntry(_artifacts.Nupkgs[id], "lib/netstandard2.0/" + id + ".dll");
                using var pe = new PEReader(new MemoryStream(dll, writable: false));
                MetadataReader reader = pe.GetMetadataReader();

                string[] references = reader.AssemblyReferences
                    .Select(h => new string(reader.GetString(reader.GetAssemblyReference(h).Name)))
                    .ToArray();

                Assert.Contains("netstandard", references);
                Assert.Equal(new Version(2, 0, 0, 0), NetstandardVersion(reader, id));
            }
        }

        private static string[] ReadAssemblyReferences(byte[] dll, string id)
        {
            using var pe = new PEReader(new MemoryStream(dll, writable: false));
            MetadataReader reader = pe.GetMetadataReader();
            return reader.AssemblyReferences
                .Select(h => new string(reader.GetString(reader.GetAssemblyReference(h).Name)))
                .ToArray();
        }

        private static Version NetstandardVersion(MetadataReader reader, string id)
        {
            foreach (AssemblyReferenceHandle h in reader.AssemblyReferences)
            {
                AssemblyReference ar = reader.GetAssemblyReference(h);
                if (string.Equals(new string(reader.GetString(ar.Name)), "netstandard", StringComparison.Ordinal))
                {
                    return ar.Version;
                }
            }

            throw new InvalidOperationException(id + " does not reference the netstandard facade (TFM changed?)");
        }
    }
}
