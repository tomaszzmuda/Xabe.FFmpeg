using System;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Video size
    /// </summary>
    public enum VideoSize
    {
        /// <summary>
        ///     720x480
        /// </summary>
        Ntsc,

        /// <summary>
        ///     720x576
        /// </summary>
        Pal,

        /// <summary>
        ///     352x240
        /// </summary>
        Qntsc,

        /// <summary>
        ///     352x288
        /// </summary>
        Qpal,

        /// <summary>
        ///     640x480
        /// </summary>
        Sntsc,

        /// <summary>
        ///     768x576
        /// </summary>
        Spal,

        /// <summary>
        ///     352x240
        /// </summary>
        Film,

        /// <summary>
        ///     352x240
        /// </summary>
        NtscFilm,

        /// <summary>
        ///     128x96
        /// </summary>
        Sqcif,

        /// <summary>
        ///     176x144
        /// </summary>
        Qcif,

        /// <summary>
        ///     352x288
        /// </summary>
        Cif,

        /// <summary>
        ///     704x576
        /// </summary>
        _4Cif,

        /// <summary>
        ///     1408x1152
        /// </summary>
        _16cif,

        /// <summary>
        ///     160x120
        /// </summary>
        Qqvga,

        /// <summary>
        ///     320x240
        /// </summary>
        Qvga,

        /// <summary>
        ///     640x480
        /// </summary>
        Vga,

        /// <summary>
        ///     800x600
        /// </summary>
        Svga,

        /// <summary>
        ///     1024x768
        /// </summary>
        Xga,

        /// <summary>
        ///     1600x1200
        /// </summary>
        Uxga,

        /// <summary>
        ///     2048x1536
        /// </summary>
        Qxga,

        /// <summary>
        ///     1280x1024
        /// </summary>
        Sxga,

        /// <summary>
        ///     2560x2048
        /// </summary>
        Qsxga,

        /// <summary>
        ///     5120x4096
        /// </summary>
        Hsxga,

        /// <summary>
        ///     852x480
        /// </summary>
        Wvga,

        /// <summary>
        ///     1366x768
        /// </summary>
        Wxga,

        /// <summary>
        ///     1600x1024
        /// </summary>
        Wsxga,

        /// <summary>
        ///     1920x1200
        /// </summary>
        Wuxga,

        /// <summary>
        ///     2560x1600
        /// </summary>
        Woxga,

        /// <summary>
        ///     3200x2048
        /// </summary>
        Wqsxga,

        /// <summary>
        ///     3840x2400
        /// </summary>
        Wquxga,

        /// <summary>
        ///     6400x4096
        /// </summary>
        Whsxga,

        /// <summary>
        ///     7680x4800
        /// </summary>
        Whuxga,

        /// <summary>
        ///     320x200
        /// </summary>
        Cga,

        /// <summary>
        ///     640x350
        /// </summary>
        Ega,

        /// <summary>
        ///     852x480
        /// </summary>
        Hd480,

        /// <summary>
        ///     1280x720
        /// </summary>
        Hd720,

        /// <summary>
        ///     1920x1080
        /// </summary>
        Hd1080,

        /// <summary>
        ///     2048x1080
        /// </summary>
        _2K,

        /// <summary>
        ///     1998x1080
        /// </summary>
        _2Kflat,

        /// <summary>
        ///     2048x858
        /// </summary>
        _2Kscope,

        /// <summary>
        ///     4096x2160
        /// </summary>
        _4K,

        /// <summary>
        ///     3996x2160
        /// </summary>
        _4Kflat,

        /// <summary>
        ///     4096x1716
        /// </summary>
        _4Kscope,

        /// <summary>
        ///     640x360
        /// </summary>
        Nhd,

        /// <summary>
        ///     240x160
        /// </summary>
        Hqvga,

        /// <summary>
        ///     400x240
        /// </summary>
        Wqvga,

        /// <summary>
        ///     432x240
        /// </summary>
        Fwqvga,

        /// <summary>
        ///     480x320
        /// </summary>
        Hvga,

        /// <summary>
        ///     960x540
        /// </summary>
        Qhd,

        /// <summary>
        ///     2048x1080
        /// </summary>
        _2Kdci,

        /// <summary>
        ///     4096x2160
        /// </summary>
        _4Kdci,

        /// <summary>
        ///     3840x2160
        /// </summary>
        Uhd2160,

        /// <summary>
        ///     7680x4320
        /// </summary>
        Uhd4320
    }

    internal static class VideoSizeExtensions
    {
        public static string ToFFmpegFormat(this VideoSize videoSize)
        {
            switch (videoSize)
            {
                case VideoSize.Ntsc:
                    return "720x480";
                case VideoSize.Pal:
                    return "720x576";
                case VideoSize.Qntsc:
                    return "352x240";
                case VideoSize.Qpal:
                    return "352x288";
                case VideoSize.Sntsc:
                    return "640x480";
                case VideoSize.Spal:
                    return "768x576";
                case VideoSize.Film:
                    return "352x240";
                case VideoSize.NtscFilm:
                    return "352x240";
                case VideoSize.Sqcif:
                    return "128x96";
                case VideoSize.Qcif:
                    return "176x144";
                case VideoSize.Cif:
                    return "352x288";
                case VideoSize._4Cif:
                    return "704x576";
                case VideoSize._16cif:
                    return "1408x1152";
                case VideoSize.Qqvga:
                    return "160x120";
                case VideoSize.Qvga:
                    return "320x240";
                case VideoSize.Vga:
                    return "640x480";
                case VideoSize.Svga:
                    return "800x600";
                case VideoSize.Xga:
                    return "1024x768";
                case VideoSize.Uxga:
                    return "1600x1200";
                case VideoSize.Qxga:
                    return "2048x1536";
                case VideoSize.Sxga:
                    return "1280x1024";
                case VideoSize.Qsxga:
                    return "2560x2048";
                case VideoSize.Hsxga:
                    return "5120x4096";
                case VideoSize.Wvga:
                    return "852x480";
                case VideoSize.Wxga:
                    return "1366x768";
                case VideoSize.Wsxga:
                    return "1600x1024";
                case VideoSize.Wuxga:
                    return "1920x1200";
                case VideoSize.Woxga:
                    return "2560x1600";
                case VideoSize.Wqsxga:
                    return "3200x2048";
                case VideoSize.Wquxga:
                    return "3840x2400";
                case VideoSize.Whsxga:
                    return "6400x4096";
                case VideoSize.Whuxga:
                    return "7680x4800";
                case VideoSize.Cga:
                    return "320x200";
                case VideoSize.Ega:
                    return "640x350";
                case VideoSize.Hd480:
                    return "852x480";
                case VideoSize.Hd720:
                    return "1280x720";
                case VideoSize.Hd1080:
                    return "1920x1080";
                case VideoSize._2K:
                    return "2048x1080";
                case VideoSize._2Kflat:
                    return "1998x1080";
                case VideoSize._2Kscope:
                    return "2048x858";
                case VideoSize._4K:
                    return "4096x2160";
                case VideoSize._4Kflat:
                    return "3996x2160";
                case VideoSize._4Kscope:
                    return "4096x1716";
                case VideoSize.Nhd:
                    return "640x360";
                case VideoSize.Hqvga:
                    return "240x160";
                case VideoSize.Wqvga:
                    return "400x240";
                case VideoSize.Fwqvga:
                    return "432x240";
                case VideoSize.Hvga:
                    return "480x320";
                case VideoSize.Qhd:
                    return "960x540";
                case VideoSize._2Kdci:
                    return "2048x1080";
                case VideoSize._4Kdci:
                    return "4096x2160";
                case VideoSize.Uhd2160:
                    return "3840x2160";
                case VideoSize.Uhd4320:
                    return "7680x4320";
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
