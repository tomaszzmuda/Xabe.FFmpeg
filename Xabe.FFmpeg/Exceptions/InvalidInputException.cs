using System.IO;

namespace Xabe.FFmpeg.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     The exception that is thrown when input does not exists.
    /// </summary>
    public class InvalidInputException: FileNotFoundException
    {
        /// <summary>
        ///     The exception that is thrown when input does not exists.
        /// </summary>
        /// <param name="msg"></param>
        public InvalidInputException(string msg): base(msg)
        {
        }
    }
}
