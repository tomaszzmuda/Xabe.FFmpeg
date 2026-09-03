using System;
using System.Globalization;

namespace Xabe.FFmpeg
{
    internal class ProbeModel
    {
        public Stream[] Streams { get; set; }

        public static Stream[] GetStreams(JsonValue document)
        {
            if (document.Type != JsonValueType.Object)
            {
                ThrowWrongShape(document);
            }

            var streams = document.Member("streams");
            if (streams == null || streams.Type == JsonValueType.Null)
            {
                return Array.Empty<Stream>();
            }

            if (streams.Type != JsonValueType.Array)
            {
                throw new JsonFormatException(string.Format(CultureInfo.InvariantCulture, "Member \"streams\" has incompatible type {0}", streams.Type));
            }

            var result = new Stream[streams.Elements.Count];
            for (var i = 0; i < streams.Elements.Count; i++)
            {
                result[i] = ReadStream(streams.Elements[i]);
            }

            return result;
        }

        private static Stream ReadStream(JsonValue element)
        {
            if (element.Type != JsonValueType.Object)
            {
                throw new JsonFormatException("Expected an object in the \"streams\" array");
            }

            return new Stream
            {
                CodecName = element.GetString("codec_name"),
                Height = RequireInt(element, "height"),
                Width = RequireInt(element, "width"),
                CodecType = element.GetString("codec_type"),
                RFrameRate = element.GetString("r_frame_rate"),
                Duration = RequireDouble(element, "duration"),
                BitRate = RequireLong(element, "bit_rate"),
                Index = RequireInt(element, "index"),
                Channels = RequireInt(element, "channels"),
                SampleRate = RequireInt(element, "sample_rate"),
                PixFmt = element.GetString("pix_fmt"),
                NbFrames = element.GetString("nb_frames"),
                Tags = ReadTags(element.Member("tags")),
                Disposition = ReadDisposition(element.Member("disposition"))
            };
        }

        private static int RequireInt(JsonValue element, string name)
        {
            RequirePresentOrNull(element, name);
            return element.GetInt(name) ?? 0;
        }

        private static long RequireLong(JsonValue element, string name)
        {
            RequirePresentOrNull(element, name);
            return element.GetLong(name) ?? 0;
        }

        private static double RequireDouble(JsonValue element, string name)
        {
            RequirePresentOrNull(element, name);
            return element.GetDouble(name) ?? 0.0;
        }

        private static void RequirePresentOrNull(JsonValue element, string name)
        {
            var value = element.Member(name);
            if (value != null && value.Type == JsonValueType.Null)
            {
                throw new JsonFormatException(string.Format(CultureInfo.InvariantCulture, "Member \"{0}\" has incompatible type {1}", name, value.Type));
            }
        }

        private static Tags ReadTags(JsonValue value)
        {
            if (value == null || value.Type == JsonValueType.Null)
            {
                return null;
            }

            if (value.Type != JsonValueType.Object)
            {
                throw new JsonFormatException(string.Format(CultureInfo.InvariantCulture, "Member \"tags\" has incompatible type {0}", value.Type));
            }

            return new Tags
            {
                Language = value.GetString("language"),
                Title = value.GetString("title"),
                Rotate = value.GetInt("rotate")
            };
        }

        private static Disposition ReadDisposition(JsonValue value)
        {
            if (value == null || value.Type == JsonValueType.Null)
            {
                return null;
            }

            if (value.Type != JsonValueType.Object)
            {
                throw new JsonFormatException(string.Format(CultureInfo.InvariantCulture, "Member \"disposition\" has incompatible type {0}", value.Type));
            }

            return new Disposition
            {
                Default = RequireInt(value, "default"),
                Forced = RequireInt(value, "forced")
            };
        }

        private static void ThrowWrongShape(JsonValue document)
        {
            throw new JsonFormatException(string.Format(CultureInfo.InvariantCulture, "Expected a JSON object, found {0}", document.Type));
        }

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
