using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg.Exceptions;

namespace Xabe.FFmpeg
{
    /// <inheritdoc cref="IMediaInfo" />
    public class MediaInfo : IMediaInfo
    {
        private MediaInfo(string path)
        {
            Path = path;
        }

        /// <inheritdoc />
        public IEnumerable<IStream> Streams => VideoStreams.Concat<IStream>(AudioStreams)
                                                           .Concat(SubtitleStreams);

        /// <inheritdoc />
        public TimeSpan Duration { get; internal set; }

        /// <inheritdoc />
        public long Size { get; internal set; }

        /// <inheritdoc />
        public DateTime? CreationTime { get; internal set; }

        /// <inheritdoc />
        public IEnumerable<IVideoStream> VideoStreams { get; internal set; }

        /// <inheritdoc />
        public IEnumerable<IAudioStream> AudioStreams { get; internal set; }

        /// <inheritdoc />
        public IEnumerable<ISubtitleStream> SubtitleStreams { get; internal set; }

        /// <inheritdoc />
        public string Path { get; }

        /// <summary>
        ///     Get MediaInfo from file
        /// </summary>
        /// <param name="filePath">FullPath to file</param>
        internal static async Task<IMediaInfo> Get(string filePath)
        {
            using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
            {
                var cancellationToken = source.Token;
                return await Get(filePath, cancellationToken);
            }
        }

        /// <summary>
        ///     Get MediaInfo from file
        /// </summary>
        /// <param name="filePath">FullPath to file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        internal static async Task<IMediaInfo> Get(string filePath, CancellationToken cancellationToken)
        {
            var mediaInfo = new MediaInfo(filePath);
            var wrapper = new FFprobeWrapper();
            mediaInfo = await wrapper.SetProperties(mediaInfo, cancellationToken);
            return mediaInfo;
        }

        /// <summary>
        ///     Get MediaInfo from file
        /// </summary>
        /// <param name="fileInfo">FileInfo</param>
        internal static async Task<IMediaInfo> Get(FileInfo fileInfo)
        {
            if (!File.Exists(fileInfo.FullName))
            {
                throw new InvalidInputException($"Input file {fileInfo.FullName} doesn't exists.");
            }

            return await Get(fileInfo.FullName);
        }
    }
}
