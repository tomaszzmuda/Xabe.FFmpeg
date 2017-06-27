using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Xabe.FFMpeg.Enums;

namespace Xabe.FFMpeg
{
    internal class ArgumentBuilder
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

        internal string Build()
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
            builder.Append(_shortestInput);
            builder.Append(_output);

            return builder.ToString();
        }

        internal ArgumentBuilder SetSpeed(Speed speed)
        {
            _speed = $"-preset {speed.ToString() .ToLower()} ";
            return this;
        }

        internal ArgumentBuilder SetSpeed(int cpu)
        {
            _speed = $"-quality good -cpu-used {cpu} -deadline realtime ";
            return this;
        }

        internal ArgumentBuilder SetAudio(AudioCodec codec, AudioQuality bitrate)
        {
            _audio = $"-codec:a {codec.ToString() .ToLower()} -b:a {(int) bitrate}k -strict experimental ";
            return this;
        }

        internal ArgumentBuilder SetVideo(VideoCodec codec, int bitrate = 0)
        {
            _video = $"-codec:v {codec.ToString() .ToLower()} ";

            if(bitrate > 0)
                _video += $"-b:v {bitrate}k ";
            return this;
        }

        internal ArgumentBuilder UseMultiThread(bool multiThread)
        {
            string threadCount = multiThread
                ? Environment.ProcessorCount.ToString()
                : "1";

            _threads = $"-threads {threadCount} ";
            return this;
        }

        internal ArgumentBuilder SetInput(Uri uri)
        {
            _input = $"-i \"{uri.AbsoluteUri}\" ";
            return this;
        }

        internal ArgumentBuilder DisableChannel(Channel type)
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

        internal ArgumentBuilder SetInput(VideoInfo input)
        {
            _input = $"-i \"{input.FullName}\" ";
            return this;
        }

        internal ArgumentBuilder SetInput(params FileInfo[] inputs)
        {
            _input = "";
            foreach(FileInfo file in inputs)
                _input += $"-i \"{file.FullName}\" ";
            return this;
        }

        internal ArgumentBuilder SetOutput(FileInfo output)
        {
            _output = $"\"{output}\"";
            return this;
        }

        internal ArgumentBuilder SetScale(VideoSize size)
        {
            if(VideoSize.Original == size)
                return this;
            _scale = $"-vf scale={(int) size} ";
            return this;
        }

        internal ArgumentBuilder SetSize(Size? size)
        {
            if(size.HasValue)
                _size = $"-s {size.Value.Width}x{size.Value.Height} ";

            return this;
        }

        internal ArgumentBuilder SetCodec(VideoCodec codec)
        {
            _codec = $"-f {codec.ToString() .ToLower()} ";
            return this;
        }

        internal ArgumentBuilder SetFilter(Channel type, Filter filter)
        {
            switch(type)
            {
                case Channel.Audio:
                    _filter = $"-bsf:a {filter.ToString() .ToLower()} ";
                    break;
                case Channel.Video:
                    _filter = $"-bsf:v {filter.ToString() .ToLower()} ";
                    break;
            }
            return this;
        }

        internal ArgumentBuilder SetChannels(Channel type)
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

        internal ArgumentBuilder SetSeek(TimeSpan? seek)
        {
            if(seek.HasValue)
                _seek = $"-ss {seek} ";

            return this;
        }

        internal ArgumentBuilder SetOutputFramesCount(int number)
        {
            _frameCount = $"-vframes {number} ";
            return this;
        }

        internal ArgumentBuilder SetLoop(int count)
        {
            _loop = $"-loop {count} ";
            return this;
        }

        internal ArgumentBuilder UseShortest(bool useShortest)
        {
            if(!useShortest)
                return this;
            _shortestInput = "-shortest ";
            return this;
        }

        internal ArgumentBuilder Concat(IEnumerable<string> paths)
        {
            _input = $"-i \"concat:{string.Join(@"|", paths)}\" ";
            return this;
        }
    }
}
