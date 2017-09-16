using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Base FFmpeg class
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public abstract class FFbase
    {
        private static string _ffmpegPath;
        private static string _ffprobePath;

        /// <summary>
        ///     Directory contains FFmpeg and FFprobe
        /// </summary>
        [CanBeNull] [UsedImplicitly] public static string FFmpegDir;

        /// <summary>
        ///     Name of FFmpeg executable name (Case insensitive)
        /// </summary>
        [CanBeNull] [UsedImplicitly] public static string FFmpegExecutableName = "fFmpeg";

        /// <summary>
        ///     Name of FFprobe executable name (Case insensitive)
        /// </summary>
        [CanBeNull] [UsedImplicitly] public static string FFprobeExecutableName = "ffprobe";


        private static readonly object _ffmpegPathLock = new object();
        private static readonly object _ffprobePathLock = new object();
        private readonly object _wasKilledLock = new object();

        private bool _wasKilled;

        /// <summary>
        ///     FFmpeg process
        /// </summary>
        protected Process Process;

        /// <summary>
        ///     Initalize new FFmpeg. Search FFmpeg and ffprobe in PATH
        /// </summary>
        protected FFbase()
        {
            if(!string.IsNullOrWhiteSpace(FFprobePath) &&
               !string.IsNullOrWhiteSpace(FFmpegPath))
                return;

            if(!string.IsNullOrWhiteSpace(FFmpegDir))
                try
                {
                    FFprobePath = new DirectoryInfo(FFmpegDir).GetFiles()
                                                              .First(x => x.Name.ToLower()
                                                                           .Contains(FFprobeExecutableName.ToLower()))
                                                              .FullName;
                    FFmpegPath = new DirectoryInfo(FFmpegDir).GetFiles()
                                                             .First(x => x.Name.ToLower()
                                                                          .Contains(FFmpegExecutableName.ToLower()))
                                                             .FullName;
                    return;
                }
                catch(InvalidOperationException)
                {
                }

            char splitChar = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? ':' : ';';

            string[] paths = Environment.GetEnvironmentVariable("PATH")
                                        .Split(splitChar);

            foreach(string path in paths)
            {
                FindProgramsFromPath(path);

                if(FFmpegPath != null &&
                   FFprobePath != null)
                    break;
            }

            if(FFmpegPath == null ||
               FFprobePath == null)
            {
                string ffmpegDir = string.IsNullOrWhiteSpace(FFmpegDir) ? "" : string.Format(FFmpegDir + " or ");
                string exceptionMessage = $"Cannot find FFmpeg in {ffmpegDir}PATH";
                throw new ArgumentException(exceptionMessage);
            }
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
        ///     FilePath to FFprobe
        /// </summary>
        protected string FFprobePath
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

        private void FindProgramsFromPath(string path)
        {
            if(!Directory.Exists(path))
                return;
            FileInfo[] files = new DirectoryInfo(path).GetFiles();

            FFprobePath = files.FirstOrDefault(x => x.Name.StartsWith("ffprobe", true, CultureInfo.InvariantCulture))
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

            Process.Start();
        }
    }
}
