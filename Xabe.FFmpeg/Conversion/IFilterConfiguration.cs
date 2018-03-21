using System.Collections.Generic;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Stream filter configuration
    /// </summary>
    public interface IFilterConfiguration
    {
        /// <summary>
        ///     Type of filter
        /// </summary>
        string FilterType { get; }

        /// <summary>
        ///     Stream filter number
        /// </summary>
        int StreamNumber { get; }

        /// <summary>
        ///     Filter with name and values
        /// </summary>
        Dictionary<string, string> Filters { get; }
    }
}