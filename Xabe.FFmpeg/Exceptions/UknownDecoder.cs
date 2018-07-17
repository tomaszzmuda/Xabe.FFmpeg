namespace Xabe.FFmpeg.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///      The exception that is thrown when a FFmpeg cannot find specified hardware accelerator.
    /// </summary>
    public class UnknownDecoderException : ConversionException
    {
        /// <inheritdoc />
        /// <summary>
        ///     The exception that is thrown when a FFmpeg cannot find specified hardware accelerator.
        /// </summary>
        /// <param name="errorMessage">FFmpeg error output</param>
        /// <param name="inputParameters">FFmpeg input parameters</param>
        internal UnknownDecoderException(string errorMessage, string inputParameters): base(errorMessage, inputParameters)
        {
        }
    }
}
