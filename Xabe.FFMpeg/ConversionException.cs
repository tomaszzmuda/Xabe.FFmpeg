using System;
using JetBrains.Annotations;

namespace Xabe.FFMpeg
{
    /// <inheritdoc />
    /// <summary>
    ///     The exception that is thrown when a ffmpeg process return error.
    /// </summary>
    public class ConversionException : Exception
    {
        /// <summary>
        ///     FFMpeg output
        /// </summary>
        [UsedImplicitly]
        public string ErrorMessage { get; }

        /// <summary>
        ///     FFMpeg input parameters
        /// </summary>
        [UsedImplicitly]
        public string InputParameters { get; }

        /// <inheritdoc />
        /// <summary>
        ///     The exception that is thrown when a ffmpeg process return error.
        /// </summary>
        /// <param name="errorMessage">FFMpeg error output</param>
        /// <param name="inputParameters">FFMPeg input parameters</param>
        internal ConversionException(string errorMessage, string inputParameters)
        {
            ErrorMessage = errorMessage;
            InputParameters = inputParameters;
        }
    }
}
