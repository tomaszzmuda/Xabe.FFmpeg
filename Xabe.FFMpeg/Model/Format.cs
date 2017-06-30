namespace Xabe.FFMpeg.Model
{
    internal class FormatModel
    {
        private FormatModel()
        {
        }

        internal class Root
        {
            public Format Format { get; set; }
        }

        internal class Format
        {
            public string Filename { get; set; }
            public int NbStreams { get; set; }
            public int NbPrograms { get; set; }
            public string FormatName { get; set; }
            public string FormatLongName { get; set; }
            public string StartTime { get; set; }
            public double Duration { get; set; }
            public string Size { get; set; }
            public double BitRate { get; set; }
            public int ProbeScore { get; set; }
            public Tags Tags { get; set; }
        }

        internal class Tags
        {
            public string Encoder { get; set; }
            public string CreationTime { get; set; }
        }
    }
}
