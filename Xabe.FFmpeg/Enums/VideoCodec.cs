// ReSharper disable InconsistentNaming

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
        public static VideoCodec h264 = new VideoCodec("h264");

        /// <summary>
        ///     H.265 / HEVC (High Efficiency Video Coding) (encoders: libx265 )
        /// </summary>
        public static VideoCodec hevc = new VideoCodec("hevc");

        /// <summary>
        ///     MPEG-4 part 2 (decoders: mpeg4 mpeg4_crystalhd mpeg4_vdpau ) (encoders: mpeg4 libxvid )
        /// </summary>
        public static VideoCodec mpeg4 = new VideoCodec("mpeg4");

        /// <summary>
        ///     PNG (Portable Network Graphics) image
        /// </summary>
        public static VideoCodec png = new VideoCodec("png");

        /// <summary>
        ///     Theora (encoders: libtheora )
        /// </summary>
        public static VideoCodec theora = new VideoCodec("theora");

        /// <summary>
        ///     TIFF image
        /// </summary>
        public static VideoCodec tiff = new VideoCodec("tiff");

        /// <summary>
        ///     On2 VP8 (decoders: vp8 libvpx ) (encoders: libvpx )
        /// </summary>
        public static VideoCodec vp8 = new VideoCodec("vp8");

        public static VideoCodec libx264 = new VideoCodec("libx264");

        /// <inheritdoc />
        public VideoCodec(string codec)
        {
            Codec = codec;
        }

        /// <summary>
        ///     Video codec
        /// </summary>
        public string Codec { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Codec;
        }
    }
}
