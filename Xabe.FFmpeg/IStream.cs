using System.IO;

namespace Xabe.FFmpeg
{
    public interface IStream
    {
        /// <summary>
        ///     Format
        /// </summary>
        string Format { get; }

        /// <summary>
        ///     File source of stream
        /// </summary>
        FileInfo Source { get; }

        /// <summary>
        ///     Index of stream
        /// </summary>
        int Index { get; }

        /// <summary>
        ///     Build FFmpeg arguments
        /// </summary>
        /// <returns>Arguments</returns>
        string Build();
    }
}
