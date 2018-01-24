using System.Threading.Tasks;
using Xabe.FFmpeg.Model;

namespace Xabe.FFmpeg
{
    public static class ConversionExtensions
    {
        public static async Task<ConversionResult> Execute(this Task<IConversion> conversionTask)
        {
            IConversion conversion = await conversionTask;
            return await conversion.Start();
        }
    }
}
