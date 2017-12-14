namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    ///     Filter
    /// </summary>
    public enum BitstreamFilter
    {
        /// <summary>
        ///     H264_Mp4ToAnnexB
        /// </summary>
        // ReSharper disable once InconsistentNaming
        H264_Mp4ToAnnexB,

        /// <summary>
        ///     Aac_AdtstoAsc
        /// </summary>
        // ReSharper disable once InconsistentNaming
        Aac_AdtstoAsc
    }

    /// <summary>
    ///     Channel
    /// </summary>
    public enum Channel
    {
        /// <summary>
        ///     Audio
        /// </summary>
        Audio,

        /// <summary>
        ///     Video
        /// </summary>
        Video,

        /// <summary>
        ///     Both
        /// </summary>
        Both
    }
}
