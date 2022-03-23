using System;
using System.Collections.Generic;
using System.Linq;
using Xabe.FFmpeg.Streams;

namespace Xabe.FFmpeg
{
    /// <inheritdoc cref="IVideoStream" />
    public class VideoStream : IVideoStream, IFilterable
    {
        private readonly ParametersList<ConversionParameter> _parameters = new ParametersList<ConversionParameter>();
        private readonly Dictionary<string, string> _videoFilters = new Dictionary<string, string>();
        private string _watermarkSource;

        internal VideoStream()
        {

        }

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
        public int? Rotation { get; internal set; }

        /// <summary>
        ///     Create parameters string
        /// </summary>
        /// <param name="forPosition">Position for parameters</param>
        /// <returns>Parameters</returns>
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
        public IVideoStream ChangeSpeed(double multiplication)
        {
            _videoFilters["setpts"] = GetVideoSpeedFilter(multiplication);
            return this;
        }

        private string GetVideoSpeedFilter(double multiplication)
        {
            if (multiplication < 0.5 || multiplication > 2.0)
            {
                throw new ArgumentOutOfRangeException(nameof(multiplication), "Value has to be greater than 0.5 and less than 2.0.");
            }

            var videoMultiplicator = multiplication >= 1 ? 1 - ((multiplication - 1) / 2) : 1 + ((multiplication - 1) * -2);
            return $"{videoMultiplicator.ToFFmpegFormat()}*PTS ";
        }

        /// <inheritdoc />
        public IVideoStream Rotate(RotateDegrees rotateDegrees)
        {
            var rotate = rotateDegrees == RotateDegrees.Invert ? "-vf \"transpose=2,transpose=2\" " : $"-vf \"transpose={(int)rotateDegrees}\" ";
            _parameters.Add(new ConversionParameter(rotate));
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
            _parameters.Add(new ConversionParameter($"-loop {count}"));
            if (delay > 0)
            {
                _parameters.Add(new ConversionParameter($"-final_delay {delay / 100}"));
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
            var filter = $"'{subtitlePath}'".Replace("\\", "\\\\")
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
            _parameters.Add(new ConversionParameter($"-vf reverse"));
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetBitrate(long bitrate)
        {
            _parameters.Add(new ConversionParameter($"-b:v {bitrate}"));
            return this;
        }

        public IVideoStream SetBitrate(long minBitrate, long maxBitrate, long bufferSize)
        {
            _parameters.Add(new ConversionParameter($"-b:v {minBitrate}"));
            _parameters.Add(new ConversionParameter($"-maxrate {maxBitrate}"));
            _parameters.Add(new ConversionParameter($"-bufsize {bufferSize}"));
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
            if (input[0] != '+')
            {
                input = "+" + input;
            }

            _parameters.Add(new ConversionParameter($"-flags {input}"));
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetFramerate(double framerate)
        {
            _parameters.Add(new ConversionParameter($"-r {framerate.ToFFmpegFormat(3)}"));
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetSize(VideoSize size)
        {
            _parameters.Add(new ConversionParameter($"-s {size.ToFFmpegFormat()}"));
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetSize(int width, int height)
        {
            _parameters.Add(new ConversionParameter($"-s {width}x{height}"));
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetCodec(VideoCodec codec)
        {
            var input = codec.ToString();
            if (codec == VideoCodec._8bps)
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
            _parameters.Add(new ConversionParameter($"-c:v {codec}"));
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
            _parameters.Add(new ConversionParameter($"-bsf:v {filter}"));
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetSeek(TimeSpan seek)
        {
            if (seek != null)
            {
                if (seek > Duration)
                {
                    throw new ArgumentException("Seek can not be greater than video duration. Seek: " + seek.TotalSeconds + " Duration: " + Duration.TotalSeconds);
                }

                _parameters.Add(new ConversionParameter($"-ss {seek.ToFFmpeg()}", ParameterPosition.PreInput));
            }

            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetOutputFramesCount(int number)
        {
            _parameters.Add(new ConversionParameter($"-frames:v {number}"));
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetWatermark(string imagePath, Position position)
        {
            _watermarkSource = imagePath;
            var argument = string.Empty;
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
            _parameters.Add(new ConversionParameter($"-ss {startTime.ToFFmpeg()}"));
            _parameters.Add(new ConversionParameter($"-t {duration.ToFFmpeg()}"));
            return this;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetSource()
        {
            if (!string.IsNullOrWhiteSpace(_watermarkSource))
            {
                return new[] { Path, _watermarkSource };
            }

            return new[] { Path };
        }

        /// <inheritdoc />
        public IVideoStream SetInputFormat(Format inputFormat)
        {
            var format = inputFormat.ToString();
            switch (inputFormat)
            {
                case Format._3dostr:
                    format = "3dostr";
                    break;
                case Format._3g2:
                    format = "3g2";
                    break;
                case Format._3gp:
                    format = "3gp";
                    break;
                case Format._4xm:
                    format = "4xm";
                    break;
            }

            return SetInputFormat(format);
        }

        /// <inheritdoc />
        public IVideoStream SetInputFormat(string format)
        {
            if (format != null)
            {
                _parameters.Add(new ConversionParameter($"-f {format}", ParameterPosition.PreInput));
            }

            return this;
        }

        /// <inheritdoc />
        public IVideoStream AddParameter(string parameter, ParameterPosition parameterPosition = ParameterPosition.PostInput)
        {
            _parameters.Add(new ConversionParameter(parameter, parameterPosition));
            return this;
        }

        /// <inheritdoc />
        public IVideoStream UseNativeInputRead(bool readInputAtNativeFrameRate)
        {
            _parameters.Add(new ConversionParameter("-re", ParameterPosition.PreInput));
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetStreamLoop(int loopCount)
        {
            _parameters.Add(new ConversionParameter($"-stream_loop {loopCount}", ParameterPosition.PreInput));
            return this;
        }
    }
}
