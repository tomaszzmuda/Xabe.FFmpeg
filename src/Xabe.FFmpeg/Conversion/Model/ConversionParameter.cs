namespace Xabe.FFmpeg
{
    internal class ConversionParameter
    {
        public string Parameter { get; set; }
        public ParameterPosition Position { get; set; } = ParameterPosition.PostInput;
    }
}
