using System;
using System.IO;
using System.Text;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg
{
    public class AudioStream: IAudioStream
    {
        private string _audio;
        private string _bitsreamFilter;
        private string _codec;
        private string _reverse;
        private string _speed;

        /// <inheritdoc />
        public IAudioStream Reverse()
        {
            _reverse = "-af areverse ";
            return this;
        }

        /// <inheritdoc />
        public string Build()
        {
            //todo: all params
            var builder = new StringBuilder();
            builder.Append(_codec);
            builder.Append(_speed);
            builder.Append(_audio);
            //builder.Append(_format);
            builder.Append(_bitsreamFilter);
            //builder.Append(_copy);
            //builder.Append(_seek);
            builder.Append(_reverse);
            builder.Append(BuildFilter());
            //builder.Append(_split);
            return builder.ToString();
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
        public IAudioStream SetCodec(AudioCodec codec, AudioQuality bitrate)
        {
            _audio = $"-codec:a {codec} -b:a {(int) bitrate}k -strict experimental ";
            return this;
        }

        /// <inheritdoc />
        public int Index { get; internal set; }

        /// <summary>
        ///     Duration
        /// </summary>
        public TimeSpan Duration { get; internal set; }

        /// <summary>
        ///     Video format
        /// </summary>
        public string Format { get; internal set; }

        /// <inheritdoc />
        public FileInfo Source { get; internal set; }

        private string BuildFilter()
        {
            var builder = new StringBuilder();
            builder.Append("-filter:a ");
            builder.Append(_speed);

            string filter = builder.ToString();
            if(filter == "-filter:a ")
                return "";
            return filter;
        }
    }
}
