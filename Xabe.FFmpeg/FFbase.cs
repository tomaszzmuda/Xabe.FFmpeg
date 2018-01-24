using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

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
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnassignedField.Global
        public static string FFmpegDir;

        /// <summary>
        ///     Name of FFmpeg executable name (Case insensitive)
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once ConvertToConstant.Global
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static string FFmpegExecutableName = "ffmpeg";

        /// <summary>
        ///     Name of FFprobe executable name (Case insensitive)
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once ConvertToConstant.Global
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static string FFprobeExecutableName = "ffprobe";


        private static readonly object _ffmpegPathLock = new object();
        private static readonly object _ffprobePathLock = new object();

        /// <summary>
        ///     FFmpeg process
        /// </summary>
        protected Process Process;

        /// <summary>
        ///     Initalize new FFmpeg. Search FFmpeg and FFprobe in PATH
        /// </summary>
        protected FFbase()
        {
            if (!string.IsNullOrWhiteSpace(FFprobePath) &&
               !string.IsNullOrWhiteSpace(FFmpegPath))
                return;

            if (!string.IsNullOrWhiteSpace(FFmpegDir))
            {
                FFprobePath = new DirectoryInfo(FFmpegDir).GetFiles()
                                                          .FirstOrDefault(x => x.Name.ToLower()
                                                                                .Contains(FFprobeExecutableName.ToLower()))
                                                          .FullName;
                FFmpegPath = new DirectoryInfo(FFmpegDir).GetFiles()
                                                         .FirstOrDefault(x => x.Name.ToLower()
                                                                               .Contains(FFmpegExecutableName.ToLower()))
                                                         .FullName;
                ValidateExecutables();
                return;
            }

            Assembly entryAssembly = Assembly.GetEntryAssembly();

            if (entryAssembly != null)
            {
                string workingDirectory = Path.GetDirectoryName(entryAssembly.Location);

                FindProgramsFromPath(workingDirectory);

                if (FFmpegPath != null &&
                   FFprobePath != null)
                    return;
            }

            string[] paths = Environment.GetEnvironmentVariable("PATH")
                                        .Split(Path.PathSeparator);

            foreach (string path in paths)
            {
                FindProgramsFromPath(path);

                if (FFmpegPath != null &&
                   FFprobePath != null)
                    break;
            }

            ValidateExecutables();
        }

        /// <summary>
        ///     FilePath to FFmpeg
        /// </summary>
        protected string FFmpegPath
        {
            get
            {
                lock (_ffmpegPathLock)
                {
                    return _ffmpegPath;
                }
            }
            private set
            {
                lock (_ffmpegPathLock)
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
                lock (_ffprobePathLock)
                {
                    return _ffprobePath;
                }
            }
            private set
            {
                lock (_ffprobePathLock)
                {
                    _ffprobePath = value;
                }
            }
        }

        private void ValidateExecutables()
        {
            if (FFmpegPath != null &&
               FFprobePath != null)
                return;

            string ffmpegDir = string.IsNullOrWhiteSpace(FFmpegDir) ? "" : string.Format(FFmpegDir + " or ");
            string exceptionMessage =
                $"Cannot find FFmpeg in {ffmpegDir}PATH. This package needs installed FFmpeg. Please add it to your PATH variable or specify path to DIRECTORY with FFmpeg executables in {nameof(FFbase)}.{nameof(FFmpegDir)}";
            throw new ArgumentException(exceptionMessage);
        }

        private void FindProgramsFromPath(string path)
        {
            if (!Directory.Exists(path))
                return;
            FileInfo[] files = new DirectoryInfo(path).GetFiles();

            FFprobePath = files.FirstOrDefault(x => x.Name.StartsWith(FFprobeExecutableName, true, CultureInfo.InvariantCulture))
                               ?.FullName;
            FFmpegPath = files.FirstOrDefault(x => x.Name.StartsWith(FFmpegExecutableName, true, CultureInfo.InvariantCulture))
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
        /// <exception cref="ObjectDisposedException"></exception>
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
