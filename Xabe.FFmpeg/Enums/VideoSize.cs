// ReSharper disable InconsistentNaming


namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    ///     Video size
    /// </summary>
    public class VideoSize
    {
        private string _resoulution;

        /// <summary>
        ///     FFmpeg resolution height
        /// </summary>
        internal readonly int Height;

        /// <summary>
        ///     FFmpeg resolution width
        /// </summary>
        internal readonly int Width;

        /// <summary>
        ///     VideoSize in FFmpeg format
        /// </summary>
        /// <param name="width">Width of output video</param>
        /// <param name="height">Height of output video</param>
        public VideoSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Convert video size to string format
        /// </summary>
        /// <returns>Formatted video size</returns>
        public override string ToString()
        {
            return _resoulution ?? (_resoulution = $"{Width}x{Height}");
        }

        /// <summary>
        ///     720x480
        /// </summary>
        public static VideoSize Ntsc => new VideoSize(720, 480);

        /// <summary>
        ///     720x576
        /// </summary>
        public static VideoSize Pal => new VideoSize(720, 576);

        /// <summary>
        ///     352x240
        /// </summary>
        public static VideoSize Qntsc => new VideoSize(352, 240);

        /// <summary>
        ///     352x288
        /// </summary>
        public static VideoSize Qpal => new VideoSize(352, 288);

        /// <summary>
        ///     640x480
        /// </summary>
        public static VideoSize Sntsc => new VideoSize(640, 480);

        /// <summary>
        ///     768x576
        /// </summary>
        public static VideoSize Spal => new VideoSize(768, 576);

        /// <summary>
        ///     352x240
        /// </summary>
        public static VideoSize Film => new VideoSize(352, 240);

        /// <summary>
        ///     352x240
        /// </summary>
        public static VideoSize NtscFilm => new VideoSize(352, 240);

        /// <summary>
        ///     128x96
        /// </summary>
        public static VideoSize Sqcif => new VideoSize(128, 96);

        /// <summary>
        ///     176x144
        /// </summary>
        public static VideoSize Qcif => new VideoSize(176, 144);

        /// <summary>
        ///     352x288
        /// </summary>
        public static VideoSize Cif => new VideoSize(352, 288);

        /// <summary>
        ///     704x576
        /// </summary>
        public static VideoSize _4Cif => new VideoSize(704, 576);

        /// <summary>
        ///     1408x1152
        /// </summary>
        public static VideoSize _16cif => new VideoSize(1408, 1152);

        /// <summary>
        ///     160x120
        /// </summary>
        public static VideoSize Qqvga => new VideoSize(160, 120);

        /// <summary>
        ///     320x240
        /// </summary>
        public static VideoSize Qvga => new VideoSize(320, 240);

        /// <summary>
        ///     640x480
        /// </summary>
        public static VideoSize Vga => new VideoSize(640, 480);

        /// <summary>
        ///     800x600
        /// </summary>
        public static VideoSize Svga => new VideoSize(800, 600);

        /// <summary>
        ///     1024x768
        /// </summary>
        public static VideoSize Xga => new VideoSize(1024, 768);

        /// <summary>
        ///     1600x1200
        /// </summary>
        public static VideoSize Uxga => new VideoSize(1600, 1200);

        /// <summary>
        ///     2048x1536
        /// </summary>
        public static VideoSize Qxga => new VideoSize(2048, 1536);

        /// <summary>
        ///     1280x1024
        /// </summary>
        public static VideoSize Sxga => new VideoSize(1280, 1024);

        /// <summary>
        ///     2560x2048
        /// </summary>
        public static VideoSize Qsxga => new VideoSize(2560, 2048);

        /// <summary>
        ///     5120x4096
        /// </summary>
        public static VideoSize Hsxga => new VideoSize(5120, 4096);

        /// <summary>
        ///     852x480
        /// </summary>
        public static VideoSize Wvga => new VideoSize(852, 480);

        /// <summary>
        ///     1366x768
        /// </summary>
        public static VideoSize Wxga => new VideoSize(1366, 768);

        /// <summary>
        ///     1600x1024
        /// </summary>
        public static VideoSize Wsxga => new VideoSize(1600, 1024);

        /// <summary>
        ///     1920x1200
        /// </summary>
        public static VideoSize Wuxga => new VideoSize(1920, 1200);

        /// <summary>
        ///     2560x1600
        /// </summary>
        public static VideoSize Woxga => new VideoSize(2560, 1600);

        /// <summary>
        ///     3200x2048
        /// </summary>
        public static VideoSize Wqsxga => new VideoSize(3200, 2048);

        /// <summary>
        ///     3840x2400
        /// </summary>
        public static VideoSize Wquxga => new VideoSize(3840, 2400);

        /// <summary>
        ///     6400x4096
        /// </summary>
        public static VideoSize Whsxga => new VideoSize(6400, 4096);

        /// <summary>
        ///     7680x4800
        /// </summary>
        public static VideoSize Whuxga => new VideoSize(7680, 4800);

        /// <summary>
        ///     320x200
        /// </summary>
        public static VideoSize Cga => new VideoSize(320, 200);

        /// <summary>
        ///     640x350
        /// </summary>
        public static VideoSize Ega => new VideoSize(640, 350);

        /// <summary>
        ///     852x480
        /// </summary>
        public static VideoSize Hd480 => new VideoSize(852, 480);

        /// <summary>
        ///     1280x720
        /// </summary>
        public static VideoSize Hd720 => new VideoSize(1280, 720);

        /// <summary>
        ///     1920x1080
        /// </summary>
        public static VideoSize Hd1080 => new VideoSize(1920, 1080);

        /// <summary>
        ///     2048x1080
        /// </summary>
        public static VideoSize _2K => new VideoSize(2048, 1080);

        /// <summary>
        ///     1998x1080
        /// </summary>
        public static VideoSize _2Kflat => new VideoSize(1998, 1080);

        /// <summary>
        ///     2048x858
        /// </summary>
        public static VideoSize _2Kscope => new VideoSize(2048, 858);

        /// <summary>
        ///     4096x2160
        /// </summary>
        public static VideoSize _4K => new VideoSize(4096, 2160);

        /// <summary>
        ///     3996x2160
        /// </summary>
        public static VideoSize _4Kflat => new VideoSize(3996, 2160);

        /// <summary>
        ///     4096x1716
        /// </summary>
        public static VideoSize _4Kscope => new VideoSize(4096, 1716);

        /// <summary>
        ///     640x360
        /// </summary>
        public static VideoSize Nhd => new VideoSize(640, 360);

        /// <summary>
        ///     240x160
        /// </summary>
        public static VideoSize Hqvga => new VideoSize(240, 160);

        /// <summary>
        ///     400x240
        /// </summary>
        public static VideoSize Wqvga => new VideoSize(400, 240);

        /// <summary>
        ///     432x240
        /// </summary>
        public static VideoSize Fwqvga => new VideoSize(432, 240);

        /// <summary>
        ///     480x320
        /// </summary>
        public static VideoSize Hvga => new VideoSize(480, 320);

        /// <summary>
        ///     960x540
        /// </summary>
        public static VideoSize Qhd => new VideoSize(960, 540);

        /// <summary>
        ///     2048x1080
        /// </summary>
        public static VideoSize _2Kdci => new VideoSize(2048, 1080);

        /// <summary>
        ///     4096x2160
        /// </summary>
        public static VideoSize _4Kdci => new VideoSize(4096, 2160);

        /// <summary>
        ///     3840x2160
        /// </summary>
        public static VideoSize Uhd2160 => new VideoSize(3840, 2160);

        /// <summary>
        ///     7680x4320
        /// </summary>
        public static VideoSize Uhd4320 => new VideoSize(7680, 4320);
    }
}
