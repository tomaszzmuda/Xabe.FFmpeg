namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Video sync method (-vsync option)
    /// </summary>
    public enum VideoSyncMethod
    {
        /// <summary>
        ///     Preserves the input timestamps
        /// </summary>
        passthrough,

        /// <summary>
        ///     Constant frame rate
        /// </summary>
        cfr,

        /// <summary>
        ///     Variable frame rate
        /// </summary>
        vfr,

        /// <summary>
        ///     Drops frames to keep the frame rate constant
        /// </summary>
        drop,

        /// <summary>
        ///     Lets FFmpeg pick the best method
        /// </summary>
        auto
    }
}
