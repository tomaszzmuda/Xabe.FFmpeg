using System;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg.Streams
{
    /// <summary>
    ///     Video Stream
    /// </summary>
    public interface IVideoStream : IStream
    {
        /// <summary>
        ///     Rotate video
        /// </summary>
        /// <param name="rotateDegrees">Rotate type</param>
        /// <returns>IVideoStream</returns>
        IVideoStream Rotate(RotateDegrees rotateDegrees);

        /// <summary>
        ///     Change speed of video
        /// </summary>
        /// <param name="multiplaction">Speed value. (0.5 - 2.0). To double the speed set this to 2.0</param>
        /// <returns>IVideoStream</returns>
        /// <exception cref="ArgumentOutOfRangeException">When speed isn't between 0.5 - 2.0.</exception>
        IVideoStream ChangeSpeed(double multiplaction);

        /// <summary>
        ///     Melt watermark into video
        /// </summary>
        /// <param name="imagePath">Watermark</param>
        /// <param name="position">Position of watermark</param>
        /// <returns>IVideoStream</returns>
        IVideoStream SetWatermark(string imagePath, Position position);

        /// <summary>
        ///     Duration
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        ///     Width
        /// </summary>
        int Width { get; }

        /// <summary>
        ///     Height
        /// </summary>
        int Height { get; }

        /// <summary>
        ///     Frame rate
        /// </summary>
        double FrameRate { get; }

        /// <summary>
        ///     Screen ratio
        /// </summary>
        string Ratio { get; }

        /// <summary>
        ///  Codec type
        /// </summary>
        CodecType CodecType { get; }

        /// <summary>
        /// Video bitrate
        /// </summary>
        double Bitrate { get; }

        /// <summary>
        ///     Reverse video
        /// </summary>
        /// <returns>IVideoStream</returns>
        IVideoStream Reverse();

        /// <summary>
        ///     Set size of video
        /// </summary>
        /// <param name="size">VideoSize</param>
        /// <returns>IVideoStream</returns>
        IVideoStream SetScale(VideoSize size);

        /// <summary>
        ///     Set size of video
        /// </summary>
        /// <param name="size">VideoSize</param>
        /// <returns>IVideoStream</returns>
        IVideoStream SetSize(VideoSize size);

        /// <summary>
        ///     Set video codec and bitrate
        /// </summary>
        /// <param name="codec">Video codec</param>
        /// <returns>IVideoStream</returns>
        IVideoStream SetCodec(VideoCodec codec);

        /// <summary>
        ///     Set stream to copy with orginal codec
        /// </summary>
        /// <returns>IVideoStream</returns>
        IVideoStream CopyStream();

        /// <summary>
        ///     Set filter
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <returns>IVideoStream</returns>
        IVideoStream SetBitstreamFilter(BitstreamFilter filter);

        /// <summary>
        ///     Loop over the input stream.(-loop)
        /// </summary>
        /// <param name="count">Number of repeats</param>
        /// <param name="delay">Delay between repeats (in seconds)</param>
        /// <returns>IVideoStream</returns>
        IVideoStream SetLoop(int count, int delay = 0);

        /// <summary>
        ///     Set output frames count
        /// </summary>
        /// <param name="number">Number of frames</param>
        /// <returns>IVideoStream</returns>
        IVideoStream SetOutputFramesCount(int number);

        /// <summary>
        ///     Seeks in input file to position. (-ss argument)
        /// </summary>
        /// <param name="seek">Position</param>
        /// <returns>IVideoStream</returns>
        IVideoStream SetSeek(TimeSpan seek);

        /// <summary>
        ///     Burn subtitle into file
        /// </summary>
        /// <param name="subtitlePath">Path to subtitle file in .srt format</param>
        /// <param name="encode">Set subtitles input character encoding. Only useful if not UTF-8.</param>
        /// <param name="style">
        ///     Override default style or script info parameters of the subtitles. It accepts a string containing
        ///     ASS style format KEY=VALUE couples separated by ","
        /// </param>
        /// <param name="originalSize">
        ///     Specify the size of the original video, the video for which the ASS style was composed. This
        ///     is necessary to correctly scale the fonts if the aspect ratio has been changed.
        /// </param>
        /// <returns>IVideoStream</returns>
        IVideoStream AddSubtitles(string subtitlePath, string encode = null, string style = null, VideoSize originalSize = null);

        /// <summary>
        ///     Get part of video
        /// </summary>
        /// <param name="startTime">Start point</param>
        /// <param name="duration">Duration of new video</param>
        /// <returns>IVideoStream</returns>
        new IVideoStream Split(TimeSpan startTime, TimeSpan duration);
    }
}
