namespace Xabe.FFMpeg.Model
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
            public string filename { get; set; }
            public int nbStreams { get; set; }
            public int nbPrograms { get; set; }
            public string formatName { get; set; }
            public string formatLongName { get; set; }
            public string startTime { get; set; }
            public double duration { get; set; }
            public string size { get; set; }
            public double bitRate { get; set; }
            public int probeScore { get; set; }
            public Tags tags { get; set; }
        }

        internal class Tags
        {
            public string encoder { get; set; }
            public string creationTime { get; set; }
        }
    }
}
