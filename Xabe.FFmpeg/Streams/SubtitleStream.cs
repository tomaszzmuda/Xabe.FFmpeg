using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg
{
    /// <inheritdoc />
    public class SubtitleStream : ISubtitleStream
    {
        private string _configuredLanguage;
        private string _format;
        private string _split;

        /// <inheritdoc />
        public ISubtitleStream SetFormat(SubtitleFormat format)
        {
            if (!string.IsNullOrEmpty(format.Format))
            {
                _format = $"-f {format} ";
            }
            return this;
        }

        /// <inheritdoc />
        public string Format { get; internal set; }

        /// <inheritdoc />
        public FileInfo Source { get; internal set; }

        /// <inheritdoc />
        public string Build()
        {
            var builder = new StringBuilder();
            builder.Append(_format);
            builder.Append(_split);
            builder.Append(BuildLanguage());
            return builder.ToString();
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
        public int? Default { get; set; }

        /// <inheritdoc />
        public int? Forced { get; set; }

        /// <inheritdoc />
        public string Title { get; set; }

        /// <inheritdoc />
        public ISubtitleStream SetLanguage(string lang)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                _configuredLanguage = lang;
            }
            return this;
        }

        /// <inheritdoc />
        void ILocalStream.Split(TimeSpan startTime, TimeSpan duration)
        {
            Split(startTime, duration);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetSource()
        {
            return new[] { Source.FullName };
        }

        private string BuildLanguage()
        {
            string language = string.Empty;
            language = !string.IsNullOrEmpty(_configuredLanguage) ? _configuredLanguage : Language;
            if (!string.IsNullOrEmpty(language))
            {
                language = $"-metadata:s:s:{Index} language={language} ";
            }
            return language;
        }

        private void Split(TimeSpan startTime, TimeSpan duration)
        {
            _split = $"-ss {startTime.ToFFmpeg()} -t {duration.ToFFmpeg()} ";
        }
    }
}
