using System.Collections.Generic;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class FFmpegExecutablesHelperTests
    {
        [Fact]
        public void TestSelectFFmpegPathForWindows()
        {
            const string expected = "ffmpeg.exe";

            IEnumerable<Model.FileInfo> files = GetWindowsPathMocks();

            string path = FFmpegExecutablesHelper.SelectFFmpegPath(files);

            Assert.Equal(expected, path);
        }

        [Fact]
        public void TestSelectFFprobePathForWindows()
        {
            const string expected = "ffprobe.exe";

            IEnumerable<Model.FileInfo> files = GetWindowsPathMocks();

            string path = FFmpegExecutablesHelper.SelectFFprobePath(files);

            Assert.Equal(expected, path);
        }

        [Fact]
        public void TestSelectFFmpegPathForLinux()
        {
            const string expected = "ffmpeg";

            IEnumerable<Model.FileInfo> files = GetLinuxPathMocks();

            string path = FFmpegExecutablesHelper.SelectFFmpegPath(files);

            Assert.Equal(expected, path);
        }

        [Fact]
        public void TestSelectFFprobePathForLinux()
        {
            const string expected = "ffprobe";

            IEnumerable<Model.FileInfo> files = GetLinuxPathMocks();

            string path = FFmpegExecutablesHelper.SelectFFprobePath(files);

            Assert.Equal(expected, path);
        }

        private IEnumerable<Model.FileInfo> GetWindowsPathMocks()
        {
            yield return new Model.FileInfo
            {
                Name = "ffmpeg",
                FullName = "ffmpeg.exe"
            };

            yield return new Model.FileInfo
            {
                Name = "FFmpeg.AutoGen",
                FullName = "FFmpeg.AutoGen.dll"
            };

            yield return new Model.FileInfo
            {
                Name = "ffprobe",
                FullName = "ffprobe.exe"
            };
        }

        private IEnumerable<Model.FileInfo> GetLinuxPathMocks()
        {
            yield return new Model.FileInfo
            {
                Name = "ffmpeg",
                FullName = "ffmpeg"
            };

            yield return new Model.FileInfo
            {
                Name = "FFmpeg.AutoGen",
                FullName = "FFmpeg.AutoGen"
            };

            yield return new Model.FileInfo
            {
                Name = "ffprobe",
                FullName = "ffprobe"
            };
        }
    }
}
