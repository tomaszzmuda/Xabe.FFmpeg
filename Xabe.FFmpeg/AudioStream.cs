using System;
using System.IO;
using System.Text;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg
{
    public class AudioStream : IAudioStream
    {
        private string _reverse;
        private string _speed;
        private string _audio;

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
            //builder.Append(_watermark);
            //builder.Append(_scale);
            //builder.Append(_codec);
            builder.Append(_speed);
            builder.Append(_audio);
            //builder.Append(_threads);
            //builder.Append(_format);
            //builder.Append(_bitsreamFilter);
            //builder.Append(_copy);
            //builder.Append(_seek);
            //builder.Append(_frameCount);
            //builder.Append(_loop);
            builder.Append(_reverse);
            //builder.Append(_rotate);
            //builder.Append(_shortestInput);
            builder.Append(BuildFilter());
            //builder.Append(_split);
            //builder.Append(_output);
            return builder.ToString();
        }

        /// <inheritdoc />
        public CodecType CodecType { get; } = CodecType.Audio;

        private string BuildFilter()
        {
            var builder = new StringBuilder();
            builder.Append("-filter:a ");
            builder.Append(_speed);

            string filter = builder.ToString();
            if (filter == "-filter:a ")
                return "";
            return filter;
        }

        /// <inheritdoc />
        public IAudioStream SetAudio(AudioCodec codec, AudioQuality bitrate)
        {
            return SetAudio(codec.ToString(), bitrate);
        }

        /// <inheritdoc />
        public IAudioStream SetAudio(string codec, AudioQuality bitrate)
        {
            _audio = $"-codec:a {codec} -b:a {(int)bitrate}k -strict experimental ";
            return this;
        }

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
    }
}
