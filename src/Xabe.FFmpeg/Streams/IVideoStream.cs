using System;
using System.Collections.Generic;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Video Stream
    /// </summary>
    public interface IVideoStream : ILocalStream
    {
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
        double Framerate { get; }

        /// <summary>
        ///     Screen ratio
        /// </summary>
        string Ratio { get; }

        /// <summary>
        ///     Codec type
        /// </summary>
        StreamType StreamType { get; }

        /// <summary>
        ///     Video bitrate
        /// </summary>
        long Bitrate { get; }

        /// <summary>
        /// Pixel Format
        /// </summary>
        string PixelFormat { get; }

        /// <summary>
        /// Default
        /// </summary>
        int? Default { get; }

        /// <summary>
        /// Forced
        /// </summary>
        int? Forced { get; }

        /// <summary>
        /// Video Codec
        /// </summary>
        string Codec { get; }

        /// <summary>
        ///     Rotate video
        /// </summary>
        /// <param name="rotateDegrees">Rotate type</param>
        /// <returns>IVideoStream</returns>
        IVideoStream Rotate(RotateDegrees rotateDegrees);

        /// <summary>
        ///     Change speed of video
        /// </summary>
        /// <param name="multiplicator">Speed value. (0.5 - 2.0). To double the speed set this to 2.0</param>
        /// <returns>IVideoStream</returns>
        /// <exception cref="ArgumentOutOfRangeException">When speed isn't between 0.5 - 2.0.</exception>
        IVideoStream ChangeSpeed(double multiplicator);

        /// <summary>
        ///     Melt watermark into video
        /// </summary>
        /// <param name="imagePath">Watermark</param>
        /// <param name="position">Position of watermark</param>
        /// <returns>IVideoStream</returns>
        IVideoStream SetWatermark(string imagePath, Position position);

        /// <summary>
        ///     Reverse video
        /// </summary>
        /// <returns>IVideoStream</returns>
        IVideoStream Reverse();

        /// <summary>
        ///     Set Flags for conversion (-flags option)
        /// </summary>
        /// <param name="flags">Flags to use</param>
        /// <returns>IVideoStream</returns>
        IVideoStream SetFlags(params Flag[] flags);

        /// <summary>
        ///     Set Flags for conversion (-flags option)
        /// </summary>
        /// <param name="flags">Flags to use</param>
        /// <returns>IVideoStream</returns>
        IVideoStream SetFlags(params string[] flags);

        /// <summary>
        ///     Set Framerate of the video (-r option)
        /// </summary>
        /// <param name="framerate">Framerates in FPS</param>
        /// <returns>IVideoStream</returns>
        IVideoStream SetFramerate(double framerate);

        /// <summary>
        ///     Set Bitrate of the video (-b:v option)
        /// </summary>
        /// <param name="bitrate">Bitrate in bits</param>
        /// <returns>IVideoStream</returns>
        IVideoStream SetBitrate(long bitrate);

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
        ///     Set video codec
        /// </summary>
        /// <param name="codec">Video codec</param>
        /// <returns>IVideoStream</returns>
        IVideoStream SetCodec(VideoCodec codec);

        /// <summary>
        ///     Set video codec
        /// </summary>
        /// <param name="codec">Video codec</param>
        /// <returns>IVideoStream</returns>
        IVideoStream SetCodec(string codec);

        /// <summary>
        ///     Set video pixel format
        /// </summary>
        /// <param name="pixelFormat">Pixel Format</param>
        /// <returns>IVideoStream</returns>
        IVideoStream SetPixelFormat(PixelFormat pixelFormat);

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

        /// <summary>
        ///     Set filter
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <returns>IVideoStream</returns>
        IVideoStream SetBitstreamFilter(string filter);
    }
}
