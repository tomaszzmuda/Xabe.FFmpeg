using System;
using JetBrains.Annotations;

namespace Xabe.FFmpeg.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     The exception that is thrown when user try to run second conversion simultaneously on VideoInfo/Conversion object
    /// </summary>
    public class MultipleConversionException: Exception
    {
        /// <inheritdoc />
        /// <summary>
        ///     The exception that is thrown when user try to run second conversion simultaneously on VideoInfo/Conversion object
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        /// <param name="inputParameters">FFmpeg input parameters</param>
        internal MultipleConversionException(string errorMessage, string inputParameters): base(errorMessage)
        {
            InputParameters = inputParameters;
        }

        /// <summary>
        ///     FFmpeg input parameters
        /// </summary>
        [UsedImplicitly]
        public string InputParameters { get; }
    }
}
