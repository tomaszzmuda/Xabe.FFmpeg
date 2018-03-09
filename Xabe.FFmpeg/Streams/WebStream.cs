using System;

namespace Xabe.FFmpeg.Streams
{
    /// <summary>
    ///     Stream with foreign web source
    /// </summary>
    public class WebStream : IStream
    {
        private readonly Uri _uri;
        private readonly int? _duration;

        /// <summary>
        /// Create web stream
        /// </summary>
        /// <param name="uri">File uri</param>
        /// <param name="format">Stream format</param>
        /// <param name="duration">Duration of video</param>
        public WebStream(Uri uri, string format, int? duration)
        {
            Format = format;
            _uri = uri;
            _duration = duration;
        }

        /// <inheritdoc />
        public string Build()
        {
            return _duration.HasValue ? $"-t {_duration} " : null;
        }

        /// <inheritdoc />
        public string GetSource()
        {
            return _uri.AbsoluteUri;
        }

        /// <inheritdoc />
        public int Index => 0;

        /// <inheritdoc />
        public string Format { get; }
    }
}
