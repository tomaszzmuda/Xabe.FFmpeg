using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xabe.FFmpeg.Downloader;
using Xabe.FFmpeg.Exceptions;
using FileInfo = Xabe.FFmpeg.Model.FileInfo;

namespace Xabe.FFmpeg
{
    /// <inheritdoc />
    /// <summary>
    ///     Wrapper for FFmpeg
    /// </summary>
    public abstract class FFmpeg
    {
        private static string s_ffmpegPath;
        private static string s_ffprobePath;

        private static readonly object s_ffmpegPathLock = new object();
        private static readonly object s_ffprobePathLock = new object();

        /// <summary>
        ///     Directory contains FFmpeg and FFprobe
        /// </summary>
        [Obsolete("Please use ExecutablePath property.")]
        public static string FFmpegDir { get => ExecutablesPath; set => ExecutablesPath = value; }

        /// <summary>
        ///     Directory containing FFmpeg and FFprobe
        /// </summary>
        public static string ExecutablesPath { get; set; }

        /// <summary>
        ///     Name of FFmpeg executable name (Case insensitive)
        /// </summary>
        public static string FFmpegExecutableName { get; } = FFmpegExecutablesHelper.FFmpegExecutableName;

        /// <summary>
        ///     Name of FFprobe executable name (Case insensitive)
        /// </summary>
        public static string FFprobeExecutableName { get; } = FFmpegExecutablesHelper.FFprobeExecutableName;

        /// <summary>
        ///     FFmpeg tool process priority
        /// </summary>
        public ProcessPriorityClass? Priority { get; set; }

        /// <summary>
        /// FFmpeg process id
        /// </summary>
        public int FFmpegProcessId { get; private set; }

        /// <summary>
        ///     Download latest FFmpeg version for current operating system to FFmpeg.ExecutablePath. If it is not set download to ".".
        /// </summary>
        public static Task GetLatestVersion()
        {
            return FFmpegDownloader.GetLatestVersion();
        }

        /// <summary>
        ///     Initalize new FFmpeg. Search FFmpeg and FFprobe in PATH
        /// </summary>
        protected FFmpeg()
        {
            if (!string.IsNullOrWhiteSpace(FFprobePath) &&
               !string.IsNullOrWhiteSpace(FFmpegPath))
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(ExecutablesPath))
            {
                FFprobePath = new DirectoryInfo(ExecutablesPath).GetFiles()
                                                          .FirstOrDefault(x => x.Name.ToLower()
                                                                                .Contains(FFprobeExecutableName.ToLower()))
                                                          .FullName;
                FFmpegPath = new DirectoryInfo(ExecutablesPath).GetFiles()
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
                {
                    return;
                }
            }

            string[] paths = Environment.GetEnvironmentVariable("PATH")
                                        .Split(Path.PathSeparator);

            foreach (string path in paths)
            {
                FindProgramsFromPath(path);

                if (FFmpegPath != null &&
                   FFprobePath != null)
                {
                    break;
                }
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
                lock (s_ffmpegPathLock)
                {
                    return s_ffmpegPath;
                }
            }

            private set
            {
                lock (s_ffmpegPathLock)
                {
                    s_ffmpegPath = value;
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
                lock (s_ffprobePathLock)
                {
                    return s_ffprobePath;
                }
            }

            private set
            {
                lock (s_ffprobePathLock)
                {
                    s_ffprobePath = value;
                }
            }
        }

        private void ValidateExecutables()
        {
            if (FFmpegPath != null &&
               FFprobePath != null)
            {
                return;
            }

            string ffmpegDir = string.IsNullOrWhiteSpace(ExecutablesPath) ? string.Empty : string.Format(ExecutablesPath + " or ");
            string exceptionMessage =
                $"Cannot find FFmpeg in {ffmpegDir}PATH. This package needs installed FFmpeg. Please add it to your PATH variable or specify path to DIRECTORY with FFmpeg executables in {nameof(FFmpeg)}.{nameof(ExecutablesPath)}";
            throw new FFmpegNotFoundException(exceptionMessage);
        }

        private void FindProgramsFromPath(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            IEnumerable<FileInfo> files = new DirectoryInfo(path).GetFiles().Select(x => new FileInfo
            {
                Name = x.Name,
                FullName = x.FullName
            });

            FFprobePath = FFmpegExecutablesHelper.SelectFFprobePath(files);
            FFmpegPath = FFmpegExecutablesHelper.SelectFFmpegPath(files);
        }

        /// <summary>
        ///     Run conversion
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <param name="processPath">FilePath to executable (FFmpeg, ffprobe)</param>
        /// <param name="priority">Process priority to run executables</param>
        /// <param name="standardInput">Should redirect standard input</param>
        /// <param name="standardOutput">Should redirect standard output</param>
        /// <param name="standardError">Should redirect standard error</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        protected Process RunProcess(
            string args,
            string processPath,
            ProcessPriorityClass? priority,
            bool standardInput = false,
            bool standardOutput = false,
            bool standardError = false)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = processPath,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = standardInput,
                    RedirectStandardOutput = standardOutput,
                    RedirectStandardError = standardError
                },
                EnableRaisingEvents = true
            };

            process.Start();
            FFmpegProcessId = process.Id;

            try
            {
                if (priority.HasValue)
                {
                    process.PriorityClass = priority.Value;
                }
                else
                {
                    process.PriorityClass = Process.GetCurrentProcess().PriorityClass;
                }
            }
            catch (Exception)
            {
            }

            return process;
        }
    }

    /// <summary>
    ///     Base FFmpeg class
    /// </summary>
    [Obsolete("Please use class FFmpeg.")]
    public abstract class FFbase : FFmpeg
    {
    }
}
