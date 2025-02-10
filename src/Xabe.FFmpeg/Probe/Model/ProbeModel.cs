namespace Xabe.FFmpeg
{
    internal class ProbeModel
    {
        public Stream[] Streams { get; set; }

        public class Stream
        {
            public string CodecName { get; set; }

            public int Height { get; set; }

            public int Width { get; set; }

            public string CodecType { get; set; }

            public string RFrameRate { get; set; }

            public double Duration { get; set; }

            public long BitRate { get; set; }

            public int Index { get; set; }

            public int Channels { get; set; }

            public int SampleRate { get; set; }

            public string PixFmt { get; set; }

            public Tags Tags { get; set; }
            public string NbFrames { get; set; }

            public Disposition Disposition { get; set; }

            //            NOT USED NOW BUT THIS VALUES IS STILL RETURNED IN FFPROBE OUTPUT
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
        }

        internal class Tags
        {
            public string Language { get; set; }
            public string Title { get; set; }
            public int? Rotate { get; set; }
        }

        internal class Disposition
        {
            public int Default { get; set; }
            public int Forced { get; set; }

            //            NOT USED NOW BUT THIS VALUES IS STILL RETURNED IN FFPROBE OUTPUT
            //            public int dub { get; set; }
            //            public int original { get; set; }
            //            public int comment { get; set; }
            //            public int lyrics { get; set; }
            //            public int karaoke { get; set; }
            //            public int hearing_impaired { get; set; }
            //            public int visual_impaired { get; set; }
            //            public int clean_effects { get; set; }
            //            public int attached_pic { get; set; }
            //            public int timed_thumbnails { get; set; }
        }
    }
}
