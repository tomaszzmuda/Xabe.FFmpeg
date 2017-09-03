using System;
using JetBrains.Annotations;

namespace Xabe.FFMpeg.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     The exception that is thrown when user try to run second conversion simultaneously on VideoInfo/Conversion object
    /// </summary>
    public class MultipleConversionException : Exception
    {
        /// <summary>
        ///     FFMpeg input parameters
        /// </summary>
        [UsedImplicitly]
        public string InputParameters { get; }

        /// <inheritdoc />
        /// <summary>
        ///     The exception that is thrown when user try to run second conversion simultaneously on VideoInfo/Conversion object
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        /// <param name="inputParameters">FFMPeg input parameters</param>
        internal MultipleConversionException(string errorMessage, string inputParameters) : base(errorMessage)
        {
            InputParameters = inputParameters;
        }
    }
}
