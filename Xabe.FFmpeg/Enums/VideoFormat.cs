// ReSharper disable InconsistentNaming
namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    ///     Video format ("ffmpeg -formats")
    /// </summary>
    public class VideoFormat
    {
        /// <summary>
        ///     Video codec
        /// </summary>
        public string Format { get; }

        /// <summary>
        ///     AVI (Audio Video Interleaved)
        /// </summary>
        public static VideoFormat avi => new VideoFormat("avi");

        /// <summary>
        ///     MPEG-2 PS (DVD VOB)
        /// </summary>
        public static VideoFormat dvd => new VideoFormat("dvd");

        /// <summary>
        ///     FLV (Flash Video)
        /// </summary>
        public static VideoFormat flv => new VideoFormat("flv");

        /// <summary>
        ///     raw H.264 video
        /// </summary>
        public static VideoFormat h264 => new VideoFormat("h264");

        /// <summary>
        ///     raw HEVC video
        /// </summary>
        public static VideoFormat hevc => new VideoFormat("h264");

        /// <summary>
        ///     Matroska
        /// </summary>
        public static VideoFormat matroska => new VideoFormat("matroska");

        /// <summary>
        ///     Quicktime / MOV
        /// </summary>
        public static VideoFormat mov => new VideoFormat("mov");

        /// <summary>
        ///     MP4 (MPEG-4 Part 14)
        /// </summary>
        public static VideoFormat mp4 => new VideoFormat("mp4");

        /// <summary>
        ///     MPEG-1 Systems / MPEG program stream
        /// </summary>
        public static VideoFormat mpeg => new VideoFormat("mpeg");

        /// <summary>
        ///     MPEG-TS (MPEG-2 Transport Stream)
        /// </summary>
        public static VideoFormat mpegts => new VideoFormat("mpegts");

        /// <summary>
        ///     Ogg
        /// </summary>
        public static VideoFormat ogg => new VideoFormat("ogg");

        /// <summary>
        ///     Raw video
        /// </summary>
        public static VideoFormat rawvideo => new VideoFormat("rawvideo");

        /// <inheritdoc />
        public override string ToString()
        {
            return Format;
        }

        /// <inheritdoc />
        public VideoFormat(string format)
        {
            Format = format;
        }
    }
}
