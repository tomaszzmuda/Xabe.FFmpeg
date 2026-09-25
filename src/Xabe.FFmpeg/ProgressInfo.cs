namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Reported progress of a byte transfer
    /// </summary>
    public class ProgressInfo
    {
        /// <summary>
        ///     Bytes transferred so far
        /// </summary>
        public long DownloadedBytes { get; set; }

        /// <summary>
        ///     Total number of bytes to transfer
        /// </summary>
        public long TotalBytes { get; set; }

        /// <summary>
        ///     Initializes an empty progress report
        /// </summary>
        public ProgressInfo()
        {

        }

        /// <summary>
        ///     Initializes with transferred and total byte counts
        /// </summary>
        public ProgressInfo(long downloadedBytes, long totalBytes)
        {
            DownloadedBytes = downloadedBytes;
            TotalBytes = totalBytes;
        }
    }
}
