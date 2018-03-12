namespace Xabe.FFmpeg.Streams
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
        string GetSource();
    }
}
