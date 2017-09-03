using System;
using JetBrains.Annotations;

namespace Xabe.FFMpeg.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     The exception that is thrown when a ffmpeg process return error.
    /// </summary>
    public class ConversionException: Exception
    {
        /// <inheritdoc />
        /// <summary>
        ///     The exception that is thrown when a ffmpeg process return error.
        /// </summary>
        /// <param name="errorMessage">FFMpeg error output</param>
        /// <param name="inputParameters">FFMPeg input parameters</param>
        internal ConversionException(string errorMessage, string inputParameters): base(errorMessage)
        {
            InputParameters = inputParameters;
        }

        /// <summary>
        ///     FFMpeg input parameters
        /// </summary>
        [UsedImplicitly]
        public string InputParameters { get; }
    }
}
