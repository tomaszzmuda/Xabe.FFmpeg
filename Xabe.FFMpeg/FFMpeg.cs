using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Xabe.FFMpeg.Enums;

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
        private List<string> _errorData;
        private TimeSpan _totalTime;

        /// <summary>
        ///     Fires when ffmpeg progress changes
        /// </summary>
        public event ConversionHandler OnProgress;

        public bool StartConversion(string arguments, string outputPath, VideoInfo[] inputFiles)
        {
            _totalTime = TimeSpan.Zero;
            if(inputFiles != null &&
               inputFiles.Length > 0)
                foreach(VideoInfo video in inputFiles)
                    _totalTime += video.Duration;
            return RunProcess(arguments, outputPath);
        }

        private bool RunProcess(string args, string outputPath)
        {
            _errorData = new List<string>();

            RunProcess(args, FFMpegPath, true, false, true);

            using(Process)
            {
                Process.ErrorDataReceived += OutputData;
                Process.BeginErrorReadLine();
                Process.WaitForExit();
                var statusCode = (FFMpegStatus) Process.ExitCode;

                if (statusCode == FFMpegStatus.Killed)
                    return false;

                if ((string.IsNullOrWhiteSpace(outputPath)
                    || !File.Exists(outputPath)
                    || new FileInfo(outputPath).Length == 0))
                    throw new InvalidOperationException(string.Join(Environment.NewLine, _errorData.ToArray()));
            }

            return true;
        }

        private void OutputData(object sender, DataReceivedEventArgs e)
        {
            if(e.Data == null)
                return;

            _errorData.Add(e.Data);

            if(OnProgress == null ||
               !IsRunning)
                return;

            var r = new Regex(@"\w\w:\w\w:\w\w");
            Match m = r.Match(e.Data);

            if(!e.Data.Contains("frame"))
                return;
            if(!m.Success)
                return;

            TimeSpan t = TimeSpan.Parse(m.Value);
            OnProgress(t, _totalTime);
        }
    }
}
