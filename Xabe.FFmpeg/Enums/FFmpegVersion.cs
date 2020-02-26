using System;
using System.Collections.Generic;
using System.Text;

namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    /// Enum to describe the versions of FFmpeg that can be automatically downloaded
    /// </summary>
    public enum FFmpegVersion
    {
        /// <summary>
        /// The official release from ffbinaries
        /// </summary>
        Official = 1,

        /// <summary>
        /// The Full Release from Zenaroe (Only Windows and macOS)
        /// </summary>
        Full = 2,

        /// <summary>
        /// The Shared Release from Zenaroe (Only Windows and macOS)
        /// </summary>
        Shared = 3,
    }
}
