using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Xabe.FFmpeg.Exceptions;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Base FFmpeg class
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public abstract class FFBase: IDisposable
    {
        private const string TryMultipleConversion =
            "Current FFmpeg process associated to this object is already in use. Please wait till the end of file conversion or create another VideoInfo/Conversion instance and run process.";

        private static string _ffmpegPath;
        private static string _ffprobePath;

        /// <summary>
        ///     Directory contains FFmpeg and FFProbe
        /// </summary>
        [CanBeNull] [UsedImplicitly] public static string FFmpegDir;

        private static readonly object _ffmpegPathLock = new object();
        private static readonly object _ffprobePathLock = new object();
        private readonly object _isRunningLock = new object();
        private readonly object _wasKilledLock = new object();

        private bool _isRunning;
        private bool _wasKilled;

        /// <summary>
        ///     FFmpeg process
        /// </summary>
        protected Process Process;

        /// <summary>
        ///     Initalize new FFmpeg. Search FFmpeg and ffprobe in PATH
        /// </summary>
        protected FFBase()
        {
            if(!string.IsNullOrWhiteSpace(FFProbePath) &&
               !string.IsNullOrWhiteSpace(FFmpegPath))
                return;

            if(!string.IsNullOrWhiteSpace(FFmpegDir))
            {
                FFProbePath = new DirectoryInfo(FFmpegDir).GetFiles()
                                                          .First(x => x.Name.Contains("ffprobe"))
                                                          .FullName;
                FFmpegPath = new DirectoryInfo(FFmpegDir).GetFiles()
                                                         .First(x => x.Name.Contains("FFmpeg"))
                                                         .FullName;
                return;
            }

            char splitChar = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? ':' : ';';

            string[] paths = Environment.GetEnvironmentVariable("PATH")
                                        .Split(splitChar);

            foreach(string path in paths)
            {
                FindProgramsFromPath(path);

                if(FFmpegPath != null &&
                   FFProbePath != null)
                    break;
            }

            if(FFmpegPath == null ||
               FFmpegPath == null)
                throw new ArgumentException("Cannot find FFmpeg.");
        }

        /// <summary>
        ///     FilePath to FFmpeg
        /// </summary>
        protected string FFmpegPath
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
        public bool IsRunning
        {
            get
            {
                lock(_isRunningLock)
                {
                    return _isRunning;
                }
            }
            private set
            {
                lock(_isRunningLock)
                {
                    _isRunning = value;
                }
            }
        }

        /// <summary>
        ///     Defines if the FFmpeg was killed by application
        /// </summary>
        protected bool WasKilled
        {
            get
            {
                lock(_wasKilledLock)
                {
                    return _wasKilled;
                }
            }
            set
            {
                lock(_wasKilledLock)
                {
                    _wasKilled = value;
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Kill FFmpeg process
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
            FFmpegPath = files.FirstOrDefault(x => x.Name.StartsWith("FFmpeg", true, CultureInfo.InvariantCulture))
                              ?.FullName;
        }

        /// <summary>
        ///     Run conversion
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <param name="processPath">FilePath to executable (FFmpeg, ffprobe)</param>
        /// <param name="rStandardInput">Should redirect standard input</param>
        /// <param name="rStandardOutput">Should redirect standard output</param>
        /// <param name="rStandardError">Should redirect standard error</param>
        /// <exception cref="InvalidOperationException"></exception>
        protected void RunProcess(string args, string processPath, bool rStandardInput = false,
            bool rStandardOutput = false, bool rStandardError = false)
        {
            if(IsRunning)
                throw new MultipleConversionException(TryMultipleConversion, args);

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
