namespace Xabe.FFMpeg.Model
{
    internal class FormatModel
    {
        private FormatModel()
        {
        }

        public class Root
        {
            public Format format { get; set; }
        }

        public class Format
        {
            public string filename { get; set; }
            public int nb_streams { get; set; }
            public int nb_programs { get; set; }
            public string format_name { get; set; }
            public string format_long_name { get; set; }
            public string start_time { get; set; }
            public double duration { get; set; }
            public string size { get; set; }
            public double bit_rate { get; set; }
            public int probe_score { get; set; }
            public Tags tags { get; set; }
        }

        public class Tags
        {
            public string encoder { get; set; }
            public string creation_time { get; set; }
        }
    }
}
