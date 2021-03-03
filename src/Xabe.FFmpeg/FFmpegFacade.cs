using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg.Streams.SubtitleStream;

namespace Xabe.FFmpeg
{
    /// <summary> 
    ///     Wrapper for FFmpeg
    /// </summary>
    public abstract partial class FFmpeg
    {
        /// <summary>
        ///     Directory containing FFmpeg and FFprobe
        /// </summary>
        public static string ExecutablesPath { get; private set; }

        /// <summary>
        ///     Get new instance of Conversion
        /// </summary>
        /// <returns>IConversion object</returns>
        public static Conversions Conversions = new Conversions();

        /// <summary>
        ///     Get MediaInfo from file
        /// </summary>
        /// <param name="filePath">FullPath to file</param>
        /// <exception cref="ArgumentException">File does not exist</exception>
        public static async Task<IMediaInfo> GetMediaInfo(string fileName)
        {
            return await MediaInfo.Get(fileName);
        }

        /// <summary>
        ///     Get MediaInfo from file
        /// </summary>
        /// <param name="filePath">FullPath to file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <exception cref="ArgumentException">File does not exist</exception>
        /// <exception cref="TaskCanceledException">Operation takes too long</exception>
        public static async Task<IMediaInfo> GetMediaInfo(string fileName, CancellationToken token)
        {
            return await MediaInfo.Get(fileName, token);
        }

        /// <summary>
        ///     Set path to irectory containing FFmpeg and FFprobe
        /// </summary>
        /// <param name="directoryWithFFmpegAndFFprobe"></param>
        /// <param name="ffmpegExeutableName">Name of FFmpeg executable name (Case insensitive)</param>
        /// <param name="ffprobeExecutableName">Name of FFprobe executable name (Case insensitive)</param>
        public static void SetExecutablesPath(string directoryWithFFmpegAndFFprobe, string ffmpegExeutableName = "ffmpeg", string ffprobeExecutableName = "ffprobe")
        {
            ExecutablesPath = directoryWithFFmpegAndFFprobe == null ? null : new DirectoryInfo(directoryWithFFmpegAndFFprobe).FullName;
            _ffmpegExecutableName = ffmpegExeutableName;
            _ffprobeExecutableName = ffprobeExecutableName;
        }

        /// <summary>
        ///     Get available audio and video devices (like cams or mics)
        /// </summary>
        /// <returns>List of available devices</returns>
        public static async Task<Device[]> GetAvailableDevices()
        {
            return await Conversion.GetAvailableDevices();
        }
    }

    public class Conversions
    {
        /// <summary>
        ///     Get new instance of Conversion
        /// </summary>
        /// <returns>IConversion object</returns>
        public IConversion New()
        {
            return Conversion.New();
        }

        /// <summary>
        ///     Get new instance of Conversion
        /// </summary>
        /// <returns>IConversion object</returns>
        public Snippets FromSnippet = new Snippets();

        internal Conversions()
        {

        }
    }

    public class Snippets
    {
        internal Snippets()
        {

        }

        /// <summary>
        ///     Extract audio from file
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Output video stream</param>
        /// <returns>Conversion result</returns>
        public async Task<IConversion> ExtractAudio(string inputPath, string outputPath)
        {
            return await Task.FromResult(Conversion.ExtractAudio(inputPath, outputPath));
        }

        /// <summary>
        ///     Add audio stream to video file
        /// </summary>
        /// <param name="videoPath">Video</param>
        /// <param name="audioPath">Audio</param>
        /// <param name="outputPath">Output file</param>
        /// <returns>Conversion result</returns>
        public async Task<IConversion> AddAudio(string videoPath, string audioPath, string outputPath)
        {
            return await Task.FromResult(Conversion.AddAudio(videoPath, audioPath, outputPath));
        }

        /// <summary>
        ///     Convert file to MP4
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Destination file</param>
        /// <returns>Conversion result</returns>
        public async Task<IConversion> ToMp4(string inputPath, string outputPath)
        {
            return await Task.FromResult(Conversion.ToMp4(inputPath, outputPath));
        }

