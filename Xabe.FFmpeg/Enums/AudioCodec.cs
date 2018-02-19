namespace Xabe.FFmpeg.Enums
{
    /// <inheritdoc />
    public class AudioCodec: IAudioCodec
    {
        /// <summary>
        ///     AAC (Advanced Audio Coding) (decoders: aac aac_fixed ) (encoders: aac libvo_aacenc )
        /// </summary>
        public static readonly AudioCodec Aac = new AudioCodec("aac");

        /// <summary>
        ///     AC3 (Dolby Digital audio codec)
        /// </summary>
        public static readonly AudioCodec Ac3 = new AudioCodec("ac3");

        /// <summary>
        ///     Vorbis (decoders: vorbis libvorbis ) (encoders: vorbis libvorbis )
        /// </summary>
        public static readonly AudioCodec Libvorbis = new AudioCodec("libvorbis");

        /// <inheritdoc />
        public AudioCodec(string codec)
        {
            Codec = codec;
        }

        /// <inheritdoc />
        public string Codec { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Codec;
        }
    }
}
