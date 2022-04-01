using System;
using System.Collections.Generic;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Information about media file
    /// </summary>
    public interface IMediaInfo
    {
        /// <summary>
        ///     All file streams
        /// </summary>
        IEnumerable<IStream> Streams { get; }

        /// <summary>
        ///     Source info
        /// </summary>
        string Path { get; }

        /// <summary>
        ///     Duration of media
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        ///     Date and Time when the media was created
        /// </summary>
        DateTime? CreationTime { get; }

        /// <summary>
        ///     Size of file
        /// </summary>
        long Size { get; }

        /// <summary>
        ///     Video streams
        /// </summary>
        IEnumerable<IVideoStream> VideoStreams { get; }

        /// <summary>
        ///     Audio streams
        /// </summary>
        IEnumerable<IAudioStream> AudioStreams { get; }

        /// <summary>
        ///     Audio streams
        /// </summary>
        IEnumerable<ISubtitleStream> SubtitleStreams { get; }
    }
}
