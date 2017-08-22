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
        ///     720x480
        /// </summary>
        public const string ntsc = "720x480";

        /// <summary>
        ///     720x576
        /// </summary>
        public const string pal = "720x576";

        /// <summary>
        ///     352x240
        /// </summary>
        public const string qntsc = "352x240";

        /// <summary>
        ///     352x288
        /// </summary>
        public const string qpal = "352x288";

        /// <summary>
        ///     640x480
        /// </summary>
        public const string sntsc = "640x480";

        /// <summary>
        ///     768x576
        /// </summary>
        public const string spal = "768x576";

        /// <summary>
        ///     352x240
        /// </summary>
        public const string film = "352x240";

        /// <summary>
        ///     352x240
        /// </summary>
        public const string ntscFilm = "352x240";

        /// <summary>
        ///     128x96
        /// </summary>
        public const string sqcif = "128x96";

        /// <summary>
        ///     176x144
        /// </summary>
        public const string qcif = "176x144";

        /// <summary>
        ///     352x288
        /// </summary>
        public const string cif = "352x288";

        /// <summary>
        ///     704x576
        /// </summary>
        public const string _4cif = "704x576";

        /// <summary>
        ///     1408x1152
        /// </summary>
        public const string _16cif = "1408x1152";

        /// <summary>
        ///     160x120
        /// </summary>
        public const string qqvga = "160x120";

        /// <summary>
        ///     320x240
        /// </summary>
        public const string qvga = "320x240";

        /// <summary>
        ///     640x480
        /// </summary>
        public const string vga = "640x480";

        /// <summary>
        ///     800x600
        /// </summary>
        public const string svga = "800x600";

        /// <summary>
        ///     1024x768
        /// </summary>
        public const string xga = "1024x768";

        /// <summary>
        ///     1600x1200
        /// </summary>
        public const string uxga = "1600x1200";

        /// <summary>
        ///     2048x1536
        /// </summary>
        public const string qxga = "2048x1536";

        /// <summary>
        ///     1280x1024
        /// </summary>
        public const string sxga = "1280x1024";

        /// <summary>
        ///     2560x2048
        /// </summary>
        public const string qsxga = "2560x2048";

        /// <summary>
        ///     5120x4096
        /// </summary>
        public const string hsxga = "5120x4096";

        /// <summary>
        ///     852x480
        /// </summary>
        public const string wvga = "852x480";

        /// <summary>
        ///     1366x768
        /// </summary>
        public const string wxga = "1366x768";

        /// <summary>
        ///     1600x1024
        /// </summary>
        public const string wsxga = "1600x1024";

        /// <summary>
        ///     1920x1200
        /// </summary>
        public const string wuxga = "1920x1200";

        /// <summary>
        ///     2560x1600
        /// </summary>
        public const string woxga = "2560x1600";

        /// <summary>
        ///     3200x2048
        /// </summary>
        public const string wqsxga = "3200x2048";

        /// <summary>
        ///     3840x2400
        /// </summary>
        public const string wquxga = "3840x2400";

        /// <summary>
        ///     6400x4096
        /// </summary>
        public const string whsxga = "6400x4096";

        /// <summary>
        ///     7680x4800
        /// </summary>
        public const string whuxga = "7680x4800";

        /// <summary>
        ///     320x200
        /// </summary>
        public const string cga = "320x200";

        /// <summary>
        ///     640x350
        /// </summary>
        public const string ega = "640x350";

        /// <summary>
        ///     852x480
        /// </summary>
        public const string hd480 = "852x480";

        /// <summary>
        ///     1280x720
        /// </summary>
        public const string hd720 = "1280x720";

        /// <summary>
        ///     1920x1080
        /// </summary>
        public const string hd1080 = "1920x1080";

        /// <summary>
        ///     2048x1080
        /// </summary>
        public const string _2k = "2048x1080";

        /// <summary>
        ///     1998x1080
        /// </summary>
        public const string _2kflat = "1998x1080";

        /// <summary>
        ///     2048x858
        /// </summary>
        public const string _2kscope = "2048x858";

        /// <summary>
        ///     4096x2160
        /// </summary>
        public const string _4k = "4096x2160";

        /// <summary>
        ///     3996x2160
        /// </summary>
        public const string _4kflat = "3996x2160";

        /// <summary>
        ///     4096x1716
        /// </summary>
        public const string _4kscope = "4096x1716";

        /// <summary>
        ///     640x360
        /// </summary>
        public const string nhd = "640x360";

        /// <summary>
        ///     240x160
        /// </summary>
        public const string hqvga = "240x160";

        /// <summary>
        ///     400x240
        /// </summary>
        public const string wqvga = "400x240";

        /// <summary>
        ///     432x240
        /// </summary>
        public const string fwqvga = "432x240";

        /// <summary>
        ///     480x320
        /// </summary>
        public const string hvga = "480x320";

        /// <summary>
        ///     960x540
        /// </summary>
        public const string qhd = "960x540";

        /// <summary>
        ///     2048x1080
        /// </summary>
        public const string _2kdci = "2048x1080";

        /// <summary>
        ///     4096x2160
        /// </summary>
        public const string _4kdci = "4096x2160";

        /// <summary>
        ///     3840x2160
        /// </summary>
        public const string uhd2160 = "3840x2160";

        /// <summary>
        ///     7680x4320
        /// </summary>
        public const string uhd4320 = "7680x4320";

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
    }
}
