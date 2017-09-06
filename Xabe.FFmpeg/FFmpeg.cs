using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xabe.FFmpeg.Exceptions;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Info about conversion progress
    /// </summary>
    /// <param name="duration">Current processing time</param>
    /// <param name="totalLength">Movie length</param>
    public delegate void ConversionHandler(TimeSpan duration, TimeSpan totalLength);

    // ReSharper disable once InconsistentNaming
    /// <inheritdoc />
    /// <summary>
    ///     Wrapper for FFmpeg
    /// </summary>
    internal class FFmpeg: FFbase
    {
        private const string TimeFormatRegex = @"\w\w:\w\w:\w\w";
        private List<string> _outputLog;
        private TimeSpan _totalTime;

        /// <summary>
        ///     Fires when FFmpeg progress changes
        /// </summary>
        public event ConversionHandler OnProgress;

        /// <summary>
        ///     Fires when FFmpeg process print sonething
        /// </summary>
        public event DataReceivedEventHandler OnDataReceived;

        internal async Task<bool> RunProcess(string args)
        {
            _outputLog = new List<string>();
            WasKilled = false;

            RunProcess(args, FFmpegPath, true, true, true);

            using(Process)
            {
                Process.ErrorDataReceived += OutputData;
                Process.BeginOutputReadLine();
                Process.BeginErrorReadLine();
                await Task.Run(() => Process.WaitForExit());

                if(WasKilled)
                    return false;

                if(Process.ExitCode != 0)
                    throw new ConversionException(string.Join(Environment.NewLine, _outputLog.ToArray()), args);
            }

            return true;
        }

        private void OutputData(object sender, DataReceivedEventArgs e)
        {
            if(e.Data == null)
                return;

            OnDataReceived?.Invoke(this, e);

            _outputLog.Add(e.Data);

            if(OnProgress == null ||
               !IsRunning)
                return;

            var regex = new Regex(TimeFormatRegex);
            if(e.Data.Contains("Duration"))
            {
                Match match = regex.Match(e.Data);
                _totalTime = TimeSpan.Parse(match.Value);
            }
            else if(e.Data.Contains("frame"))
            {
                Match match = regex.Match(e.Data);
                if(match.Success)
                    OnProgress(TimeSpan.Parse(match.Value), _totalTime);
            }
        }
    }
}
