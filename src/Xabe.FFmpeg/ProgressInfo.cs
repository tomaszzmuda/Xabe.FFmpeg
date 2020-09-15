using System;
using System.Collections.Generic;
using System.Text;

namespace Xabe.FFmpeg
{
    public class ProgressInfo
    {
        public long DownloadedBytes { get; set; }
        public long TotalBytes { get; set; }

        public ProgressInfo()
        {

        }

        public ProgressInfo(long downloadedBytes, long totalBytes)
        {
            DownloadedBytes = downloadedBytes;
            TotalBytes = totalBytes;
        }
    }
}
