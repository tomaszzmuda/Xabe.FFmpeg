using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg.Events;
using Xabe.FFmpeg.Exceptions;

namespace Xabe.FFmpeg
{
    // ReSharper disable once InconsistentNaming

    /// <inheritdoc />
    /// <summary>
    ///     Wrapper for FFmpeg
    /// </summary>
    internal class FFmpegWrapper : FFmpeg
    {
        private const string TimeFormatRegex = @"\w\w:\w\w:\w\w";
        private List<string> _outputLog;
        private TimeSpan _totalTime;

        /// <summary>
        ///     Fires when FFmpeg progress changes
        /// </summary>
        internal event ConversionProgressEventHandler OnProgress;

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
                    {
                        return false;
                    }

                    if(_outputLog.Any(x => x.Contains("Unrecognized hwaccel: ")))
                    {
                        throw new HardwareAcceleratorNotFoundException(string.Join(Environment.NewLine, _outputLog.ToArray()), args);
                    }

                    if(Process.ExitCode != 0)
                    {
                        throw new ConversionException(string.Join(Environment.NewLine, _outputLog.ToArray()), args);
                    }
                }
                return true;
            });
        }

        private void ProcessOutputData(DataReceivedEventArgs e, string args)
        {
            if(e.Data == null)
            {
                return;
            }

            OnDataReceived?.Invoke(this, e);

            _outputLog.Add(e.Data);

            if(OnProgress == null)
            {
                return;
            }

            CalculateTime(e, args);
        }

        private void CalculateTime(DataReceivedEventArgs e, string args)
        {
            if(e.Data.Contains("Duration: N/A"))
            {
                return;
            }

            var regex = new Regex(TimeFormatRegex);
            if(e.Data.Contains("Duration"))
            {
                GetDuration(e, regex, args);
            }
            else if(e.Data.Contains("frame"))
            {
                Match match = regex.Match(e.Data);
                if(match.Success)
                {
                    OnProgress(this, new ConversionProgressEventArgs(TimeSpan.Parse(match.Value), _totalTime));
                }
            }
        }

        private void GetDuration(DataReceivedEventArgs e, Regex regex, string args)
        {
            string t = GetArgumentValue("-t", args);
            if(!string.IsNullOrWhiteSpace(t))
            {
                _totalTime = TimeSpan.Parse(t);
                return;
            }

            Match match = regex.Match(e.Data);
            _totalTime = TimeSpan.Parse(match.Value);

            string ss = GetArgumentValue("-ss", args);
            if(!string.IsNullOrWhiteSpace(ss))
            {
                _totalTime -= TimeSpan.Parse(ss);
            }
        }

        private string GetArgumentValue(string option, string args)
        {
            List<string> words = args.Split(' ')
                                     .ToList();
            int index = words.IndexOf(option);
            if(index >= 0)
            {
                return words[index + 1];
            }
            return string.Empty;
        }
    }
}
