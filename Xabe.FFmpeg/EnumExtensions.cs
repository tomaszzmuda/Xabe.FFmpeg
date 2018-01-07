using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Xabe.FFmpeg
{
    internal static class EnumExtensions
    {
        private static readonly Dictionary<Type, Dictionary<Enum, string>> _enums = new Dictionary<Type, Dictionary<Enum, string>>();

        internal static string GetDescription(this Enum value)
        {
            if(!_enums.TryGetValue(value.GetType(), out Dictionary<Enum, string> dic))
            {
                dic = new Dictionary<Enum, string>();
                _enums.Add(value.GetType(), dic);
            }
            else if(dic.TryGetValue(value, out string description))
            {
                return description;
            }

            FieldInfo fi = value.GetType()
                                .GetField(value.ToString());

            var attributes =
                (DescriptionAttribute[]) fi.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);

            if(attributes != null &&
               attributes.Length > 0)
            {
                dic[value] = attributes[0].Description;
                return attributes[0].Description;
            }

            return value.ToString();
        }
    }
}
