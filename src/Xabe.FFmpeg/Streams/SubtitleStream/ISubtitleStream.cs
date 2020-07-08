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
    }
}
