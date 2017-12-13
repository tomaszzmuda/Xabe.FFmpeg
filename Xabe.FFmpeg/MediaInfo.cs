using System;
using System.IO;
using Xabe.FFmpeg.Model;

namespace Xabe.FFmpeg
{
    /// <inheritdoc cref="IMediaInfo" />
    public class MediaInfo: IMediaInfo
    {
        private MediaInfo(string fullName)
        {
            if(!File.Exists(fullName))
                throw new ArgumentException($"Input file {fullName} doesn't exists.");
            FileInfo = new FileInfo(fullName);
            Properties = new FFprobe().GetProperties(FileInfo.FullName);
            if(Properties == null)
                throw new ArgumentException($"Input file {fullName} doesn't recognized.");
        }

        /// <summary>
        ///     Get MediaInfo from file
        /// </summary>
        /// <param name="filePath">FullPath to file</param>
        public static IMediaInfo Get(string filePath)
        {
            var mediaInfo = new MediaInfo(filePath);
            return mediaInfo;
        }

        /// <summary>
        ///     Get MediaInfo from file
        /// </summary>
        /// <param name="fileInfo">FileInfo</param>
        public static IMediaInfo Get(FileInfo fileInfo)
        {
            return MediaInfo.Get(fileInfo.FullName);
        }

        /// <inheritdoc />
        public MediaProperties Properties { get; }

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
