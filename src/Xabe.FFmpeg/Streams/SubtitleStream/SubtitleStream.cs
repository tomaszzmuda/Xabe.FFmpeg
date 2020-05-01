using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Xabe.FFmpeg
{
    /// <inheritdoc />
    public class SubtitleStream : ISubtitleStream
    {
        private string _language;

        /// <inheritdoc />
        public string Codec { get; internal set; }

        /// <inheritdoc />
        public string Path { get; internal set; }

        /// <inheritdoc />
        public string Build()
        {
            var builder = new StringBuilder();
            builder.Append(BuildLanguage());
            return builder.ToString();
        }

        internal SubtitleStream()
        {

        }

        /// <inheritdoc />
        public string BuildInputArguments()
        {
            return null;
        }

        /// <inheritdoc />
        public int Index { get; internal set; }

        /// <inheritdoc />
        public string Language { get; internal set; }

        /// <inheritdoc />
        public int? Default { get; internal set; }

        /// <inheritdoc />
        public int? Forced { get; internal set; }

        /// <inheritdoc />
        public string Title { get; internal set; }

        /// <inheritdoc />
        public StreamType StreamType => StreamType.Subtitle;

        /// <inheritdoc />
        public ISubtitleStream SetLanguage(string lang)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                _language = lang;
            }
            return this;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetSource()
        {
            return new[] { Path };
        }

        private string BuildLanguage()
        {
            string language = !string.IsNullOrEmpty(_language) ? _language : Language;
            if (!string.IsNullOrEmpty(language))
            {
                language = $"-metadata:s:s:{Index} language={language} ";
            }
            return language;
        }
    }
}
