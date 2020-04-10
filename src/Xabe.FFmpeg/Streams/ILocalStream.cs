using System;
using System.IO;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Stream with local source
    /// </summary>
    public interface ILocalStream : IStream
    {
        /// <summary>
        ///     File source of stream
        /// </summary>
        string Path { get; }

        /// <summary>
        ///     Get part of media
        /// </summary>
        /// <param name="startTime">Start point</param>
        /// <param name="duration">Duration of new video</param>
        void Split(TimeSpan startTime, TimeSpan duration);
    }
}
