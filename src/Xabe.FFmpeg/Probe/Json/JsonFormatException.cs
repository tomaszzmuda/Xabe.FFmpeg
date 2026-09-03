using System;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Raised by the internal JSON document reader when the input is not valid JSON
    ///     or a value has the wrong shape for the requested member. Carries no payload
    ///     fragments: messages contain offsets and single token characters only.
    /// </summary>
    internal class JsonFormatException : Exception
    {
        public JsonFormatException(string message) : base(message)
        {
        }
    }
}
