using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class InputBuilderTests
    {
        [Fact]
        public async Task PrepareInputFilesTest()
        {
            List<string> files = Directory.EnumerateFiles(Resources.Images).ToList();
            InputBuilder builder = new InputBuilder();
            string directory = string.Empty;

            Func<string, string> inputBuilder = builder.PrepareInputFiles(files, out directory);
            List<string> preparedFiles = Directory.EnumerateFiles(directory).ToList();

            Assert.Equal(12, builder.FileList.Count);
            Assert.Equal(builder.FileList.Count, preparedFiles.Count);
        }
    }
}
