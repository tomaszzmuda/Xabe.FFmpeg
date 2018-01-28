// ReSharper disable InconsistentNaming

namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    ///     Audio formats ("ffmpeg -formats")
    /// </summary>
    public class AudioFormat
    {
        /// <summary>
        ///     PCM unsigned 16-bit little-endian
        /// </summary>
        public static AudioFormat u16le = new AudioFormat("u16le");

        /// <inheritdoc />
        public AudioFormat(string format)
        {
            Format = format;
        }

        /// <summary>
        ///     Audio codec
        /// </summary>
        public string Format { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Format;
        }
    }
}
