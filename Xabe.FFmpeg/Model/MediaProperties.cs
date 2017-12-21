using System;
using System.Collections.Generic;

namespace Xabe.FFmpeg.Model
{
    /// <summary>
    ///     Information about media file
    /// </summary>
    public class MediaProperties
    {
        /// <summary>
        ///     Duration of media
        /// </summary>
        public TimeSpan Duration { get; internal set; }

        /// <summary>
        ///     Size of file
        /// </summary>
        public long Size { get; internal set; }

        /// <summary>
        ///     Video streams
        /// </summary>
        public IEnumerable<VideoStream> VideoStreams { get; internal set; }

        /// <summary>
        ///     Audio streams
        /// </summary>
        public IEnumerable<AudioStream> AudioStreams { get; internal set; }

        /// <summary>
        ///     Audio streams
        /// </summary>
        public IEnumerable<Subtitle> Subtitles { get; internal set; }
    }
}
