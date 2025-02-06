using System.IO;
using System.Linq;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class InputBuilderTests
    {
        [Fact]
        public void PrepareInputFilesTest()
        {
            var builder = new InputBuilder();
            var directory = string.Empty;

            var preparedFiles = Directory.EnumerateFiles(directory).ToList();

            Assert.Equal(12, builder.FileList.Count);
            Assert.Equal(builder.FileList.Count, preparedFiles.Count);
        }
    }
}
