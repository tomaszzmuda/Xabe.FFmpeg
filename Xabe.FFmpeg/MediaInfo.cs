using System;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFmpeg.Model;

namespace Xabe.FFmpeg
{
    /// <inheritdoc cref="IMediaInfo" />
    public class MediaInfo: IMediaInfo
    {
        private MediaInfo(FileInfo fileInfo, MediaProperties properties)
        {
            FileInfo = fileInfo;
            Properties = properties;
        }

        /// <summary>
        ///     Get MediaInfo from file
        /// </summary>
        /// <param name="filePath">FullPath to file</param>
        public static async Task<IMediaInfo> Get(string filePath)
        {
            return await Get(new FileInfo(filePath));
        }

        /// <summary>
        ///     Get MediaInfo from file
        /// </summary>
        /// <param name="fileInfo">FileInfo</param>
        public static async Task<IMediaInfo> Get(FileInfo fileInfo)
        {
            if (!File.Exists(fileInfo.FullName))
                throw new ArgumentException($"Input file {fileInfo.FullName} doesn't exists.");
            MediaProperties properties = await new FFprobe().GetProperties(fileInfo.FullName);
            if (properties == null)
                throw new ArgumentException($"Input file {fileInfo.FullName} doesn't recognized.");

            var mediaInfo = new MediaInfo(fileInfo, properties);
            return mediaInfo;

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
