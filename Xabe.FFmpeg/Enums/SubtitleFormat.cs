using System.ComponentModel;

namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    ///     Output subtitle format
    /// </summary>
    public enum SubtitleFormat
    {
        /// <summary>
        ///     SubRip
        /// </summary>
        [Description("srt")]
        SRT,

        /// <summary>
        ///     WebVTT
        /// </summary>
        [Description("webvtt")]
        WebVTT,

        /// <summary>
        ///     Advanced SubStation Alpha
        /// </summary>
        [Description("ass")]
        ASS
    }
}
