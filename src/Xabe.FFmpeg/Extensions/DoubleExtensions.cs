using System.Globalization;

namespace System
{
    /// <summary>
    ///     Number formatting helpers for the FFmpeg command line
    /// </summary>
    public static class DoubleExtensions
    {
        /// <summary>
        ///     Formats the number with en-US separators so FFmpeg parses it as a decimal
        /// </summary>
        /// <param name="number">Number to format</param>
        /// <param name="decimalPlaces">Decimal places</param>
        public static string ToFFmpegFormat(this double number, int decimalPlaces = 1)
        {
            return string.Format(CultureInfo.GetCultureInfo("en-US"), $"{{0:N{decimalPlaces}}}", number);
        }
    }
}
