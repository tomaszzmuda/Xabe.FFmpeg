using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        ///     Melt watermark into video
        /// </summary>
        /// <param name="imagePath">Watermark</param>
        /// <param name="position">Position of watermark</param>
        /// <returns>IConversion object</returns>
        IConversion SetWatermark(string imagePath, Position position);

        /// <summary>
        ///     Clear saved parameters
        /// </summary>
        void Clear();

        /// <summary>
        ///     Set preset of IConversion. Slower speed equals better compression and quality.
        /// </summary>
        /// <param name="preset">Preset</param>
        /// <returns>IConversion object</returns>
        IConversion SetPreset(ConversionPreset preset);

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
        ///     Finish encoding when the shortest input stream ends. (-shortest)
        /// </summary>
        /// <param name="useShortest"></param>
        /// <returns>IConversion object</returns>
        IConversion UseShortest(bool useShortest);

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
        Task<IConversionResult> Start();

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
        Task<IConversionResult> Start(CancellationToken cancellationToken);

        /// <summary>
        ///     Start an FFmpeg process with specified arguments
        /// </summary>
        /// <param name="parameters">FFmpeg parameters eg. "-i sample.mp4 -v 0 -vcodec mpeg4 -f mpegts udp://127.0.0.1:23000"</param>
        /// <returns>Conversion result</returns>
        /// <exception cref="ConversionException">Occurs when FFmpeg process return error.</exception>
        /// <exception cref="ArgumentException">Occurs when no FFmpeg executables were found.</exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        Task<IConversionResult> Start(string parameters);

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
        Task<IConversionResult> Start(string parameters, CancellationToken cancellationToken);

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
        IConversion AddStream<T>(params T[] streams) where T : IStream;
    }
}
