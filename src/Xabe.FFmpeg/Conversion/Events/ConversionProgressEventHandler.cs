using System;
using System.Diagnostics;

namespace Xabe.FFmpeg.Events
{
    /// <summary>
    ///     Info about conversion progress
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="args">Conversion info</param>
    public delegate void ConversionDataEventHandler(object sender, DataReceivedEventArgs args, Int64 processid);
    public delegate void ConversionProgressEventHandler(object sender, ConversionProgressEventArgs args);
    public delegate void ConversionErrorEventHandler(object Sender, ConversionErrorEventsArg args);

    public class ConversionErrorEventsArg : EventArgs
    {
        public ConversionErrorEventsArg(string output, string args, int processid)
        {
            this.Output = output;
            this.Args = args;
            this.ProcessId = processid;
        }
        public string Output { get; }
        public string Args { get; }
        public long ProcessId { get; }
    }
    /// <summary>
    ///     Conversion information
    /// </summary>
    public class ConversionProgressEventArgs : EventArgs
    {
        /// <inheritdoc />
        public ConversionProgressEventArgs(TimeSpan timeSpan, TimeSpan totalTime, int processId)
        {
            Duration = timeSpan;
            TotalLength = totalTime;
            ProcessId = processId;
        }

        /// <summary>
        ///     Current processing time
        /// </summary>
        public TimeSpan Duration { get; }

        /// <summary>
        ///     Input movie length
        /// </summary>
        public TimeSpan TotalLength { get; }

        /// <summary>
        ///     Process id
        /// </summary>
        public long ProcessId { get; }

        /// <summary>
        ///     Percent of conversion
        /// </summary>
        public int Percent => (int)(Math.Round(Duration.TotalSeconds / TotalLength.TotalSeconds, 2) * 100);
    }
}
