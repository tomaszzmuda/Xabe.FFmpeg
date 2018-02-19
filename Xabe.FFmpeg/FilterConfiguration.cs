using System.Collections.Generic;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Stream filter configuration
    /// </summary>
    public class FilterConfiguration
    {
        /// <summary>
        ///     Type of filter
        /// </summary>
        public string FilterType { get; set; }

        /// <summary>
        ///     Stream filter number
        /// </summary>
        public int StreamNumber { get; set; }

        /// <summary>
        ///     Filter with name and values
        /// </summary>
        public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
    }
}
