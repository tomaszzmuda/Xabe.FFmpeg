using System;

namespace Xabe.FFmpeg.Model
{
    /// <inheritdoc />
    internal class ConversionResult : IConversionResult
    {
        /// <inheritdoc />
        public bool Success { get; internal set; }

        /// <inheritdoc />
        public Lazy<IMediaInfo> MediaInfo { get; internal set; }

        /// <inheritdoc />
        public DateTime StartTime { get; internal set; }

        /// <inheritdoc />
        public DateTime EndTime { get; internal set; }

        /// <inheritdoc />
        public TimeSpan Duration => EndTime - StartTime;

        /// <inheritdoc />
        public string ConversionParameters { get; internal set; }
    }
}
