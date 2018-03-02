using System;
using System.Collections.Generic;
using System.Linq;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        ///     Return ffmpeg formatted time
        /// </summary>
        /// <param name="ts">TimeSpan</param>
        /// <returns>FFmpeg formated time</returns>
        public static string ToFFmpeg(this TimeSpan ts)
        {
            int milliseconds = ts.Milliseconds;
            int seconds = ts.Seconds;
            int minutes = ts.Minutes;
            var hours = (int)ts.TotalHours;

            return $"{hours:D}:{minutes:D2}:{seconds:D2}.{milliseconds:D3}";
        }

        /// <summary>
        ///     Parse FFmpeg formated time
        /// </summary>
        /// <param name="text">FFmpeg time</param>
        /// <returns>TimeSpan</returns>
        public static TimeSpan ParseFFmpegTime(this string text)
        {
            List<string> parts = text.Split(':')
                                     .Reverse()
                                     .ToList();

            var milliseconds = 0;
            var seconds = 0;

            if(parts[0].Contains('.'))
            {
                string[] secondsSplit = parts[0].Split('.');
                seconds = int.Parse(secondsSplit[0]);
                milliseconds = int.Parse(secondsSplit[1]);
            }
            else
            {
                seconds = int.Parse(parts[0]);
            }

            int minutes = int.Parse(parts[1]);
            int hours = int.Parse(parts[2]);

            return new TimeSpan(0, hours, minutes, seconds, milliseconds);
        }
    }
}
