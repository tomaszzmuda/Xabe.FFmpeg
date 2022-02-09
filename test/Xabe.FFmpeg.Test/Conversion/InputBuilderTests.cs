using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class InputBuilderTests
    {
        [Fact]
        public async Task PrepareInputFilesTest()
        {
            var files = Directory.EnumerateFiles(Resources.Images).ToList();
            var builder = new InputBuilder();
            var directory = string.Empty;

            Func<string, string> inputBuilder = builder.PrepareInputFiles(files, out directory);
            var preparedFiles = Directory.EnumerateFiles(directory).ToList();

            Assert.Equal(12, builder.FileList.Count);
            Assert.Equal(builder.FileList.Count, preparedFiles.Count);
        }
    }
}
