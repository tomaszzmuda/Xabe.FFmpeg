namespace Xabe.FFMpeg.Enums
{
    internal enum VideoCodec
    {
        LibX264,
        LibVpx,
        LibTheora,
        Png,
        MpegTs
    }

    internal enum AudioCodec
    {
        Aac,
        LibVorbis
    }

    internal enum Filter
    {
        H264Mp4ToAnnexB,
        AacAdtstoAsc
    }

    internal enum Channel
    {
        Audio,
        Video,
        Both
    }
}
