namespace Xabe.FFmpeg.Downloader
{
    /// <summary>
    ///     Enum to describe the versions of FFmpeg that can be automatically downloaded. Official is best option for most cases
    /// </summary>
    public enum FFmpegVersion
    {
        /// <summary>
        /// The official release from ffbinaries
        /// </summary>
        Official = 1,

        /// <summary>
        /// The Full Release from Zenaroe (Only Windows and macOS)
        /// </summary>
        Full = 2,

        /// <summary>
        /// The Shared Release from Zenaroe (Only Windows and macOS)
        /// </summary>
        Shared = 3,

        /// <summary>
        /// The Android Release Based on Mobile-FFmpeg
        /// </summary>
        Android = 4
    }
}
