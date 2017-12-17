using System.IO;
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
        ///     Media properties
        /// </summary>
        MediaProperties Properties { get; }

        /// <summary>
        ///     Get formated info about video
        /// </summary>
        /// <returns>Formated info about vidoe</returns>
        string ToString();
    }
}
