using System;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg.Streams
{
    /// <summary>
    ///     Audio stream
    /// </summary>
    public interface IAudioStream : ILocalStream
    {
        /// <summary>
        ///     Duration
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        ///     Codec type
        /// </summary>
        CodecType CodecType { get; }

        /// <summary>
        ///     Bit Rate
        /// </summary>
        double Bitrate { get; set; }


        /// <summary>
        ///     Set stream to copy with orginal codec
        /// </summary>
        /// <returns>IAudioStream</returns>
        IAudioStream CopyStream();

        /// <summary>
        ///     Reverse audio stream
        /// </summary>
        /// <returns>IAudioStream</returns>
        IAudioStream Reverse();

        /// <summary>
        ///     Set audio codec and bitrate
        /// </summary>
        /// <param name="codec">Audio odec</param>
        /// <returns>IAudioStream</returns>
        IAudioStream SetCodec(AudioCodec codec);

        /// <summary>
        ///     Set filter
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <returns>IAudioStream</returns>
        IAudioStream SetBitstreamFilter(BitstreamFilter filter);

        /// <summary>
        ///     Change speed of media
        /// </summary>
        /// <param name="multiplaction">Speed value. (0.5 - 2.0). To double the speed set this to 2.0</param>
        /// <returns>IAudioStream</returns>
        /// <exception cref="ArgumentOutOfRangeException">When speed isn't between 0.5 - 2.0.</exception>
        IAudioStream ChangeSpeed(double multiplaction);

        /// <summary>
        ///     Get part of audio
        /// </summary>
        /// <param name="startTime">Start point</param>
        /// <param name="duration">Duration of new audio</param>
        /// <returns>IAudioStream</returns>
        new IAudioStream Split(TimeSpan startTime, TimeSpan duration);

        /// <summary>
        ///     Seeks in input file to position. (-ss argument)
        /// </summary>
        /// <param name="seek">Position</param>
        /// <returns>IAudioStream</returns>
        IAudioStream SetSeek(TimeSpan? seek);
        
        /// <summary>
        /// Gets Audio Codec for stream
        /// </summary>
        /// <returns></returns>
        AudioCodec GetCodec();
    }
}
