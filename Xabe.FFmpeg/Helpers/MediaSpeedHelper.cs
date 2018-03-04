using System;
using System.Globalization;

namespace Xabe.FFmpeg
{
    internal static class MediaSpeedHelper
    {
        internal static string GetAudioSpeed(double multiplication)
        {
            CheckMultiplicationRange(multiplication);
            string audioSpeed = $"atempo={string.Format(CultureInfo.GetCultureInfo("en-US"), "{0:N1}", multiplication)} ";
            return audioSpeed;
        }

        internal static string GetVideoSpeedFilter(double multiplication)
        {
            CheckMultiplicationRange(multiplication);
            double videoMultiplicator = 1;
            if(multiplication >= 1)
            {
                videoMultiplicator = 1 - (multiplication - 1) / 2;
            }
            else
            {
                videoMultiplicator = 1 + (multiplication - 1) * -2;
            }
            return $"{string.Format(CultureInfo.GetCultureInfo("en-US"), "{0:N1}", videoMultiplicator)}*PTS ";
        }

        private static void CheckMultiplicationRange(double multiplication)
        {
            if(multiplication < 0.5 ||
               multiplication > 2.0)
            {
                throw new ArgumentOutOfRangeException(nameof(multiplication), "Value has to be greater than 0.5 and less than 2.0.");
            }
        }
    }
}
