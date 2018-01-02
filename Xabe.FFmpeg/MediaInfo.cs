using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Xabe.FFmpeg
{
    /// <inheritdoc cref="IMediaInfo" />
    public class MediaInfo: IMediaInfo
    {
        /// <summary>
        ///     Duration of media
        /// </summary>
        public TimeSpan Duration { get; internal set; }

        /// <summary>
        ///     Size of file
        /// </summary>
        public long Size { get; internal set; }

        public IEnumerable<IStream> Streams { get; internal set; }

        /// <summary>
        ///     Video streams
        /// </summary>
        public IEnumerable<IVideoStream> VideoStreams => Streams.Where(x => x.CodecType == CodecType.Video).Cast<IVideoStream>();

        /// <summary>
        ///     Audio streams
        /// </summary>
        public IEnumerable<IAudioStream> AudioStreams => Streams.Where(x => x.CodecType == CodecType.Audio).Cast<IAudioStream>();

        /// <summary>
        ///     Audio streams
        /// </summary>
        public IEnumerable<ISubtitle> Subtitles { get; internal set; }

        /// <summary>
        ///     Get MediaInfo from file
        /// </summary>
        /// <param name="filePath">FullPath to file</param>
        public static async Task<IMediaInfo> Get(string filePath)
        {
            return await Get(new FileInfo(filePath));
        }

        private MediaInfo(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }

        /// <summary>
        ///     Get MediaInfo from file
        /// </summary>
        /// <param name="fileInfo">FileInfo</param>
        public static async Task<IMediaInfo> Get(FileInfo fileInfo)
        {
            if (!File.Exists(fileInfo.FullName))
                throw new ArgumentException($"Input file {fileInfo.FullName} doesn't exists.");

            var mediaInfo = new MediaInfo(fileInfo);
            mediaInfo = await new FFprobe().GetProperties(fileInfo, mediaInfo);

            return mediaInfo;
        }

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
            builder.AppendLine($"Size : {Size} b");

            builder.AppendLine();
            builder.AppendLine("Video streams:");
            foreach(VideoStream videoStream in VideoStreams)
            {
                builder.AppendLine($"{margin}Duration : {Duration}");
                builder.AppendLine($"{margin}Format : {videoStream.Format}");
                builder.AppendLine($"{margin}Aspect Ratio : {videoStream.Ratio}");
                builder.AppendLine($"{margin}Framerate : {videoStream.FrameRate} fps");
                builder.AppendLine($"{margin}Resolution : {videoStream.Width} x {videoStream.Height}");
            }

            builder.AppendLine();
            builder.AppendLine("Audio streams:");
            foreach(AudioStream audioStream in AudioStreams)
            {
                builder.AppendLine($"{margin}Format : {audioStream.Format}");
                builder.AppendLine($"{margin}Duration : {audioStream.Duration}");
            }
            return builder.ToString();
        }
    }
}
