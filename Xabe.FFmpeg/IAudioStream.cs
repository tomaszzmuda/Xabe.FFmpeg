using System;
using System.IO;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg
{
    public interface IAudioStream : IStream
    {
        IAudioStream Reverse();

        /// <summary>
        ///     Set audio codec and bitrate
        /// </summary>
        /// <param name="codec">Audio odec</param>
        /// <param name="bitrate">Audio bitrade</param>
        /// <returns>IConversion object</returns>
        IAudioStream SetAudio(AudioCodec codec, AudioQuality bitrate);

        /// <summary>
        ///     Set audio codec and bitrate
        /// </summary>
        /// <param name="codec">Audio odec</param>
        /// <param name="bitrate">Audio bitrade</param>
        /// <returns>IConversion object</returns>
        IAudioStream SetAudio(string codec, AudioQuality bitrate);

        /// <summary>
        ///     Duration
        /// </summary>
        TimeSpan Duration { get; }
    }
}
