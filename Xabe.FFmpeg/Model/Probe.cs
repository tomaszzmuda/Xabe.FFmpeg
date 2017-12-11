// ReSharper disable InconsistentNaming

namespace Xabe.FFmpeg.Model
{
    internal class ProbeModel
    {
        public Stream[] streams { get; set; }

        public class Stream
        {
            public string codec_name { get; set; }
            public int height { get; set; }
            public int width { get; set; }
            public string codec_type { get; set; }
            public string r_frame_rate { get; set; }
            public double duration { get; set; }
            public double bit_rate { get; set; }
//            NOT USED NOW BUT THIS VALUES IS STILL RETURNED IN FFPROBE OUTPUT
//            public int index { get; set; }
//            public string codec_long_name { get; set; }
//            public string profile { get; set; }
//            public string codec_time_base { get; set; }
//            public string codec_tag_string { get; set; }
//            public string codec_tag { get; set; }
//            public int coded_width { get; set; }
//            public int coded_height { get; set; }
//            public int has_b_frames { get; set; }
//            public string sample_aspect_ratio { get; set; }
//            public string display_aspect_ratio { get; set; }
//            public string pix_fmt { get; set; }
//            public int level { get; set; }
//            public string chroma_location { get; set; }
//            public int refs { get; set; }
//            public string quarter_sample { get; set; }
//            public string divx_packed { get; set; }
//            public string avg_frame_rate { get; set; }
//            public string time_base { get; set; }
//            public int start_pts { get; set; }
//            public string start_time { get; set; }
//            public int duration_ts { get; set; }
//            public string nb_frames { get; set; }
//            public Disposition disposition { get; set; }
        }

//        NOT USED NOW BUT THIS VALUES IS STILL RETURNED IN FFPROBE OUTPUT
//        [UsedImplicitly]
//        public class Disposition
//        {
//            public int _default { get; set; }
//            public int dub { get; set; }
//            public int original { get; set; }
//            public int comment { get; set; }
//            public int lyrics { get; set; }
//            public int karaoke { get; set; }
//            public int forced { get; set; }
//            public int hearing_impaired { get; set; }
//            public int visual_impaired { get; set; }
//            public int clean_effects { get; set; }
//            public int attached_pic { get; set; }
//            public int timed_thumbnails { get; set; }
//        }
    }
}
