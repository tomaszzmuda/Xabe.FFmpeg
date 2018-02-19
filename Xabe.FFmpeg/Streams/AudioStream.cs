using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg.Streams
{
    /// <inheritdoc cref="IAudioStream" />
    public class AudioStream: IAudioStream, IFilterable
    {
        private readonly Dictionary<string, string> _audioFilters = new Dictionary<string, string>();
        private string _audio;
        private string _bitsreamFilter;
        private string _reverse;
        private string _seek;
        private string _split;

        /// <inheritdoc />
        public IAudioStream Reverse()
        {
            _reverse = "-af areverse ";
            return this;
        }

        /// <inheritdoc />
        public string Build()
        {
            var builder = new StringBuilder();
            builder.Append(_audio);
            builder.Append(_bitsreamFilter);
            builder.Append(_seek);
            builder.Append(_reverse);
            builder.Append(_split);
            return builder.ToString();
        }

        /// <inheritdoc />
        public IAudioStream Split(TimeSpan startTime, TimeSpan duration)
        {
            _split = $"-ss {startTime.ToFFmpeg()} -t {duration.ToFFmpeg()} ";
            return this;
        }

        /// <inheritdoc />
        public IAudioStream CopyStream()
        {
            _audioFilters["-c:v copy"] = string.Empty;
            return this;
        }

        /// <inheritdoc />
        public CodecType CodecType { get; } = CodecType.Audio;

        /// <inheritdoc />
        public IAudioStream SetBitstreamFilter(BitstreamFilter filter)
        {
            _bitsreamFilter = $"-bsf:a {filter} ";
            return this;
        }

        /// <inheritdoc />
        public IAudioStream ChangeSpeed(double multiplication)
        {
            _audioFilters["atempo"] = $"{string.Format(CultureInfo.GetCultureInfo("en-US"), "{0:N1}", MediaSpeedHelper.GetAudioSpeed(multiplication))}";
            return this;
        }

        /// <inheritdoc />
        public IAudioStream SetCodec(AudioCodec codec)
        {
            _audio = $"-codec:a {codec} ";
            return this;
        }

        /// <inheritdoc />
        public int Index { get; internal set; }

        /// <inheritdoc />
        public TimeSpan Duration { get; internal set; }

        /// <inheritdoc />
        public string Format { get; internal set; }

        /// <inheritdoc />
        public FileInfo Source { get; internal set; }

        /// <inheritdoc />
        public IAudioStream SetSeek(TimeSpan? seek)
        {
            if(seek.HasValue)
            {
                _seek = $"-ss {seek.Value.ToFFmpeg()} ";
            }
            return this;
        }

        void IStream.Split(TimeSpan startTime, TimeSpan duration)
        {
            Split(startTime, duration);
        }

        /// <inheritdoc />
        public IEnumerable<FilterConfiguration> GetFilters()
        {
            if(_audioFilters.Any())
            {
                yield return new FilterConfiguration
                {
                    FilterType = ":a",
                    StreamNumber = Index,
                    Filters = _audioFilters
                };
            }
        }
    }
}
