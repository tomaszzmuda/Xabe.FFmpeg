using System;
using System.Collections.Generic;
using System.Text;

namespace Xabe.FFmpeg
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
            return _duration.HasValue ? $"-t {_duration.Value.ToFFmpeg()} " : null;
        }

        /// <inheritdoc />
        public string BuildInputArguments()
        {
            return null;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetSource()
        {
            return new[] { _uri.AbsoluteUri };
        }

        /// <inheritdoc />
        public int Index => 0;

        /// <inheritdoc />
        public string Format { get; }
    }
}
