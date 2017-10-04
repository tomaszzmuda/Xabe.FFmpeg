using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
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
        internal event ConversionHandler OnProgress;

        /// <summary>
        ///     Fires when FFmpeg process print something
        /// </summary>
        internal event DataReceivedEventHandler OnDataReceived;

        internal async Task<bool> RunProcess(string args, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                _outputLog = new List<string>();

                RunProcess(args, FFmpegPath, true, true, true);

                using(Process)
                {
                    Process.ErrorDataReceived += ProcessOutputData;
                    Process.BeginErrorReadLine();
                    cancellationToken.Register(() => { Process.Kill(); });
                    Process.WaitForExit();

                    if(cancellationToken.IsCancellationRequested)
                        return false;

                    if(Process.ExitCode != 0)
                        throw new ConversionException(string.Join(Environment.NewLine, _outputLog.ToArray()), args);
                }
                return true;
            }, cancellationToken);
        }

        private void ProcessOutputData(object sender, DataReceivedEventArgs e)
        {
            if(e.Data == null)
                return;

            OnDataReceived?.Invoke(this, e);

            _outputLog.Add(e.Data);

            if(OnProgress == null)
                return;

            CalculateTime(e);
        }

        private void CalculateTime(DataReceivedEventArgs e)
        {
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
