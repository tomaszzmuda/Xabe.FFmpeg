using System;
using System.Drawing;
using System.IO;
using Xabe.FFMpeg.Enums;

namespace Xabe.FFMpeg
{
    /// <summary>
    ///     Information about media file
    /// </summary>
    public interface IVideoInfo
    {
        /// <summary>
        ///     Fires when conversion progress changed
        /// </summary>
        ConversionHandler OnConversionProgress { get; set; }

        /// <summary>
        ///     Audio format
        /// </summary>
        string AudioFormat { get; }

        /// <summary>
        ///     duration of video
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        ///     Return extension of file
        /// </summary>
        string Extension { get; }

        /// <summary>
        ///     Frame rate
        /// </summary>
        double FrameRate { get; }

        /// <summary>
        ///     Height
        /// </summary>
        int Height { get; }

        /// <summary>
        ///     Returns true if the associated process is still alive/running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        ///     Screen ratio
        /// </summary>
        string Ratio { get; }

        /// <summary>
        ///     size
        /// </summary>
        double Size { get; }

        /// <summary>
        ///     Video format
        /// </summary>
        string VideoFormat { get; }

        /// <summary>
        ///     Width
        /// </summary>
        int Width { get; }

        /// <summary>
        ///     Source file path
        /// </summary>
        string FilePath { get; }

        /// <summary>
        ///     Add audio to file
        /// </summary>
        /// <param name="audio">Audio file</param>
        /// <param name="output">Output file</param>
        /// <returns>Conversion result</returns>
        bool AddAudio(FileInfo audio, string output);

        /// <summary>
        ///     Stop FFMpeg process
        /// </summary>
        void Dispose();

        /// <summary>
        ///     Extract audio from file
        /// </summary>
        /// <param name="output">Output video stream</param>
        /// <returns>Conversion result</returns>
        bool ExtractAudio(string output);

        /// <summary>
        ///     Extract video from file
        /// </summary>
        /// <param name="output">Output audio stream</param>
        /// <returns>Conversion result</returns>
        bool ExtractVideo(string output);

        /// <summary>
        ///     Concat multiple videos
        /// </summary>
        /// <param name="output">Concatenated videos</param>
        /// <param name="videos">Videos to add</param>
        /// <returns>Conversion result</returns>
        bool JoinWith(string output, params IVideoInfo[] videos);

        /// <summary>
        ///     Get formated info about video
        /// </summary>
        /// <returns>Formated info about vidoe</returns>
        string ToString();

        /// <summary>
        ///     Get snapshot of video
        /// </summary>
        /// <param name="size">Dimension of snapshot</param>
        /// <param name="captureTime"></param>
        /// <returns>Snapshot</returns>
        Bitmap Snapshot(Size? size = null, TimeSpan? captureTime = null);

        /// <summary>
        ///     Saves snapshot of video
        /// </summary>
        /// <param name="output">Output file</param>
        /// <param name="size">Dimension of snapshot</param>
        /// <param name="captureTime"></param>
        /// <returns>Snapshot</returns>
        Bitmap Snapshot(string output, Size? size = null, TimeSpan? captureTime = null);

        /// <summary>
        ///     Convert file to MP4
        /// </summary>
        /// <param name="outputPath">Destination file</param>
        /// <param name="speed">Conversion speed</param>
        /// <param name="size">Dimension</param>
        /// <param name="audioQuality">Audio quality</param>
        /// <param name="multithread">Use multithread</param>
        /// <returns>Conversion result</returns>
        bool ToMp4(string outputPath, Speed speed = Speed.SuperFast, VideoSize size = null, AudioQuality audioQuality = AudioQuality.Normal,
            bool multithread = false);

        /// <summary>
        ///     Convert file to OGV
        /// </summary>
        /// <param name="outputPath">Destination file</param>
        /// <param name="size">Dimension</param>
        /// <param name="audioQuality">Audio quality</param>
        /// <param name="multithread">Use multithread</param>
        /// <returns>Conversion result</returns>
        bool ToOgv(string outputPath, VideoSize size = null, AudioQuality audioQuality = AudioQuality.Normal, bool multithread = false);

        /// <summary>
        ///     Convert file to TS
        /// </summary>
        /// <param name="outputPath">Destination file</param>
        /// <returns>Conversion result</returns>
        bool ToTs(string outputPath);

        /// <summary>
        ///     Convert file to WebM
        /// </summary>
        /// <param name="outputPath">Destination file</param>
        /// <param name="size">Dimension</param>
        /// <param name="audioQuality">Audio quality</param>
        /// <returns>Conversion result</returns>
        bool ToWebM(string outputPath, VideoSize size = null, AudioQuality audioQuality = AudioQuality.Normal);

        /// <summary>
        ///     Records M3U8 streams to the specified output.
        /// </summary>
        /// <param name="uri">URI to stream.</param>
        /// <param name="outputPath">Output file</param>
        /// <returns>Conversion result</returns>
        bool SaveM3U8Stream(Uri uri, string outputPath);

        /// <summary>
        ///     Saves snapshot of video
        /// </summary>
        /// <param name="source">Video</param>
        /// <param name="outputPath">Output file</param>
        /// <param name="size">Dimension of snapshot</param>
        /// <param name="captureTime"></param>
        /// <returns>Conversion result</returns>
        bool Snapshot(VideoInfo source, string outputPath, Size? size = null, TimeSpan? captureTime = null);
    }
}
