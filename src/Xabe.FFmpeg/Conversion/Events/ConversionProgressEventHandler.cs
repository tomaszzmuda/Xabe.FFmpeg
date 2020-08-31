using System;

namespace Xabe.FFmpeg.Events
{
    /// <summary>
    ///     Info about conversion progress
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="args">Conversion info</param>
    public delegate void ConversionProgressEventHandler(object sender, ConversionProgressEventArgs args);

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
