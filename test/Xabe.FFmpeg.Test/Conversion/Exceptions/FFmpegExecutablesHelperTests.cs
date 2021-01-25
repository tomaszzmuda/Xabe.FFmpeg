using System;
using System.Collections.Generic;
using System.IO;
using Xabe.FFmpeg.Test.Fixtures;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class FFmpegExecutablesHelperTests : IClassFixture<StorageFixture>
    {
        private readonly StorageFixture _storageFixture;

        public FFmpegExecutablesHelperTests(StorageFixture storageFixture)
        {
            _storageFixture = storageFixture;
        }

        [Fact]
        public void TestSelectFFmpegPathForWindows()
        {
            const string expected = "ffmpeg.exe";

            IEnumerable<FileInfo> files = GetWindowsPathMocks();

            string path = FFmpeg.GetFullName(files, "ffmpeg");

            Assert.EndsWith(expected, path);
        }

        [Fact]
        public void TestSelectFFprobePathForWindows()
        {
            const string expected = "ffprobe.exe";

            IEnumerable<FileInfo> files = GetWindowsPathMocks();

            string path = FFmpeg.GetFullName(files, "ffprobe");

            Assert.EndsWith(expected, path);
        }

        [Fact]
        public void TestSelectFFmpegPathForLinux()
        {
            const string expected = "ffmpeg";

            IEnumerable<FileInfo> files = GetLinuxPathMocks();

            string path = FFmpeg.GetFullName(files, "ffmpeg");

            Assert.EndsWith(expected, path);
        }

        [Fact]
        public void TestSelectFFprobePathForLinux()
        {
            const string expected = "ffprobe";

            IEnumerable<FileInfo> files = GetLinuxPathMocks();

            string path = FFmpeg.GetFullName(files, "ffprobe");

            Assert.EndsWith(expected, path);
        }

        private IEnumerable<FileInfo> GetWindowsPathMocks()
        {
            var tmpDir = _storageFixture.GetTempDirectory();

            File.Create(Path.Combine(tmpDir, "ffmpeg.exe"));
            File.Create(Path.Combine(tmpDir, "FFmpeg.AutoGen.dll"));
            File.Create(Path.Combine(tmpDir, "ffprobe.exe"));
            return new DirectoryInfo(tmpDir).EnumerateFiles();
        }

        private IEnumerable<FileInfo> GetLinuxPathMocks()
        {
            var tmpDir = _storageFixture.GetTempDirectory();

            File.Create(Path.Combine(tmpDir, "ffmpeg"));
            File.Create(Path.Combine(tmpDir, "FFmpeg.AutoGen.dll"));
            File.Create(Path.Combine(tmpDir, "ffprobe"));
            return new DirectoryInfo(tmpDir).EnumerateFiles();
        }
    }
}
