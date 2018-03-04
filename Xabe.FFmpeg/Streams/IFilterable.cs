using System.Collections.Generic;

namespace Xabe.FFmpeg.Streams
{
    internal interface IFilterable
    {
        IEnumerable<IFilterConfiguration> GetFilters();
    }
}
