using System.Collections.Generic;

namespace Xabe.FFmpeg
{
    internal interface IFilterable
    {
        IEnumerable<IFilterConfiguration> GetFilters();
    }
}
