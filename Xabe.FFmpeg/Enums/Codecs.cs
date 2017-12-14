namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    ///     Audio codec
    /// </summary>
    public enum AudioCodec
    {
        /// <summary>
        ///     Aac
        /// </summary>
        Aac,

        /// <summary>
        ///     LibVorbis
        /// </summary>
        LibVorbis
    }

    /// <summary>
    ///     Filter
    /// </summary>
    public enum Filter
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
