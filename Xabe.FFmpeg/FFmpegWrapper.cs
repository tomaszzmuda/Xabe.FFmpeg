using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        internal Task<bool> RunProcess(
            string args,
            CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                _outputLog = new List<string>();
                var process = RunProcess(args, FFmpegPath, Priority, true, true, true);
                using (process)
                {
                    process.ErrorDataReceived += (sender, e) => ProcessOutputData(e, args);
                    process.BeginErrorReadLine();
                    // VSTHRD101: Avoid using async lambda for a void returning delegate type, becaues any exceptions not handled by the delegate will crash the process
                    // https://github.com/Microsoft/vs-threading/blob/master/doc/analyzers/VSTHRD101.md
                    var ctr = cancellationToken.Register(() =>
                    {
                        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                        {
                            try
                            {
                                process.StandardInput.Write("q");
                                Task.Delay(1000 * 5).ConfigureAwait(false).GetAwaiter().GetResult();
                            }
                            catch (InvalidOperationException)
                            {
                            }
                            finally
                            {
                                if (!process.HasExited)
                                {
                                    process.CloseMainWindow();
                                    process.Kill();
                                    _wasKilled = true;
                                }
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

                        if (_outputLog.Any(x => x.Contains("Invalid NAL unit size")))
                        {
                            throw new ConversionException(string.Join(Environment.NewLine, _outputLog.ToArray()), args);
                        }

                        if (_outputLog.Any(x => x.Contains("Packet mismatch") && _outputLog.Any(y => y.Contains("Output file is empty") )))
                        {
                            throw new ConversionException(string.Join(Environment.NewLine, _outputLog.ToArray()), args);
                        }
                        
                        if (_outputLog.Any(x => x.Contains("multiple fourcc not supported")))
                        {
                            throw new UnknownDecoderException(string.Join(Environment.NewLine, _outputLog.ToArray()), args);
                        }

                        if (_outputLog.Any(x => x.Contains("Unrecognized hwaccel: ")))
                        {
                            throw new HardwareAcceleratorNotFoundException(string.Join(Environment.NewLine, _outputLog.ToArray()), args);
                        }

                        if (_outputLog.Any(x => x.Contains("Unknown decoder")))
                        {
                            throw new UnknownDecoderException(string.Join(Environment.NewLine, _outputLog.ToArray()), args);
                        }

                        if (_outputLog.Any(x => x.Contains("asf_read_pts failed")))
                        {
                            throw new UnknownDecoderException(string.Join(Environment.NewLine, _outputLog.ToArray()), args);
                        }

                        if (_outputLog.Any(x => x.Contains("Missing key frame while searching for timestamp") && _outputLog.Any(y => y.Contains("Output file is empty"))) )
                        {
                            throw new UnknownDecoderException(string.Join(Environment.NewLine, _outputLog.ToArray()), args);
                        }

                        if (_outputLog.Any(x => x.Contains("Old interlaced mode is not supported")))
                        {
                            throw new UnknownDecoderException(string.Join(Environment.NewLine, _outputLog.ToArray()), args);
                        }

                        if (_outputLog.Any(x => x.Contains("Failed to open codec in avformat_find_stream_info")))
                        {
                            throw new UnknownDecoderException(string.Join(Environment.NewLine, _outputLog.ToArray()), args);
                        }

                        if (_outputLog.Any(x => x.Contains("mpeg1video") && _outputLog.Any(y => y.Contains("Output file is empty"))))
                        {
                            throw new UnknownDecoderException(string.Join(Environment.NewLine, _outputLog.ToArray()), args);
                        }

                        if (_outputLog.Any(x => x.Contains("Frame rate very high for a muxer not efficiently supporting it") && _outputLog.Any(y => y.Contains("Output file is empty"))))
                        {
                            throw new UnknownDecoderException(string.Join(Environment.NewLine, _outputLog.ToArray()), args);
                        }

                        if (process.ExitCode != 0)
                        {
                            throw new ConversionException(string.Join(Environment.NewLine, _outputLog.ToArray()), args);
                        }
                    }
                }

                return true;
            },
            cancellationToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
        }

        private void ProcessOutputData(DataReceivedEventArgs e, string args)
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

            CalculateTime(e, args);
        }

        private void CalculateTime(DataReceivedEventArgs e, string args)
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
                    OnProgress(this, new ConversionProgressEventArgs(TimeSpan.Parse(match.Value), _totalTime));
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
