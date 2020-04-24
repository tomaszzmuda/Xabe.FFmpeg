using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class FFmpegExecutablesHelperTests
    {
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
            var tmpDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("n"));
            var dir = new DirectoryInfo(tmpDir);
            dir.Create();
            File.Create(Path.Combine(tmpDir, "ffmpeg.exe"));
            File.Create(Path.Combine(tmpDir, "FFmpeg.AutoGen.dll"));
            File.Create(Path.Combine(tmpDir, "ffprobe.exe"));
            return dir.EnumerateFiles();
        }

        private IEnumerable<FileInfo> GetLinuxPathMocks()
        {
            var tmpDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("n"));
            var dir = new DirectoryInfo(tmpDir);
            dir.Create();
            File.Create(Path.Combine(tmpDir, "ffmpeg"));
            File.Create(Path.Combine(tmpDir, "FFmpeg.AutoGen.dll"));
            File.Create(Path.Combine(tmpDir, "ffprobe"));
            return dir.EnumerateFiles();
        }
    }
}
