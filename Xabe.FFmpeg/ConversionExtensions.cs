using System.Threading.Tasks;
using Xabe.FFmpeg.Model;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Extensions for IConversion
    /// </summary>
    public static class ConversionExtensions
    {
        /// <summary>
        /// Execute conversion task
        /// </summary>
        /// <param name="conversionTask">Task to execute</param>
        /// <returns>IConversionResult</returns>
        public static async Task<IConversionResult> Execute(this Task<IConversion> conversionTask)
        {
            IConversion conversion = await conversionTask;
            return await conversion.Start();
        }
    }
}
