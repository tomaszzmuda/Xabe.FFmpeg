using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg.Streams;

namespace Xabe.FFmpeg
{
    /// <inheritdoc cref="IMediaInfo" />
    public class MediaInfo: IMediaInfo
    {
        private MediaInfo(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }

        /// <inheritdoc />
        public IEnumerable<IStream> Streams => VideoStreams.Concat<IStream>(AudioStreams)
                                                           .Concat(SubtitleStreams);

        /// <inheritdoc />
        public TimeSpan Duration { get; internal set; }

        /// <inheritdoc />
        public long Size { get; internal set; }

        /// <inheritdoc />
        public IEnumerable<IVideoStream> VideoStreams { get; internal set; }

        /// <inheritdoc />
        public IEnumerable<IAudioStream> AudioStreams { get; internal set; }

        /// <inheritdoc />
        public IEnumerable<ISubtitleStream> SubtitleStreams { get; internal set; }

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
            foreach(IVideoStream videoStream in VideoStreams)
            {
                builder.AppendLine($"{margin}Duration : {Duration}");
                builder.AppendLine($"{margin}Format : {videoStream.Format}");
                builder.AppendLine($"{margin}Aspect Ratio : {videoStream.Ratio}");
                builder.AppendLine($"{margin}Framerate : {videoStream.FrameRate} fps");
                builder.AppendLine($"{margin}Resolution : {videoStream.Width} x {videoStream.Height}");
            }

            builder.AppendLine();
            builder.AppendLine("Audio streams:");
            foreach(IAudioStream audioStream in AudioStreams)
            {
                builder.AppendLine($"{margin}Format : {audioStream.Format}");
                builder.AppendLine($"{margin}Duration : {audioStream.Duration}");
            }
            return builder.ToString();
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
            if(!File.Exists(fileInfo.FullName))
            {
                throw new ArgumentException($"Input file {fileInfo.FullName} doesn't exists.");
            }

            var mediaInfo = new MediaInfo(fileInfo);
            mediaInfo = await new FFprobe().GetProperties(fileInfo, mediaInfo);

            return mediaInfo;
        }
    }
}
