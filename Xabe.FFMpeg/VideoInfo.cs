using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Xabe.FFMpeg.Enums;

namespace Xabe.FFMpeg
{
    /// <inheritdoc cref="IVideoInfo" />
    public class VideoInfo: IVideoInfo
    {
        /// <inheritdoc />
        public VideoInfo(FileInfo sourceFileInfo): this(sourceFileInfo.FullName)
        {
        }

        /// <summary>
        ///     Get VideoInfo from file
        /// </summary>
        /// <param name="filePath">FilePath to file</param>
        [UsedImplicitly]
        public VideoInfo(string filePath)
        {
            if(!File.Exists(filePath))
                throw new ArgumentException($"Input file {filePath} doesn't exists.");
            FilePath = filePath;
            new FFProbe().ProbeDetails(this);
        }

        /// <inheritdoc />
        public string FilePath { get; }

        /// <inheritdoc />
        public TimeSpan VideoDuration { get; internal set; }

        /// <inheritdoc />
        public string Extension => Path.GetExtension(FilePath);

        /// <inheritdoc />
        public TimeSpan Duration { get; internal set; }

        /// <inheritdoc />
        public string AudioFormat { get; internal set; } = "none";

        /// <inheritdoc />
        public TimeSpan AudioDuration { get; internal set; }

        /// <inheritdoc />
        public string VideoFormat { get; internal set; } = "none";

        /// <inheritdoc />
        public string Ratio { get; internal set; }

        /// <inheritdoc />
        public double FrameRate { get; internal set; }

        /// <inheritdoc />
        public int Height { get; internal set; }

        /// <inheritdoc />
        public int Width { get; internal set; }

        /// <inheritdoc />
        public long Size { get; internal set; }

        /// <inheritdoc cref="IVideoInfo.ToString" />
        [UsedImplicitly]
        public override string ToString()
        {
            return $"Video filePath : {FilePath}{Environment.NewLine}" +
                   $"Video root : {Path.GetDirectoryName(FilePath)}{Environment.NewLine}" +
                   $"Video name: {Path.GetFileName(FilePath)}{Environment.NewLine}" +
                   $"Video extension : {Extension}{Environment.NewLine}" +
                   $"Video duration : {VideoDuration}{Environment.NewLine}" +
                   $"Video format : {VideoFormat}{Environment.NewLine}" +
                   $"Audio format : {AudioFormat}{Environment.NewLine}" +
                   $"Audio duration : {AudioDuration}{Environment.NewLine}" +
                   $"Aspect Ratio : {Ratio}{Environment.NewLine}" +
                   $"Framerate : {Ratio} fps{Environment.NewLine}" +
                   $"Resolution : {Width} x {Height}{Environment.NewLine}" +
                   $"Size : {Size} b";
        }
    }
}
