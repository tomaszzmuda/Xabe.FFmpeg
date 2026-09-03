using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Xabe.FFmpeg.Downloader
{
    /// <summary>
    ///     Dependency-free reader for the small, fixed JSON payloads this assembly consumes
    ///     (the ffbinaries manifest and the stored version.json). Deliberately duplicated from
    ///     Xabe.FFmpeg's probe JSON boundary so this assembly stays dependency-free on its own.
    ///     Produces a <see cref="JsonValue"/> tree. Structural failures surface as
    ///     <see cref="InvalidDataException"/> with the originating parser exception as
    ///     <see cref="Exception.InnerException"/>; messages carry a stable context label and
    ///     positions only, never payload content.
    /// </summary>
    internal static class JsonDocument
    {
        private const int MAX_NESTING_DEPTH = 64;

        public static JsonValue Parse(string json, string sourceLabel)
        {
            return Map(json, sourceLabel, document => document);
        }

        /// <summary>
        ///     Parses <paramref name="json"/> and applies <paramref name="mapper"/> to the resulting
        ///     document. Parsing failures and shape mismatches raised by the mapper both surface here as
        ///     <see cref="InvalidDataException"/> — the single outward-facing failure for malformed input.
        /// </summary>
        public static T Map<T>(string json, string sourceLabel, Func<JsonValue, T> mapper)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            if (json.StartsWith("\uFEFF", StringComparison.Ordinal))
            {
                json = json.Substring(1);
            }

            try
            {
                var tokenizer = new Tokenizer(json);
                return mapper(tokenizer.ParseDocument());
            }
            catch (JsonFormatException ex)
            {
                throw new InvalidDataException(string.Concat("Malformed JSON in ", sourceLabel, ": ", ex.Message), ex);
            }
        }

        private sealed class Tokenizer
        {
            private readonly string _source;

            private int _position;

            private int _depth;

            public Tokenizer(string source)
            {
                _source = source;
            }

            public JsonValue ParseDocument()
            {
                var value = ParseValue();
                SkipWhitespace();
                if (_position < _source.Length)
                {
                    Fail("Trailing content after JSON document");
                }

                return value;
            }

            private JsonValue ParseValue()
            {
                SkipWhitespace();
                if (_position >= _source.Length)
                {
                    Fail("Unexpected end of input");
                }

                var c = _source[_position];
                switch (c)
                {
                    case '{':
                        return ParseObject();
                    case '[':
                        return ParseArray();
                    case '"':
                        return JsonValue.String(ParseString());
                    case 't':
                        ExpectLiteral("true");
                        return JsonValue.Boolean(true);
                    case 'f':
                        ExpectLiteral("false");
                        return JsonValue.Boolean(false);
                    case 'n':
                        ExpectLiteral("null");
                        return JsonValue.Null;
                    default:
                        if (c == '-' || (c >= '0' && c <= '9'))
                        {
                            return JsonValue.Number(ParseNumber());
                        }

                        Fail("Unexpected token character '" + c + "'");
                        return null;
                }
            }

            private JsonValue ParseObject()
            {
                EnterScope();
                Consume('{');
                var members = new List<JsonMember>();
                SkipWhitespace();

                if (Peek() == '}')
                {
                    Consume('}');
                    ExitScope();
                    return JsonValue.Object(members);
                }

                while (true)
                {
                    SkipWhitespace();
                    if (Peek() != '"')
                    {
                        Fail("Expected a property name");
                    }

                    var name = ParseString();
                    SkipWhitespace();
                    Consume(':');
                    var value = ParseValue();
                    members.Add(new JsonMember { Name = name, Value = value });

                    SkipWhitespace();
                    var separator = Peek();
                    if (separator == ',')
                    {
                        Consume(',');
                        continue;
                    }

                    if (separator == '}')
                    {
                        Consume('}');
                        ExitScope();
                        return JsonValue.Object(members);
                    }

                    Fail("Expected ',' or '}' in object");
                }
            }

            private JsonValue ParseArray()
            {
                EnterScope();
                Consume('[');
                var elements = new List<JsonValue>();
                SkipWhitespace();

                if (Peek() == ']')
                {
                    Consume(']');
                    ExitScope();
                    return JsonValue.Array(elements);
                }

                while (true)
                {
                    elements.Add(ParseValue());
                    SkipWhitespace();
                    var separator = Peek();
                    if (separator == ',')
                    {
                        Consume(',');
                        continue;
                    }

                    if (separator == ']')
                    {
                        Consume(']');
                        ExitScope();
                        return JsonValue.Array(elements);
                    }

                    Fail("Expected ',' or ']' in array");
                }
            }

            private string ParseString()
            {
                Consume('"');
                var builder = new StringBuilder();

                while (true)
                {
                    if (_position >= _source.Length)
                    {
                        Fail("Unterminated string");
                    }

                    var c = _source[_position];
                    if (c == '"')
                    {
                        _position++;
                        return builder.ToString();
                    }

                    if (c == '\\')
                    {
                        AppendEscape(builder);
                        continue;
                    }

                    if (c < ' ')
                    {
                        Fail("Unescaped control character in string");
                    }

                    builder.Append(c);
                    _position++;
                }
            }

            private void AppendEscape(StringBuilder builder)
            {
                _position++;
                if (_position >= _source.Length)
                {
                    Fail("Unterminated escape sequence");
                }

                var c = _source[_position];
                switch (c)
                {
                    case '"':
                        builder.Append('"');
                        break;
                    case '\\':
                        builder.Append('\\');
                        break;
                    case '/':
                        builder.Append('/');
                        break;
                    case 'b':
                        builder.Append('\b');
                        break;
                    case 'f':
                        builder.Append('\f');
                        break;
                    case 'n':
                        builder.Append('\n');
                        break;
                    case 'r':
                        builder.Append('\r');
                        break;
                    case 't':
                        builder.Append('\t');
                        break;
                    case 'u':
                        _position++;
                        if (_position + 4 > _source.Length)
                        {
                            Fail("Incomplete unicode escape");
                        }

                        var hex = _source.Substring(_position, 4);
                        if (!int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out var codeUnit))
                        {
                            Fail("Invalid unicode escape");
                        }

                        builder.Append((char)codeUnit);
                        _position += 4;
                        return;
                    default:
                        Fail("Invalid escape character '" + c + "'");
                        break;
                }

                _position++;
            }

            private string ParseNumber()
            {
                var start = _position;
                if (Peek() == '-')
                {
                    _position++;
                }

                if (_position >= _source.Length || !IsDigit(_source[_position]))
                {
                    Fail("Invalid number");
                }

                if (_source[_position] == '0')
                {
                    _position++;
                }
                else
                {
                    while (_position < _source.Length && IsDigit(_source[_position]))
                    {
                        _position++;
                    }
                }

                if (_position < _source.Length && _source[_position] == '.')
                {
                    _position++;
                    if (_position >= _source.Length || !IsDigit(_source[_position]))
                    {
                        Fail("Invalid number fraction");
                    }

                    while (_position < _source.Length && IsDigit(_source[_position]))
                    {
                        _position++;
                    }
                }

                if (_position < _source.Length && (_source[_position] == 'e' || _source[_position] == 'E'))
                {
                    _position++;
                    if (_position < _source.Length && (_source[_position] == '+' || _source[_position] == '-'))
                    {
                        _position++;
                    }

                    if (_position >= _source.Length || !IsDigit(_source[_position]))
                    {
                        Fail("Invalid number exponent");
                    }

                    while (_position < _source.Length && IsDigit(_source[_position]))
                    {
                        _position++;
                    }
                }

                return _source.Substring(start, _position - start);
            }

            private void ExpectLiteral(string literal)
            {
                if (_source.Length - _position < literal.Length || string.CompareOrdinal(_source, _position, literal, 0, literal.Length) != 0)
                {
                    Fail("Invalid literal");
                }

                _position += literal.Length;
            }

            private void EnterScope()
            {
                _depth++;
                if (_depth > MAX_NESTING_DEPTH)
                {
                    Fail("Document nested deeper than " + MAX_NESTING_DEPTH + " levels");
                }
            }

            private void ExitScope()
            {
                _depth--;
            }

            private void SkipWhitespace()
            {
                while (_position < _source.Length)
                {
                    var c = _source[_position];
                    if (c == ' ' || c == '\t' || c == '\n' || c == '\r')
                    {
                        _position++;
                        continue;
                    }

                    break;
                }
            }

            private char Peek()
            {
                if (_position >= _source.Length)
                {
                    Fail("Unexpected end of input");
                }

                return _source[_position];
            }

            private void Consume(char expected)
            {
                var c = Peek();
                if (c != expected)
                {
                    Fail("Expected '" + expected + "', found '" + c + "'");
                }

                _position++;
            }

            private static bool IsDigit(char c)
            {
                return c >= '0' && c <= '9';
            }

            private void Fail(string reason)
            {
                throw new JsonFormatException(reason + " (offset " + _position + ")");
            }
        }
    }
}
