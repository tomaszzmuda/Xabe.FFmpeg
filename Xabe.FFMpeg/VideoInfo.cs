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
    public class VideoInfo
    {
        private readonly FileInfo _file;
        private FFMpeg _ffmpeg;

        /// <summary>
        ///     Fires when conversion progress changed
        /// </summary>
        public ConversionHandler OnConversionProgress;

        /// <summary>
        ///     Get VideoInfo from file
        /// </summary>
        /// <param name="fileInfo">File</param>
        public VideoInfo(FileInfo fileInfo)
        {
            fileInfo.Refresh();

            if(!fileInfo.Exists)
                throw new ArgumentException($"Input file {fileInfo.FullName} does not exist!");

            _file = fileInfo;
            new FFProbe().ProbeDetails(this);
        }

        /// <summary>
        ///     Get VideoInfo from file
        /// </summary>
        /// <param name="path">Path to file</param>
        public VideoInfo(string path): this(new FileInfo(path))
        {
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
        ///     Duration of video
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
        ///     Size
        /// </summary>
        public double Size { get; internal set; }

        /// <summary>
        ///     Filename
        /// </summary>
        public string Name => _file.Name;

        /// <summary>
        ///     Fullname (Path + Filename)
        /// </summary>
        public string FullName => _file.FullName;

        /// <summary>
        ///     Extension
        /// </summary>
        public string Extension => _file.Extension;

        /// <summary>
        ///     Is readonly
        /// </summary>
        public bool IsReadOnly => _file.IsReadOnly;

        /// <summary>
        ///     Gets a value indicating whether a file exists
        /// </summary>
        public bool Exists => _file.Exists;

        /// <summary>
        ///     Creation time
        /// </summary>
        public DateTime CreationTime => _file.CreationTime;

        /// <summary>
        ///     Directory
        /// </summary>
        public DirectoryInfo Directory => _file.Directory;

        /// <summary>
        ///     Get the ffmpeg process status
        /// </summary>
        public bool IsRunning => FFmpeg.IsRunning;

        /// <summary>
        ///     Convert file to specified format. Output file will be in the same directory as source with changed extension.
        /// </summary>
        /// <param name="type">Destination format</param>
        /// <param name="speed">Conversion speed</param>
        /// <param name="size">Dimension</param>
        /// <param name="audioQuality">Audio quality</param>
        /// <param name="multithread">Use multithread</param>
        /// <param name="deleteSource"></param>
        /// <returns>Output VideoInfo</returns>
        public VideoInfo ConvertTo(VideoType type, Speed speed = Speed.Medium, VideoSize size = VideoSize.Original,
            AudioQuality audioQuality = AudioQuality.Normal, bool multithread = true, bool deleteSource = false)
        {
            string outputPath = FullName.Replace(Extension, $".{type.ToString() .ToLower()}");
            var output = new FileInfo(outputPath);
            return ConvertTo(type, output, speed, size, audioQuality, multithread, deleteSource);
        }

        /// <summary>
        ///     Create VideoInfo from file
        /// </summary>
        /// <param name="fileInfo">File</param>
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
            return "Video Path : " + FullName + Environment.NewLine +
                   "Video Root : " + Directory.FullName + Environment.NewLine +
                   "Video Name: " + Name + Environment.NewLine +
                   "Video Extension : " + Extension + Environment.NewLine +
                   "Video Duration : " + Duration + Environment.NewLine +
                   "Audio Format : " + AudioFormat + Environment.NewLine +
                   "Video Format : " + VideoFormat + Environment.NewLine +
                   "Aspect Ratio : " + Ratio + Environment.NewLine +
                   "Framerate : " + FrameRate + "fps" + Environment.NewLine +
                   "Resolution : " + Width + "x" + Height + Environment.NewLine +
                   "Size : " + Size + " MB";
        }

        /// <summary>
        ///     Convert file to specified format
        /// </summary>
        /// <param name="type">Destination format</param>
        /// <param name="output">Destination file</param>
        /// <param name="speed">Conversion speed</param>
        /// <param name="size">Dimension</param>
        /// <param name="audioQuality">Audio quality</param>
        /// <param name="multithread">Use multithread</param>
        /// <param name="deleteSource"></param>
        /// <returns>Output VideoInfo</returns>
        public VideoInfo ConvertTo(VideoType type, FileInfo output, Speed speed = Speed.SuperFast,
            VideoSize size = VideoSize.Original, AudioQuality audioQuality = AudioQuality.Normal, bool multithread = false,
            bool deleteSource = false)
        {
            bool success;
            FFmpeg.OnProgress += OnConversionProgress;
            switch(type)
            {
                case VideoType.Mp4:
                    success = FFmpeg.ToMp4(this, output, speed, size, audioQuality, multithread);
                    break;
                case VideoType.Ogv:
                    success = FFmpeg.ToOgv(this, output, size, audioQuality, multithread);
                    break;
                case VideoType.WebM:
                    success = FFmpeg.ToWebM(this, output, size, audioQuality);
                    break;
                case VideoType.Ts:
                    success = FFmpeg.ToTs(this, output);
                    break;
                default:
                    throw new ArgumentException("VideoType not recognized");
            }

            if(!success)
                throw new OperationCanceledException("The conversion process could not be completed.");

            if(deleteSource)
                if(Exists)
                    _file.Delete();

            FFmpeg.OnProgress -= OnConversionProgress;

            return FromFile(output);
        }

        /// <summary>
        ///     Extract video from file
        /// </summary>
        /// <param name="output">Output audio stream</param>
        /// <returns>Conversion result</returns>
        public bool ExtractVideo(FileInfo output)
        {
            return FFmpeg.ExtractVideo(this, output);
        }

        /// <summary>
        ///     Extract audio from file
        /// </summary>
        /// <param name="output">Output video stream</param>
        /// <returns>Conversion result</returns>
        public bool ExtractAudio(FileInfo output)
        {
            return FFmpeg.ExtractAudio(this, output);
        }

        /// <summary>
        ///     Add audio to file
        /// </summary>
        /// <param name="audio">Audio file</param>
        /// <param name="output">Output file</param>
        /// <returns>Conversion result</returns>
        public bool AddAudio(FileInfo audio, FileInfo output)
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
            var output = new FileInfo($"{Environment.TickCount}.png");

            bool success = FFmpeg.Snapshot(this, output, size, captureTime);

            if(!success)
                throw new OperationCanceledException("Could not take snapshot!");

            output.Refresh();

            Bitmap result;

            using(Image bmp = Image.FromFile(output.FullName))
            {
                using(var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);

                    result = new Bitmap(ms);
                }
            }

            if(output.Exists)
                output.Delete();

            return result;
        }

        /// <summary>
        ///     Saves snapshot of video
        /// </summary>
        /// <param name="output">Output file</param>
        /// <param name="size">Dimension of snapshot</param>
        /// <param name="captureTime"></param>
        /// <returns>Snapshot</returns>
        public Bitmap Snapshot(FileInfo output, Size? size = null, TimeSpan? captureTime = null)
        {
            bool success = FFmpeg.Snapshot(this, output, size, captureTime);

            if(!success)
                throw new OperationCanceledException("Could not take snapshot!");

            Bitmap result;


            using(Image bmp = Image.FromFile(Path.ChangeExtension(output.FullName, ".png")))
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
        public bool JoinWith(FileInfo output, params VideoInfo[] videos)
        {
            List<VideoInfo> queuedVideos = videos.ToList();

            queuedVideos.Insert(0, this);

            return FFmpeg.Join(output, queuedVideos.ToArray());
        }

        /// <summary>
        ///     Delete file
        /// </summary>
        private void Delete()
        {
            _file.Delete();
        }

        /// <summary>
        ///     Stop conversion
        /// </summary>
        public void CancelOperation()
        {
            FFmpeg.Stop();
        }
    }
}
