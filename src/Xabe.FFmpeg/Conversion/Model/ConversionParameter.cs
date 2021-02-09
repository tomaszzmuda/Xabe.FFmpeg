namespace Xabe.FFmpeg
{
    internal class ConversionParameter
    {
        public ConversionParameter(string parameter, ParameterPosition position = ParameterPosition.PostInput)
        {
            Parameter = $"{parameter.Trim()} ";
            Position = position;
        }

        public string Parameter { get; set; }
        public ParameterPosition Position { get; set; } = ParameterPosition.PostInput;
    }
}
