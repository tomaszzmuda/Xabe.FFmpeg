namespace Xabe.FFmpeg.Downloader
{
    /// <summary>
    ///     A named member of a JSON object value, in document order.
    /// </summary>
    internal struct JsonMember
    {
        public string Name { get; set; }

        public JsonValue Value { get; set; }
    }
}
