using System;
using System.Globalization;
using System.IO;
using System.Text;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg
{
    /// <inheritdoc />
    public class AudioStream : IAudioStream
    {
        private string _audio;
        private string _bitsreamFilter;
        private string _codec;
        private string _reverse;
        private string _speed;
        private string _split;
        private string _seek;
        private string _format;

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
            builder.Append(_codec);
            builder.Append(_speed);
            builder.Append(_audio);
            builder.Append(_format);
            builder.Append(_bitsreamFilter);
            builder.Append(_seek);
            builder.Append(_reverse);
            builder.Append(BuildFilter());
            builder.Append(_split);
            return builder.ToString();
        }

        /// <inheritdoc />
        public IAudioStream SetFormat(AudioFormat format)
        {
            _format = $"-f {format} ";
            return this;
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
            _codec = "-c:v copy ";
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
            _speed = $"atempo={string.Format(CultureInfo.GetCultureInfo("en-US"), "{0:N1}", MediaSpeedHelper.GetAudioSpeed(multiplication))} ";
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

        private string BuildFilter()
        {
            var builder = new StringBuilder();
            builder.Append("-filter:a ");
            builder.Append(_speed);
            builder.Append(_codec);

            string filter = builder.ToString();
            if(filter == "-filter:a ")
                return "";
            return filter;
        }

        /// <inheritdoc />
        public IAudioStream SetSeek(TimeSpan? seek)
        {
            if(seek.HasValue)
                _seek = $"-ss {seek.Value.ToFFmpeg()} ";
            return this;
        }

        void IStream.Split(TimeSpan startTime, TimeSpan duration)
        {
            Split(startTime, duration);
        }
    }
}
