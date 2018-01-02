using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xabe.FFmpeg.Model;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Information about media file
    /// </summary>
    public interface IMediaInfo
    {
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

        /// <summary>
        ///     Get formated info about video
        /// </summary>
        /// <returns>Formated info about vidoe</returns>
        string ToString();
    }
}
