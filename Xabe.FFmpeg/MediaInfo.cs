using System;
using System.IO;
using JetBrains.Annotations;
using Xabe.FFmpeg.Model;

namespace Xabe.FFmpeg
{
    /// <inheritdoc cref="IMediaInfo" />
    public class MediaInfo: IMediaInfo
    {
        private readonly object _propertiesLock = new object();
        private VideoProperties _videoProperties;

        /// <inheritdoc />
        public MediaInfo(FileInfo sourceFileInfo): this(sourceFileInfo.FullName)
        {
        }

        /// <summary>
        ///     Get MediaInfo from file
        /// </summary>
        /// <param name="fullName">FullName to file</param>
        public MediaInfo(string fullName)
        {
            if(!File.Exists(fullName))
                throw new ArgumentException($"Input file {fullName} doesn't exists.");
            FileInfo = new FileInfo(fullName);
        }

        /// <inheritdoc />
        public VideoProperties VideoProperties
        {
            get
            {
                lock(_propertiesLock)
                {
                    if(_videoProperties == null)
                        _videoProperties = new FFprobe().GetProperties(FileInfo.FullName);
                    return _videoProperties;
                }
            }
        }

        /// <inheritdoc />
        public FileInfo FileInfo { get; }

        /// <inheritdoc cref="IMediaInfo.ToString" />
        [UsedImplicitly]
        public override string ToString()
        {
            return $"Video fullName : {FileInfo.FullName}{Environment.NewLine}" +
                   $"Video root : {Path.GetDirectoryName(FileInfo.FullName)}{Environment.NewLine}" +
                   $"Video name: {FileInfo.Name}{Environment.NewLine}" +
                   $"Video extension : {FileInfo.Extension}{Environment.NewLine}" +
                   $"Video duration : {VideoProperties.VideoDuration}{Environment.NewLine}" +
                   $"Video format : {VideoProperties.VideoFormat}{Environment.NewLine}" +
                   $"Audio format : {VideoProperties.AudioFormat}{Environment.NewLine}" +
                   $"Audio duration : {VideoProperties.AudioDuration}{Environment.NewLine}" +
                   $"Aspect Ratio : {VideoProperties.Ratio}{Environment.NewLine}" +
                   $"Framerate : {VideoProperties.Ratio} fps{Environment.NewLine}" +
                   $"Resolution : {VideoProperties.Width} x {VideoProperties.Height}{Environment.NewLine}" +
                   $"Size : {VideoProperties.Size} b";
        }
    }
}
