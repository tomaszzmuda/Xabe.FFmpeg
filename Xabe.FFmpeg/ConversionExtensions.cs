using System.Threading.Tasks;

namespace Xabe.FFmpeg
{
    public static class ConversionExtensions
    {
        public static async Task<bool> Execute(this Task<IConversion> conversionTask)
        {
            IConversion conversion = await conversionTask;
            return await conversion.Start();
        }
    }
}
