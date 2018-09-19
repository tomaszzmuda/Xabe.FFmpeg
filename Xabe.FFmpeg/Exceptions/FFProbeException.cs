namespace Xabe.FFmpeg.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     The exception that is thrown when a FFProbe process return error.
    /// </summary>
    public class FFprobeException : ConversionException
    {
        /// <inheritdoc />
        internal FFprobeException(string errorMessage, string inputParameters): base(errorMessage, inputParameters)
        {

        }
    }
}
