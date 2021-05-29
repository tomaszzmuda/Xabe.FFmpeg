using System;

namespace Xabe.FFmpeg.Events
{
    /// <summary>
    ///     Info about conversion progress
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="args">Video data</param>
    public delegate void VideoDataEventHandler(object sender, VideoDataEventArgs args);

    /// <summary>
    ///     Video data
    /// </summary>
    public class VideoDataEventArgs : EventArgs
    {
        /// <inheritdoc />
        public VideoDataEventArgs(byte[] data)
        {
            Data = data;
        }

        /// <summary>
        ///     Binary video data
        /// </summary>
        public byte[] Data;
    }
}
