using System.Collections.Generic;

namespace Xabe.FFmpeg
{
    /// <inheritdoc />
    internal class FilterConfiguration : IFilterConfiguration
    {
        /// <inheritdoc />
        public string FilterType { get; set; }

        /// <inheritdoc />
        public int StreamNumber { get; set; }

        /// <inheritdoc />
        public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();

        /// <summary>
        ///     Source file feeding the main input pad. Resolved to an input index at build time
        /// </summary>
        public string MainInputSource { get; set; }

        /// <summary>
        ///     Additional input sources (for example a watermark image) consumed by a multi-pad filter
        /// </summary>
        public IEnumerable<string> ExtraInputs { get; set; }

        /// <summary>
        ///     Label assigned to the filter graph output. When set the output is mapped by this label
        /// </summary>
        public string OutputLabel { get; set; }
    }
}
