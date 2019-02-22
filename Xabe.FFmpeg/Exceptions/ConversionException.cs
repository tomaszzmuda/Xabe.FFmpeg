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
