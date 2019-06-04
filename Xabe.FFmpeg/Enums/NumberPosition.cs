using System;
using System.Collections.Generic;
using System.Text;

namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    /// Enum to define the position within the output file where file numbers will be appended
    /// </summary>
    public enum NumberPosition : int
    {
        /// <summary>
        /// Place the number at the beginning of the file name
        /// </summary>
        BEGIN = 0, 

        /// <summary>
        /// Place the number at the end of the file name
        /// </summary>
        END = 1
    }
}