        /// <summary>
        ///     Convert file to TS
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Destination file</param>
        /// <returns>Conversion result</returns>
        public async Task<IConversion> ToTs(string inputPath, string outputPath)
        {
            return await Task.FromResult(Conversion.ToTs(inputPath, outputPath));
        }

        /// <summary>
        ///     Convert file to OGV
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Destination file</param>
        /// <returns>Conversion result</returns>
        public async Task<IConversion> ToOgv(string inputPath, string outputPath)
        {
            return await Task.FromResult(Conversion.ToOgv(inputPath, outputPath));
        }

        /// <summary>
        ///     Convert file to WebM
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Destination file</param>
        /// <returns>Conversion result</returns>
        public async Task<IConversion> ToWebM(string inputPath, string outputPath)
        {
            return await Task.FromResult(Conversion.ToWebM(inputPath, outputPath));
        }

        /// <summary>
        ///     Convert image video stream to gif
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Output path</param>
        /// <param name="loop">Number of repeats</param>
        /// <param name="delay">Delay between repeats (in seconds)</param>
        /// <returns>Conversion result</returns>
        public async Task<IConversion> ToGif(string inputPath, string outputPath, int loop, int delay = 0)
        {
            return await Task.FromResult(Conversion.ToGif(inputPath, outputPath, loop, delay));
        }

        /// <summary>
        ///     Convert one file to another with destination format using hardware acceleration (if possible). Using cuvid. Works only on Windows/Linux with NVidia GPU.
        /// </summary>
        /// <param name="inputFilePath">Path to file</param>
        /// <param name="outputFilePath">Path to file</param>
        /// <param name="hardwareAccelerator">Hardware accelerator. List of all acceclerators available for your system - "ffmpeg -hwaccels"</param>
        /// <param name="decoder">Codec using to decoding input video (e.g. h264_cuvid)</param>
        /// <param name="encoder">Codec using to encode output video (e.g. h264_nvenc)</param>
        /// <param name="device">Number of device (0 = default video card) if more than one video card.</param>
        /// <returns>IConversion object</returns>
        public async Task<IConversion> ConvertWithHardware(string inputFilePath, string outputFilePath, HardwareAccelerator hardwareAccelerator, VideoCodec decoder, VideoCodec encoder, int device = 0)
        {
            return await Task.FromResult(Conversion.ConvertWithHardware(inputFilePath, outputFilePath, hardwareAccelerator, decoder, encoder, device));
        }

        /// <summary>
        ///     Add subtitles to video stream
        /// </summary>
        /// <param name="inputPath">Video</param>
        /// <param name="outputPath">Output file</param>
        /// <param name="subtitlesPath">Subtitles</param>
        /// <returns>Conversion result</returns>
        public async Task<IConversion> BurnSubtitle(string inputPath, string outputPath, string subtitlesPath)
        {
            return await Task.FromResult(Conversion.AddSubtitles(inputPath, outputPath, subtitlesPath));
        }

        /// <summary>
        ///     Add subtitle to file. It will be added as new stream so if you want to burn subtitles into video you should use
        ///     BurnSubtitle method.
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Output path</param>
        /// <param name="subtitlePath">Path to subtitle file in .srt format</param>
        /// <param name="language">Language code in ISO 639. Example: "eng", "pol", "pl", "de", "ger"</param>
        /// <returns>Conversion result</returns>
        public async Task<IConversion> AddSubtitle(string inputPath, string outputPath, string subtitlePath, string language = null)
        {
            return await Task.FromResult(Conversion.AddSubtitle(inputPath, outputPath, subtitlePath, language));
        }


