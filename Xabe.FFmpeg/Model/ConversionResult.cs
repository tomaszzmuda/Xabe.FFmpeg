using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Information about conversion
    /// </summary>
    public class ConversionResult
    {
        internal ConversionResult()
        {
        }

        /// <summary>
        ///     Result of conversion
        /// </summary>
        public bool Result { get; internal set; }

        /// <summary>
        ///     Output IMediaInfo
        /// </summary>
        public Lazy<IMediaInfo> MediaInfo { get; internal set; }

        /// <summary>
        ///     Date and time of starting conversion
        /// </summary>
        public DateTime StartTime { get; internal set; }

        /// <summary>
        ///     Date and time of starting conversion
        /// </summary>
        public DateTime EndTime { get; internal set; }

        /// <summary>
        ///     Conversion duration
        /// </summary>
        public TimeSpan Duration => EndTime - StartTime;
    }
}
