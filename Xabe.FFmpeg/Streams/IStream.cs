using System;
using System.IO;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Base stream class
    /// </summary>
    public interface IStream
    {
        /// <summary>
        ///     Format
        /// </summary>
        string Format { get; }

        /// <summary>
        ///     File source of stream
        /// </summary>
        FileInfo Source { get; }

        /// <summary>
        ///     Index of stream
        /// </summary>
        int Index { get; }

        /// <summary>
        ///     Build FFmpeg arguments
        /// </summary>
        /// <returns>Arguments</returns>
        string Build();

        /// <summary>
        ///     Get part of video
        /// </summary>
        /// <param name="startTime">Start point</param>
        /// <param name="duration">Duration of new video</param>
        /// <returns>IVideoStream</returns>
        void Split(TimeSpan startTime, TimeSpan duration);
    }
}
