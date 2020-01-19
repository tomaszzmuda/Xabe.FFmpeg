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


    internal class ExceptionCheck
    {
        private string lookFor;
        private bool ContainsOutputfileisempty;
        Exception ex;
        public ExceptionCheck(Exception ex, string lookFor, bool ContainsOutputfileisempty = false) 
        {
            this.ex = ex;
            this.lookFor = lookFor;
            this.ContainsOutputfileisempty = ContainsOutputfileisempty;
        }

        /// <summary>
        /// Checks outputlog and throws Exception - some errors are only fatal if the text "Output file is empty" is found in the log
        /// </summary>
        /// <param name="log"></param>
        public void checkLog(List<string> log)
        {
            if (log.Any(x => x.Contains(this.lookFor) && (!this.ContainsOutputfileisempty || log.Any(y => y.Contains("Output file is empty")))))
            {
                throw ex;
            }
        }

    }

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

        private readonly string[] ConversionExceptionStrings = { "", "" };
        private readonly string[] UnknownDecoderExceptionStrings = { "", "" };

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

                        List<ExceptionCheck> allExceptions = new List<ExceptionCheck>();
                        string exceptionOutput = string.Join(Environment.NewLine, _outputLog.ToArray());

                        ConversionException conversionException = new ConversionException(exceptionOutput, args);
                        allExceptions.Add(new ExceptionCheck(conversionException, "Invalid NAL unit size"));
                        allExceptions.Add(new ExceptionCheck(conversionException, "Packet mismatch", true));

                        UnknownDecoderException unknownDecoderException = new UnknownDecoderException(exceptionOutput, args);
                        allExceptions.Add(new ExceptionCheck(unknownDecoderException, "asf_read_pts failed", true));
                        allExceptions.Add(new ExceptionCheck(unknownDecoderException, "Missing key frame while searching for timestamp", true));
                        allExceptions.Add(new ExceptionCheck(unknownDecoderException, "Old interlaced mode is not supported", true));
                        allExceptions.Add(new ExceptionCheck(unknownDecoderException, "mpeg1video", true));
                        allExceptions.Add(new ExceptionCheck(unknownDecoderException, "Frame rate very high for a muxer not efficiently supporting it", true));
                        allExceptions.Add(new ExceptionCheck(unknownDecoderException, "multiple fourcc not supported"));
                        allExceptions.Add(new ExceptionCheck(unknownDecoderException, "Unknown decoder"));
                        allExceptions.Add(new ExceptionCheck(unknownDecoderException, "Failed to open codec in avformat_find_stream_info"));

                        HardwareAcceleratorNotFoundException hardwareAcceleratorNotFoundException = new HardwareAcceleratorNotFoundException(exceptionOutput, args);
                        allExceptions.Add(new ExceptionCheck(hardwareAcceleratorNotFoundException, "Unrecognized hwaccel: "));

                        foreach(ExceptionCheck item in allExceptions)
                        {
                            item.checkLog(_outputLog);
                        }

                        if (process.ExitCode != 0)
                        {
                            throw new ConversionException(exceptionOutput, args);
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
