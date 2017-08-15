using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Xabe.FFMpeg.Enums;

namespace Xabe.FFMpeg
{
    /// <summary>
    ///     Information about media file
    /// </summary>
    public class VideoInfo : IDisposable
    {
        /// <summary>
        ///     _sourceFile info
        /// </summary>

        public readonly string FilePath;

        /// <summary>
        ///     Return extension of file
        /// </summary>
        public string Extension => Path.GetExtension(FilePath);

        private FFMpeg _ffmpeg;

        /// <summary>
        ///     Fires when conversion progress changed
        /// </summary>
        public ConversionHandler OnConversionProgress;

        /// <summary>
        ///     Get VideoInfo from file
        /// </summary>
        /// <param name="sourceFileInfo">_sourceFile</param>
        public VideoInfo(FileInfo sourceFileInfo) : this(sourceFileInfo.FullName)
        {
        }

        /// <summary>
        ///     Get VideoInfo from file
        /// </summary>
        /// <param name="filePath">FilePath to file</param>
        public VideoInfo(string filePath)
        {
            if(!File.Exists(filePath))
            {
                throw new ArgumentException($"Input file {filePath} doesn't exists.");
            }
            FilePath = filePath;
            new FFProbe().ProbeDetails(this);
        }

        private FFMpeg FFmpeg
        {
            get
            {
                if(_ffmpeg != null &&
                   _ffmpeg.IsRunning)
                    throw new InvalidOperationException(
                        "Operation on this file is in progress.");

                return _ffmpeg ?? (_ffmpeg = new FFMpeg());
            }
        }

        /// <summary>
        ///     duration of video
        /// </summary>
        public TimeSpan Duration { get; internal set; }

        /// <summary>
        ///     Audio format
        /// </summary>
        public string AudioFormat { get; internal set; }

        /// <summary>
        ///     Video format
        /// </summary>
        public string VideoFormat { get; internal set; }

        /// <summary>
        ///     Screen ratio
        /// </summary>
        public string Ratio { get; internal set; }

        /// <summary>
        ///     Frame rate
        /// </summary>
        public double FrameRate { get; internal set; }

        /// <summary>
        ///     Height
        /// </summary>
        public int Height { get; internal set; }

        /// <summary>
        ///     Width
        /// </summary>
        public int Width { get; internal set; }

        /// <summary>
        ///     size
        /// </summary>
        public double Size { get; internal set; }

        /// <summary>
        ///     Get the ffmpeg process status
        /// </summary>
        public bool IsRunning => FFmpeg.IsRunning;

        /// <summary>
        ///     Create VideoInfo from file
        /// </summary>
        /// <param name="fileInfo">_sourceFile</param>
        /// <returns>VideoInfo</returns>
        public static VideoInfo FromFile(FileInfo fileInfo)
        {
            return new VideoInfo(fileInfo);
        }

        /// <summary>
        ///     Get formated info about video
        /// </summary>
        /// <returns>Formated info about vidoe</returns>
        public override string ToString()
        {
            return "Video FilePath : " + FilePath + Environment.NewLine +
                   "Video Root : " + Path.GetDirectoryName(FilePath)+ Environment.NewLine +
                   "Video Name: " + Path.GetFileName(FilePath) + Environment.NewLine +
                   "Video Extension : " + Extension + Environment.NewLine +
                   "Video duration : " + Duration + Environment.NewLine +
                   "Audio format : " + AudioFormat + Environment.NewLine +
                   "Video format : " + VideoFormat + Environment.NewLine +
                   "Aspect Ratio : " + Ratio + Environment.NewLine +
                   "Framerate : " + FrameRate + "fps" + Environment.NewLine +
                   "Resolution : " + Width + "x" + Height + Environment.NewLine +
                   "Size : " + Size + " MB";
        }

        /// <summary>
        ///     Convert file to TS
        /// </summary>
        /// <param name="outputPath">Destination file</param>
        /// <returns>Output VideoInfo</returns>
        public VideoInfo ToTs(string outputPath)
        {
            FFmpeg.OnProgress += OnConversionProgress;
            var success = FFmpeg.ToTs(this, outputPath);
            if (!success)
                throw new OperationCanceledException("The conversion process could not be completed.");
            FFmpeg.OnProgress -= OnConversionProgress;
            return new VideoInfo(outputPath);
        }

        /// <summary>
        ///     Convert file to WebM
        /// </summary>
        /// <param name="outputPath">Destination file</param>
        /// <param name="size">Dimension</param>
        /// <param name="audioQuality">Audio quality</param>
        /// <returns>Output VideoInfo</returns>
        public VideoInfo ToWebM(string outputPath, VideoSize size = VideoSize.Original, AudioQuality audioQuality = AudioQuality.Normal)
        {
            FFmpeg.OnProgress += OnConversionProgress;
            var success = FFmpeg.ToWebM(this, outputPath, size, audioQuality);
            if (!success)
                throw new OperationCanceledException("The conversion process could not be completed.");
            FFmpeg.OnProgress -= OnConversionProgress;
            return new VideoInfo(outputPath);
        }

