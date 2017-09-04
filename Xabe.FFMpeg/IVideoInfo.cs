using System.IO;
using Xabe.FFMpeg.Model;

namespace Xabe.FFMpeg
{
    /// <summary>
    ///     Information about media file
    /// </summary>
    public interface IVideoInfo
    {
        /// <summary>
        ///     Source file info
        /// </summary>
        FileInfo FileInfo { get; }

        /// <summary>
        ///     Video properties
        /// </summary>
        VideoProperties VideoProperties { get; }

        /// <summary>
        ///     Get formated info about video
        /// </summary>
        /// <returns>Formated info about vidoe</returns>
        string ToString();
    }
}
