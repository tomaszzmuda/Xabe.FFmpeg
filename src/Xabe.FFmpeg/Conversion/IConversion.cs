using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg.Events;
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
        /// Set priority of ffmpeg process
        /// </summary>
        /// <param name="priority">FFmpeg process priority</param>
        /// <returns></returns>
        IConversion SetPriority(ProcessPriorityClass? priority);

        /// <summary>
        /// Extracts Every frameNo frame of the input video and outputs as a png image
        /// </summary>
        /// <param name="frameNo">The frame interval to extract </param>
        /// <param name="buildOutputFileName"> Delegate Function to build up custom filename when outputting multiple files </param>
        /// <returns></returns>
        IConversion ExtractEveryNthFrame(int frameNo, Func<string, string> buildOutputFileName);

        /// <summary>
        /// Extracts the frameNo'th frame of the input video and outputs as a png image
        /// </summary>
        /// <param name="frameNo">The frame to extract</param>
        /// <param name="buildOutputFileName"> Delegate Function to build up custom filename when outputting multiple files </param>
        /// <returns></returns>
        IConversion ExtractNthFrame(int frameNo, Func<string, string> buildOutputFileName);

        /// <summary>
        /// Builds a video from a directory containing one or more sequentially named images
        /// </summary>
        /// <param name="startNumber">The number of the image to start building video from</param>
        /// <param name="buildInputFileName"> Delegate Function to build up custom filename when inputting multiple files </param>
        /// <returns>IConversion object</returns>
        IConversion BuildVideoFromImages(int startNumber, Func<string, string> buildInputFileName);

        /// <summary>
        /// Builds a video from a directory containing one or more sequentially named images
        /// </summary>
        /// <param name="imageFiles"> List of Image Files to Build into a Video</param>
        /// <returns>IConversion object</returns>
        IConversion BuildVideoFromImages(IEnumerable<string> imageFiles);

        /// <summary>
        /// Builds the -framerate option for the output of this conversion
        /// </summary>
        /// <param name="frameRate">The desired framerate of the output</param>
        /// <returns>IConversion object</returns>
        IConversion SetFrameRate(double frameRate);

        /// <summary>
        /// Builds the -framerate option for the input of this conversion
        /// </summary>
        /// <param name="frameRate">the desired framerate of the input in bytes</param>
        /// <returns>IConversion object</returns>
        IConversion SetInputFrameRate(double frameRate);

        /// <summary>
        ///     Seeks in output file to position. (-ss argument)
        /// </summary>
        /// <param name="seek">Position</param>
        /// <returns>IConversion</returns>
        IConversion SetSeek(TimeSpan? seek);

        /// <summary>
        ///     Sets input capture length (-t input argument)
        ///     Typically used with the GetScreenCapture Function to stop capturing after a time interval
        /// </summary>
        /// <param name="seek">Output Length</param>
        /// <returns>IConversion</returns>
        IConversion SetInputTime(TimeSpan? seek);

        /// <summary>
        ///     Sets output file length (-t output argument)
        /// </summary>
        /// <param name="seek">Output Length</param>
        /// <returns>IConversion</returns>
        IConversion SetOutputTime(TimeSpan? seek);

        /// <summary>
        ///     Set preset of IConversion. Slower speed equals better compression and quality.
        /// </summary>
        /// <param name="preset">Preset</param>
        /// <returns>IConversion object</returns>
        IConversion SetPreset(ConversionPreset preset);

        /// <summary>
        ///     Set the hash format of IConversion.
        /// </summary>
        /// <param name="format">The required hash format</param>
        /// <returns>IConversion object</returns>
        IConversion SetHashFormat(Hash format = Hash.SHA256);

        /// <summary>
        ///     Set the hash format of IConversion.
        /// </summary>
        /// <param name="format">The required hash format</param>
        /// <returns>IConversion object</returns>
        IConversion SetHashFormat(string format);

        /// <summary>
        ///     Sets The bitrate of the video streams to the supplied value in bytes
        /// </summary>
        /// <param name="bitrate">The required Bitrate Value</param>
        /// <returns>IConversion object</returns>
        IConversion SetVideoBitrate(long bitrate);

        /// <summary>
        ///     Sets The bitrate of the audio streams to the supplied value in bytes
        /// </summary>
        /// <param name="bitrate">The required Bitrate Value</param>
        /// <returns>IConversion object</returns>
        IConversion SetAudioBitrate(long bitrate);

        /// <summary>
        ///     Defines thread count used by converter
        /// </summary>
        /// <param name="threadCount">Number of used threads</param>
        /// <returns>IConversion object</returns>
        IConversion UseMultiThread(int threadCount);

        /// <summary>
        ///     Defines if converter should use all CPU cores. If set to true FFmpeg use max 16 cores due to compatibility reason
        /// </summary>
        /// <param name="multiThread">Use multiThreads</param>
        /// <returns>IConversion object</returns>
        IConversion UseMultiThread(bool multiThread);

        /// <summary>
        ///     Set output path
        /// </summary>
        /// <param name="outputPath">Output media file</param>
        /// <returns>IConversion object</returns>
        IConversion SetOutput(string outputPath);

        /// <summary>
        ///     Set overwrite output file parameter
        /// </summary>
        /// <param name="overwrite">Should be output file overwritten or not. If not overwrite and file exists conversion will throw ConversionException</param>
        /// <returns>IConversion object</returns>
        IConversion SetOverwriteOutput(bool overwrite);

        /// <summary>
        /// Sets the format for the input file using the -f option before the input file name
        /// </summary>
        /// <param name="inputFormat">The input format to set</param>
        /// <returns>IConversion object</returns>
        IConversion SetInputFormat(string inputFormat);

        /// <summary>
        /// Sets the format for the input file using the -f option before the input file name
        /// </summary>
        /// <param name="inputFormat">The input format to set</param>
        /// <returns>IConversion object</returns>
        IConversion SetInputFormat(Format inputFormat);

        /// <summary>
        /// Sets the format for the output file using the -f option before the output file name
        /// </summary>
        /// <param name="outputFormat">The output format to set</param>
        /// <returns>IConversion object</returns>
        IConversion SetOutputFormat(Format outputFormat);

        /// <summary>
        /// Sets the format for the output file using the -f option before the output file name
        /// </summary>
        /// <param name="outputFormat">The output format to set</param>
        /// <returns>IConversion object</returns>
        IConversion SetOutputFormat(string outputFormat);

        /// <summary>
        /// Sets the pixel format for the output file using the -pix_fmt option before the output file name
        /// </summary>
        /// <param name="pixelFormat">The output pixel format to set</param>
        /// <returns>IConversion object</returns>
        IConversion SetPixelFormat(string pixelFormat);

        /// <summary>
        /// Sets the pixel format for the output file using the -pix_fmt option before the output file name
        /// </summary>
        /// <param name="pixelFormat">The output pixel format to set</param>
        /// <returns>IConversion object</returns>
        IConversion SetPixelFormat(PixelFormat pixelFormat);

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
        /// <param name="parameterPosition">Position of parameter</param>
        /// <returns>IConversion object</returns>
        IConversion AddParameter(string parameter, ParameterPosition parameterPosition = ParameterPosition.PostInput);

        /// <summary>
        ///     Add streams to output file
        /// </summary>
        /// <param name="streams">Streams to add</param>
        /// <returns>IConversion object</returns>
        IConversion AddStream<T>(params T[] streams) where T : IStream;

        /// <summary>
        ///     Add streams to output file
        /// </summary>
        /// <param name="streams">Streams to add</param>
        /// <returns>IConversion object</returns>
        IConversion AddStream(IEnumerable<IStream> streams);

        /// <summary>
        ///     Use hardware acceleration. This option set -threads to 1 for compatibility reasons. This should be use with proper codec (e.g. -c:v h264_nvenc or h264_cuvid)
        /// </summary>
        /// <param name="hardwareAccelerator">Hardware accelerator. List of all accelerators available for your system - "ffmpeg -hwaccels"</param>
        /// <param name="decoder">Codec using to decode input video.</param>
        /// <param name="encoder">Codec using to encode output video.</param>
        /// <param name="device">Number of device (0 = default video card) if more than one video card.</param>
        /// <returns>IConversion object</returns>
        IConversion UseHardwareAcceleration(HardwareAccelerator hardwareAccelerator, VideoCodec decoder, VideoCodec encoder, int device = 0);

        /// <summary>
        ///     Use hardware acceleration. This option set -threads to 1 for compatibility reasons. This should be use with proper codec (e.g. -c:v h264_nvenc or h264_cuvid)
        /// </summary>
        /// <param name="hardwareAccelerator">Hardware accelerator. List of all accelerators available for your system - "ffmpeg -hwaccels"</param>
        /// <param name="decoder">Codec using to decode input video.</param>
        /// <param name="encoder">Codec using to encode output video.</param>
        /// <param name="device">Number of device (0 = default video card) if more than one video card.</param>
        /// <returns>IConversion object</returns>
        IConversion UseHardwareAcceleration(string hardwareAccelerator, string decoder, string encoder, int device = 0);

        /// <summary>
        ///    Set video sync method.
        /// </summary>
        /// <param name="method">Vsync Mode - auto for skip</param>
        /// <returns>IConversion object</returns>
        IConversion SetVideoSyncMethod(VideoSyncMethod method);

        /// <summary>
        ///     List of all streams
        /// </summary>
        IEnumerable<IStream> Streams { get; }

        /// <summary>
        ///     Capture desktop to output file
        /// </summary>
        /// <param name="xOffset">X Offset</param>
        /// <param name="yOffset">Y Offset</param>
        /// <param name="videoSize">Input video size</param>
        /// <param name="framerate">The desired framerate of the output</param>
        /// <returns>IConversion object</returns>
        //Get desktop stream with "draw_mouse, x and y offsets, video_size and framerate"
        IConversion AddDesktopStream(string videoSize = null, double framerate = 30, int xOffset = 0, int yOffset = 0);

        /// <summary>
        ///     Capture desktop to output file
        /// </summary>
        /// <param name="xOffset">X Offset</param>
        /// <param name="yOffset">Y Offset</param>
        /// <param name="videoSize">Input video size</param>
        /// <param name="framerate">The desired framerate of the output</param>
        /// <returns>IConversion object</returns>
        //Get desktop stream with "draw_mouse, x and y offsets, video_size and framerate"
        IConversion AddDesktopStream(VideoSize videoSize, double framerate = 30, int xOffset = 0, int yOffset = 0);
    }
}
