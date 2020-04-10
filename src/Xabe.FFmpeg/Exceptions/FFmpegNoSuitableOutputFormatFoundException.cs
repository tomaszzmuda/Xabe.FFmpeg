using System.IO;

namespace Xabe.FFmpeg.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     The exception that is thrown when a FFmpeg process cannot find suitable output format.
    /// </summary>
    public class FFmpegNoSuitableOutputFormatFoundException : ConversionException
    {
        /// <inheritdoc />
        /// <summary>
        ///     The exception that is thrown when a FFmpeg process cannot find suitable output format.
        /// </summary>
        /// <param name="errorMessage">FFmpeg error output</param>
        internal FFmpegNoSuitableOutputFormatFoundException(string errorMessage, string inputParameters) : base(errorMessage, inputParameters)
        {
        }
    }
}
