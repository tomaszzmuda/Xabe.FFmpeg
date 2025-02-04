using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg.Events;
using Xabe.FFmpeg.Streams;

namespace Xabe.FFmpeg
{
    /// <inheritdoc />
    public partial class Conversion : IConversion
    {
        private readonly object _builderLock = new object();
        private readonly Dictionary<string, int> _inputFileMap = new Dictionary<string, int>();
        private readonly ParametersList<ConversionParameter> _parameters = new ParametersList<ConversionParameter>();
        private readonly IDictionary<ParameterPosition, List<string>> _userDefinedParameters = new Dictionary<ParameterPosition, List<string>>();
        private readonly List<IStream> _streams = new List<IStream>();

        private string _output;
        private bool _hasInputBuilder = false;

        private ProcessPriorityClass? _priority = null;
        private FFmpegWrapper _ffmpeg;
        private Func<string, string> _buildInputFileName = null;
        private Func<string, string> _buildOutputFileName = null;

        public Conversion()
        {
            _userDefinedParameters[ParameterPosition.PostInput] = new List<string>();
            _userDefinedParameters[ParameterPosition.PreInput] = new List<string>();
        }

        /// <inheritdoc />
        public string Build()
        {
            lock (_builderLock)
            {
                var builder = new StringBuilder();

                if (_buildOutputFileName == null)
                {
                    _buildOutputFileName = (number) => { return _output; };
                }

                builder.Append(string.Join(" ", _userDefinedParameters[ParameterPosition.PreInput].Select(x => x.Trim())) + " ");
                builder.Append(GetParameters(ParameterPosition.PreInput));
                builder.Append(GetStreamsPreInputs());

                if (_buildInputFileName == null)
                {
                    builder.Append(GetInputs());
                }
                else
                {
                    _hasInputBuilder = true;
                    builder.Append(_buildInputFileName("_%03d"));
                    builder.Append(GetInputs());
                }

                builder.Append(GetStreamsPostInputs());
                builder.Append(GetFilters());
                builder.Append(GetMap());
                builder.Append(GetParameters(ParameterPosition.PostInput));
                builder.Append(string.Join(" ", _userDefinedParameters[ParameterPosition.PostInput].Select(x => x.Trim())) + " ");
                builder.Append(_buildOutputFileName("_%03d"));

                return builder.ToString();
            }
        }

        /// <inheritdoc />
        public event ConversionProgressEventHandler OnProgress;

        /// <inheritdoc />
        public event DataReceivedEventHandler OnDataReceived;

        /// <inheritdoc />
        public event VideoDataEventHandler OnVideoDataReceived;

        /// <inheritdoc />
        public string OutputFilePath { get; private set; }

        /// <inheritdoc />
        public PipeDescriptor? OutputPipeDescriptor { get; private set; }

        /// <inheritdoc />
        public IEnumerable<IStream> Streams => _streams;

        /// <inheritdoc />
        public Task<IConversionResult> Start()
        {
            return Start(Build());
        }

        /// <inheritdoc />
        public Task<IConversionResult> Start(CancellationToken cancellationToken)
        {
            return Start(Build(), cancellationToken);
        }

        /// <inheritdoc />
        public Task<IConversionResult> Start(string parameters)
        {
            return Start(parameters, new CancellationToken());
        }

        /// <inheritdoc />
        public async Task<IConversionResult> Start(string parameters, CancellationToken cancellationToken)
        {
            if (_ffmpeg != null)
            {
                throw new InvalidOperationException("Conversion has already been started. ");
            }

            DateTime startTime = DateTime.Now;

            _ffmpeg = new FFmpegWrapper();
            try
            {
                _ffmpeg.OnProgress += OnProgress;
                _ffmpeg.OnDataReceived += OnDataReceived;
                _ffmpeg.OnVideoDataReceived += OnVideoDataReceived;
                CreateOutputDirectoryIfNotExists();
                await _ffmpeg.RunProcess(parameters, cancellationToken, _priority);
            }
            finally
            {
                _ffmpeg.OnProgress -= OnProgress;
                _ffmpeg.OnDataReceived -= OnDataReceived;
                _ffmpeg.OnVideoDataReceived -= OnVideoDataReceived;
                _ffmpeg = null;
            }

            return new ConversionResult
            {
                StartTime = startTime,
                EndTime = DateTime.Now,
                Arguments = parameters
            };
        }

