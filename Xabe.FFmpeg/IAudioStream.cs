using System;
using System.IO;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg
{
    public interface IAudioStream
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

        /// <summary>
        ///     Video format
        /// </summary>
        string Format { get; }

        /// <summary>
        /// File source of stream
        /// </summary>
        FileInfo Source { get; }

        /// <summary>
        ///     Build FFmpeg arguments
        /// </summary>
        /// <returns>Arguments</returns>
        string Build();
    }
}
