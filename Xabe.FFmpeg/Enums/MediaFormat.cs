namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    ///     Video format ("ffmpeg -formats")
    /// </summary>
    public class MediaFormat
    {
        /// <summary>
        ///     Create new media format
        /// </summary>
        /// <param name="format">Media format name</param>
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
        public static MediaFormat Avi => new MediaFormat("avi");

        /// <summary>
        ///     MPEG-2 PS (DVD VOB)
        /// </summary>
        public static MediaFormat Dvd => new MediaFormat("dvd");

        /// <summary>
        ///     FLV (Flash Video)
        /// </summary>
        public static MediaFormat Flv => new MediaFormat("flv");

        /// <summary>
        ///     raw H.264 video
        /// </summary>
        public static MediaFormat H264 => new MediaFormat("h264");

        /// <summary>
        ///     raw HEVC video
        /// </summary>
        public static MediaFormat Hevc => new MediaFormat("hevc");

        /// <summary>
        ///     Matroska
        /// </summary>
        public static MediaFormat Matroska => new MediaFormat("matroska");

        /// <summary>
        ///     Quicktime / MOV
        /// </summary>
        public static MediaFormat Mov => new MediaFormat("mov");

        /// <summary>
        ///     MP4 (MPEG-4 Part 14)
        /// </summary>
        public static MediaFormat Mp4 => new MediaFormat("mp4");

        /// <summary>
        ///     MPEG-1 Systems / MPEG program stream
        /// </summary>
        public static MediaFormat Mpeg => new MediaFormat("mpeg");

        /// <summary>
        ///     MPEG-TS (MPEG-2 Transport Stream)
        /// </summary>
        public static MediaFormat Mpegts => new MediaFormat("mpegts");

        /// <summary>
        ///     Ogg
        /// </summary>
        public static MediaFormat Ogg => new MediaFormat("ogg");

        /// <summary>
        ///     Raw video
        /// </summary>
        public static MediaFormat Rawvideo => new MediaFormat("rawvideo");

        /// <inheritdoc />
        public override string ToString()
        {
            return Format;
        }
    }
}
