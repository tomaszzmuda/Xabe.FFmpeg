using System;
using Xunit;

namespace Xabe.FFmpeg.ExecDownload.Tests
{
    public class LastestFFmpegBinariesTests
    {
        [Fact]
        public void Test1()
        {
            LastestFFmpegBinaries.AcquireLastestsBinaries();
        }
    }
}
