using System.Globalization;
using System.Text;

namespace Xabe.FFmpeg.Downloader
{
    internal class BaseBinaries
    {
        public string Ffmpeg { get; set; }

        public string Ffplay { get; set; }

        public string Ffprobe { get; set; }
    }

    internal class Windows32 : BaseBinaries
    {
    }

    internal class Windows64 : BaseBinaries
    {
    }

    internal class Linux32 : BaseBinaries
    {
    }

    internal class Linux64 : BaseBinaries
    {
    }

    internal class LinuxArmhf : BaseBinaries
    {
    }

    internal class LinuxArmel : BaseBinaries
    {
    }

    internal class LinuxArm64 : BaseBinaries
    {
    }

    internal class Osx64
    {
        public string Ffmpeg { get; set; }
        public string Ffplay { get; set; }
        public string Ffprobe { get; set; }
    }

    internal class Bin
    {
        public Windows32 Windows32 { get; set; }

        public Windows64 Windows64 { get; set; }

        public Linux32 Linux32 { get; set; }

        public Linux64 Linux64 { get; set; }

        public LinuxArmhf LinuxArmhf { get; set; }

        public LinuxArmel LinuxArmel { get; set; }

        public LinuxArm64 LinuxArm64 { get; set; }

        public Osx64 Osx64 { get; set; }
    }

    internal class Links
    {
        public string FFmpegLink { get; set; }
        public string FFprobeLink { get; set; }
    }

    internal class FFbinariesVersionInfo
    {
        public string Version { get; set; }

        public Bin BinariesUrl { get; set; }

        public static FFbinariesVersionInfo FromManifest(JsonValue document)
        {
            EnsureObject(document);

            var result = new FFbinariesVersionInfo
            {
                Version = ReadString(document, "version")
            };

            var bin = document.Member("bin");
            if (bin != null && bin.Type != JsonValueType.Null)
            {
                if (bin.Type != JsonValueType.Object)
                {
                    ThrowWrongShape(bin, "bin");
                }

                result.BinariesUrl = new Bin
                {
                    Windows32 = ReadTools<Windows32>(bin, "windows-32"),
                    Windows64 = ReadTools<Windows64>(bin, "windows-64"),
                    Linux32 = ReadTools<Linux32>(bin, "linux-32"),
                    Linux64 = ReadTools<Linux64>(bin, "linux-64"),
                    LinuxArmhf = ReadTools<LinuxArmhf>(bin, "linux-armhf"),
                    LinuxArmel = ReadTools<LinuxArmel>(bin, "linux-armel"),
                    LinuxArm64 = ReadTools<LinuxArm64>(bin, "linux-arm64"),
                    Osx64 = ReadOsx(bin, "osx-64")
                };
            }

            return result;
        }

        public static string ReadSavedVersion(JsonValue document)
        {
            EnsureObject(document);
            return ReadString(document, "version");
        }

        public static string RenderSavedVersion(string version)
        {
            var builder = new StringBuilder();
            builder.Append("{\n  \"version\": \"");
            AppendEscaped(builder, version);
            builder.Append("\"\n}");
            return builder.ToString();
        }

        private static T ReadTools<T>(JsonValue bin, string name) where T : BaseBinaries, new()
        {
            var entry = bin.Member(name);
            if (entry == null || entry.Type == JsonValueType.Null)
            {
                return null;
            }

            if (entry.Type != JsonValueType.Object)
            {
                ThrowWrongShape(entry, "bin." + name);
            }

            return new T
            {
                Ffmpeg = entry.GetString("ffmpeg"),
                Ffplay = entry.GetString("ffplay"),
                Ffprobe = entry.GetString("ffprobe")
            };
        }

        private static Osx64 ReadOsx(JsonValue bin, string name)
        {
            var entry = bin.Member(name);
            if (entry == null || entry.Type == JsonValueType.Null)
            {
                return null;
            }

            if (entry.Type != JsonValueType.Object)
            {
                ThrowWrongShape(entry, "bin." + name);
            }

            return new Osx64
            {
                Ffmpeg = entry.GetString("ffmpeg"),
                Ffplay = entry.GetString("ffplay"),
                Ffprobe = entry.GetString("ffprobe")
            };
        }

        private static string ReadString(JsonValue document, string name)
        {
            var value = document.Member(name);
            if (value == null || value.Type == JsonValueType.Null)
            {
                return null;
            }

            if (value.Type != JsonValueType.String)
            {
                ThrowWrongShape(value, name);
            }

            return value.Scalar;
        }

        private static void EnsureObject(JsonValue document)
        {
            if (document.Type != JsonValueType.Object)
            {
                throw new JsonFormatException(string.Format(CultureInfo.InvariantCulture, "Expected a JSON object, found {0}", document.Type));
            }
        }

        private static void ThrowWrongShape(JsonValue value, string name)
        {
            throw new JsonFormatException(string.Format(CultureInfo.InvariantCulture, "Member \"{0}\" has incompatible type {1}", name, value.Type));
        }

        private static void AppendEscaped(StringBuilder builder, string value)
        {
            for (var i = 0; i < value.Length; i++)
            {
                var ch = value[i];
                if (ch == '\\')
                {
                    builder.Append("\\\\");
                }
                else if (ch == '"')
                {
                    builder.Append("\\u0022");
                }
                else if (ch < 0x20)
                {
                    switch (ch)
                    {
                        case '\b':
                            builder.Append("\\b");
                            break;
                        case '\f':
                            builder.Append("\\f");
                            break;
                        case '\n':
                            builder.Append("\\n");
                            break;
                        case '\r':
                            builder.Append("\\r");
                            break;
                        case '\t':
                            builder.Append("\\t");
                            break;
                        default:
                            builder.Append("\\u").Append(((int)ch).ToString("X4", CultureInfo.InvariantCulture));
                            break;
                    }
                }
                else if (ch == 0x7f || ch == '&' || ch == '\'' || ch == '+' || ch == '<' || ch == '>' || ch == '`' || ch > 0x7e)
                {
                    builder.Append("\\u").Append(((int)ch).ToString("X4", CultureInfo.InvariantCulture));
                }
                else
                {
                    builder.Append(ch);
                }
            }
        }
    }
}