        /// <summary>
        ///     Add subtitle to file. It will be added as new stream so if you want to burn subtitles into video you should use
        ///     BurnSubtitle method.
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Output path</param>
        /// <param name="subtitlePath">Path to subtitle file in .srt format</param>
        /// <param name="subtitleCodec">The Subtitle Codec to Use to Encode the Subtitles</param>
        /// <param name="language">Language code in ISO 639. Example: "eng", "pol", "pl", "de", "ger"</param>
        /// <returns>Conversion result</returns>
        public async Task<IConversion> AddSubtitle(string inputPath, string outputPath, string subtitlePath, SubtitleCodec subtitleCodec, string language = null)
        {
            return await Task.FromResult(Conversion.AddSubtitle(inputPath, outputPath, subtitlePath, subtitleCodec, language));
        }

        /// <summary>
        ///     Melt watermark into video
        /// </summary>
        /// <param name="inputPath">Input video path</param>
        /// <param name="outputPath">Output file</param>
        /// <param name="inputImage">Watermark</param>
        /// <param name="position">Position of watermark</param>
        /// <returns>Conversion result</returns>
        public async Task<IConversion> SetWatermark(string inputPath, string outputPath, string inputImage, Position position)
        {
            return await Task.FromResult(Conversion.SetWatermark(inputPath, outputPath, inputImage, position));
        }

        /// <summary>
        ///     Extract video from file
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Output audio stream</param>
        /// <returns>Conversion result</returns>
        public async Task<IConversion> ExtractVideo(string inputPath, string outputPath)
        {
            return await Task.FromResult(Conversion.ExtractVideo(inputPath, outputPath));
        }

        /// <summary>
        ///     Saves snapshot of video
        /// </summary>
        /// <param name="inputPath">Video</param>
        /// <param name="outputPath">Output file</param>
        /// <param name="captureTime">TimeSpan of snapshot</param>
        /// <returns>Conversion result</returns>
        public async Task<IConversion> Snapshot(string inputPath, string outputPath, TimeSpan captureTime)
        {
            return await Task.FromResult(Conversion.Snapshot(inputPath, outputPath, captureTime));
        }

        /// <summary>
        ///     Change video size
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Output path</param>
        /// <param name="width">Expected width</param>
        /// <param name="height">Expected height</param>
        /// <returns>Conversion result</returns>
        public async Task<IConversion> ChangeSize(string inputPath, string outputPath, int width, int height)
        {
            return await Task.FromResult(Conversion.ChangeSize(inputPath, outputPath, width, height));
        }

        /// <summary>
        ///     Change video size
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Output path</param>
        /// <param name="size">Expected size</param>
        /// <returns>Conversion result</returns>
        public async Task<IConversion> ChangeSize(string inputPath, string outputPath, VideoSize size)
        {
            return await Task.FromResult(Conversion.ChangeSize(inputPath, outputPath, size));
        }

        /// <summary>
        ///     Get part of video
        /// </summary>
        /// <param name="inputPath">Video</param>
        /// <param name="outputPath">Output file</param>
        /// <param name="startTime">Start point</param>
        /// <param name="duration">Duration of new video</param>
        /// <returns>Conversion result</returns>
        public async Task<IConversion> Split(string inputPath, string outputPath, TimeSpan startTime, TimeSpan duration)
        {
            return await Task.FromResult(Conversion.Split(inputPath, outputPath, startTime, duration));
        }

        /// <summary>
        /// Save M3U8 stream
        /// </summary>
        /// <param name="uri">Uri to stream</param>
        /// <param name="outputPath">Output path</param>
        /// <param name="duration">Duration of stream</param>
        /// <returns>Conversion result</returns>
        public async Task<IConversion> SaveM3U8Stream(Uri uri, string outputPath, TimeSpan? duration = null)
        {
            return await Task.FromResult(Conversion.SaveM3U8Stream(uri, outputPath, duration));
        }

        /// <summary>
        ///     Concat multiple inputVideos.
        /// </summary>
        /// <param name="output">Concatenated inputVideos</param>
        /// <param name="inputVideos">Videos to add</param>
        /// <returns>Conversion result</returns>
        public async Task<IConversion> Concatenate(string output, params string[] inputVideos)
        {
            return await Conversion.Concatenate(output, inputVideos);
        }


