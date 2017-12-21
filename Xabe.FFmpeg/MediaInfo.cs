using System;
using System.IO;
using System.Text;
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
            const string margin = "    ";
            var builder = new StringBuilder();
            builder.AppendLine($"Video fullName : {FileInfo.FullName}");
            builder.AppendLine($"Video root : {Path.GetDirectoryName(FileInfo.FullName)}");
            builder.AppendLine($"Video name: {FileInfo.Name}{Environment.NewLine}");
            builder.AppendLine($"Video extension : {FileInfo.Extension}{Environment.NewLine}");
            builder.AppendLine($"Size : {Properties.Size} b");

            builder.AppendLine();
            builder.AppendLine("Video streams:");
            foreach(VideoStream videoStream in Properties.VideoStreams)
            {
                builder.AppendLine($"{margin}Duration : {Properties.Duration}");
                builder.AppendLine($"{margin}Format : {videoStream.Format}");
                builder.AppendLine($"{margin}Aspect Ratio : {videoStream.Ratio}");
                builder.AppendLine($"{margin}Framerate : {videoStream.FrameRate} fps");
                builder.AppendLine($"{margin}Resolution : {videoStream.Width} x {videoStream.Height}");
            }

            builder.AppendLine();
            builder.AppendLine("Audio streams:");
            foreach(AudioStream audioStream in Properties.AudioStreams)
            {
                builder.AppendLine($"{margin}Format : {audioStream.Format}");
                builder.AppendLine($"{margin}Duration : {audioStream.Duration}");
            }
            return builder.ToString();
        }
    }
}
