using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFMpeg.Enums;

namespace Xabe.FFMpeg
{
    /// <summary>
    ///     Information about media file
    /// </summary>
    public interface IVideoInfo
    {
        /// <summary>
        ///     Audio format
        /// </summary>
        string AudioFormat { get; }

        /// <summary>
        ///     Duration of media
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        ///     Duration of audio
        /// </summary>
        TimeSpan AudioDuration { get; }

        /// <summary>
        ///     Duration of video
        /// </summary>
        TimeSpan VideoDuration { get; }

        /// <summary>
        ///     Return extension of file
        /// </summary>
        string Extension { get; }

        /// <summary>
        ///     Frame rate
        /// </summary>
        double FrameRate { get; }

        /// <summary>
        ///     Height
        /// </summary>
        int Height { get; }

        /// <summary>
        ///     Screen ratio
        /// </summary>
        string Ratio { get; }

        /// <summary>
        ///     Size of file
        /// </summary>
        long Size { get; }

        /// <summary>
        ///     Video format
        /// </summary>
        string VideoFormat { get; }

        /// <summary>
        ///     Width
        /// </summary>
        int Width { get; }

        /// <summary>
        ///     Source file path
        /// </summary>
        string FullName { get; }

        /// <summary>
        ///     Get formated info about video
        /// </summary>
        /// <returns>Formated info about vidoe</returns>
        string ToString();
    }
}
