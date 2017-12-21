namespace Xabe.FFmpeg.Model
{
    public class VideoStream : FfmpegStream
    {
        /// <summary>
        ///     Width
        /// </summary>
        public int Width { get; internal set; }

        /// <summary>
        ///     Height
        /// </summary>
        public int Height { get; internal set; }

        /// <summary>
        ///     Frame rate
        /// </summary>
        public double FrameRate { get; internal set; }

        /// <summary>
        ///     Screen ratio
        /// </summary>
        public string Ratio { get; internal set; }
    }
}
