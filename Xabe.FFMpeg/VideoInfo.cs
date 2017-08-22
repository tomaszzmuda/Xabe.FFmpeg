using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Xabe.FFMpeg.Enums;

namespace Xabe.FFMpeg
{
    /// <inheritdoc />
    public class VideoInfo: IDisposable, IVideoInfo
    {
        private FFMpeg _ffmpeg;

        /// <inheritdoc />
        public VideoInfo(FileInfo sourceFileInfo): this(sourceFileInfo.FullName)
        {
        }

        /// <summary>
        ///     Get VideoInfo from file
        /// </summary>
        /// <param name="filePath">FilePath to file</param>
        public VideoInfo(string filePath)
        {
            if(!File.Exists(filePath))
                throw new ArgumentException($"Input file {filePath} doesn't exists.");
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

        /// <inheritdoc cref="IVideoInfo.Dispose" />
        public void Dispose()
        {
            FFmpeg.Stop();
            _ffmpeg?.Dispose();
        }

        /// <inheritdoc />
        public string FilePath { get; }

        /// <inheritdoc />
        public ConversionHandler OnConversionProgress { get; set; }


        /// <inheritdoc />
        public string Extension => Path.GetExtension(FilePath);


        /// <inheritdoc />
        public TimeSpan Duration { get; internal set; }

        /// <inheritdoc />
        public string AudioFormat { get; internal set; }


        /// <inheritdoc />
        public string VideoFormat { get; internal set; }


        /// <inheritdoc />
        public string Ratio { get; internal set; }

        /// <inheritdoc />
        public double FrameRate { get; internal set; }

        /// <inheritdoc />
        public int Height { get; internal set; }

        /// <inheritdoc />
        public int Width { get; internal set; }

        /// <inheritdoc />
        public double Size { get; internal set; }

        /// <inheritdoc />
        public bool IsRunning => FFmpeg.IsRunning;


        /// <inheritdoc />
        public override string ToString()
        {
            return "Video FilePath : " + FilePath + Environment.NewLine +
                   "Video Root : " + Path.GetDirectoryName(FilePath) + Environment.NewLine +
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


        /// <inheritdoc />
        public VideoInfo ToTs(string outputPath)
        {
            FFmpeg.OnProgress += OnConversionProgress;
            bool success = FFmpeg.ToTs(this, outputPath);
            if(!success)
                throw new OperationCanceledException("The conversion process could not be completed.");
            FFmpeg.OnProgress -= OnConversionProgress;
            return new VideoInfo(outputPath);
        }


        /// <inheritdoc />
        public VideoInfo ToWebM(string outputPath, string size = "", AudioQuality audioQuality = AudioQuality.Normal)
        {
            FFmpeg.OnProgress += OnConversionProgress;
            bool success = FFmpeg.ToWebM(this, outputPath, size, audioQuality);
            if(!success)
                throw new OperationCanceledException("The conversion process could not be completed.");
            FFmpeg.OnProgress -= OnConversionProgress;
            return new VideoInfo(outputPath);
        }


        /// <inheritdoc />
        public VideoInfo ToOgv(string outputPath, string size = "", AudioQuality audioQuality = AudioQuality.Normal, bool multithread = false)
        {
            FFmpeg.OnProgress += OnConversionProgress;
            bool success = FFmpeg.ToOgv(this, outputPath, size, audioQuality, multithread);
            if(!success)
                throw new OperationCanceledException("The conversion process could not be completed.");
            FFmpeg.OnProgress -= OnConversionProgress;
            return new VideoInfo(outputPath);
        }


        /// <inheritdoc />
        public VideoInfo ToMp4(string outputPath, Speed speed = Speed.SuperFast,
            string size = "", AudioQuality audioQuality = AudioQuality.Normal, bool multithread = false)
        {
            FFmpeg.OnProgress += OnConversionProgress;
            bool success = FFmpeg.ToMp4(this, outputPath, speed, size, audioQuality, multithread);
            if(!success)
                throw new OperationCanceledException("The conversion process could not be completed.");
            FFmpeg.OnProgress -= OnConversionProgress;
            return new VideoInfo(outputPath);
        }


        /// <inheritdoc />
        public bool ExtractVideo(string output)
        {
            return FFmpeg.ExtractVideo(this, output);
        }

        /// <inheritdoc />
        public bool ExtractAudio(string output)
        {
            return FFmpeg.ExtractAudio(this, output);
        }


        /// <inheritdoc />
        public bool AddAudio(FileInfo audio, string output)
        {
            return FFmpeg.AddAudio(this, audio, output);
        }


        /// <inheritdoc />
        public Bitmap Snapshot(Size? size = null, TimeSpan? captureTime = null)
        {
            string output = $"{Environment.TickCount}.png";

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


        /// <inheritdoc />
        public Bitmap Snapshot(string output, Size? size = null, TimeSpan? captureTime = null)
        {
            bool success = FFmpeg.Snapshot(this, output, size, captureTime);

            if(!success)
                throw new OperationCanceledException("Could not take snapshot!");

            Bitmap result;


            using(Image bmp = Image.FromFile(Path.ChangeExtension(output, ".png")))
            {
                result = (Bitmap) bmp.Clone();
            }

            return result;
        }


        /// <inheritdoc />
        public bool JoinWith(string output, params VideoInfo[] videos)
        {
            List<VideoInfo> queuedVideos = videos.ToList();

            queuedVideos.Insert(0, this);

            return FFmpeg.Join(output, queuedVideos.ToArray());
        }

        /// <summary>
        ///     Create VideoInfo from file
        /// </summary>
        /// <param name="fileInfo">_sourceFile</param>
        /// <returns>VideoInfo</returns>
        public static IVideoInfo FromFile(FileInfo fileInfo)
        {
            return new VideoInfo(fileInfo);
        }
    }
}