        /// <summary>
        ///     Convert one file to another with destination format.
        /// </summary>
        /// <param name="inputFilePath">Path to file</param>
        /// <param name="outputFilePath">Path to file</param>
        /// <param name="keepSubtitles">Whether to Keep Subtitles in the output video</param>
        /// <returns>IConversion object</returns>
        public async Task<IConversion> Convert(string inputFilePath, string outputFilePath, bool keepSubtitles = false)
        {
            return await Task.FromResult(Conversion.Convert(inputFilePath, outputFilePath, keepSubtitles));
        }

        /// <summary>
        ///     Transcode one file to another with destination format and codecs.
        /// </summary>
        /// <param name="inputFilePath">Path to file</param>
        /// <param name="outputFilePath">Path to file</param>
        /// <param name="audioCodec"> The Audio Codec to Transcode the input to</param>
        /// <param name="videoCodec"> The Video Codec to Transcode the input to</param>
        /// <param name="videoCodec"> The Subtitle Codec to Transcode the input to</param>
        /// <param name="keepSubtitles">Whether to Keep Subtitles in the output video</param>
        /// <returns>IConversion object</returns>
        public async Task<IConversion> Transcode(string inputFilePath, string outputFilePath, VideoCodec videoCodec, AudioCodec audioCodec, SubtitleCodec subtitleCodec, bool keepSubtitles = false)
        {
            return await Task.FromResult(Conversion.Transcode(inputFilePath, outputFilePath, videoCodec, audioCodec, subtitleCodec, keepSubtitles));
        }

        /// <summary>
        /// Generates a visualisation of an audio stream using the 'showfreqs' filter
        /// </summary>
        /// <param name="inputPath">Path to the input file containing the audio stream to visualise</param>
        /// <param name="outputPath">Path to output the visualised audio stream to</param>
        /// <param name="size">The Size of the outputted video stream</param>
        /// <param name="pixelFormat">The output pixel format (default is yuv420p)</param>
        /// <param name="mode">The visualisation mode (default is bar)</param>
        /// <param name="amplitudeScale">The frequency scale (default is lin)</param>
        /// <param name="frequencyScale">The amplitude scale (default is log)</param>
        /// <returns>IConversion object</returns>
        public async Task<IConversion> VisualiseAudio(string inputPath, string outputPath, VideoSize size,
            PixelFormat pixelFormat = PixelFormat.yuv420p,
            VisualisationMode mode = VisualisationMode.bar,
            AmplitudeScale amplitudeScale = AmplitudeScale.lin,
            FrequencyScale frequencyScale = FrequencyScale.log)
        {
            return await Task.FromResult(Conversion.VisualiseAudio(inputPath, outputPath, size, pixelFormat, mode, amplitudeScale, frequencyScale));
        }

        /// <summary>
        ///     Loop file infinitely to rtsp streams with some default parameters like: -re, -preset ultrafast
        /// </summary>
        /// <param name="inputFilePath">Path to file</param>
        /// <param name="rtspServerUri">Uri of RTSP Server in format: rtsp://127.0.0.1:8554/name</param>
        /// <returns>IConversion object</returns>
        public async Task<IConversion> SendToRtspServer(string inputFilePath, Uri rtspServerUri)
        {
            return await Task.FromResult(Conversion.SendToRtspServer(inputFilePath, rtspServerUri));
        }

        /// <summary>
        ///     Loop file infinitely to rtsp streams with some default parameters like: -re, -preset ultrafast
        /// </summary>
        /// <param name="inputFilePath">Path to file</param>
        /// <param name="rtspServerUri">Uri of RTSP Server in format: rtsp://127.0.0.1:8554/name</param>
        /// <returns>IConversion object</returns>
        public async Task<IConversion> SendDesktopToRtspServer(Uri rtspServerUri)
        {
            return await Task.FromResult(Conversion.SendDesktopToRtspServer(rtspServerUri));
        }
    }
}
