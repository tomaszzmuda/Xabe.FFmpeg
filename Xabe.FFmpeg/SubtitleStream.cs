using System.IO;
using System.Text;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Reference to subtitle file
    /// </summary>
    public class SubtitleStream: ISubtitleStream
    {
        private string _format;
        private string _language;

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
            builder.Append(_language);
            return builder.ToString();
        }

        /// <inheritdoc />
        public int Index { get; internal set; }

        /// <inheritdoc />
        public string Language { get; internal set; }

        /// <inheritdoc />
        public ISubtitleStream SetLanguage(string lang)
        {
            if(!string.IsNullOrEmpty(lang))
                _language = $"-metadata:s:s:{Index} language={lang} ";
            return this;
        }
    }
}
