using System;
using System.IO;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg
{
    public interface IVideoStream
    {
        /// <summary>
        ///     Melt watermark into video file
        /// </summary>
        /// <param name="imagePath">Watermark</param>
        /// <param name="position">Position of watermark</param>
        /// <returns>Video stream object</returns>
        IVideoStream SetWatermark(string imagePath, Position position);

        IVideoStream Reverse();

        /// <summary>
        ///     Set size of video
        /// </summary>
        /// <param name="size">VideoSize</param>
        /// <returns>IConversion object</returns>
        IVideoStream SetScale(VideoSize size);

        /// <summary>
        ///     Set size of video
        /// </summary>
        /// <param name="size">VideoSize</param>
        /// <returns>IConversion object</returns>
        IVideoStream SetSize(VideoSize size);

        /// <summary>
        ///     Set video codec and bitrate
        /// </summary>
        /// <param name="codec">Video codec</param>
        /// <param name="bitrate">Video bitrate</param>
        /// <returns>IConversion object</returns>
        IVideoStream SetCodec(VideoCodec codec, int bitrate = 0);

        /// <summary>
        ///     Duration
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        ///     Video format
        /// </summary>
        string Format { get; }

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
        /// File source of stream
        /// </summary>
        FileInfo Source { get; }

        /// <summary>
        ///     Build FFmpeg arguments
        /// </summary>
        /// <returns>Arguments</returns>
        string Build();
    }
}
