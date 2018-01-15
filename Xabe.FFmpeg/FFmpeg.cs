using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                    Process.ErrorDataReceived += (sender, e) => ProcessOutputData(e, args);
                    Process.BeginErrorReadLine();
                    cancellationToken.Register(() => { Process.StandardInput.Write("q"); });
                    Process.WaitForExit();

                    if(cancellationToken.IsCancellationRequested)
                        return false;

                    if(Process.ExitCode != 0)
                        throw new ConversionException(string.Join(Environment.NewLine, _outputLog.ToArray()), args);
                }
                return true;
            });
        }

        private void ProcessOutputData(DataReceivedEventArgs e, string args)
        {
            if(e.Data == null)
                return;

            OnDataReceived?.Invoke(this, e);

            _outputLog.Add(e.Data);

            if(OnProgress == null)
                return;

            CalculateTime(e, args);
        }

        private void CalculateTime(DataReceivedEventArgs e, string args)
        {
            if(e.Data.Contains("Duration: N/A"))
                return;

            var regex = new Regex(TimeFormatRegex);
            if(e.Data.Contains("Duration"))
            {
                GetDuration(e, regex, args);
            }
            else if(e.Data.Contains("frame"))
            {
                Match match = regex.Match(e.Data);
                if(match.Success)
                    OnProgress(ParseDuration(match.Value), _totalTime);
            }
        }

        private void GetDuration(DataReceivedEventArgs e, Regex regex, string args)
        {
            string t = GetArgumentValue("-t", args);
            if(!string.IsNullOrWhiteSpace(t))
            {
                _totalTime = ParseDuration(t);
                return;
            }

            Match match = regex.Match(e.Data);
            _totalTime = ParseDuration(match.Value);


            string ss = GetArgumentValue("-ss", args);
            if(!string.IsNullOrWhiteSpace(ss))
                _totalTime -= ParseDuration(ss);
        }

        private string GetArgumentValue(string option, string args)
        {
            List<string> words = args.Split(' ')
                                     .ToList();
            int index = words.IndexOf(option);
            if(index >= 0)
                return words[index + 1];
            return "";
        }

        private TimeSpan ParseDuration(string duration)
        {
            List<string> parts = duration.Split(':').Reverse().ToList();

            int milliseconds = 0;
            int seconds = 0;

            if (parts[0].Contains('.'))
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
