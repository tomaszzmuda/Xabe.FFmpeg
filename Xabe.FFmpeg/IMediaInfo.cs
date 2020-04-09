using System;
using System.Collections.Generic;
using System.IO;

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
        ///     Source file info
        /// </summary>
        FileInfo FileInfo { get; }

        /// <summary>
        ///     Duration of media
        /// </summary>
        TimeSpan Duration { get; }

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
