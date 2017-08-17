using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Xabe.FFMpeg.Enums;

namespace Xabe.FFMpeg
{
    /// <summary>
    ///     Allows to prepare and start conversion. Only for advanced users.
    /// </summary>
    public class Conversion
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

        /// <summary>
        ///     Start conversion
        /// </summary>
        public bool Start()
        {
            return new FFMpeg().StartConversion(Build());

        }

        /// <summary>
        ///     Set speed of conversion. Slower speed equals better compression and quality.
        /// </summary>
        /// <param name="speed">Speed</param>
        /// <returns>Conversion object</returns>
        public Conversion SetSpeed(Speed speed)
        {
            _speed = $"-preset {speed.ToString() .ToLower()} ";
            return this;
        }

        /// <summary>
        ///     Set max cpu threads
        /// </summary>
        /// <param name="cpu">Threads</param>
        /// <returns>Conversion object</returns>
        public Conversion SetSpeed(int cpu)
        {
            _speed = $"-quality good -cpu-used {cpu} -deadline realtime ";
            return this;
        }

        /// <summary>
        ///     Set audio codec and bitrate
        /// </summary>
        /// <param name="codec">Audio odec</param>
        /// <param name="bitrate">Audio bitrade</param>
        /// <returns>Conversion object</returns>
        public Conversion SetAudio(AudioCodec codec, AudioQuality bitrate)
        {
            return this.SetAudio(codec.ToString(), bitrate);
        }

        /// <summary>
        ///     Set audio codec and bitrate
        /// </summary>
        /// <param name="codec">Audio odec</param>
        /// <param name="bitrate">Audio bitrade</param>
        /// <returns>Conversion object</returns>
        public Conversion SetAudio(string codec, AudioQuality bitrate)
        {
            _audio = $"-codec:a {codec.ToLower()} -b:a {(int)bitrate}k -strict experimental ";
            return this;
        }

        /// <summary>
        ///     Set video codec and bitrate
        /// </summary>
        /// <param name="codec">Video codec</param>
        /// <param name="bitrate">Video bitrate</param>
        /// <returns>Conversion object</returns>
        public Conversion SetVideo(VideoCodec codec, int bitrate = 0)
        {
            return this.SetVideo(codec.ToString(), bitrate);
        }

        /// <summary>
        ///     Set video codec and bitrate
        /// </summary>
        /// <param name="codec">Video codec</param>
        /// <param name="bitrate">Video bitrate</param>
        /// <returns>Conversion object</returns>
        public Conversion SetVideo(string codec, int bitrate = 0)
        {
            _video = $"-codec:v {codec.ToLower()} ";

            if (bitrate > 0)
                _video += $"-b:v {bitrate}k ";
            return this;
        }

        /// <summary>
        ///     Defines if converter should use all CPU cores
        /// </summary>
        /// <param name="multiThread">Use all CPU cores</param>
        /// <returns>Conversion object</returns>
        public Conversion UseMultiThread(bool multiThread)
        {
            string threadCount = multiThread
                ? Environment.ProcessorCount.ToString()
                : "1";

            _threads = $"-threads {threadCount} ";
            return this;
        }

        /// <summary>
        ///     Set URI of stream
        /// </summary>
        /// <param name="uri">URI</param>
        /// <returns>Conversion object</returns>
        public Conversion SetInput(Uri uri)
        {
            _input = $"-i \"{uri.AbsoluteUri}\" ";
            return this;
        }

        /// <summary>
        ///     Disable channel. Can remove audio or video from media file.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Conversion object</returns>
        public Conversion DisableChannel(Channel type)
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

        /// <summary>
        ///     Set input file
        /// </summary>
        /// <param name="input">Media file to convert</param>
        /// <returns>Conversion object</returns>
        public Conversion SetInput(VideoInfo input)
        {
            _input = $"-i \"{input.FilePath}\" ";
            return this;
        }

        /// <summary>
        ///     Set input files
        /// </summary>
        /// <param name="inputs">Media files to convert</param>
        /// <returns>Conversion object</returns>
        public Conversion SetInput(params FileInfo[] inputs)
        {
            _input = "";
            foreach(FileInfo file in inputs)
                _input += $"-i \"{file.FullName}\" ";
            return this;
        }

