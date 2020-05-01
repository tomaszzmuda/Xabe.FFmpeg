using System;
using System.Threading.Tasks;

namespace Xabe.FFmpeg
{
    /// <inheritdoc />
    internal class ConversionResult : IConversionResult
    {
        /// <inheritdoc />
        public DateTime StartTime { get; internal set; }

        /// <inheritdoc />
        public DateTime EndTime { get; internal set; }

        /// <inheritdoc />
        public TimeSpan Duration => EndTime - StartTime;

        /// <inheritdoc />
        public string Arguments { get; internal set; }
    }
}
