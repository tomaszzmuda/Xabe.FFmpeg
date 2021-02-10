using System;
using System.Collections.Generic;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Base stream class
    /// </summary>
    public interface IStream
    {
        /// <summary>
        ///     File source of stream
        /// </summary>
        string Path { get; }

        /// <summary>
        ///     Index of stream
        /// </summary>
        int Index { get; }

        /// <summary>
        ///     Format
        /// </summary>
        string Codec { get; }

        /// <summary>
        ///     Build FFmpeg arguments
        /// </summary>
        /// <returns>Arguments</returns>
        string Build();

        /// <summary>
        ///     Build FFmpeg arguments for input
        /// </summary>
        /// <returns>Arguments</returns>
        string BuildParameters(ParameterPosition forPosition);

        /// <summary>
        ///     Get stream input
        /// </summary>
        /// <returns>Input path</returns>
        IEnumerable<string> GetSource();

        /// <summary>
        ///     Codec type
        /// </summary>
        StreamType StreamType { get; }
    }
}
