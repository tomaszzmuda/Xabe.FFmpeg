using System;
using System.ComponentModel;
using System.Reflection;

namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    ///     Video files extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        ///     .mp4
        /// </summary>
        public const string Mp4 = ".mp4";

        /// <summary>
        ///     .mp3
        /// </summary>
        public const string Mp3 = ".mp3";

        /// <summary>
        ///     .ts
        /// </summary>
        public const string Ts = ".ts";

        /// <summary>
        ///     .webm
        /// </summary>
        public const string WebM = ".webm";

        /// <summary>
        ///     .ogv
        /// </summary>
        public const string Ogv = ".ogv";

        /// <summary>
        ///     .png
        /// </summary>
        public const string Png = ".png";

        /// <summary>
        ///     .mkv
        /// </summary>
        public const string Mkv = ".mkv";

        /// <summary>
        ///     .gif
        /// </summary>
        public const string Gif = ".gif";

        internal static string GetDescription(Enum value)
        {
            FieldInfo fi = value.GetType()
                                .GetField(value.ToString());

            var attributes =
                (DescriptionAttribute[]) fi.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);

            if(attributes != null &&
               attributes.Length > 0)
                return attributes[0].Description;

            return value.ToString();
        }
    }
}
