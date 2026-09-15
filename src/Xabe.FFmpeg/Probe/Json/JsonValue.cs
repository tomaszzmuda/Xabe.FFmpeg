using System.Collections.Generic;
using System.Globalization;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     An immutable parsed JSON value. Objects keep their document order; unknown members are kept
    ///     as well so callers can deliberately ignore them. Numeric members store the raw JSON number text
    ///     and are converted lazily, accepting both JSON numbers and quoted numeric strings, mirroring the
    ///     reader behaviour this replaces.
    /// </summary>
    internal sealed class JsonValue
    {
        private static JsonValue NullInstance { get; } = new JsonValue(JsonValueType.Null);

        private JsonValue(JsonValueType type)
        {
            Type = type;
        }

        public JsonValueType Type { get; }

        public string Scalar { get; private set; }

        public IReadOnlyList<JsonMember> Members { get; private set; }

        public IReadOnlyList<JsonValue> Elements { get; private set; }

        public static JsonValue Object(IReadOnlyList<JsonMember> members)
        {
            var value = new JsonValue(JsonValueType.Object) { Members = members };
            return value;
        }

        public static JsonValue Array(IReadOnlyList<JsonValue> elements)
        {
            return new JsonValue(JsonValueType.Array) { Elements = elements };
        }

        public static JsonValue String(string scalar)
        {
            return new JsonValue(JsonValueType.String) { Scalar = scalar };
        }

        public static JsonValue Number(string raw)
        {
            return new JsonValue(JsonValueType.Number) { Scalar = raw };
        }

        public static JsonValue Boolean(bool scalar)
        {
            return new JsonValue(JsonValueType.Boolean) { Scalar = scalar ? "true" : "false" };
        }

        public static JsonValue Null
        {
            get
            {
                return NullInstance;
            }
        }

        public JsonValue Member(string name)
        {
            if (Type != JsonValueType.Object || Members == null)
            {
                return null;
            }

            for (var i = 0; i < Members.Count; i++)
            {
                if (Members[i].Name == name)
                {
                    return Members[i].Value;
                }
            }

            return null;
        }

        public string GetString(string name)
        {
            var value = RequireScalar(name, JsonValueType.String);
            return value?.Scalar;
        }

        public int? GetInt(string name)
        {
            var value = RequireNumeric(name);
            if (value == null)
            {
                return null;
            }

            if (!int.TryParse(value.Scalar, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var result))
            {
                ThrowWrongShape(name, value);
            }

            return result;
        }

        public long? GetLong(string name)
        {
            var value = RequireNumeric(name);
            if (value == null)
            {
                return null;
            }

            if (!long.TryParse(value.Scalar, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var result))
            {
                ThrowWrongShape(name, value);
            }

            return result;
        }

        public double? GetDouble(string name)
        {
            var value = RequireNumeric(name);
            if (value == null)
            {
                return null;
            }

            if (!double.TryParse(value.Scalar, NumberStyles.Float | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out var result))
            {
                ThrowWrongShape(name, value);
            }

            return result;
        }

        private JsonValue RequireScalar(string name, JsonValueType expected)
        {
            var value = Member(name);
            if (value == null || value.Type == JsonValueType.Null)
            {
                return null;
            }

            if (value.Type != expected)
            {
                ThrowWrongShape(name, value);
            }

            return value;
        }

        private JsonValue RequireNumeric(string name)
        {
            var value = Member(name);
            if (value == null || value.Type == JsonValueType.Null)
            {
                return null;
            }

            if (value.Type != JsonValueType.Number && value.Type != JsonValueType.String)
            {
                ThrowWrongShape(name, value);
            }

            return value;
        }

        private static void ThrowWrongShape(string name, JsonValue value)
        {
            throw new JsonFormatException(string.Format(CultureInfo.InvariantCulture, "Member \"{0}\" has incompatible type {1}", name, value.Type));
        }
    }
}
