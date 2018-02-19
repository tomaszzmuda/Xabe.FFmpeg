namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    ///     Video codec ("ffmpeg -codecs")
    /// </summary>
    public interface IVideoCodec
    {
        /// <summary>
        ///     Video codec
        /// </summary>
        string Codec { get; }

        /// <summary>
        ///     Convert to string format
        /// </summary>
        /// <returns>Codec string</returns>
        string ToString();
    }
}
