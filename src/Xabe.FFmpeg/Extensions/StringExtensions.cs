using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System
{
    public static class StringExtensions
    {
        public static string Escape(this string output)
        {
            if ((output.Last() == '\"' && output.First() == '\"') || (output.Last() == '\'' && output.First() == '\''))
            {
                output = output.Substring(1, output.Length - 2);
            }

            output = $"\"{output}\"";
            return output;
        }
    }
}