        /// <summary>
        ///     Convert file to OGV
        /// </summary>
        /// <param name="outputPath">Destination file</param>
        /// <param name="size">Dimension</param>
        /// <param name="audioQuality">Audio quality</param>
        /// <param name="multithread">Use multithread</param>
        /// <returns>Output VideoInfo</returns>
        public VideoInfo ToOgv(string outputPath, VideoSize size = VideoSize.Original, AudioQuality audioQuality = AudioQuality.Normal, bool multithread = false)
        {
            FFmpeg.OnProgress += OnConversionProgress;
            var success = FFmpeg.ToOgv(this, outputPath, size, audioQuality, multithread);
            if (!success)
                throw new OperationCanceledException("The conversion process could not be completed.");
            FFmpeg.OnProgress -= OnConversionProgress;
            return new VideoInfo(outputPath);
        }

        /// <summary>
        ///     Convert file to MP4
        /// </summary>
        /// <param name="outputPath">Destination file</param>
        /// <param name="speed">Conversion speed</param>
        /// <param name="size">Dimension</param>
        /// <param name="audioQuality">Audio quality</param>
        /// <param name="multithread">Use multithread</param>
        /// <returns>Output VideoInfo</returns>
        public VideoInfo ToMp4(string outputPath, Speed speed = Speed.SuperFast,
            VideoSize size = VideoSize.Original, AudioQuality audioQuality = AudioQuality.Normal, bool multithread = false)
        {
            FFmpeg.OnProgress += OnConversionProgress;
            var success = FFmpeg.ToMp4(this, outputPath, speed, size, audioQuality, multithread);
            if (!success)
                throw new OperationCanceledException("The conversion process could not be completed.");
            FFmpeg.OnProgress -= OnConversionProgress;
            return new VideoInfo(outputPath);
        }

        /// <summary>
        ///     Extract video from file
        /// </summary>
        /// <param name="output">Output audio stream</param>
        /// <returns>Conversion result</returns>
        public bool ExtractVideo(string output)
        {
            return FFmpeg.ExtractVideo(this, output);
        }

        /// <summary>
        ///     Extract audio from file
        /// </summary>
        /// <param name="output">Output video stream</param>
        /// <returns>Conversion result</returns>
        public bool ExtractAudio(string output)
        {
            return FFmpeg.ExtractAudio(this, output);
        }

        /// <summary>
        ///     Add audio to file
        /// </summary>
        /// <param name="audio">Audio file</param>
        /// <param name="output">Output file</param>
        /// <returns>Conversion result</returns>
        public bool AddAudio(FileInfo audio, string output)
        {
            return FFmpeg.AddAudio(this, audio, output);
        }

        /// <summary>
        ///     Get snapshot of video
        /// </summary>
        /// <param name="size">Dimension of snapshot</param>
        /// <param name="captureTime"></param>
        /// <returns>Snapshot</returns>
        public Bitmap Snapshot(Size? size = null, TimeSpan? captureTime = null)
        {
            var output = $"{Environment.TickCount}.png";

            bool success = FFmpeg.Snapshot(this, output, size, captureTime);

            if(!success)
                throw new OperationCanceledException("Could not take snapshot!");

            Bitmap result;

            using(Image bmp = Image.FromFile(output))
            {
                using(var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);

                    result = new Bitmap(ms);
                }
            }

            if(File.Exists(output))
                File.Delete(output);

            return result;
        }

        /// <summary>
        ///     Saves snapshot of video
        /// </summary>
        /// <param name="output">Output file</param>
        /// <param name="size">Dimension of snapshot</param>
        /// <param name="captureTime"></param>
        /// <returns>Snapshot</returns>
        public Bitmap Snapshot(string output, Size? size = null, TimeSpan? captureTime = null)
        {
            bool success = FFmpeg.Snapshot(this, output, size, captureTime);

            if(!success)
                throw new OperationCanceledException("Could not take snapshot!");

            Bitmap result;


            using(Image bmp = Image.FromFile(System.IO.Path.ChangeExtension(output, ".png")))
            {
                result = (Bitmap) bmp.Clone();
            }

            return result;
        }

        /// <summary>
        ///     Concat multiple videos
        /// </summary>
        /// <param name="output">Concatenated videos</param>
        /// <param name="videos">Videos to add</param>
        /// <returns>Conversion result</returns>
        public bool JoinWith(string output, params VideoInfo[] videos)
        {
            List<VideoInfo> queuedVideos = videos.ToList();

            queuedVideos.Insert(0, this);

            return FFmpeg.Join(output, queuedVideos.ToArray());
        }

        /// <inheritdoc />
        public void Dispose()
        {
            FFmpeg.Stop();
            _ffmpeg?.Dispose();
        }
    }
}
