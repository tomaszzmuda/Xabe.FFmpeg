namespace Xabe.FFmpeg.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     The exception that is thrown when a FFmpeg process cannot find suitable output format.
    /// </summary>
    public class InvalidBitstreamFilterException : ConversionException
    {
        /// <inheritdoc />
        /// <summary>
        ///     The exception that is thrown when a FFmpeg process cannot find suitable output format.
        /// </summary>
        /// <param name="errorMessage">FFmpeg error output</param>
        /// <param name="inputParameters">FFmpeg error output</param>
        internal InvalidBitstreamFilterException(string errorMessage, string inputParameters) : base(errorMessage, inputParameters)
        {
        }
    }
}
