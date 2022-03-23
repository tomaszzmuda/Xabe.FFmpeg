using System.Linq;

namespace System
{
    /// <summary>
    ///     Extension methods
    /// </summary>
    public static class TimeExtensions
    {
        /// <summary>
        ///     Return ffmpeg formatted time
        /// </summary>
        /// <param name="ts">TimeSpan</param>
        /// <returns>FFmpeg formated time</returns>
        public static string ToFFmpeg(this TimeSpan ts)
        {
            var milliseconds = ts.Milliseconds;
            var seconds = ts.Seconds;
            var minutes = ts.Minutes;
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
            var parts = text.Split(':')
                                     .Reverse()
                                     .ToList();

            var milliseconds = 0;
            int seconds;
            if (parts[0].Contains('.'))
            {
                var secondsSplit = parts[0].Split('.');
                seconds = int.Parse(secondsSplit[0]);
                milliseconds = int.Parse(secondsSplit[1]);
            }
            else
            {
                seconds = int.Parse(parts[0]);
            }

            var minutes = int.Parse(parts[1]);
            var hours = int.Parse(parts[2]);

            return new TimeSpan(0, hours, minutes, seconds, milliseconds);
        }
    }
}
