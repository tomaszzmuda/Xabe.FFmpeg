using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xabe.FFMpeg
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    ///     Base FFMpeg class
    /// </summary>
    internal abstract class FFBase: IDisposable
    {
        /// <summary>
        ///     Directory contains FFMpeg and FFProbe
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static string FFMpegDir;

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Path to FFMpeg
        /// </summary>
        protected string FFMpegPath;

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Path to FFProbe
        /// </summary>
        protected string FFProbePath;

        /// <summary>
        ///     FFMpeg process
        /// </summary>
        protected Process Process;

        /// <summary>
        ///     Initalize new FFMpeg. Search ffmpeg and ffprobe in PATH
        /// </summary>
        protected FFBase()
        {
            if(!string.IsNullOrWhiteSpace(FFMpegDir))
            {
                FFProbePath = new DirectoryInfo(FFMpegDir).GetFiles()
                                                          .First(x => x.Name.Contains("ffprobe"))
                                                          .FullName;
                FFMpegPath = new DirectoryInfo(FFMpegDir).GetFiles()
                                                         .First(x => x.Name.Contains("ffmpeg"))
                                                         .FullName;
                return;
            }
            
            var splitChar = ';';
            bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            if(isLinux)
                splitChar = ':';

            string[] paths = Environment.GetEnvironmentVariable("PATH")
                                        .Split(splitChar);

            foreach(string path in paths)
            {
                FindProgramsFromPath(path);

                if(FFMpegPath != null &&
                   FFProbePath != null)
                    break;
            }

            if(FFMpegPath == null ||
               FFMpegPath == null)
                throw new ArgumentException("Cannot find FFMpeg.");
        }

        /// <summary>
        ///     Returns true if the associated process is still alive/running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        ///     Dispose process
        /// </summary>
        public void Dispose()
        {
            Process?.Dispose();
        }

        private void FindProgramsFromPath(string path)
        {
            try
            {
                FFProbePath = new DirectoryInfo(path).GetFiles()
                                                     .FirstOrDefault(x => x.Name.StartsWith("ffprobe", true, CultureInfo.InvariantCulture))
                                                     ?.FullName;
                FFMpegPath = new DirectoryInfo(path).GetFiles()
                                                    .FirstOrDefault(x => x.Name.StartsWith("ffmpeg", true, CultureInfo.InvariantCulture))
                                                    ?.FullName;
            }
            catch(Exception)
            {
            }
        }

        /// <summary>
        ///     Run conversion
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <param name="processPath">Path to executable (ffmpeg, ffprobe)</param>
        /// <param name="rStandardInput">Should redirect standard input</param>
        /// <param name="rStandardOutput">Should redirect standard output</param>
        /// <param name="rStandardError">Should redirect standard error</param>
        protected void RunProcess(string args, string processPath, bool rStandardInput = false,
            bool rStandardOutput = false, bool rStandardError = false)
        {
            if(IsRunning)
                throw new InvalidOperationException(
                    "The current FFMpeg process is busy with another operation. Create a new object for parallel executions.");

            Process = new Process
            {
                StartInfo =
                {
                    FileName = processPath,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = rStandardInput,
                    RedirectStandardOutput = rStandardOutput,
                    RedirectStandardError = rStandardError
                },
                EnableRaisingEvents = true
            };

            Process.Exited += OnProcessExit;
            Process.Start();

            IsRunning = true;
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            IsRunning = false;
        }

        /// <summary>
        ///     Kill ffmpeg process.
        /// </summary>
        public void Kill()
        {
            if(IsRunning)
                Process.Kill();
        }
    }
}
