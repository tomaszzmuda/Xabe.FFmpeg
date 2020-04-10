namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    ///     Output subtitle format
    /// </summary>
    public class SubtitleFormat : MediaFormat
    {
        /// <summary>
        ///     Subtitle format
        /// </summary>
        /// <param name="format">Subtitles format</param>
        public SubtitleFormat(string format) : base(format)
        {
        }

        /// <summary>
        ///     SubRip
        /// </summary>
        public static SubtitleFormat Srt => new SubtitleFormat("srt");

        /// <summary>
        ///     SubRip
        /// </summary>
        public static SubtitleFormat WebVtt => new SubtitleFormat("webvtt");

        /// <summary>
        ///     SubRip
        /// </summary>
        public static SubtitleFormat Ass => new SubtitleFormat("ass");
    }
}
