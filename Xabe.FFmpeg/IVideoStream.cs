using System;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg
{
    public interface IVideoStream: IStream
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
        double FrameRate { get; }

        /// <summary>
        ///     Screen ratio
        /// </summary>
        string Ratio { get; }

        CodecType CodecType { get; }

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
        /// <returns>IVideoStream object</returns>
        IVideoStream SetScale(VideoSize size);

        /// <summary>
        ///     Set size of video
        /// </summary>
        /// <param name="size">VideoSize</param>
        /// <returns>IVideoStream object</returns>
        IVideoStream SetSize(VideoSize size);

        /// <summary>
        ///     Set video codec and bitrate
        /// </summary>
        /// <param name="codec">Video codec</param>
        /// <param name="bitrate">Video bitrate</param>
        /// <returns>IVideoStream object</returns>
        IVideoStream SetCodec(VideoCodec codec, int bitrate = 0);

        /// <summary>
        ///     Set stream to copy with orginal codec
        /// </summary>
        /// <returns>IVideoStream object</returns>
        IVideoStream CopyStream();

        /// <summary>
        ///     Set filter
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <returns>IVideoStream object</returns>
        IVideoStream SetBitstreamFilter(BitstreamFilter filter);

        /// <summary>
        ///     Loop over the input stream.(-loop)
        /// </summary>
        /// <param name="count">Number of repeats</param>
        /// <param name="delay">Delay between repeats (in seconds)</param>
        /// <returns>IVideoStream object</returns>
        IVideoStream SetLoop(int count, int delay = 0);

        /// <summary>
        ///     Set output frames count
        /// </summary>
        /// <param name="number">Number of frames</param>
        /// <returns>IConversion object</returns>
        IVideoStream SetOutputFramesCount(int number);

        /// <summary>
        ///     Seeks in input file to position. (-ss argument)
        /// </summary>
        /// <param name="seek">Position</param>
        /// <returns>IConversion object</returns>
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
        /// <returns>Conversion result</returns>
        IVideoStream AddSubtitles(string subtitlePath, string encode = null, string style = null, VideoSize originalSize = null);
    }
}
