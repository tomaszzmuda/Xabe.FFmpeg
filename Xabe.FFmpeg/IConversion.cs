using System;
using System.Diagnostics;
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
        ///     Output file path
        /// </summary>
        string OutputFilePath { get; }

        /// <summary>
        ///     Clear saved parameters
        /// </summary>
        void Clear();

        /// <summary>
        ///     Reverse all streams in media
        /// </summary>
        /// <returns>IConversion object</returns>
        IConversion Reverse();

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
        ///     Defines if converter should use all CPU cores
        /// </summary>
        /// <param name="multiThread">Use all CPU cores</param>
        /// <returns>IConversion object</returns>
        IConversion UseMultiThread(bool multiThread);

        /// <summary>
        ///     Set output path
        /// </summary>
        /// <param name="outputPath">Output media file</param>
        /// <returns>IConversion object</returns>
        IConversion SetOutput(string outputPath);

        /// <summary>
        ///     Fires when FFmpeg progress changes
        /// </summary>
        event ConversionProgressEventHandler OnProgress;

        /// <summary>
        ///     Fires when FFmpeg process print sonething
        /// </summary>
        event DataReceivedEventHandler OnDataReceived;

        /// <summary>
        ///     It makes FFmpeg omit the decoding and encoding step for the specified stream, so it does only demuxing and muxing.
        ///     It is useful for changing the container format or modifying container-level metadata.
        ///     Cannot be used with operations which require encoding and decoding video (scaling, changing codecs etc.)
        /// </summary>
        /// <param name="type">Channel type</param>
        /// <returns>IConversion object</returns>
        IConversion StreamCopy(Channel type);

        /// <summary>
        ///     Change speed of media
        /// </summary>
        /// <param name="multiplaction">Speed value. (0.5 - 2.0). To double the speed set this to 2.0</param>
        /// <returns>IConversion object</returns>
        /// <exception cref="ArgumentOutOfRangeException">When speed isn't between 0.5 - 2.0.</exception>
        IConversion ChangeSpeed(double multiplaction);

        /// <summary>
        ///     Set media format
        /// </summary>
        /// <param name="format">Media format</param>
        /// <returns>IConversion object</returns>
        IConversion SetFormat(MediaFormat format);

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
        /// <returns></returns>
        IConversion AddSubtitle(string subtitlePath, string language);

        /// <summary>
        ///     Add additional parameters for the conversion (They must be well formed)
        /// </summary>
        /// <param name="parameter"> Parameter to set</param>
        /// <returns>IConversion object</returns>
        IConversion AddParameter(string parameter);

        /// <summary>
        ///     Add streams to output file
        /// </summary>
        /// <param name="streams">Streams to add</param>
        /// <returns>IConversion object</returns>
        IConversion AddStream(params IStream[] streams);
    }
}
