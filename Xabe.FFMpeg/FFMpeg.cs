using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

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
            if(inputFiles != null && inputFiles.Length > 0)
                foreach(var video in inputFiles)
                    _totalTime += video.Duration;
            return RunProcess(arguments, outputPath);
        }

        /// <summary>
        ///     Send exit signal to the ffmpeg process.
        /// </summary>
        public void Stop()
        {
            if(IsRunning)
                Process.StandardInput.Write('q');
            while(IsRunning)
                Thread.Sleep(10);
        }

        private bool RunProcess(string args, string outputPath)
        {
            var result = false;
            _errorData = new List<string>();

            RunProcess(args, FFMpegPath, true, false, true);

            using(Process)
            {
                Process.ErrorDataReceived += OutputData;
                Process.BeginErrorReadLine();
                Process.WaitForExit();
                result = Process.ExitCode == 0;

                if(!result &&
                   (string.IsNullOrWhiteSpace(outputPath)
                    || !File.Exists(outputPath)
                    || new FileInfo(outputPath).Length == 0))
                    throw new InvalidOperationException(string.Join("\r\n", _errorData.ToArray()));
            }

            return result;
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
