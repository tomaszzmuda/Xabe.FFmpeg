using System.Drawing;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Xabe.FFMpeg.Enums
{
    /// <summary>
    ///     Video size
    /// </summary>
    public class VideoSize
    {
        /// <summary>
        ///     FFMpeg resolution string
        /// </summary>
        internal readonly string Resolution;

        /// <summary>
        ///     VideoSize in ffmpeg format
        /// </summary>
        /// <param name="width">Width of output video</param>
        /// <param name="height">Height of output video</param>
        public VideoSize(int width, int height)
        {
            Resolution = $"{width}x{height}";
        }

        public const string ntsc = "720x480";
        public const string pal = "720x576";
        public const string qntsc = "352x240";
        public const string qpal = "352x288";
        public const string sntsc = "640x480";
        public const string spal = "768x576";
        public const string film = "352x240";
        public const string ntscFilm = "352x240";
        public const string sqcif = "128x96";
        public const string qcif = "176x144";
        public const string cif = "352x288";
        public const string _4cif = "704x576";
        public const string _16cif = "1408x1152";
        public const string qqvga = "160x120";
        public const string qvga = "320x240";
        public const string vga = "640x480";
        public const string svga = "800x600";
        public const string xga = "1024x768";
        public const string uxga = "1600x1200";
        public const string qxga = "2048x1536";
        public const string sxga = "1280x1024";
        public const string qsxga = "2560x2048";
        public const string hsxga = "5120x4096";
        public const string wvga = "852x480";
        public const string wxga = "1366x768";
        public const string wsxga = "1600x1024";
        public const string wuxga = "1920x1200";
        public const string woxga = "2560x1600";
        public const string wqsxga = "3200x2048";
        public const string wquxga = "3840x2400";
        public const string whsxga = "6400x4096";
        public const string whuxga = "7680x4800";
        public const string cga = "320x200";
        public const string ega = "640x350";
        public const string hd480 = "852x480";
        public const string hd720 = "1280x720";
        public const string hd1080 = "1920x1080";
        public const string _2k = "2048x1080";
        public const string _2kflat = "1998x1080";
        public const string _2kscope = "2048x858";
        public const string _4k = "4096x2160";
        public const string _4kflat = "3996x2160";
        public const string _4kscope = "4096x1716";
        public const string nhd = "640x360";
        public const string hqvga = "240x160";
        public const string wqvga = "400x240";
        public const string fwqvga = "432x240";
        public const string hvga = "480x320";
        public const string qhd = "960x540";
        public const string _2kdci = "2048x1080";
        public const string _4kdci = "4096x2160";
        public const string uhd2160 = "3840x2160";
        public const string uhd4320 = "7680x4320";
    }
}
