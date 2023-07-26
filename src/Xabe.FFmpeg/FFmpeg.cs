using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Xabe.FFmpeg.Exceptions;

namespace Xabe.FFmpeg
{
    public enum FileNameFilterMethod
    {
        Contains,
        Exact,
        StartWith
    }

    public enum UserOperatingSystem
    {
        WINDOWS,
        MAC,
        ANDROID,
        LINUX
    }

    /// <summary> 
    ///     Wrapper for FFmpeg
    /// </summary>
    public abstract partial class FFmpeg
    {
        private static string _ffmpegPath;
        private static string _ffprobePath;
        private static string _lastExecutablePath = Guid.NewGuid().ToString();

        private static readonly object _ffmpegPathLock = new object();
        private static readonly object _ffprobePathLock = new object();
        private static string _ffmpegExecutableName = "ffmpeg";
        private static string _ffprobeExecutableName = "ffprobe";

        /// <summary>
        ///     Initalize new FFmpeg. Search FFmpeg and FFprobe in PATH
        /// </summary>
        /// 
        protected FFmpeg()
        {
            FindAndValidateExecutables();
        }

        private void FindAndValidateExecutables()
        {
            if (!string.IsNullOrWhiteSpace(FFprobePath) &&
               !string.IsNullOrWhiteSpace(FFmpegPath) && _lastExecutablePath.Equals(ExecutablesPath))
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(ExecutablesPath))
            {
                var files = new DirectoryInfo(ExecutablesPath).GetFiles();
                switch (FilterMethod)
                {
                    case FileNameFilterMethod.Contains:
                        FFprobePath = files.FirstOrDefault(x => x.Name.ToString(FormatProvider)
                                                                .Contains(_ffprobeExecutableName) && IsExecutable(x.FullName.ToString(FormatProvider)))?.FullName;
                        FFmpegPath = files.FirstOrDefault(x => x.Name.ToString(FormatProvider)
                                                                .Contains(_ffmpegExecutableName) && IsExecutable(x.FullName.ToString(FormatProvider)))?.FullName;
                        break;
                    case FileNameFilterMethod.Exact:
                        FFprobePath = files.FirstOrDefault(x => x.Name.ToString(FormatProvider)
                                                                .Equals(_ffprobeExecutableName) && IsExecutable(x.FullName.ToString(FormatProvider)))?.FullName;
                        FFmpegPath = files.FirstOrDefault(x => x.Name.ToString(FormatProvider)
                                                                .Equals(_ffmpegExecutableName) && IsExecutable(x.FullName.ToString(FormatProvider)))?.FullName;
                        break;
                    case FileNameFilterMethod.StartWith:
                        FFprobePath = files.FirstOrDefault(x => x.Name.ToString(FormatProvider)
                                                                .StartsWith(_ffprobeExecutableName) && IsExecutable(x.FullName.ToString(FormatProvider)))?.FullName;
                        FFmpegPath = files.FirstOrDefault(x => x.Name.ToString(FormatProvider)
                                                                .StartsWith(_ffmpegExecutableName) && IsExecutable(x.FullName.ToString(FormatProvider)))?.FullName;
                        break;
                    default:
                        break;
                }

                ValidateExecutables();
                _lastExecutablePath = ExecutablesPath;
                return;
            }

            var entryAssembly = Assembly.GetEntryAssembly();

            if (entryAssembly != null)
            {
                var workingDirectory = Path.GetDirectoryName(entryAssembly.Location);

                FindProgramsFromPath(workingDirectory);

                if (FFmpegPath != null &&
                   FFprobePath != null)
                {
                    return;
                }
            }

            var paths = Environment.GetEnvironmentVariable("PATH")
                                        .Split(Path.PathSeparator);

            foreach (var path in paths)
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
            {
                return;
            }

            var ffmpegDir = string.IsNullOrWhiteSpace(ExecutablesPath) ? string.Empty : string.Format(ExecutablesPath + " or ");
            var exceptionMessage =
                $"Cannot find FFmpeg in {ffmpegDir}PATH. This package needs installed FFmpeg. Please add it to your PATH variable or specify path to DIRECTORY with FFmpeg executables in {nameof(FFmpeg)}.{nameof(ExecutablesPath)}";
            throw new FFmpegNotFoundException(exceptionMessage);
        }

        private bool IsExecutable(string file)
        {
            try
            {
                using (var fileStream = File.OpenRead(file))
                {
                    var magicNumber = new byte[4];
                    var appMagicNumber = new byte[4];
                    fileStream.Read(magicNumber, 0, 4);
                    var provider = new OperatingSystemProvider();

                    switch (provider.GetOperatingSystem())
                    {
                        case OperatingSystem.Windows64:
                            return magicNumber[0] == 0x4D && magicNumber[1] == 0x5A;
                        case OperatingSystem.Windows32:
                            return magicNumber[0] == 0x4D && magicNumber[1] == 0x5A;
                        case OperatingSystem.Osx64:
                            return magicNumber[0] == 0xCE && magicNumber[1] == 0xFA && magicNumber[2] == 0xED && magicNumber[3] == 0xFE;
                        case OperatingSystem.Linux64:
                            return magicNumber[0] == 0x7F && magicNumber[1] == 0x45 && magicNumber[2] == 0x4C && magicNumber[3] == 0x46;
                        case OperatingSystem.Linux32:
                            return magicNumber[0] == 0x7F && magicNumber[1] == 0x45 && magicNumber[2] == 0x4C && magicNumber[3] == 0x46;
                        case OperatingSystem.LinuxArmhf:
                            fileStream.Seek(0x30, SeekOrigin.Begin);
                            fileStream.Read(appMagicNumber, 0, 4);
                            return appMagicNumber[0] == 0x50 && appMagicNumber[1] == 0x4B && appMagicNumber[2] == 0x03 && appMagicNumber[3] == 0x04;
                        case OperatingSystem.LinuxArm64:
                            fileStream.Seek(0x30, SeekOrigin.Begin);
                            fileStream.Read(appMagicNumber, 0, 4);
                            return appMagicNumber[0] == 0x50 && appMagicNumber[1] == 0x4B && appMagicNumber[2] == 0x03 && appMagicNumber[3] == 0x04;
                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {
                //??
            }

            return false;
        }

        private void FindProgramsFromPath(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            IEnumerable<FileInfo> files = new DirectoryInfo(path).GetFiles();

            FFprobePath = GetFullName(files, _ffprobeExecutableName);
            FFmpegPath = GetFullName(files, _ffmpegExecutableName);
        }

        internal static string GetFullName(IEnumerable<FileInfo> files, string fileName)
        {
            return files.FirstOrDefault(x => x.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)
                   || x.Name.Equals($"{fileName}.exe", StringComparison.InvariantCultureIgnoreCase))
                        ?.FullName;
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

            try
            {
                process.PriorityClass = priority ?? Process.GetCurrentProcess().PriorityClass;
            }
            catch (Exception)
            {
            }

            return process;
        }
    }
}
