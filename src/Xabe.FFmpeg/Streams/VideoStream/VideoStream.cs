using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Xabe.FFmpeg
{ 
    /// <inheritdoc cref="IVideoStream" />
    public class VideoStream : IVideoStream, IFilterable
    {
        private readonly List<string> _parameters = new List<string>();
        private readonly Dictionary<string, string> _videoFilters = new Dictionary<string, string>();
        private string _watermarkSource;
        private string _bitrate;
        private string _bitsreamFilter;
        private string _frameCount;
        private string _framerate;
        private string _loop;
        private string _reverse;
        private string _rotate;
        private string _seek;
        private string _size;
        private string _split;
        private string _flags;
        private string _codec;

        /// <inheritdoc />
        public IEnumerable<IFilterConfiguration> GetFilters()
        {
            if (_videoFilters.Any())
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
        public double Framerate { get; internal set; }

        /// <inheritdoc />
        public string Ratio { get; internal set; }

        /// <inheritdoc />
        public TimeSpan Duration { get; internal set; }

        /// <inheritdoc />
        public string Codec { get; internal set; }

        /// <inheritdoc />
        public int Index { get; internal set; }

        /// <inheritdoc />
        public string Path { get; internal set; }

        /// <inheritdoc />
        public int? Default { get; internal set; }

        /// <inheritdoc />
        public int? Forced { get; internal set; }

        /// <inheritdoc />
        public string PixelFormat { get; internal set; }

        /// <inheritdoc />
        public string Build()
        {
            var builder = new StringBuilder();
            builder.Append(string.Join(" ", _parameters));
            builder.Append(BuildVideoCodec());
            builder.Append(_bitsreamFilter);
            builder.Append(_bitrate);
            builder.Append(_framerate);
            builder.Append(_frameCount);
            builder.Append(_loop);
            builder.Append(_split);
            builder.Append(_reverse);
            builder.Append(_rotate);
            builder.Append(_size);
            builder.Append(_flags);
            return builder.ToString();
        }

        /// <inheritdoc />
        public string BuildInputArguments()
        {
            return _seek;
        }

        /// <inheritdoc />
        public string BuildVideoCodec()
        {
            if (_codec != null)
                return $"-c:v {_codec.ToString()} ";
            else
                return string.Empty;
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
        public StreamType StreamType => StreamType.Video;

        /// <inheritdoc />
        public long Bitrate { get; internal set; }

        /// <inheritdoc />
        public IVideoStream CopyStream()
        {
            return SetCodec(VideoCodec.copy);
        }

        /// <inheritdoc />
        public IVideoStream SetLoop(int count, int delay)
        {
            _loop = $"-loop {count} ";
            if (delay > 0)
            {
                _loop += $"-final_delay {delay / 100} ";
            }
            return this;
        }

        /// <inheritdoc />
        public IVideoStream AddSubtitles(string subtitlePath, VideoSize originalSize, string encode, string style)
        {
            return BuildSubtitleFilter(subtitlePath, originalSize, encode, style);
        }

        /// <inheritdoc />
        public IVideoStream AddSubtitles(string subtitlePath, string encode, string style)
        {
            return BuildSubtitleFilter(subtitlePath, null, encode, style);
        }

        private IVideoStream BuildSubtitleFilter(string subtitlePath, VideoSize? originalSize, string encode, string style)
        {
            string filter = $"'{subtitlePath}'".Replace("\\", "\\\\")
                                               .Replace(":", "\\:");
            if (!string.IsNullOrEmpty(encode))
            {
                filter += $":charenc={encode}";
            }
            if (!string.IsNullOrEmpty(style))
            {
                filter += $":force_style=\'{style}\'";
            }
            if (originalSize != null)
            {
                filter += $":original_size={originalSize.Value.ToFFmpegFormat()}";
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
        public IVideoStream SetBitrate(long bitrate)
        {
            _bitrate = $"-b:v {bitrate} ";
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetFlags(params Flag[] flags)
        {
            return SetFlags(flags.Select(x => x.ToString()).ToArray());
        }

        /// <inheritdoc />
        public IVideoStream SetFlags(params string[] flags)
        {
            var input = string.Join("+", flags);
            if(input[0] != '+')
            {
                input = "+" + input;
            }
            _flags = $"-flags {input} ";
            return this;
        }


        /// <inheritdoc />
        public IVideoStream SetFramerate(double framerate)
        {
            _framerate = $"-r {string.Format(CultureInfo.GetCultureInfo("en-US"), "{0}", framerate)} ";
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetSize(VideoSize size)
        {
            _size = $"-s {size.ToFFmpegFormat()} ";
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetSize(int width, int height)
        {
            _size = $"-s {width}x{height} ";
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetCodec(VideoCodec codec)
        {
            string input = codec.ToString();
            if(codec == VideoCodec._8bps)
            {
                input = "8bps";
            }
            else if (codec == VideoCodec._4xm)
            {
                input = "4xm";
            }
            else if (codec == VideoCodec._012v)
            {
                input = "012v";
            }
            return SetCodec($"{input}");
        }

        /// <inheritdoc />
        public IVideoStream SetCodec(string codec)
        {
            _codec = codec;
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetBitstreamFilter(BitstreamFilter filter)
        {
            return SetBitstreamFilter($"{filter}");
        }

        /// <inheritdoc />
        public IVideoStream SetBitstreamFilter(string filter)
        {
            _bitsreamFilter = $"-bsf:v {filter} ";
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetSeek(TimeSpan seek)
        {
            if (seek != null)
            {
                if (seek > Duration)
                {
                    throw new ArgumentException("Seek can not be greater than video duration. Seek: " + seek.TotalSeconds  + " Duration: " + Duration.TotalSeconds );
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
        public IVideoStream SetWatermark(string imagePath, Position position)
        {
            _watermarkSource = imagePath;
            string argument = string.Empty;
            switch (position)
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

        /// <inheritdoc />
        public IEnumerable<string> GetSource()
        {
            if (!string.IsNullOrWhiteSpace(_watermarkSource))
                return new[] { Path, _watermarkSource };
            return new[] { Path };
        }
    }
}
