using System.Collections.Generic;
using System.Linq;
using Xabe.FFmpeg.Streams;
using Xabe.FFmpeg.Streams.SubtitleStream;

namespace Xabe.FFmpeg
{
    /// <inheritdoc />
    public class SubtitleStream : ISubtitleStream
    {
        private readonly ParametersList<ConversionParameter> _parameters = new ParametersList<ConversionParameter>();

        /// <inheritdoc />
        public string Codec { get; internal set; }

        /// <inheritdoc />
        public string Path { get; internal set; }

        internal SubtitleStream()
        {

        }

        /// <inheritdoc />
        public string BuildParameters(ParameterPosition forPosition)
        {
            IEnumerable<ConversionParameter> parameters = _parameters?.Where(x => x.Position == forPosition);
            if (parameters != null &&
                parameters.Any())
            {
                return string.Join(string.Empty, parameters.Select(x => x.Parameter));
            }
            else
            {
                return string.Empty;
            }
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
            var language = !string.IsNullOrEmpty(lang) ? lang : Language;
            if (!string.IsNullOrEmpty(language))
            {
                language = $"-metadata:s:s:{Index} language={language}";
                _parameters.Add(new ConversionParameter(language));
            }

            return this;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetSource()
        {
            return new[] { Path };
        }

        /// <inheritdoc />
        public ISubtitleStream SetCodec(SubtitleCodec codec)
        {
            return SetCodec(codec.ToString());
        }

        /// <inheritdoc />
        public ISubtitleStream SetCodec(string codec)
        {
            _parameters.Add(new ConversionParameter($"-c:s {codec}"));
            return this;
        }

        /// <inheritdoc />
        public ISubtitleStream UseNativeInputRead(bool readInputAtNativeFrameRate)
        {
            _parameters.Add(new ConversionParameter($"-re", ParameterPosition.PreInput));
            return this;
        }

        /// <inheritdoc />
        public ISubtitleStream SetStreamLoop(int loopCount)
        {
            _parameters.Add(new ConversionParameter($"-stream_loop {loopCount}", ParameterPosition.PreInput));
            return this;
        }
    }
}
