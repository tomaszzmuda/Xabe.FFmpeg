namespace Xabe.FFmpeg.Downloader
{
    /// <summary>
    ///     The kind of a parsed JSON value.
    /// </summary>
    internal enum JsonValueType
    {
        Object,
        Array,
        String,
        Number,
        Boolean,
        Null
    }
}
