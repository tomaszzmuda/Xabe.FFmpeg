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
        /// Title
        /// </summary>
        string Title { get; }

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

        /// <summary>
        /// Sets the format for the input file using the -f option before the input file name
        /// </summary>
        /// <param name="inputFormat">The input format to set</param>
        /// <returns>IConversion object</returns>
        IAudioStream SetInputFormat(string inputFormat);

        /// <summary>
        /// Sets the format for the input file using the -f option before the input file name
        /// </summary>
        /// <param name="inputFormat">The input format to set</param>
        /// <returns>IConversion object</returns>
        IAudioStream SetInputFormat(Format inputFormat);

        /// <summary>
        ///     "-re" parameter. Read input at native frame rate. Mainly used to simulate a grab device, or live input stream (e.g. when reading from a file). Should not be used with actual grab devices or live input streams (where it can cause packet loss). By default ffmpeg attempts to read the input(s) as fast as possible. This option will slow down the reading of the input(s) to the native frame rate of the input(s). It is useful for real-time output (e.g. live streaming).
        /// </summary>
        /// <param name="readInputAtNativeFrameRate">Read input at native frame rate. False set parameter to default value.</param>
        /// <returns>IConversion object</returns>
        IAudioStream UseNativeInputRead(bool readInputAtNativeFrameRate);

        /// <summary>
        ///     "-stream_loop" parameter. Set number of times input stream shall be looped. 
        /// </summary>
        /// <param name="loopCount">Loop 0 means no loop, loop -1 means infinite loop.</param>
        /// <returns>IConversion object</returns>
        IAudioStream SetStreamLoop(int loopCount);
    }
}
