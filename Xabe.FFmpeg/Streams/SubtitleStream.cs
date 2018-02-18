using System;
using System.IO;
using System.Text;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg.Streams
{
    /// <inheritdoc />
    public class SubtitleStream : ISubtitleStream
    {
        private string _format;
        private string _language;
        private string _split;

        /// <inheritdoc />
        public ISubtitleStream SetFormat(SubtitleFormat format)
        {
            if(!string.IsNullOrEmpty(format.Format))
                _format = $"-f {format} ";
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

        private string BuildLanguage()
        {
            string language = string.Empty;
            if(!string.IsNullOrEmpty(_language))
            {
                language = _language;
            }
            else if(!string.IsNullOrEmpty(Language))
            {
                language = Language;
            }
            if(!string.IsNullOrEmpty(language))
            {
                language = $"-metadata:s:s:{Index} language={language} ";
            }
            return language;
        }

        /// <inheritdoc />
        public int Index { get; internal set; }

        /// <inheritdoc />
        public string Language { get; internal set; }

        /// <inheritdoc />
        public ISubtitleStream SetLanguage(string lang)
        {
            if(!string.IsNullOrEmpty(lang))
            {
                _language = lang;
            }
            return this;
        }

        private void Split(TimeSpan startTime, TimeSpan duration)
        {
            _split = $"-ss {startTime.ToFFmpeg()} -t {duration.ToFFmpeg()} ";
        }

        /// <inheritdoc />
        void IStream.Split(TimeSpan startTime, TimeSpan duration)
        {
            Split(startTime, duration);
        }
    }
}
