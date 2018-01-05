namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    ///     Output subtitle format
    /// </summary>
    public class SubtitleFormat : MediaFormat
    {

        public SubtitleFormat(string format) : base(format)
        {
            
        }

        /// <summary>
        ///     SubRip
        /// </summary>
        public static MediaFormat Srt => new MediaFormat("srt");

        /// <summary>
        ///     SubRip
        /// </summary>
        public static MediaFormat WebVtt => new MediaFormat("webvtt");

        /// <summary>
        ///     SubRip
        /// </summary>
        public static MediaFormat Ass => new MediaFormat("ass");
    }
}
