using System;
using System.Text;

namespace Xabe.FFmpeg.Streams
{
    /// <summary>
    ///     Stream with web source
    /// </summary>
    public class WebStream : IStream
    {
        private readonly Uri _uri;
        private readonly TimeSpan? _duration;

        /// <summary>
        /// Create web stream
        /// </summary>
        /// <param name="uri">File uri</param>
        /// <param name="format">Stream format</param>
        /// <param name="duration">Duration of video</param>
        public WebStream(Uri uri, string format, TimeSpan? duration)
        {
            Format = format;
            _uri = uri;
            _duration = duration;
        }

        /// <inheritdoc />
        public string Build()
        {
            var builder = new StringBuilder();
            builder.Append(_duration.HasValue ? $"-t {_duration.Value.ToFFmpeg()} " : null);
            builder.Append("-timeout 5 ");
            builder.Append("-stimeout 500000 ");
            builder.Append("-max_delay 500000 ");
            return builder.ToString();
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
