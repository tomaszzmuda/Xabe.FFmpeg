namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    ///     Audio codec ("ffmpeg -codecs")
    /// </summary>
    public interface IAudioCodec
    {
        /// <summary>
        ///     Audio codec
        /// </summary>
        string Codec { get; }

        /// <summary>
        ///     Format to string
        /// </summary>
        /// <returns>Codec as string</returns>
        string ToString();
    }
}
