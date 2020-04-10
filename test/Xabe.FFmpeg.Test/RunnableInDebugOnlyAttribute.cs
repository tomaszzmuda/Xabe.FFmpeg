using System.Diagnostics;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class RunnableInDebugOnlyAttribute : FactAttribute
    {
        public RunnableInDebugOnlyAttribute()
        {
#if !DEBUG
            Skip = "Only running in interactive mode.";
#endif
        }
    }
}