        /// <summary>
        ///     Set output path
        /// </summary>
        /// <param name="outputPath">Output media file</param>
        /// <returns>Conversion object</returns>
        public Conversion SetOutput(string outputPath)
        {
            _output = $"\"{outputPath}\"";
            return this;
        }

        /// <summary>
        ///     Set size of video
        /// </summary>
        /// <param name="size">VideoSize</param>
        /// <returns>Conversion object</returns>
        public Conversion SetScale(VideoSize size)
        {
            if(VideoSize.Original == size)
                return this;
            _scale = $"-vf scale={(int) size} ";
            return this;
        }

        /// <summary>
        ///     Set size of video
        /// </summary>
        /// <param name="size">VideoSize</param>
        /// <returns>Conversion object</returns>
        public Conversion SetSize(Size? size)
        {
            if(size.HasValue)
                _size = $"-s {size.Value.Width}x{size.Value.Height} ";

            return this;
        }

        /// <summary>
        ///     Set video codec
        /// </summary>
        /// <param name="codec">Video codec</param>
        /// <returns>Conversion object</returns>
        public Conversion SetCodec(VideoCodec codec)
        {
            return this.SetCodec(codec.ToString());
        }

        /// <summary>
        ///     Set video codec
        /// </summary>
        /// <param name="codec">Video codec</param>
        /// <returns>Conversion object</returns>
        public Conversion SetCodec(string codec)
        {
            _codec = $"-f {codec.ToLower()} ";
            return this;
        }

        /// <summary>
        ///     Set filter
        /// </summary>
        /// <param name="type">Channel type</param>
        /// <param name="filter">Filter</param>
        /// <returns>Conversion object</returns>
        public Conversion SetFilter(Channel type, Filter filter)
        {
            return this.SetFilter(type, filter.ToString());
        }

        /// <summary>
        ///     Set filter
        /// </summary>
        /// <param name="type">Channel type</param>
        /// <param name="filter">Filter</param>
        /// <returns>Conversion object</returns>
        public Conversion SetFilter(Channel type, string filter)
        {
            switch (type)
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

        /// <summary>
        ///     Set channels
        /// </summary>
        /// <param name="type">Channel type</param>
        /// <returns>Conversion object</returns>
        public Conversion SetChannels(Channel type)
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

        /// <summary>
        ///     Seeks in input file to position. (-ss argument)
        /// </summary>
        /// <param name="seek">Position</param>
        /// <returns>Conversion object</returns>
        public Conversion SetSeek(TimeSpan? seek)
        {
            if(seek.HasValue)
                _seek = $"-ss {seek} ";

            return this;
        }

        /// <summary>
        ///     Set output frames count
        /// </summary>
        /// <param name="number">Number of frames</param>
        /// <returns>Conversion object</returns>
        public Conversion SetOutputFramesCount(int number)
        {
            _frameCount = $"-vframes {number} ";
            return this;
        }

        /// <summary>
        ///     Loop over the input stream. Currently it works only for image streams. (-loop)
        /// </summary>
        /// <param name="count">Number of repeats</param>
        /// <returns>Conversion object</returns>
        public Conversion SetLoop(int count)
        {
            _loop = $"-loop {count} ";
            return this;
        }

        /// <summary>
        ///     Finish encoding when the shortest input stream ends. (-shortest)
        /// </summary>
        /// <param name="useShortest"></param>
        /// <returns>Conversion object</returns>
        public Conversion UseShortest(bool useShortest)
        {
            if(!useShortest)
                return this;
            _shortestInput = "-shortest ";
            return this;
        }

        /// <summary>
        ///     Concat multiple media files
        /// </summary>
        /// <param name="paths">Media files</param>
        /// <returns>Conversion object</returns>
        public Conversion Concat(IEnumerable<string> paths)
        {
            _input = $"-i \"concat:{string.Join(@"|", paths)}\" ";
            return this;
        }
    }
}
