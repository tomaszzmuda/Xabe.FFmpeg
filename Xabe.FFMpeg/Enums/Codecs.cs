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
        H264_Mp4ToAnnexB,
        Aac_AdtstoAsc
    }

    internal enum Channel
    {
        Audio,
        Video,
        Both
    }
}
