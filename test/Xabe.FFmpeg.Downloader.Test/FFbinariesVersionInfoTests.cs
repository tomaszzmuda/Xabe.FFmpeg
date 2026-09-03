using System;
using System.IO;
using Xabe.FFmpeg.Downloader;
using Xabe.FFmpeg.Test.Common;
using Xunit;

namespace Xabe.FFmpeg.Downloader.Test
{
    public class FFbinariesVersionInfoTests
    {
        [Fact]
        public void ManifestFixtureRoundTripsThroughMappedFields()
        {
            var payload = File.ReadAllText(Resources.FFbinariesInfo);
            var info = JsonDocument.Map(payload, "test fixture", FFbinariesVersionInfo.FromManifest);

            Assert.Equal("3.4", info.Version);
            Assert.NotNull(info.BinariesUrl);
            Assert.StartsWith("http", info.BinariesUrl.Linux64.Ffmpeg);
            Assert.StartsWith("http", info.BinariesUrl.Windows64.Ffprobe);
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("{\"version\":\"9.9\"}")]
        public void ManifestWithoutBinHasNoUrl(string payload)
        {
            var info = JsonDocument.Map(payload, "unit", FFbinariesVersionInfo.FromManifest);
            Assert.Null(info.BinariesUrl);
        }

        [Theory]
        [InlineData("{\"version\":7}")]
        [InlineData("{\"bin\":\"x\"}")]
        [InlineData("{\"bin\":{\"linux-64\":\"x\"}}")]
        [InlineData("{\"bin\":{\"linux-64\":{\"ffmpeg\":42}}}")]
        [InlineData("[\"x\"]")]
        [InlineData("42")]
        [InlineData("\"x\"")]
        public void MalformedManifestSurfacesAsInvalidDataWithoutEcho(string payload)
        {
            var ex = Assert.Throws<InvalidDataException>(() => JsonDocument.Map(payload, "unit-label", FFbinariesVersionInfo.FromManifest));
            Assert.Contains("unit-label", ex.Message);
            Assert.DoesNotContain("SECRETX", ex.Message);
        }

        [Fact]
        public void UnknownManifestMembersAreIgnored()
        {
            const string payload = "{\"version\":\"9.9\",\"permalink\":\"http://x\",\"bin\":{\"linux-64\":{\"ffmpeg\":\"f\",\"ffplay\":\"p\",\"ffprobe\":\"b\"}},\"providerMeta\":{\"a\":[1,2]}}";
            var info = JsonDocument.Map(payload, "unit", FFbinariesVersionInfo.FromManifest);
            Assert.Equal("9.9", info.Version);
            Assert.Equal("f", info.BinariesUrl.Linux64.Ffmpeg);
            Assert.Null(info.BinariesUrl.Windows32);
        }

        [Theory]
        [InlineData("{\n  \"version\": \"7.1.0\"\n}")]
        [InlineData("{ \"version\" : \"3.4\" }\r\n")]
        public void SavedVersionIsReadBack(string payload)
        {
            var version = JsonDocument.Map(payload, "unit", FFbinariesVersionInfo.ReadSavedVersion);
            Assert.Equal(payload.Contains("7.1.0") ? "7.1.0" : "3.4", version);
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("{\"version\":null}")]
        public void SavedVersionMissingIsNull(string payload)
        {
            var version = JsonDocument.Map(payload, "unit", FFbinariesVersionInfo.ReadSavedVersion);
            Assert.Null(version);
        }

        [Theory]
        [InlineData("{\"version\":7}")]
        [InlineData("[1]")]
        public void SavedVersionMalformedSurfacesAsInvalidData(string payload)
        {
            var ex = Assert.Throws<InvalidDataException>(() => JsonDocument.Map(payload, "unit-label", FFbinariesVersionInfo.ReadSavedVersion));
            Assert.Contains("unit-label", ex.Message);
        }

        [Fact]
        public void RenderedVersionMatchesLegacyBytesExactly()
        {
            Assert.Equal("{\n  \"version\": \"7.1.0\"\n}", FFbinariesVersionInfo.RenderSavedVersion("7.1.0"));
            Assert.Equal("{\n  \"version\": \"3.4\"\n}", FFbinariesVersionInfo.RenderSavedVersion("3.4"));
        }

        [Theory]
        [InlineData("7.1.0", "{\n  \"version\": \"7.1.0\"\n}")]
        [InlineData("a/b", "{\n  \"version\": \"a/b\"\n}")]
        [InlineData("it's", "{\n  \"version\": \"it\\u0027s\"\n}")]
        [InlineData("a<b>c", "{\n  \"version\": \"a\\u003Cb\\u003Ec\"\n}")]
        [InlineData("a&amp;b", "{\n  \"version\": \"a\\u0026amp;b\"\n}")]
        [InlineData("smiley😀", "{\n  \"version\": \"smiley\\uD83D\\uDE00\"\n}")]
        [InlineData("tab\tnewline", "{\n  \"version\": \"tab\\tnewline\"\n}")]
        [InlineData("quote\"back\\slash", "{\n  \"version\": \"quote\\u0022back\\\\slash\"\n}")]
        public void RendererAppliesLegacyEncoderRules(string version, string expected)
        {
            Assert.Equal(expected, FFbinariesVersionInfo.RenderSavedVersion(version));
        }

        [Fact]
        public void RenderedThenParsedReturnsOriginalVersion()
        {
            const string version = "mixed \"chars\" \u00e9 / & < + 😀\t";
            var rendered = FFbinariesVersionInfo.RenderSavedVersion(version);
            var parsed = JsonDocument.Map(rendered, "unit", FFbinariesVersionInfo.ReadSavedVersion);
            Assert.Equal(version, parsed);
        }
    }
}
