// ReSharper disable InconsistentNaming

namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    ///     Video format ("ffmpeg -formats")
    /// </summary>
    public class MediaFormat
    {
        /// <inheritdoc />
        public MediaFormat(string format)
        {
            Format = format;
        }

        /// <summary>
        ///     Video codec
        /// </summary>
        public string Format { get; }

        /// <summary>
        ///     AVI (Audio Video Interleaved)
        /// </summary>
        public static MediaFormat avi => new MediaFormat("avi");

        /// <summary>
        ///     MPEG-2 PS (DVD VOB)
        /// </summary>
        public static MediaFormat dvd => new MediaFormat("dvd");

        /// <summary>
        ///     FLV (Flash Video)
        /// </summary>
        public static MediaFormat flv => new MediaFormat("flv");

        /// <summary>
        ///     raw H.264 video
        /// </summary>
        public static MediaFormat h264 => new MediaFormat("h264");

        /// <summary>
        ///     raw HEVC video
        /// </summary>
        public static MediaFormat hevc => new MediaFormat("h264");

        /// <summary>
        ///     Matroska
        /// </summary>
        public static MediaFormat matroska => new MediaFormat("matroska");

        /// <summary>
        ///     Quicktime / MOV
        /// </summary>
        public static MediaFormat mov => new MediaFormat("mov");

        /// <summary>
        ///     MP4 (MPEG-4 Part 14)
        /// </summary>
        public static MediaFormat mp4 => new MediaFormat("mp4");

        /// <summary>
        ///     MPEG-1 Systems / MPEG program stream
        /// </summary>
        public static MediaFormat mpeg => new MediaFormat("mpeg");

        /// <summary>
        ///     MPEG-TS (MPEG-2 Transport Stream)
        /// </summary>
        public static MediaFormat mpegts => new MediaFormat("mpegts");

        /// <summary>
        ///     Ogg
        /// </summary>
        public static MediaFormat ogg => new MediaFormat("ogg");

        /// <summary>
        ///     Raw video
        /// </summary>
        public static MediaFormat rawvideo => new MediaFormat("rawvideo");

        /// <inheritdoc />
        public override string ToString()
        {
            return Format;
        }
    }
}
