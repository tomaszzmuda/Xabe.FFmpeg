using System.Collections.Generic;
using System.Linq;

namespace Xabe.FFmpeg
{
    internal class ConversionParameter
    {
        public ConversionParameter(string parameter, ParameterPosition position = ParameterPosition.PostInput)
        {
            Parameter = $"{parameter.Trim()} ";
            Key = parameter.Split(' ').First();
            Position = position;
        }

        public string Parameter { get; set; }
        public string Key { get; }
        public ParameterPosition Position { get; set; } = ParameterPosition.PostInput;

        public override bool Equals(object obj)
        {
            return obj is ConversionParameter parameter &&
                   Key == parameter.Key &&
                   Position == parameter.Position &&
                   Key != "-i";
        }

        public override int GetHashCode()
        {
            int hashCode = 495346454;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Key);
            hashCode = hashCode * -1521134295 + Position.GetHashCode();
            return hashCode;
        }
    }
}
