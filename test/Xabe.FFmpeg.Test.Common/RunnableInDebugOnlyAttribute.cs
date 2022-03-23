using Xunit;

namespace Xabe.FFmpeg.Test.Common
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
