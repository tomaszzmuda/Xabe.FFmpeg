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

        /// <inheritdoc />
        public string MainInputSource { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> ExtraInputs { get; set; }

        /// <inheritdoc />
        public string OutputLabel { get; set; }
    }
}
