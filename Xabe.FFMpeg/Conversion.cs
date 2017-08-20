using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Xabe.FFMpeg.Enums;

namespace Xabe.FFMpeg
{
    /// <inheritdoc />
    public class Conversion: IConversion
    {
        private string _audio;
        private string _codec;
        private string _copy;
        private string _disabled;
        private string _filter;
        private string _frameCount;
        private string _input;
        private string _loop;
        private string _output;
        private string _scale;
        private string _seek;
        private string _shortestInput;
        private string _size;
        private string _speed;
        private string _threads;
        private string _video;
        private string _reverse;

        /// <inheritdoc />
        public string Build()
        {
            var builder = new StringBuilder();
            builder.Append(_input);
            builder.Append(_scale);
            builder.Append(_video);
            builder.Append(_speed);
            builder.Append(_audio);
            builder.Append(_threads);
            builder.Append(_disabled);
            builder.Append(_size);
            builder.Append(_codec);
            builder.Append(_filter);
            builder.Append(_copy);
            builder.Append(_seek);
            builder.Append(_frameCount);
            builder.Append(_loop);
            builder.Append(_reverse);
            builder.Append(_shortestInput);
            builder.Append(_output);

            return builder.ToString();
        }

        /// <inheritdoc />
        public bool Start()
        {
            return new FFMpeg().StartConversion(Build());
        }

        /// <inheritdoc />
        public IConversion SetSpeed(Speed speed)
        {
            _speed = $"-preset {speed.ToString() .ToLower()} ";
            return this;
        }

        /// <inheritdoc />
        public IConversion SetSpeed(int cpu)
        {
            _speed = $"-quality good -cpu-used {cpu} -deadline realtime ";
            return this;
        }

        /// <inheritdoc />
        public IConversion SetAudio(AudioCodec codec, AudioQuality bitrate)
        {
            return SetAudio(codec.ToString(), bitrate);
        }

        /// <inheritdoc />
        public IConversion SetAudio(string codec, AudioQuality bitrate)
        {
            _audio = $"-codec:a {codec.ToLower()} -b:a {(int) bitrate}k -strict experimental ";
            return this;
        }

        /// <inheritdoc />
        public IConversion SetVideo(VideoCodec codec, int bitrate = 0)
        {
            return SetVideo(codec.ToString(), bitrate);
        }

        /// <inheritdoc />
        public IConversion SetVideo(string codec, int bitrate = 0)
        {
            _video = $"-codec:v {codec.ToLower()} ";

            if(bitrate > 0)
                _video += $"-b:v {bitrate}k ";
            return this;
        }

        /// <inheritdoc />
        public IConversion UseMultiThread(bool multiThread)
        {
            string threadCount = multiThread
                ? Environment.ProcessorCount.ToString()
                : "1";

            _threads = $"-threads {threadCount} ";
            return this;
        }

        /// <inheritdoc />
        public IConversion SetInput(Uri uri)
        {
            _input = $"-i \"{uri.AbsoluteUri}\" ";
            return this;
        }

        /// <inheritdoc />
        public IConversion DisableChannel(Channel type)
        {
            switch(type)
            {
                case Channel.Video:
                    _disabled = "-vn ";
                    break;
                case Channel.Audio:
                    _disabled = "-an ";
                    break;
            }
            return this;
        }

        /// <inheritdoc />
        public IConversion SetInput(VideoInfo input)
        {
            _input = $"-i \"{input.FilePath}\" ";
            return this;
        }

        /// <inheritdoc />
        public IConversion SetInput(params FileInfo[] inputs)
        {
            _input = "";
            foreach(FileInfo file in inputs)
                _input += $"-i \"{file.FullName}\" ";
            return this;
        }

        /// <inheritdoc />
        public IConversion SetOutput(string outputPath)
        {
            _output = $"\"{outputPath}\"";
            return this;
        }

        /// <inheritdoc />
        public IConversion SetScale(VideoSize size)
        {
            if(VideoSize.Original == size)
                return this;
            _scale = $"-vf scale={(int) size} ";
            return this;
        }

        /// <inheritdoc />
        public IConversion SetSize(Size? size)
        {
            if(size.HasValue)
                _size = $"-s {size.Value.Width}x{size.Value.Height} ";

            return this;
        }

        /// <inheritdoc />
        public IConversion SetCodec(VideoCodec codec)
        {
            return SetCodec(codec.ToString());
        }

        /// <inheritdoc />
        public IConversion SetCodec(string codec)
        {
            _codec = $"-f {codec.ToLower()} ";
            return this;
        }

        /// <inheritdoc />
        public IConversion SetFilter(Channel type, Filter filter)
        {
            return SetFilter(type, filter.ToString());
        }

        /// <inheritdoc />
        public IConversion SetFilter(Channel type, string filter)
        {
            switch(type)
            {
                case Channel.Audio:
                    _filter = $"-bsf:a {filter.ToLower()} ";
                    break;
                case Channel.Video:
                    _filter = $"-bsf:v {filter.ToLower()} ";
                    break;
            }
            return this;
        }

        /// <inheritdoc />
        public IConversion SetChannels(Channel type)
        {
            switch(type)
            {
                case Channel.Audio:
                    _copy = "-c:a copy ";
                    break;
                case Channel.Video:
                    _copy = "-c:v copy ";
                    break;
                default:
                    _copy = "-c copy ";
                    break;
            }
            return this;
        }

        /// <inheritdoc />
        public IConversion Reverse(Channel type)
        {
            switch (type)
            {
                case Channel.Audio:
                    _reverse = "-af areverse ";
                    break;
                case Channel.Video:
                    _reverse = "-vf reverse ";
                    break;
                case Channel.Both:
                    _reverse = "-vf reverse -af areverse ";
                    break;
            }
            return this;
        }

        /// <inheritdoc />
        public IConversion SetSeek(TimeSpan? seek)
        {
            if(seek.HasValue)
                _seek = $"-ss {seek} ";

            return this;
        }

        /// <inheritdoc />
        public IConversion SetOutputFramesCount(int number)
        {
            _frameCount = $"-vframes {number} ";
            return this;
        }

        /// <inheritdoc />
        public IConversion SetLoop(int count)
        {
            _loop = $"-loop {count} ";
            return this;
        }

        /// <inheritdoc />
        public IConversion UseShortest(bool useShortest)
        {
            if(!useShortest)
                return this;
            _shortestInput = "-shortest ";
            return this;
        }

        /// <inheritdoc />
        public IConversion Concat(IEnumerable<string> paths)
        {
            _input = $"-i \"concat:{string.Join(@"|", paths)}\" ";
            return this;
        }
    }
}
