using System;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Audio stream
    /// </summary>
    public interface IAudioStream : IStream
    {
        /// <summary>
        ///     Duration
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        ///     Bitrate
        /// </summary>
        long Bitrate { get; }

        /// <summary>
        ///     Sample Rate
        /// </summary>
        int SampleRate { get; }

        /// <summary>
        ///     Channels
        /// </summary>
        int Channels { get; }

        /// <summary>
        /// Language 
        /// </summary>
        string Language { get; }

        /// <summary>
        /// Default
        /// </summary>
        int? Default { get; }

        /// <summary>
        /// Forced
        /// </summary>
        int? Forced { get; }

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
        ///     Set audio Channels (-ac option)
        /// </summary>
        /// <param name="channels">Number of channels to use in the output stream</param>
        /// <returns>IAudioStream</returns>
        IAudioStream SetChannels(int channels);

        /// <summary>
        ///     Set audio codec and bitrate
        /// </summary>
        /// <param name="codec">Audio odec</param>
        /// <returns>IAudioStream</returns>
        IAudioStream SetCodec(AudioCodec codec);

        /// <summary>
        ///     Set audio codec and bitrate
        /// </summary>
        /// <param name="codec">Audio odec</param>
        /// <returns>IAudioStream</returns>
        IAudioStream SetCodec(string codec);

        /// <summary>
        ///     Set filter
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <returns>IAudioStream</returns>
        IAudioStream SetBitstreamFilter(BitstreamFilter filter);

        /// <summary>
        ///     Sets the Bitrate of the AudioStream
        /// </summary>
        /// <param name="bitRate">Bitrate for the AudioStream in bytes</param>
        /// <returns>IAudioStream</returns>
        IAudioStream SetBitrate(long bitRate);

        /// <summary>
        ///     Set Bitrate of the AudioStream
        /// </summary>
        /// <param name="minBitrate">Bitrate in bits</param>
        /// <param name="maxBitrate">Bitrate in bits</param>
        /// <param name="buffersize">Buffersize in bits</param>
        /// <returns>IAudioStream</returns>
        IAudioStream SetBitrate(long minBitrate, long maxBitrate, long bufferSize);

        /// <summary>
        ///     Sets the SampleRate of the AudioStream (-ar option)
        /// </summary>
        /// <param name="sampleRate">SampleRate in HZ for the Audio Stream</param>
        /// <returns>IAudioStream</returns>
        IAudioStream SetSampleRate(int sampleRate);

        /// <summary>
        ///     Change speed of media
        /// </summary>
        /// <param name="multiplicator">Speed value. (0.5 - 2.0). To double the speed set this to 2.0</param>
        /// <returns>IAudioStream</returns>
        /// <exception cref="ArgumentOutOfRangeException">When speed isn't between 0.5 - 2.0.</exception>
        IAudioStream ChangeSpeed(double multiplicator);

        /// <summary>
        ///     Get part of audio
        /// </summary>
        /// <param name="startTime">Start point</param>
        /// <param name="duration">Duration of new audio</param>
        /// <returns>IAudioStream</returns>
        IAudioStream Split(TimeSpan startTime, TimeSpan duration);

        /// <summary>
        ///     Seeks in input file to position. (-ss argument)
        /// </summary>
        /// <param name="seek">Position</param>
        /// <returns>IAudioStream</returns>
        IAudioStream SetSeek(TimeSpan? seek);

        /// <summary>
        ///     Set filter
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <returns>IAudioStream</returns>
        IAudioStream SetBitstreamFilter(string filter);
    }
}
