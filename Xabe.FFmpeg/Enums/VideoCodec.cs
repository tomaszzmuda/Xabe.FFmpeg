namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    ///     Video codec ("ffmpeg -codecs")
    /// </summary>
    public class VideoCodec
    {
        /// <summary>
        ///     H.264 / AVC / MPEG-4 AVC / MPEG-4 part 10 (decoders: h264 h264_crystalhd h264_vdpau ) (encoders: libx264 libx264rgb
        ///     )
        /// </summary>
        public static readonly VideoCodec H264 = new VideoCodec("h264");

        /// <summary>
        ///     H.265 / HEVC (High Efficiency Video Coding) (encoders: libx265 )
        /// </summary>
        public static readonly VideoCodec Hevc = new VideoCodec("hevc");

        /// <summary>
        ///     MPEG-4 part 2 (decoders: mpeg4 mpeg4_crystalhd mpeg4_vdpau ) (encoders: mpeg4 libxvid )
        /// </summary>
        public static readonly VideoCodec Mpeg4 = new VideoCodec("mpeg4");

        /// <summary>
        ///     PNG (Portable Network Graphics) image
        /// </summary>
        public static readonly VideoCodec Png = new VideoCodec("png");

        /// <summary>
        ///     Theora (encoders: libtheora )
        /// </summary>
        public static readonly VideoCodec Theora = new VideoCodec("theora");

        /// <summary>
        ///     TIFF image
        /// </summary>
        public static readonly VideoCodec Tiff = new VideoCodec("tiff");

        /// <summary>
        ///     On2 VP8 (decoders: vp8 libvpx ) (encoders: libvpx )
        /// </summary>
        public static readonly VideoCodec Vp8 = new VideoCodec("vp8");

        /// <summary>
        ///     Free x264 codec
        /// </summary>
        public static readonly VideoCodec Libx264 = new VideoCodec("libx264");

        /// <inheritdoc />
        public VideoCodec(string codec)
        {
            Codec = codec;
        }

        /// <summary>
        ///     Video codec
        /// </summary>
        public string Codec { get; }

        /// <summary>
        ///     Convert to string format
        /// </summary>
        /// <returns>Codec string</returns>
        public override string ToString()
        {
            return Codec;
        }
    }
}
