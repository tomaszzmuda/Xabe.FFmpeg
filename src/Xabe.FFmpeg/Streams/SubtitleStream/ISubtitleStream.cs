using System;

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
    }
}
