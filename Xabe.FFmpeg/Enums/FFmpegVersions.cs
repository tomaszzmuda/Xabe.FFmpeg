using System;
using System.Collections.Generic;
using System.Text;

namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    /// Enum to describe the version of FFmpeg that can be automatically downloaded
    /// </summary>
    public enum FFmpegVersions
    {
        /// <summary>
        /// The official release from ffbinaries
        /// </summary>
        Official = 1,
        
        /// <summary>
        /// The Full Release from Zenaroe
        /// </summary>
        Full = 2,

        /// <summary>
        /// The Shared Release from Zenaroe
        /// </summary>
        Shared = 3,
    }
}
