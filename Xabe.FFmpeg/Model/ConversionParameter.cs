using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg.Model
{
    internal class ConversionParameter
    {
        public string Parameter { get; set; }
        public ParameterPosition Position { get; set; } = ParameterPosition.PostInput;
    }
}
