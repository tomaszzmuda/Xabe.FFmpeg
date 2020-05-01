using System;

namespace Xabe.FFmpeg.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     The exception that is thrown when a FFmpeg process return error.
    /// </summary>
    public class ConversionException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        ///     The exception that is thrown when a FFmpeg process return error.
        /// </summary>
        /// <param name="message">FFmpeg error output</param>
        /// <param name="inputParameters">FFmpeg input parameters</param>
        /// <param name="innerException">Inner exception</param>
        public ConversionException(string message, Exception innerException, string inputParameters) : base(message, innerException)
        {
            InputParameters = inputParameters;
        }

        /// <inheritdoc />
        /// <summary>
        ///     The exception that is thrown when a FFmpeg process return error.
        /// </summary>
        /// <param name="errorMessage">FFmpeg error output</param>
        /// <param name="inputParameters">FFmpeg input parameters</param>
        internal ConversionException(string errorMessage, string inputParameters) : base(errorMessage)
        {
            InputParameters = inputParameters;
        }

        /// <summary>
        ///     FFmpeg input parameters
        /// </summary>
        public string InputParameters { get; }
    }
}
