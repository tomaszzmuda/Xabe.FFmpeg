using System.Collections.Generic;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Base stream class
    /// </summary>
    public interface IStream
    {
        /// <summary>
        ///     Build FFmpeg arguments
        /// </summary>
        /// <returns>Arguments</returns>
        string Build();

        /// <summary>
        ///     Build FFmpeg arguments for input
        /// </summary>
        /// <returns>Arguments</returns>
        string BuildInputArguments();

        /// <summary>
        ///     Index of stream
        /// </summary>
        int Index { get; }

        /// <summary>
        ///     Format
        /// </summary>
        string Format { get; }

        /// <summary>
        ///     Get stream input
        /// </summary>
        /// <returns>Input path</returns>
        IEnumerable<string> GetSource();
    }
}
