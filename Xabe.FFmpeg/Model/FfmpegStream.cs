using System;

namespace Xabe.FFmpeg.Model
{
    public abstract class FfmpegStream
    {
        /// <summary>
        ///     Duration
        /// </summary>
        public TimeSpan Duration { get; internal set; }

        /// <summary>
        ///     Video format
        /// </summary>
        public string Format { get; internal set; }

    }
}
