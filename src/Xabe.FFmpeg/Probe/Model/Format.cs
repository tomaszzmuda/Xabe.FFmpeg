
using System.Globalization;

#pragma warning disable IDE1006 // Naming Styles

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

            public static Root FromJson(JsonValue document)
            {
                if (document.Type != JsonValueType.Object)
                {
                    throw new JsonFormatException(string.Format(CultureInfo.InvariantCulture, "Expected a JSON object, found {0}", document.Type));
                }

                var format = document.Member("format");
                if (format == null || format.Type == JsonValueType.Null)
                {
                    throw new JsonFormatException("Missing required member \"format\"");
                }

                if (format.Type != JsonValueType.Object)
                {
                    throw new JsonFormatException(string.Format(CultureInfo.InvariantCulture, "Member \"format\" has incompatible type {0}", format.Type));
                }

                var tags = format.Member("tags");
                Tags formatTags = null;
                if (tags != null && tags.Type != JsonValueType.Null)
                {
                    if (tags.Type != JsonValueType.Object)
                    {
                        throw new JsonFormatException(string.Format(CultureInfo.InvariantCulture, "Member \"format.tags\" has incompatible type {0}", tags.Type));
                    }

                    formatTags = new Tags
                    {
                        creation_time = tags.GetString("creation_time")
                    };
                }

                return new Root
                {
                    format = new Format
                    {
                        size = format.GetString("size"),
                        bit_Rate = RequireLong(format, "bit_rate"),
                        duration = RequireDouble(format, "duration"),
                        tags = formatTags
                    }
                };
            }
        }

        private static long RequireLong(JsonValue format, string name)
        {
            RequirePresentOrNull(format, name);
            return format.GetLong(name) ?? 0;
        }

        private static double RequireDouble(JsonValue format, string name)
        {
            RequirePresentOrNull(format, name);
            return format.GetDouble(name) ?? 0.0;
        }

        private static void RequirePresentOrNull(JsonValue format, string name)
        {
            var value = format.Member(name);
            if (value != null && value.Type == JsonValueType.Null)
            {
                throw new JsonFormatException(string.Format(CultureInfo.InvariantCulture, "Member \"{0}\" has incompatible type {1}", name, value.Type));
            }
        }

        internal class Tags
        {
            public string creation_time { get; set; }
        }

        internal class Format
        {
            public string size { get; set; }

            public long bit_Rate { get; set; }

            public double duration { get; set; }

            public Tags tags { get; set; }
        }
    }
}
#pragma warning restore IDE1006 // Naming Styles
