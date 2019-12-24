// ReSharper disable InconsistentNaming

namespace Xabe.FFmpeg.Model
{
    internal class FormatModel
    {
        private FormatModel()
        {
        }

        internal class Root
        {
            public Format format { get; set; }
        }

        internal class Format
        {
            public string size { get; set; }

            public double bit_Rate { get; set; }

            public double duration { get; set; }
        }
    }
}
