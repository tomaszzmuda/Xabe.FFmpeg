// ReSharper disable InconsistentNaming

namespace Xabe.FFmpeg
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

            public long bit_Rate { get; set; }

            public double duration { get; set; }
        }
    }
}
