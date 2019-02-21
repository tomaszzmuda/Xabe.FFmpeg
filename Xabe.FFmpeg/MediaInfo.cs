using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Exceptions;
using Xabe.FFmpeg.Streams;

namespace Xabe.FFmpeg
{
    /// <inheritdoc cref="IMediaInfo" />
    public class MediaInfo : IMediaInfo
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
            {
                throw new InvalidInputException($"Input file {fileInfo.FullName} doesn't exists.");
            }

            var mediaInfo = new MediaInfo(fileInfo);
            mediaInfo = await new FFprobeWrapper().GetProperties(fileInfo, mediaInfo);

            return mediaInfo;
        }
    }
}
