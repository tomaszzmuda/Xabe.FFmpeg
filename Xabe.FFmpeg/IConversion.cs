using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;
using Xabe.FFmpeg.Exceptions;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Allows to prepare and start IConversion.
    /// </summary>
    public interface IConversion
    {
        /// <summary>
        ///     Clear saved parameters
        /// </summary>
        void Clear();

        /// <summary>
        ///     Reverse media
        /// </summary>
        /// <param name="type">Channel type</param>
        /// <returns>IConversion object</returns>
        IConversion Reverse(Channel type);

        /// <summary>
        ///     Set speed of IConversion. Slower speed equals better compression and quality.
        /// </summary>
        /// <param name="speed">Speed</param>
        /// <returns>IConversion object</returns>
        IConversion SetSpeed(Speed speed);

        /// <summary>
        ///     Set max cpu threads
        /// </summary>
        /// <param name="cpu">Threads</param>
        /// <returns>IConversion object</returns>
        IConversion SetSpeed(int cpu);

        /// <summary>
        ///     Set audio codec and bitrate
        /// </summary>
        /// <param name="codec">Audio odec</param>
        /// <param name="bitrate">Audio bitrade</param>
        /// <returns>IConversion object</returns>
        IConversion SetAudio(AudioCodec codec, AudioQuality bitrate);

        /// <summary>
        ///     Set audio codec and bitrate
        /// </summary>
        /// <param name="codec">Audio odec</param>
        /// <param name="bitrate">Audio bitrade</param>
        /// <returns>IConversion object</returns>
        IConversion SetAudio(string codec, AudioQuality bitrate);

        /// <summary>
        ///     Set video codec and bitrate
        /// </summary>
        /// <param name="codec">Video codec</param>
        /// <param name="bitrate">Video bitrate</param>
        /// <returns>IConversion object</returns>
        IConversion SetVideo(VideoCodec codec, int bitrate = 0);

        /// <summary>
        ///     Set video codec and bitrate
        /// </summary>
        /// <param name="codec">Video codec</param>
        /// <param name="bitrate">Video bitrate</param>
        /// <returns>IConversion object</returns>
        IConversion SetVideo(string codec, int bitrate = 0);

        /// <summary>
        ///     Defines if converter should use all CPU cores
        /// </summary>
        /// <param name="multiThread">Use all CPU cores</param>
        /// <returns>IConversion object</returns>
        IConversion UseMultiThread(bool multiThread);

        /// <summary>
        ///     Set URI of stream
        /// </summary>
        /// <param name="uri">URI</param>
        /// <returns>IConversion object</returns>
        IConversion SetInput(Uri uri);

        /// <summary>
        ///     Disable channel. Can remove audio or video from media file.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>IConversion object</returns>
        IConversion DisableChannel(Channel type);

        /// <summary>
        ///     Set input file
        /// </summary>
        /// <param name="input">Media file to convert</param>
        /// <returns>IConversion object</returns>
        IConversion SetInput(string input);

        /// <summary>
        ///     Set input file
        /// </summary>
        /// <param name="input">Media file to convert</param>
        /// <returns>IConversion object</returns>
        IConversion SetInput(FileInfo input);

        /// <summary>
        ///     Set input files
        /// </summary>
        /// <param name="inputs">Media files to convert</param>
        /// <returns>IConversion object</returns>
        IConversion SetInput(params string[] inputs);

        /// <summary>
        ///     Set output path
        /// </summary>
        /// <param name="outputPath">Output media file</param>
        /// <returns>IConversion object</returns>
        IConversion SetOutput(string outputPath);

        /// <summary>
        ///     Set size of video
        /// </summary>
        /// <param name="size">VideoSize</param>
        /// <returns>IConversion object</returns>
        IConversion SetScale(VideoSize size);

        /// <summary>
        ///     Set size of video
        /// </summary>
        /// <param name="size">VideoSize</param>
        /// <returns>IConversion object</returns>
        IConversion SetSize(Size? size);

        /// <summary>
        ///     Fires when FFmpeg progress changes
        /// </summary>
        event ConversionHandler OnProgress;

        /// <summary>
        ///     Fires when FFmpeg process print sonething
        /// </summary>
        event DataReceivedEventHandler OnDataReceived;

        /// <summary>
        ///     Set video codec
        /// </summary>
        /// <param name="codec">Video codec</param>
        /// <returns>IConversion object</returns>
        IConversion SetCodec(VideoCodec codec);

        /// <summary>
        ///     Set video codec
        /// </summary>
        /// <param name="codec">Video codec</param>
        /// <returns>IConversion object</returns>
        IConversion SetCodec(string codec);

        /// <summary>
        ///     Set filter
        /// </summary>
        /// <param name="type">Channel type</param>
        /// <param name="filter">Filter</param>
        /// <returns>IConversion object</returns>
        IConversion SetBitstreamFilter(Channel type, Filter filter);

        /// <summary>
        ///     Set filter
        /// </summary>
        /// <param name="type">Channel type</param>
        /// <param name="filter">Filter</param>
        /// <returns>IConversion object</returns>
        IConversion SetBitstreamFilter(Channel type, string filter);

        /// <summary>
        ///     It makes FFmpeg omit the decoding and encoding step for the specified stream, so it does only demuxing and muxing.
        ///     It is useful for changing the container format or modifying container-level metadata.
        ///     Cannot be used with operations which require encoding and decoding video (scaling, changing codecs etc.)
        /// </summary>
        /// <param name="type">Channel type</param>
        /// <returns>IConversion object</returns>
        IConversion StreamCopy(Channel type);

        /// <summary>
        ///     Melt watermark into video file
        /// </summary>
        /// <param name="imagePath">Watermark</param>
        /// <param name="position">Position of watermark</param>
        /// <returns>IConversion object</returns>
        IConversion SetWatermark(string imagePath, Position position);

        /// <summary>
        ///     Change speed of media
        /// </summary>
        /// <param name="channel">Channel</param>
        /// <param name="multiplaction">Speed value. (0.5 - 2.0). To double the speed set this to 2.0</param>
        /// <returns>IConversion object</returns>
        /// <exception cref="ArgumentOutOfRangeException">When speed isn't between 0.5 - 2.0.</exception>
        IConversion ChangeSpeed(Channel channel, double multiplaction);

        /// <summary>
        ///     Seeks in input file to position. (-ss argument)
        /// </summary>
        /// <param name="seek">Position</param>
        /// <returns>IConversion object</returns>
        IConversion SetSeek(TimeSpan? seek);

        /// <summary>
        ///     Set output frames count
        /// </summary>
        /// <param name="number">Number of frames</param>
        /// <returns>IConversion object</returns>
        IConversion SetOutputFramesCount(int number);

        /// <summary>
        ///     Loop over the input stream. Currently it works only for image streams. (-loop)
        /// </summary>
        /// <param name="count">Number of repeats</param>
        /// <param name="delay">Delay between repeats (in seconds)</param>
        /// <returns>IConversion object</returns>
        IConversion SetLoop(int count, int delay = 0);

        /// <summary>
        ///     Finish encoding when the shortest input stream ends. (-shortest)
        /// </summary>
        /// <param name="useShortest"></param>
        /// <returns>IConversion object</returns>
        IConversion UseShortest(bool useShortest);

        /// <summary>
        ///     Concat multiple media files. All files must have the same streams (same codecs, same time base, etc.)
        /// </summary>
        /// <param name="paths">Media files</param>
        /// <returns>IConversion object</returns>
        /// <exception cref="ArgumentException">When try to concatenate different formats.</exception>
        IConversion Concatenate(params string[] paths);

        /// <summary>
        ///     Build FFmpeg arguments
        /// </summary>
        /// <returns>Arguments</returns>
        string Build();

        /// <summary>
        ///     Start conversion
        /// </summary>
        /// <returns>Conversion result</returns>
        /// <exception cref="ConversionException">Occurs when FFmpeg process return error.</exception>
        /// <exception cref="ArgumentException">Occurs when no FFmpeg executables were found.</exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        Task<bool> Start();

        /// <summary>
        ///     Start conversion
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Conversion result</returns>
        /// <exception cref="ConversionException">Occurs when FFmpeg process return error.</exception>
        /// <exception cref="ArgumentException">Occurs when no FFmpeg executables were found.</exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="TaskCanceledException">Occurs when task was cancalled.</exception>
        Task<bool> Start(CancellationToken cancellationToken);

        /// <summary>
        ///     Start an FFmpeg process with specified arguments
        /// </summary>
        /// <param name="parameters">FFmpeg parameters eg. "-i sample.mp4 -v 0 -vcodec mpeg4 -f mpegts udp://127.0.0.1:23000"</param>
        /// <returns>Conversion result</returns>
        /// <exception cref="ConversionException">Occurs when FFmpeg process return error.</exception>
        /// <exception cref="ArgumentException">Occurs when no FFmpeg executables were found.</exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        Task<bool> Start(string parameters);

        /// <summary>
        ///     Start an FFmpeg process with specified arguments
        /// </summary>
        /// <param name="parameters">FFmpeg parameters eg. "-i sample.mp4 -v 0 -vcodec mpeg4 -f mpegts udp://127.0.0.1:23000"</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Conversion result</returns>
        /// <exception cref="ConversionException">Occurs when FFmpeg process return error.</exception>
        /// <exception cref="ArgumentException">Occurs when no FFmpeg executables were found.</exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="TaskCanceledException">Occurs when task was cancalled.</exception>
        Task<bool> Start(string parameters, CancellationToken cancellationToken);

        /// <summary>
        ///     Rotate video
        /// </summary>
        /// <param name="rotateDegrees">Rotate type</param>
        /// <returns>Conversion result</returns>
        IConversion Rotate(RotateDegrees rotateDegrees);

        /// <summary>
        ///     Get part of video
        /// </summary>
        /// <param name="startTime">Start point</param>
        /// <param name="duration">Duration of new video</param>
        /// <returns>Conversion result</returns>
        IConversion Split(TimeSpan startTime, TimeSpan duration);

        /// <summary>
        ///     Add subtitle to file
        /// </summary>
        /// <param name="subtitlePath">Path to subtitle file in .srt format</param>
        /// <param name="language">Language code in ISO 639. Example: "eng", "pol", "pl", "de", "ger"</param>
        /// <returns>Conversion result</returns>
        IConversion AddSubtitle(string subtitlePath, string language);


        /// <summary>
        ///     Burn subtitle into file
        /// </summary>
        /// <param name="subtitlePath">Path to subtitle file in .srt format</param>
        /// <returns>Conversion result</returns>
        IConversion SetSubtitle(string subtitlePath);

        /// <summary>
        ///     Output file path
        /// </summary>
        string OutputFilePath { get; }
    }
}
