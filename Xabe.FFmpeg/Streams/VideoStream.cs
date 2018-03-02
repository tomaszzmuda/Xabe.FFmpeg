using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg.Streams
{
    /// <inheritdoc cref="IVideoStream" />
    public class VideoStream : IVideoStream, IFilterable
    {
        private readonly List<string> _parameters = new List<string>();
        private readonly Dictionary<string, string> _videoFilters = new Dictionary<string, string>();
        private string _bitsreamFilter;
        private string _codec;
        private string _frameCount;
        private string _loop;
        private string _reverse;
        private string _rotate;
        private string _scale;
        private string _seek;
        private string _size;
        private string _split;

        /// <inheritdoc />
        public IEnumerable<FilterConfiguration> GetFilters()
        {
            if(_videoFilters.Any())
            {
                yield return new FilterConfiguration
                {
                    FilterType = "-filter_complex",
                    StreamNumber = Index,
                    Filters = _videoFilters
                };
            }
        }

        /// <inheritdoc />
        public int Width { get; internal set; }

        /// <inheritdoc />
        public int Height { get; internal set; }

        /// <inheritdoc />
        public double FrameRate { get; internal set; }

        /// <inheritdoc />
        public string Ratio { get; internal set; }

        /// <inheritdoc />
        public FileInfo Source { get; internal set; }

        /// <inheritdoc />
        public string Build()
        {
            var builder = new StringBuilder();
            builder.Append(string.Join(" ", _parameters));
            builder.Append(_scale);
            builder.Append(_codec);
            builder.Append(_bitsreamFilter);
            builder.Append(_seek);
            builder.Append(_frameCount);
            builder.Append(_loop);
            builder.Append(_split);
            builder.Append(_reverse);
            builder.Append(_rotate);
            builder.Append(_size);
            return builder.ToString();
        }

        /// <inheritdoc />
        public IVideoStream ChangeSpeed(double multiplication)
        {
            _videoFilters["setpts"] = MediaSpeedHelper.GetVideoSpeedFilter(multiplication);
            return this;
        }

        /// <inheritdoc />
        public IVideoStream Rotate(RotateDegrees rotateDegrees)
        {
            _rotate = rotateDegrees == RotateDegrees.Invert ? "-vf \"transpose=2,transpose=2\" " : $"-vf \"transpose={(int)rotateDegrees}\" ";
            return this;
        }

        /// <inheritdoc />
        public CodecType CodecType { get; } = CodecType.Video;

        /// <inheritdoc />
        public double Bitrate { get; internal set; }

        /// <inheritdoc />
        public IVideoStream CopyStream()
        {
            _codec = "-c:v copy ";
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetLoop(int count, int delay)
        {
            _loop = $"-loop {count} ";
            if(delay > 0)
            {
                _loop += $"-final_delay {delay / 100} ";
            }
            return this;
        }

        /// <inheritdoc />
        public IVideoStream AddSubtitles(string subtitlePath, string encode, string style, VideoSize originalSize)
        {
            string filter = $"'{subtitlePath}'".Replace("\\", "\\\\")
                                               .Replace(":", "\\:");
            if(!string.IsNullOrEmpty(encode))
            {
                filter += $":charenc={encode}";
            }
            if(!string.IsNullOrEmpty(style))
            {
                filter += $":force_style=\'{style}\'";
            }
            if(originalSize != null)
            {
                filter += $":original_size={originalSize}";
            }
            _videoFilters.Add("subtitles", filter);
            return this;
        }

        /// <inheritdoc />
        public IVideoStream Reverse()
        {
            _reverse = "-vf reverse ";
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetScale(VideoSize size)
        {
            if(size != null)
            {
                _scale = $"-vf scale={size} ";
            }
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetSize(VideoSize size)
        {
            if(size != null)
            {
                _size = $"-s {size} ";
            }
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetCodec(VideoCodec codec)
        {
            _codec = $"-codec:v {codec} ";
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetBitstreamFilter(BitstreamFilter filter)
        {
            _bitsreamFilter = $"-bsf:v {filter} ";
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetSeek(TimeSpan seek)
        {
            if(seek != null)
            {
                if(seek > Duration)
                {
                    throw new ArgumentException("Seek can not be greater than video duration");
                }
                _seek = $"-ss {seek} ";
            }
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetOutputFramesCount(int number)
        {
            _frameCount = $"-frames:v {number} ";
            return this;
        }

        /// <inheritdoc />
        public TimeSpan Duration { get; internal set; }

        /// <inheritdoc />
        public string Format { get; internal set; }

        /// <inheritdoc />
        public int Index { get; internal set; }

        /// <inheritdoc />
        public IVideoStream SetWatermark(string imagePath, Position position)
        {
            _parameters.Add($"-i \"{imagePath}\" ");
            string argument = string.Empty;
            switch(position)
            {
                case Position.Bottom:
                    argument += "(main_w-overlay_w)/2:main_h-overlay_h";
                    break;
                case Position.Center:
                    argument += "x=(main_w-overlay_w)/2:y=(main_h-overlay_h)/2";
                    break;
                case Position.BottomLeft:
                    argument += "5:main_h-overlay_h";
                    break;
                case Position.UpperLeft:
                    argument += "5:5";
                    break;
                case Position.BottomRight:
                    argument += "(main_w-overlay_w):main_h-overlay_h";
                    break;
                case Position.UpperRight:
                    argument += "(main_w-overlay_w):5";
                    break;
                case Position.Left:
                    argument += "5:(main_h-overlay_h)/2";
                    break;
                case Position.Right:
                    argument += "(main_w-overlay_w-5):(main_h-overlay_h)/2";
                    break;
                case Position.Up:
                    argument += "(main_w-overlay_w)/2:5";
                    break;
            }
            _videoFilters["overlay"] = argument;
            return this;
        }

        /// <inheritdoc />
        public IVideoStream Split(TimeSpan startTime, TimeSpan duration)
        {
            _split = $"-ss {startTime.ToFFmpeg()} -t {duration.ToFFmpeg()} ";
            return this;
        }

        void IStream.Split(TimeSpan startTime, TimeSpan duration)
        {
            Split(startTime, duration);
        }
    }
}
