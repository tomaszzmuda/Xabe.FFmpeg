using System.Linq;

namespace System
{
    /// <summary>
    ///     Quoting helpers for values on the FFmpeg command line
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     Wraps the value in double quotes, stripping a matching pair of outer quotes
        /// </summary>
        public static string Escape(this string output)
        {
            if (output == null)
            {
                return output;
            }

            if ((output.Last() == '\"' && output.First() == '\"') || (output.Last() == '\'' && output.First() == '\''))
            {
                output = output.Substring(1, output.Length - 2);
            }

            output = $"\"{output}\"";
            return output;
        }

        /// <summary>
        ///     Strips a matching pair of surrounding single or double quotes
        /// </summary>
        public static string Unescape(this string output)
        {
            if (output == null || output.Length < 2)
            {
                return output;
            }

            if ((output.Last() == '\"' && output.First() == '\"') || (output.Last() == '\'' && output.First() == '\''))
            {
                return output.Substring(1, output.Length - 2);
            }

            return output;
        }
    }
}
