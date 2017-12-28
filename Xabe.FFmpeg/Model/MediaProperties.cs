using System;
using System.Collections.Generic;
using System.Linq;

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
        public IEnumerable<IVideoStream> VideoStreams { get; internal set; }

        /// <summary>
        ///     Audio streams
        /// </summary>
        public IEnumerable<IAudioStream> AudioStreams { get; internal set; }

        /// <summary>
        ///     Audio streams
        /// </summary>
        public IEnumerable<ISubtitle> Subtitles { get; internal set; }

        /// <summary>
        /// Main video stream if exists
        /// </summary>
        public IVideoStream MainVideoStream => VideoStreams.FirstOrDefault();

        /// <summary>
        /// Main audio stream if exists
        /// </summary>
        public IAudioStream MainAudioStream => AudioStreams.FirstOrDefault();
    }
}
