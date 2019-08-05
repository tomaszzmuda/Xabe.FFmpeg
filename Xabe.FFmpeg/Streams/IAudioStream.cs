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
        ///     Sample Rate
        /// </summary>
        int SampleRate { get; set; }

        /// <summary>
        ///     Channels
        /// </summary>
        int Channels { get; set; }

        /// <summary>
        /// Language 
        /// </summary>
        string Language { get; set; }

        /// <summary>
        /// Default
        /// </summary>
        int? Default { get; set; }

        /// <summary>
        /// Forced
        /// </summary>
        int? Forced { get; set; }

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
        IAudioStream ChangeBitrate(double bitRate);

        /// <summary>
        ///     Sets the SampleRate of the AudioStream (-ar option)
        /// </summary>
        /// <param name="sampleRate">SampleRate in HZ for the Audio Stream</param>
        /// <returns>IAudioStream</returns>
        IAudioStream SetSampleRate(int sampleRate);

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
    }
}
