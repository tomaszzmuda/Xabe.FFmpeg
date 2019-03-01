using System.ComponentModel;
using System.Diagnostics;

namespace Xabe.FFmpeg.Model
{
    /// <summary>
    ///     Ffmpeg process priority
    /// </summary>
    public class FFmpegProcessPriority
    {
        private readonly ProcessPriorityClass _priority;

        private FFmpegProcessPriority(ProcessPriorityClass priority)
        {
            _priority = priority;
        }

        /// <summary>
        ///     Get ProcessPriorityClass
        /// </summary>
        /// <returns>ProcessPriorityClass enum</returns>
        public ProcessPriorityClass? GetPriorityClass()
        {
            return _priority;
        }

        /// <summary>
        ///     Priority from default enum
        /// </summary>
        /// <param name="priority">Priority</param>
        public static FFmpegProcessPriority FromPriorityClass(ProcessPriorityClass priority)
        {
            return new FFmpegProcessPriority(priority);
        }

        /// <summary>
        ///     Priority from current process
        /// </summary>
        public static FFmpegProcessPriority FromCurrentProcess()
        {
            try
            {
                return new FFmpegProcessPriority(Process.GetCurrentProcess()
                                                        .PriorityClass);
            }
            catch (Win32Exception e) when (e.Message.Contains("Permission denied "))
            {
                return Default();
            }
        }

        /// <summary>
        ///     Default process priority
        /// </summary>
        public static FFmpegProcessPriority Default()
        {
            return null;
        }
    }
}
