// ReSharper disable InconsistentNaming


namespace Xabe.FFmpeg.Model
{
    /// <summary>
    ///     Resolution
    /// </summary>
    public class Resolution
    {
        /// <summary>
        ///     Resolution in FFmpeg format
        /// </summary>
        /// <param name="width">Width of output video</param>
        /// <param name="height">Height of output video</param>
        public Resolution(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        ///     720, 480
        /// </summary>
        public static Resolution Ntsc => new Resolution(720, 480);

        /// <summary>
        ///     720, 576
        /// </summary>
        public static Resolution Pal => new Resolution(720, 576);

        /// <summary>
        ///     352, 240
        /// </summary>
        public static Resolution Qntsc => new Resolution(352, 240);

        /// <summary>
        ///     352, 288
        /// </summary>
        public static Resolution Qpal => new Resolution(352, 288);

        /// <summary>
        ///     640, 480
        /// </summary>
        public static Resolution Sntsc => new Resolution(640, 480);

        /// <summary>
        ///     768, 576
        /// </summary>
        public static Resolution Spal => new Resolution(768, 576);

        /// <summary>
        ///     352, 240
        /// </summary>
        public static Resolution Film => new Resolution(352, 240);

        /// <summary>
        ///     352, 240
        /// </summary>
        public static Resolution NtscFilm => new Resolution(352, 240);

        /// <summary>
        ///     128, 96
        /// </summary>
        public static Resolution Sqcif => new Resolution(128, 96);

        /// <summary>
        ///     176, 144
        /// </summary>
        public static Resolution Qcif => new Resolution(176, 144);

        /// <summary>
        ///     352, 288
        /// </summary>
        public static Resolution Cif => new Resolution(352, 288);

        /// <summary>
        ///     704, 576
        /// </summary>
        public static Resolution _4Cif => new Resolution(704, 576);

        /// <summary>
        ///     1408, 1152
        /// </summary>
        public static Resolution _16cif => new Resolution(1408, 1152);

        /// <summary>
        ///     160, 120
        /// </summary>
        public static Resolution Qqvga => new Resolution(160, 120);

        /// <summary>
        ///     320, 240
        /// </summary>
        public static Resolution Qvga => new Resolution(320, 240);

        /// <summary>
        ///     640, 480
        /// </summary>
        public static Resolution Vga => new Resolution(640, 480);

        /// <summary>
        ///     800, 600
        /// </summary>
        public static Resolution Svga => new Resolution(800, 600);

        /// <summary>
        ///     1024, 768
        /// </summary>
        public static Resolution Xga => new Resolution(1024, 768);

        /// <summary>
        ///     1600, 1200
        /// </summary>
        public static Resolution Uxga => new Resolution(1600, 1200);

        /// <summary>
        ///     2048, 1536
        /// </summary>
        public static Resolution Qxga => new Resolution(2048, 1536);

        /// <summary>
        ///     1280, 1024
        /// </summary>
        public static Resolution Sxga => new Resolution(1280, 1024);

        /// <summary>
        ///     2560, 2048
        /// </summary>
        public static Resolution Qsxga => new Resolution(2560, 2048);

        /// <summary>
        ///     5120, 4096
        /// </summary>
        public static Resolution Hsxga => new Resolution(5120, 4096);

        /// <summary>
        ///     852, 480
        /// </summary>
        public static Resolution Wvga => new Resolution(852, 480);

        /// <summary>
        ///     1366, 768
        /// </summary>
        public static Resolution Wxga => new Resolution(1366, 768);

        /// <summary>
        ///     1600, 1024
        /// </summary>
        public static Resolution Wsxga => new Resolution(1600, 1024);

        /// <summary>
        ///     1920, 1200
        /// </summary>
        public static Resolution Wuxga => new Resolution(1920, 1200);

        /// <summary>
        ///     2560, 1600
        /// </summary>
        public static Resolution Woxga => new Resolution(2560, 1600);

        /// <summary>
        ///     3200, 2048
        /// </summary>
        public static Resolution Wqsxga => new Resolution(3200, 2048);

        /// <summary>
        ///     3840, 2400
        /// </summary>
        public static Resolution Wquxga => new Resolution(3840, 2400);

        /// <summary>
        ///     6400, 4096
        /// </summary>
        public static Resolution Whsxga => new Resolution(6400, 4096);

        /// <summary>
        ///     7680, 4800
        /// </summary>
        public static Resolution Whuxga => new Resolution(7680, 4800);

        /// <summary>
        ///     320, 200
        /// </summary>
        public static Resolution Cga => new Resolution(320, 200);

        /// <summary>
        ///     640, 350
        /// </summary>
        public static Resolution Ega => new Resolution(640, 350);

        /// <summary>
        ///     852, 480
        /// </summary>
        public static Resolution Hd480 => new Resolution(852, 480);

        /// <summary>
        ///     1280, 720
        /// </summary>
        public static Resolution Hd720 => new Resolution(1280, 720);

        /// <summary>
        ///     1920, 1080
        /// </summary>
        public static Resolution Hd1080 => new Resolution(1920, 1080);

        /// <summary>
        ///     2048, 1080
        /// </summary>
        public static Resolution _2K => new Resolution(2048, 1080);

        /// <summary>
        ///     1998, 1080
        /// </summary>
        public static Resolution _2Kflat => new Resolution(1998, 1080);

        /// <summary>
        ///     2048, 858
        /// </summary>
        public static Resolution _2Kscope => new Resolution(2048, 858);

        /// <summary>
        ///     4096, 2160
        /// </summary>
        public static Resolution _4K => new Resolution(4096, 2160);

        /// <summary>
        ///     3996, 2160
        /// </summary>
        public static Resolution _4Kflat => new Resolution(3996, 2160);

        /// <summary>
        ///     4096, 1716
        /// </summary>
        public static Resolution _4Kscope => new Resolution(4096, 1716);

        /// <summary>
        ///     640, 360
        /// </summary>
        public static Resolution Nhd => new Resolution(640, 360);

        /// <summary>
        ///     240, 160
        /// </summary>
        public static Resolution Hqvga => new Resolution(240, 160);

        /// <summary>
        ///     400, 240
        /// </summary>
        public static Resolution Wqvga => new Resolution(400, 240);

        /// <summary>
        ///     432, 240
        /// </summary>
        public static Resolution Fwqvga => new Resolution(432, 240);

        /// <summary>
        ///     480, 320
        /// </summary>
        public static Resolution Hvga => new Resolution(480, 320);

        /// <summary>
        ///     960, 540
        /// </summary>
        public static Resolution Qhd => new Resolution(960, 540);

        /// <summary>
        ///     2048, 1080
        /// </summary>
        public static Resolution _2Kdci => new Resolution(2048, 1080);

        /// <summary>
        ///     4096, 2160
        /// </summary>
        public static Resolution _4Kdci => new Resolution(4096, 2160);

        /// <summary>
        ///     3840, 2160
        /// </summary>
        public static Resolution Uhd2160 => new Resolution(3840, 2160);

        /// <summary>
        ///     7680, 4320
        /// </summary>
        public static Resolution Uhd4320 => new Resolution(7680, 4320);

        /// <summary>
        ///     Width
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        ///     Height
        /// </summary>
        public int Height { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Width}x{Height}";
        }
    }
}