        private void CreateOutputDirectoryIfNotExists()
        {
            if (OutputFilePath == null || OutputPipeDescriptor != null)
            {
                return;
            }

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(OutputFilePath.Unescape())))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(OutputFilePath.Unescape()));
                }
            }
            catch (IOException)
            {
            }
        }

        /// <inheritdoc />
        public IConversion AddParameter(string parameter, ParameterPosition parameterPosition = ParameterPosition.PostInput)
        {
            _userDefinedParameters[parameterPosition].Add(parameter);
            return this;
        }

        /// <inheritdoc />
        public IConversion AddStream<T>(params T[] streams) where T : IStream
        {
            foreach (T stream in streams)
            {
                if (stream != null)
                {
                    _streams.Add(stream);
                }
            }

            return this;
        }

        /// <inheritdoc />
        public IConversion AddStream(IEnumerable<IStream> streams)
        {
            foreach (var stream in streams)
            {
                AddStream(stream);
            }

            return this;
        }

        /// <inheritdoc />
        public IConversion SetHashFormat(Hash hashFormat = Hash.SHA256)
        {
            var format = hashFormat.ToString();
            if (hashFormat == Hash.SHA512_256)
            {
                format = "SHA512/256";
            }
            else if (hashFormat == Hash.SHA512_224)
            {
                format = "SHA512/224";
            }

            SetOutputFormat(Format.hash);
            return SetHashFormat(format);
        }

        /// <inheritdoc />
        public IConversion SetHashFormat(string hashFormat)
        {
            _parameters.Add(new ConversionParameter($"-hash {hashFormat}", ParameterPosition.PostInput));
            return this;
        }

        /// <inheritdoc />
        public IConversion SetPreset(ConversionPreset preset)
        {
            _parameters.Add(new ConversionParameter($"-preset {preset.ToString().ToLower()}", ParameterPosition.PostInput));
            return this;
        }

        /// <inheritdoc />
        public IConversion SetSeek(TimeSpan? seek)
        {
            if (seek.HasValue)
            {
                _parameters.Add(new ConversionParameter($"-ss {seek.Value.ToFFmpeg()}", ParameterPosition.PostInput));
            }

            return this;
        }

        /// <inheritdoc />
        public IConversion SetInputTime(TimeSpan? time)
        {
            if (time.HasValue)
            {
                _parameters.Add(new ConversionParameter($"-t {time.Value.ToFFmpeg()}", ParameterPosition.PreInput));
            }

            return this;
        }

        /// <inheritdoc />
        public IConversion SetOutputTime(TimeSpan? time)
        {
            if (time.HasValue)
            {
                _parameters.Add(new ConversionParameter($"-t {time.Value.ToFFmpeg()}", ParameterPosition.PostInput));
            }

            return this;
        }

        /// <inheritdoc />
        public IConversion UseMultiThread(bool multiThread)
        {
            var threads = multiThread ? Environment.ProcessorCount : 1;
            _parameters.Add(new ConversionParameter($"-threads {Math.Min(threads, 16)}"));
            return this;
        }

        /// <inheritdoc />
        public IConversion UseMultiThread(int threadsCount)
        {
            _parameters.Add(new ConversionParameter($"-threads {threadsCount}"));
            return this;
        }

        /// <inheritdoc />
        public IConversion SetOutput(string outputPath)
        {
            OutputFilePath = new FileInfo(outputPath).FullName;
            _output = outputPath.Escape();
            return this;
        }

        /// <inheritdoc />
        public IConversion PipeOutput(PipeDescriptor descriptor = PipeDescriptor.stdout)
        {
            SetOutput($"pipe:{descriptor}");
            OutputPipeDescriptor = descriptor;
            return this;
        }

        /// <inheritdoc />
        public IConversion SetVideoBitrate(long bitrate)
        {
            _parameters.Add(new ConversionParameter($"-b:v {bitrate}", ParameterPosition.PostInput));
            _parameters.Add(new ConversionParameter($"-minrate {bitrate}", ParameterPosition.PostInput));
            _parameters.Add(new ConversionParameter($"-maxrate {bitrate}", ParameterPosition.PostInput));
            _parameters.Add(new ConversionParameter($"-bufsize {bitrate}", ParameterPosition.PostInput));

            if (HasH264Stream())
            {
                _parameters.Add(new ConversionParameter($"-x264opts nal-hrd=cbr:force-cfr=1", ParameterPosition.PostInput));
            }

            return this;
        }

        /// <inheritdoc />
        public IConversion SetAudioBitrate(long bitrate)
        {
            _parameters.Add(new ConversionParameter($"-b:a {bitrate}", ParameterPosition.PostInput));
            return this;
        }

        /// <inheritdoc />
        public IConversion UseShortest(bool useShortest)
        {
            if (useShortest)
            {
                _parameters.Add(new ConversionParameter($"-shortest", ParameterPosition.PostInput));
            }
            else
            {
                _parameters.Remove(new ConversionParameter($"-shortest", ParameterPosition.PostInput));
            }

            return this;
        }

        /// <inheritdoc />
        public IConversion SetPriority(ProcessPriorityClass? priority)
        {
            _priority = priority;
            return this;
        }

        /// <inheritdoc />
        public IConversion ExtractEveryNthFrame(int frameNo, Func<string, string> buildOutputFileName)
        {
            _buildOutputFileName = buildOutputFileName;
            OutputFilePath = buildOutputFileName("");
            _parameters.Add(new ConversionParameter($"-vf select='not(mod(n\\,{frameNo}))'", ParameterPosition.PostInput));
            SetVideoSyncMethod(VideoSyncMethod.vfr);

            return this;
        }

        /// <inheritdoc />
        public IConversion ExtractNthFrame(int frameNo, Func<string, string> buildOutputFileName)
        {
            _buildOutputFileName = buildOutputFileName;
            _parameters.Add(new ConversionParameter($"-vf select='eq(n\\,{frameNo})'", ParameterPosition.PostInput));
            OutputFilePath = buildOutputFileName("");
            SetVideoSyncMethod(VideoSyncMethod.passthrough);
            return this;
        }

        /// <inheritdoc />
        public IConversion BuildVideoFromImages(int startNumber, Func<string, string> buildInputFileName)
        {
            _buildInputFileName = buildInputFileName;
            _parameters.Add(new ConversionParameter($"-start_number {startNumber}", ParameterPosition.PreInput));
            return this;
        }

        /// <inheritdoc />
        public IConversion BuildVideoFromImages(IEnumerable<string> imageFiles)
        {
            var builder = new InputBuilder();
            _buildInputFileName = builder.PrepareInputFiles(imageFiles.ToList(), out _);

            return this;
        }

        /// <inheritdoc />
        public IConversion SetInputFrameRate(double frameRate)
        {
            _parameters.Add(new ConversionParameter($"-framerate {frameRate.ToFFmpegFormat(3)}", ParameterPosition.PreInput));
            _parameters.Add(new ConversionParameter($"-r {frameRate.ToFFmpegFormat(3)}", ParameterPosition.PreInput));
            return this;
        }

        /// <inheritdoc />
        public IConversion SetFrameRate(double frameRate)
        {
            _parameters.Add(new ConversionParameter($"-framerate {frameRate.ToFFmpegFormat(3)}", ParameterPosition.PostInput));
            _parameters.Add(new ConversionParameter($"-r {frameRate.ToFFmpegFormat(3)}", ParameterPosition.PostInput));
            return this;
        }

        private string GetStreamsPostInputs()
        {
            var builder = new StringBuilder();
            foreach (IStream stream in _streams)
            {
                builder.Append(stream.BuildParameters(ParameterPosition.PostInput));
            }

            return builder.ToString();
        }

        private string GetStreamsPreInputs()
        {
            var builder = new StringBuilder();
            foreach (IStream stream in _streams)
            {
                builder.Append(stream.BuildParameters(ParameterPosition.PreInput));
            }

            return builder.ToString();
        }

        private string GetFilters()
        {
            var builder = new StringBuilder();
            var configurations = new List<IFilterConfiguration>();
            foreach (IStream stream in _streams)
            {
                if (stream is IFilterable filterable)
                {
                    configurations.AddRange(filterable.GetFilters());
                }
            }

            IEnumerable<IGrouping<string, IFilterConfiguration>> filterGroups = configurations.GroupBy(configuration => configuration.FilterType);
            foreach (IGrouping<string, IFilterConfiguration> filterGroup in filterGroups)
            {
                builder.Append($"{filterGroup.Key} \"");
                foreach (IFilterConfiguration configuration in configurations.Where(x => x.FilterType == filterGroup.Key))
                {
                    var values = new List<string>();
                    foreach (KeyValuePair<string, string> filter in configuration.Filters)
                    {
                        var map = $"[{configuration.StreamNumber}]";
                        var value = string.IsNullOrEmpty(filter.Value) ? $"{filter.Key} " : $"{filter.Key}={filter.Value}";
                        values.Add($"{map} {value} ");
                    }

                    builder.Append(string.Join(";", values));
                }

                builder.Append("\" ");
            }

            return builder.ToString();
        }

        /// <summary>
        ///     Create map for included streams, including the InputBuilder if required
        /// </summary>
        /// <returns>Map argument</returns>
        private string GetMap()
        {
            var builder = new StringBuilder();
            foreach (IStream stream in _streams)
            {
                if (_hasInputBuilder) // If we have an input builder we always want to map the first video stream as it will be created by our input builder
                {
                    builder.Append($"-map 0:0 ");
                }

                foreach (var source in stream.GetSource())
                {
                    if (_hasInputBuilder)
                    {
                        // If we have an input builder we need to add one to the input file index to account for the input created by our input builder.
                        builder.Append($"-map {_inputFileMap[source] + 1}:{stream.Index} ");
                    }
                    else
                    {
                        builder.Append($"-map {_inputFileMap[source]}:{stream.Index} ");
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>
        ///     Create parameters string
        /// </summary>
        /// <param name="forPosition">Position for parameters</param>
        /// <returns>Parameters</returns>
        private string GetParameters(ParameterPosition forPosition)
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

        /// <summary>
        ///     Create input string for all streams
        /// </summary>
        /// <returns>Input argument</returns>
        private string GetInputs()
        {
            var builder = new StringBuilder();
            var index = 0;
            foreach (var source in _streams.SelectMany(x => x.GetSource()).Distinct())
            {
                _inputFileMap[source] = index++;
                builder.Append($"-i {source.Escape()} ");
            }

            return builder.ToString();
        }

        private bool HasH264Stream()
        {
            foreach (IStream stream in _streams)
            {
                if (stream is IVideoStream s)
                {
                    if (s.Codec == "libx264" ||
                        s.Codec == VideoCodec.h264.ToString())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static IConversion New()
        {
            var conversion = new Conversion();
            return conversion
                .SetOverwriteOutput(false);
        }

        /// <inheritdoc />
        public IConversion UseHardwareAcceleration(HardwareAccelerator hardwareAccelerator, VideoCodec decoder, VideoCodec encoder, int device = 0)
        {
            return UseHardwareAcceleration($"{hardwareAccelerator}", decoder.ToString(), encoder.ToString(), device);
        }

        /// <inheritdoc />
        public IConversion UseHardwareAcceleration(string hardwareAccelerator, string decoder, string encoder, int device = 0)
        {
            _parameters.Add(new ConversionParameter($"-hwaccel {hardwareAccelerator}", ParameterPosition.PreInput));
            _parameters.Add(new ConversionParameter($"-c:v {decoder}", ParameterPosition.PreInput));

            _parameters.Add(new ConversionParameter($"-c:v {encoder?.ToString()}", ParameterPosition.PostInput));

            if (device != 0)
            {
                _parameters.Add(new ConversionParameter($"-hwaccel_device {device}", ParameterPosition.PreInput));
            }

            UseMultiThread(false);
            return this;
        }

        /// <inheritdoc />
        public IConversion SetOverwriteOutput(bool overwrite)
        {
            if (overwrite)
            {
                _parameters.Add(new ConversionParameter($"-y", ParameterPosition.PostInput));
                _parameters.Remove(new ConversionParameter($"-n", ParameterPosition.PostInput));
            }
            else
            {
                _parameters.Remove(new ConversionParameter($"-y", ParameterPosition.PostInput));
                _parameters.Add(new ConversionParameter($"-n", ParameterPosition.PostInput));
            }

            return this;
        }

        /// <inheritdoc />
        public IConversion SetInputFormat(Format inputFormat)
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
                default:
                    break;
            }

            return SetInputFormat(format);
        }

        /// <inheritdoc />
        public IConversion SetInputFormat(string format)
        {
            if (format != null)
            {
                _parameters.Add(new ConversionParameter($"-f {format}", ParameterPosition.PreInput));
            }

            return this;
        }

        /// <inheritdoc />
        public IConversion SetOutputFormat(Format outputFormat)
        {
            var format = outputFormat.ToString();
            switch (outputFormat)
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
                default:
                    break;
            }

            return SetOutputFormat(format);
        }

        /// <inheritdoc />
        public IConversion SetOutputFormat(string format)
        {
            if (format != null)
            {
                _parameters.Add(new ConversionParameter($"-f {format}", ParameterPosition.PostInput));
            }

            return this;
        }

        /// <inheritdoc />
        public IConversion SetPixelFormat(PixelFormat pixelFormat)
        {
            var format = pixelFormat.ToString();
            switch (pixelFormat)
            {
                case PixelFormat._0bgr:
                    format = "0bgr";
                    break;
                case PixelFormat._0rgb:
                    format = "0rgb";
                    break;
                default:
                    break;
            }

            return SetPixelFormat(format);
        }

        /// <inheritdoc />
        public IConversion SetPixelFormat(string pixelFormat)
        {
            if (pixelFormat != null)
            {
                _parameters.Add(new ConversionParameter($"-pix_fmt {pixelFormat}", ParameterPosition.PostInput));
            }

            return this;
        }

        /// <inheritdoc />
        public IConversion SetVideoSyncMethod(VideoSyncMethod method)
        {
            if (method == VideoSyncMethod.auto)
            {
                _parameters.Add(new ConversionParameter($"-vsync -1", ParameterPosition.PostInput));
            }
            else
            {
                _parameters.Add(new ConversionParameter($"-vsync {method}", ParameterPosition.PostInput));
            }

            return this;
        }

        /// <inheritdoc />
        public IConversion AddDesktopStream(string videoSize = null, double framerate = 30, int xOffset = 0, int yOffset = 0)
        {
            var stream = new VideoStream() { Index = _streams.Any() ? _streams.Max(x => x.Index) + 1 : 0 };
            stream.AddParameter($"-framerate {framerate.ToFFmpegFormat(4)}", ParameterPosition.PreInput);
            stream.AddParameter($"-offset_x {xOffset}", ParameterPosition.PreInput);
            stream.AddParameter($"-offset_y {yOffset}", ParameterPosition.PreInput);

            if (videoSize != null)
            {
                stream.AddParameter($"-video_size {videoSize}", ParameterPosition.PreInput);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                stream.SetInputFormat(Format.gdigrab);
                stream.Path = "desktop";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                stream.SetInputFormat(Format.avfoundation);
                stream.Path = "1:1";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                stream.SetInputFormat(Format.x11grab);
                stream.Path = ":0.0+0,0";
            }

            _streams.Add(stream);
            return this;
        }

        /// <inheritdoc />
        public IConversion AddDesktopStream(VideoSize videoSize, double framerate = 30, int xOffset = 0, int yOffset = 0)
        {
            return AddDesktopStream(videoSize.ToFFmpegFormat(), framerate, xOffset, yOffset);
        }
    }
}
