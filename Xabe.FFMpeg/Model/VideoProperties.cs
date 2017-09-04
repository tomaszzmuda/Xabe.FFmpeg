using System;

namespace Xabe.FFMpeg.Model
{
    /// <summary>
    ///     Information about media file
    /// </summary>
    public class VideoProperties
    {
        /// <summary>
        ///     Audio format
        /// </summary>
        public string AudioFormat { get; internal set; }

        /// <summary>
        ///     Duration of media
        /// </summary>
        public TimeSpan Duration { get; internal set; }

        /// <summary>
        ///     Duration of audio
        /// </summary>
        public TimeSpan AudioDuration { get; internal set; }

        /// <summary>
        ///     Duration of video
        /// </summary>
        public TimeSpan VideoDuration { get; internal set; }

        /// <summary>
        ///     Frame rate
        /// </summary>
        public double FrameRate { get; internal set; }

        /// <summary>
        ///     Height
        /// </summary>
        public int Height { get; internal set; }

        /// <summary>
        ///     Screen ratio
        /// </summary>
        public string Ratio { get; internal set; }

        /// <summary>
        ///     Size of file
        /// </summary>
        public long Size { get; internal set; }

        /// <summary>
        ///     Video format
        /// </summary>
        public string VideoFormat { get; internal set; }

        /// <summary>
        ///     Width
        /// </summary>
        public int Width { get; internal set; }
    }
}
