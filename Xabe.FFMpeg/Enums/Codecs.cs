namespace Xabe.FFMpeg.Enums
{
    /// <summary>
    ///     Video codec
    /// </summary>
    public enum VideoCodec
    {
        LibX264,
        LibVpx,
        LibTheora,
        Png,
        MpegTs
    }

    /// <summary>
    ///     Audio codec
    /// </summary>
    public enum AudioCodec
    {
        Aac,
        LibVorbis
    }

    /// <summary>
    ///     Filter
    /// </summary>
    public enum Filter
    {
        H264_Mp4ToAnnexB,
        Aac_AdtstoAsc
    }

    /// <summary>
    ///     Channel
    /// </summary>
    public enum Channel
    {
        Audio,
        Video,
        Both
    }
}
