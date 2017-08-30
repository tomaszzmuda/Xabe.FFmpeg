using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

        private static readonly object _ffmpegPathLock = new object();
        private static readonly object _ffprobePathLock = new object();

        /// <summary>
        ///     Defines if the ffmpeg was killed by application
        /// </summary>
        protected bool _wasKilled;

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

            char splitChar = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? ':' : ';';

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

        /// <inheritdoc />
        /// <summary>
        ///     Kill ffmpeg process
        /// </summary>
        public void Dispose()
        {
            if(IsRunning)
            {
                _wasKilled = true;
                Process.Kill();
            }
            while(IsRunning)
            {
            }
        }

        private void FindProgramsFromPath(string path)
        {
            if(!Directory.Exists(path))
                return;
            FileInfo[] files = new DirectoryInfo(path).GetFiles();

            FFProbePath = files.FirstOrDefault(x => x.Name.StartsWith("ffprobe", true, CultureInfo.InvariantCulture))
                               ?.FullName;
            FFMpegPath = files.FirstOrDefault(x => x.Name.StartsWith("ffmpeg", true, CultureInfo.InvariantCulture))
                              ?.FullName;
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

            Process.Exited += Process_Exited;
            Process.Start();

            IsRunning = true;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            IsRunning = false;
        }
    }
}
