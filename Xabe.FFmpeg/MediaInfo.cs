using System;
using System.IO;
using Xabe.FFmpeg.Model;

namespace Xabe.FFmpeg
{
    /// <inheritdoc cref="IMediaInfo" />
    public class MediaInfo: IMediaInfo
    {
        private readonly object _propertiesLock = new object();
        private MediaProperties _properties;

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
        public MediaProperties Properties
        {
            get
            {
                lock(_propertiesLock)
                {
                    if(_properties == null)
                        _properties = new FFprobe().GetProperties(FileInfo.FullName);
                    return _properties;
                }
            }
        }

        /// <inheritdoc />
        public FileInfo FileInfo { get; }

        /// <inheritdoc cref="IMediaInfo.ToString" />
        public override string ToString()
        {
            return $"Video fullName : {FileInfo.FullName}{Environment.NewLine}" +
                   $"Video root : {Path.GetDirectoryName(FileInfo.FullName)}{Environment.NewLine}" +
                   $"Video name: {FileInfo.Name}{Environment.NewLine}" +
                   $"Video extension : {FileInfo.Extension}{Environment.NewLine}" +
                   $"Video duration : {Properties.VideoDuration}{Environment.NewLine}" +
                   $"Video format : {Properties.VideoFormat}{Environment.NewLine}" +
                   $"Audio format : {Properties.AudioFormat}{Environment.NewLine}" +
                   $"Audio duration : {Properties.AudioDuration}{Environment.NewLine}" +
                   $"Aspect Ratio : {Properties.Ratio}{Environment.NewLine}" +
                   $"Framerate : {Properties.Ratio} fps{Environment.NewLine}" +
                   $"Resolution : {Properties.Width} x {Properties.Height}{Environment.NewLine}" +
                   $"Size : {Properties.Size} b";
        }
    }
}
