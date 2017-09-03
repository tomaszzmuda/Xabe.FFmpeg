using System;
using System.IO;
using JetBrains.Annotations;

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
        /// <param name="fullName">FullName to file</param>
        [UsedImplicitly]
        public VideoInfo(string fullName)
        {
            if(!File.Exists(fullName))
                throw new ArgumentException($"Input file {fullName} doesn't exists.");
            FullName = fullName;
            new FFProbe().ProbeDetails(this);
        }

        /// <inheritdoc />
        public string FullName { get; }

        /// <inheritdoc />
        public TimeSpan VideoDuration { get; internal set; }

        /// <inheritdoc />
        public string Extension => Path.GetExtension(FullName);

        /// <inheritdoc />
        public TimeSpan Duration { get; internal set; }

        /// <inheritdoc />
        public string AudioFormat { get; internal set; }

        /// <inheritdoc />
        public TimeSpan AudioDuration { get; internal set; }

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
        public long Size { get; internal set; }

        /// <inheritdoc cref="IVideoInfo.ToString" />
        [UsedImplicitly]
        public override string ToString()
        {
            return $"Video fullName : {FullName}{Environment.NewLine}" +
                   $"Video root : {Path.GetDirectoryName(FullName)}{Environment.NewLine}" +
                   $"Video name: {Path.GetFileName(FullName)}{Environment.NewLine}" +
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
