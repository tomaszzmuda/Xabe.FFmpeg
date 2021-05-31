using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
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
        private const string TimeFormatPattern = @"\w\w:\w\w:\w\w";
        private static readonly Regex s_timeFormatRegex = new Regex(TimeFormatPattern, RegexOptions.Compiled);
        private List<string> _outputLog;
        private TimeSpan _totalTime;
        private bool _wasKilled = false;

        /// <summary>
        ///     Fires when FFmpeg progress changes
        /// </summary>
        internal event ConversionProgressEventHandler OnProgress;

        /// <summary>
        ///     Fires when FFmpeg process print something
        /// </summary>
        internal event DataReceivedEventHandler OnDataReceived;

        /// <summary>
        ///     Fires when FFmpeg process writes video data to stdout
        /// </summary>
        internal event VideoDataEventHandler OnVideoDataReceived;

        internal Task<bool> RunProcess(
            string args,
            CancellationToken cancellationToken,
            ProcessPriorityClass? priority)
        {
            return Task.Factory.StartNew(() =>
            {
                _outputLog = new List<string>();
                bool pipedOutput = OnVideoDataReceived != null;
                var process = RunProcess(args, FFmpegPath, priority, true, pipedOutput, true);
                var processId = process.Id;
                using (process)
                {
                    process.ErrorDataReceived += (sender, e) => ProcessOutputData(e, args, processId);
                    process.BeginErrorReadLine();
                    if (pipedOutput)
                    {
                        Task.Run(() => ProcessVideoData(process, cancellationToken), cancellationToken);
                    }
                    var ctr = cancellationToken.Register(() =>
                    {
                        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                        {
                            try
                            {
                                process.StandardInput.Write("q");
                                Task.Delay(1000 * 5).GetAwaiter().GetResult();

                                if (!process.HasExited)
                                {
                                    process.CloseMainWindow();
                                    process.Kill();
                                    _wasKilled = true;
                                }
                            }
                            catch (InvalidOperationException)
                            {
                            }
                        }
                    });

                    using (ctr)
                    {
                        using (var processEnded = new ManualResetEvent(false))
                        {
                            processEnded.SetSafeWaitHandle(new SafeWaitHandle(process.Handle, false));
                            int index = WaitHandle.WaitAny(new[] { processEnded, cancellationToken.WaitHandle });

                            // If the signal came from the caller cancellation token close the window
                            if (index == 1
                                && !process.HasExited)
                            {
                                process.CloseMainWindow();
                                process.Kill();
                                _wasKilled = true;
                            }
                            else if (index == 0 && !process.HasExited)
                            {
                                // Workaround for linux: https://github.com/dotnet/corefx/issues/35544
                                process.WaitForExit();
                            }
                        }

                        cancellationToken.ThrowIfCancellationRequested();
                        if (_wasKilled)
                        {
                            throw new ConversionException("Cannot stop process. Killed it.", args);
                        }

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return false;
                        }

                        string output = string.Join(Environment.NewLine, _outputLog.ToArray());
                        var exceptionsCatcher = new FFmpegExceptionCatcher();
                        exceptionsCatcher.CatchFFmpegErrors(output, args);

                        if (process.ExitCode != 0 && _outputLog.Any() && !_outputLog.Last().Contains("dummy"))
                        {
                            throw new ConversionException(output, args);
                        }
                    }
                }

                return true;
            },
            cancellationToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
        }

        private void ProcessOutputData(DataReceivedEventArgs e, string args, int processId)
        {
            if (e.Data == null)
            {
                return;
            }

            OnDataReceived?.Invoke(this, e);

            _outputLog.Add(e.Data);

            if (OnProgress == null)
            {
                return;
            }

            CalculateTime(e, args, processId);
        }

        private void ProcessVideoData(Process process, CancellationToken cancellationToken)
        {
            BinaryReader br = new BinaryReader(process.StandardOutput.BaseStream);
            byte[] buffer;

            while ((buffer = br.ReadBytes(4096)).Length > 0)
            {
                VideoDataEventArgs args = new VideoDataEventArgs(buffer);
                OnVideoDataReceived?.Invoke(this, args);

                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private void CalculateTime(DataReceivedEventArgs e, string args, int processId)
        {
            if (e.Data.Contains("Duration: N/A"))
            {
                return;
            }

            if (e.Data.Contains("Duration"))
            {
                GetDuration(e, s_timeFormatRegex, args);
            }
            else if (e.Data.Contains("size"))
            {
                Match match = s_timeFormatRegex.Match(e.Data);
                if (match.Success)
                {
                    OnProgress(this, new ConversionProgressEventArgs(TimeSpan.Parse(match.Value), _totalTime, processId));
                }
            }
        }

        private void GetDuration(DataReceivedEventArgs e, Regex regex, string args)
        {
            string t = GetArgumentValue("-t", args);
            if (!string.IsNullOrWhiteSpace(t))
            {
                _totalTime = TimeSpan.Parse(t);
                return;
            }

            Match match = regex.Match(e.Data);
            if (!match.Success)
            {
                return;
            }
            _totalTime = TimeSpan.Parse(match.Value);

            string ss = GetArgumentValue("-ss", args);
            if (!string.IsNullOrWhiteSpace(ss))
            {
                _totalTime -= TimeSpan.Parse(ss);
            }
        }

        private string GetArgumentValue(string option, string args)
        {
            List<string> words = args.Split(' ')
                                     .ToList();
            int index = words.IndexOf(option);
            if (index >= 0)
            {
                return words[index + 1];
            }
            return string.Empty;
        }
    }
}
