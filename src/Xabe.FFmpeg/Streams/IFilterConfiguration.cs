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

        /// <summary>
        ///     Source file feeding the main input pad. Resolved to an input index at build time
        /// </summary>
        string MainInputSource { get; }

        /// <summary>
        ///     Additional input sources (for example a watermark image) consumed by a multi-pad filter
        /// </summary>
        IEnumerable<string> ExtraInputs { get; }

        /// <summary>
        ///     Label assigned to the filter graph output. When set the output is mapped by this label
        /// </summary>
        string OutputLabel { get; }
    }
}