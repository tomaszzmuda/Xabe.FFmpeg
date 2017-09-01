using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Xabe.FFMpeg
{
    /// <summary>
    ///     Info about conversion progress
    /// </summary>
    /// <param name="duration">Current processing time</param>
    /// <param name="totalLength">Movie length</param>
    public delegate void ConversionHandler(TimeSpan duration, TimeSpan totalLength);

    // ReSharper disable once InconsistentNaming
    /// <summary>
    ///     Wrapper for FFMpeg
    /// </summary>
    internal class FFMpeg: FFBase
    {
        private List<string> _outputLog;
        private TimeSpan _totalTime;
        private const string TimeFormatRegex = @"\w\w:\w\w:\w\w";

        /// <summary>
        ///     Fires when ffmpeg progress changes
        /// </summary>
        public event ConversionHandler OnProgress;

        internal async Task<bool> RunProcess(string args)
        {
            _outputLog = new List<string>();
            WasKilled = false;

            RunProcess(args, FFMpegPath, true, false, true);

            using(Process)
            {
                Process.ErrorDataReceived += OutputData;
                Process.BeginErrorReadLine();
                await Task.Run(() => Process.WaitForExit());

                if(WasKilled)
                    return false;

                if(Process.ExitCode != 0)
                    throw new ArgumentException(string.Join(Environment.NewLine, _outputLog.ToArray()));
            }

            return true;
        }

        private void OutputData(object sender, DataReceivedEventArgs e)
        {
            if(e.Data == null)
                return;

            _outputLog.Add(e.Data);

            if(OnProgress == null ||
               !IsRunning)
                return;

            var regex = new Regex(TimeFormatRegex);
            if (e.Data.Contains("Duration"))
            {
                Match match = regex.Match(e.Data);
                _totalTime = TimeSpan.Parse(match.Value);
            }
            else if(e.Data.Contains("frame"))
            {
                Match match = regex.Match(e.Data);
                if(match.Success)
                {
                    OnProgress(TimeSpan.Parse(match.Value), _totalTime);
                }

            }
        }
    }
}
