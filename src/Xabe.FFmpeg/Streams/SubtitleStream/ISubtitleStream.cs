using System;
using Xabe.FFmpeg.Streams.SubtitleStream;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Subtitle stream
    /// </summary>
    public interface ISubtitleStream : IStream
    {
        /// <summary>
        ///     Subtitle language
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
        /// Title
        /// </summary>
        string Title { get; }

        /// <summary>
        ///     Set subtitle language
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>ISubtitleStream</returns>
        ISubtitleStream SetLanguage(string language);

        /// <summary>
        ///     Set subtitle codec
        /// </summary>
        /// <param name="codec">Subtitle codec</param>
        /// <returns>ISubtitleStream</returns>
        ISubtitleStream SetCodec(SubtitleCodec codec);

        /// <summary>
        ///     Set Subtitle codec
        /// </summary>
        /// <param name="codec">Subtitle codec</param>
        /// <returns>IVideoStream</returns>
        ISubtitleStream SetCodec(string codec);

        /// <summary>
        ///     "-re" parameter.  Read input at native frame rate. Mainly used to simulate a grab device, or live input stream (e.g. when reading from a file). Should not be used with actual grab devices or live input streams (where it can cause packet loss). By default ffmpeg attempts to read the input(s) as fast as possible. This option will slow down the reading of the input(s) to the native frame rate of the input(s). It is useful for real-time output (e.g. live streaming).
        /// </summary>
        /// <param name="readInputAtNativeFrameRate">Read input at native frame rate. False set parameter to default value.</param>
        /// <returns>IConversion object</returns>
        ISubtitleStream UseNativeInputRead(bool readInputAtNativeFrameRate);

        /// <summary>
        ///     "-stream_loop" parameter. Set number of times input stream shall be looped. 
        /// </summary>
        /// <param name="loopCount">Loop 0 means no loop, loop -1 means infinite loop.</param>
        /// <returns>IConversion object</returns>
        ISubtitleStream SetStreamLoop(int loopCount);
    }
}
