using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using JetBrains.Annotations;

namespace Xabe.FFMpeg
{
    /// <summary>
    ///     Base FFMpeg class
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public abstract class FFBase: IDisposable
    {
        private static string _ffmpegPath;
        private static string _ffprobePath;

        /// <summary>
        ///     Directory contains FFMpeg and FFProbe
        /// </summary>
        [CanBeNull] [UsedImplicitly] public static string FFMpegDir;

        private readonly object _ffmpegPathLock = new object();
        private readonly object _ffprobePathLock = new object();

        /// <summary>
        ///     FFMpeg process
        /// </summary>
        protected Process Process;

        /// <summary>
        ///     Initalize new FFMpeg. Search ffmpeg and ffprobe in PATH
        /// </summary>
        protected FFBase()
        {
            if(!string.IsNullOrWhiteSpace(FFProbePath) &&
               !string.IsNullOrWhiteSpace(FFMpegPath))
                return;

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
        ///     FilePath to FFMpeg
        /// </summary>
        protected string FFMpegPath
        {
            get
            {
                lock(_ffmpegPathLock)
                {
                    return _ffmpegPath;
                }
            }
            private set
            {
                lock(_ffmpegPathLock)
                {
                    _ffmpegPath = value;
                }
            }
        }

        /// <summary>
        ///     FilePath to FFProbe
        /// </summary>
        protected string FFProbePath
        {
            get
            {
                lock(_ffprobePathLock)
                {
                    return _ffprobePath;
                }
            }
            private set
            {
                lock(_ffprobePathLock)
                {
                    _ffprobePath = value;
                }
            }
        }

        /// <summary>
        ///     Returns true if the associated process is still alive/running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        ///     Kill process
        /// </summary>
        public void Dispose()
        {
            if(IsRunning)
                Process.Kill();
            while(!Process.HasExited)
                Thread.Sleep(10);
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
                // ReSharper disable once EmptyGeneralCatchClause
            catch(Exception)
            {
            }
        }

        /// <summary>
        ///     Run conversion
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <param name="processPath">FilePath to executable (ffmpeg, ffprobe)</param>
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
    }
}
