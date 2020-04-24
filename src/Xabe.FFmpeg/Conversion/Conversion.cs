using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg.Events;

namespace Xabe.FFmpeg
{
    /// <inheritdoc />
    public partial class Conversion : IConversion
    {
        private readonly object _builderLock = new object();
        private readonly Dictionary<string, int> _inputFileMap = new Dictionary<string, int>();
        private readonly IList<ConversionParameter> _parameters = new List<ConversionParameter>();
        private readonly List<IStream> _streams = new List<IStream>();

        private IEnumerable<FieldInfo> _fields;
        private string _output;
        private string _preset;
        private string _hashFormat;
        private string _hardwareAcceleration;
        private bool _overwriteOutput;
        private string _shortestInput;
        private string _seek;
        private bool _useMultiThreads = true;
        private bool _capturing = false;
        private int? _threadsCount;
        private string _inputTime;
        private string _outputTime;
        private string _outputFormat;
        private string _inputFormat;
        private string _outputPixelFormat;

        private ProcessPriorityClass? _priority = null;
        private FFmpegWrapper _ffmpeg;
        private Func<string, string> _buildInputFileName = null;
        private Func<string, string> _buildOutputFileName = null;

        private int? _processId = null;

        /// <inheritdoc />
        public string Build()
        {
            lock (_builderLock)
            {
                var builder = new StringBuilder();

                if (_buildOutputFileName == null)
                    _buildOutputFileName = (number) => { return _output; };

                builder.Append(_hardwareAcceleration);
                builder.Append(_inputFormat);
                builder.Append(_inputTime);
                builder.Append(BuildParameters(ParameterPosition.PreInput));

                if (!_capturing)
                {
                    builder.Append(BuildInputParameters());

                    if (_buildInputFileName == null)
                        _buildInputFileName = (number) => { return BuildInput(); };

                    builder.Append(_buildInputFileName("_%03d"));
                }

                builder.Append(BuildOverwriteOutputParameter(_overwriteOutput));
                builder.Append(BuildThreadsArgument(_useMultiThreads));
                builder.Append(BuildConversionParameters());
                builder.Append(BuildStreamParameters());
                builder.Append(BuildFilters());
                builder.Append(BuildMap());
                builder.Append(BuildParameters(ParameterPosition.PostInput));
                builder.Append(_outputTime);
                builder.Append(_outputPixelFormat);
                builder.Append(_outputFormat);
                builder.Append(_hashFormat);
                builder.Append(_buildOutputFileName("_%03d"));

                return builder.ToString();
            }
        }

        /// <inheritdoc />
        public event ConversionProgressEventHandler OnProgress;

        /// <inheritdoc />
        public event DataReceivedEventHandler OnDataReceived;

        /// <inheritdoc />
        public int? FFmpegProcessId => _processId;

        /// <inheritdoc />
        public string OutputFilePath { get; private set; }

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

            _ffmpeg = new FFmpegWrapper();
            _ffmpeg.OnProgress += OnProgress;
            _ffmpeg.OnDataReceived += OnDataReceived;
            await _ffmpeg.RunProcess(parameters, cancellationToken, _priority);
            var result = new ConversionResult
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                Arguments = parameters
            };
            _processId = null;
            return result;
        }

        /// <inheritdoc />
        public void Clear()
        {
            _parameters.Clear();
            if (_fields == null)
            {
                _fields = GetType()
                    .GetFields(BindingFlags.NonPublic |
                               BindingFlags.Instance)
                    .Where(x => x.FieldType == typeof(string));
            }
            foreach (FieldInfo fieldinfo in _fields)
            {
                fieldinfo.SetValue(this, null);
            }
        }

        /// <inheritdoc />
        public IConversion AddParameter(string parameter, ParameterPosition parameterPosition = ParameterPosition.PostInput)
        {
            _parameters.Add(new ConversionParameter
            {
                Parameter = $"{parameter.Trim()} ",
                Position = parameterPosition
            });
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
        public IConversion SetHashFormat(Hash hashFormat)
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

            return SetHashFormat(format);
        }

        /// <inheritdoc />
        public IConversion SetHashFormat(string hashFormat)
        {
            _hashFormat = $"-hash {hashFormat} ";
            return this;
        }

        /// <inheritdoc />
        public IConversion SetPreset(ConversionPreset preset)
        {
            _preset = $"-preset {preset.ToString().ToLower()} ";
            return this;
        }

        /// <inheritdoc />
        public IConversion SetSeek(TimeSpan? seek)
        {
            if (seek.HasValue)
            {
                _seek = $"-ss {seek.Value.ToFFmpeg()} ";
            }
            return this;
        }

        /// <inheritdoc />
        public IConversion SetInputTime(TimeSpan? time)
        {
            if (time.HasValue)
            {
                _inputTime = $"-t {time.Value.ToFFmpeg()} ";
            }
            return this;
        }

        /// <inheritdoc />
        public IConversion SetOutputTime(TimeSpan? time)
        {
            if (time.HasValue)
            {
                _outputTime = $"-t {time.Value.ToFFmpeg()} ";
            }
            return this;
        }

        /// <inheritdoc />
        public IConversion UseMultiThread(bool multiThread)
        {
            _useMultiThreads = multiThread;
            return this;
        }

        /// <inheritdoc />
        public IConversion UseMultiThread(int threadsCount)
        {
            UseMultiThread(true);
            _threadsCount = threadsCount;
            return this;
        }

        /// <inheritdoc />
        public IConversion SetOutput(string outputPath)
        {
            OutputFilePath = outputPath;
            _output = $"\"{outputPath}\"";
            return this;
        }

        /// <inheritdoc />
        public IConversion SetVideoBitrate(long bitrate)
        {
            AddParameter(string.Format("-b:v {0}", bitrate));
            AddParameter(string.Format("-minrate {0}", bitrate));
            AddParameter(string.Format("-maxrate {0}", bitrate));
            AddParameter(string.Format("-bufsize {0}", bitrate));

            if (HasH264Stream())
                AddParameter("-x264opts nal-hrd=cbr:force-cfr=1");

            return this;
        }

        /// <inheritdoc />
        public IConversion SetAudioBitrate(long bitrate)
        {
            AddParameter($"-b:a {bitrate} ");
            return this;
        }

        /// <inheritdoc />
        public IConversion UseShortest(bool useShortest)
        {
            _shortestInput = !useShortest ? string.Empty : "-shortest ";
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
            AddParameter(string.Format("-vf select='not(mod(n\\,{0}))'", frameNo));
            AddParameter("-vsync vfr", ParameterPosition.PostInput);

            return this;
        }

        /// <inheritdoc />
        public IConversion ExtractNthFrame(int frameNo, Func<string, string> buildOutputFileName)
        {
            _buildOutputFileName = buildOutputFileName;
            AddParameter(string.Format("-vf select='eq(n\\,{0})'", frameNo));
            AddParameter("-vsync 0", ParameterPosition.PostInput);
            return this;
        }

        /// <inheritdoc />
        public IConversion BuildVideoFromImages(int startNumber, Func<string, string> buildInputFileName)
        {
            _buildInputFileName = buildInputFileName;
            AddParameter($"-start_number {startNumber}", ParameterPosition.PreInput);
            return this;
        }

        /// <inheritdoc />
        public IConversion BuildVideoFromImages(IEnumerable<string> imageFiles)
        {
            InputBuilder builder = new InputBuilder();
            string directory = string.Empty;

            _buildInputFileName = builder.PrepareInputFiles(imageFiles.ToList(), out directory);

            return this;
        }

        /// <inheritdoc />
        public IConversion SetInputFrameRate(double frameRate)
        {
            AddParameter($"-framerate {frameRate}", ParameterPosition.PreInput);
            return this;
        }

        /// <inheritdoc />
        public IConversion SetFrameRate(double frameRate)
        {
            AddParameter($"-framerate {frameRate}", ParameterPosition.PostInput);
            return this;
        }

        /// <inheritdoc />
        public IConversion GetScreenCapture(double frameRate)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _capturing = true;

                SetInputFormat(Format.gdigrab);
                SetFrameRate(frameRate);
                AddParameter("-i desktop ", ParameterPosition.PreInput);
                SetPixelFormat(PixelFormat.yuv420p);
                AddParameter("-preset ultrafast");
                return this;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                _capturing = true;

                SetInputFormat(Format.avfoundation);
                SetFrameRate(frameRate);
                AddParameter("-i 1:1 ", ParameterPosition.PreInput);
                SetPixelFormat(PixelFormat.yuv420p);
                AddParameter("-preset ultrafast");
                return this;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _capturing = true;

                SetInputFormat(Format.x11grab);
                SetFrameRate(frameRate);
                AddParameter("-i :0.0+0,0 ", ParameterPosition.PreInput);
                SetPixelFormat(PixelFormat.yuv420p);
                AddParameter("-preset ultrafast");
                return this;
            }

            _capturing = false;
            return this;
        }

        private string BuildConversionParameters()
        {
            var builder = new StringBuilder();
            builder.Append(_preset);
            builder.Append(_shortestInput);
            builder.Append(_seek);
            return builder.ToString();
        }

        private string BuildStreamParameters()
        {
            var builder = new StringBuilder();
            foreach (IStream stream in _streams)
            {
                builder.Append(stream.Build());
            }
            return builder.ToString();
        }

        private string BuildInputParameters()
        {
            var builder = new StringBuilder();
            foreach (IStream stream in _streams)
            {
                builder.Append(stream.BuildInputArguments());
            }
            return builder.ToString();
        }

        private string BuildOverwriteOutputParameter(bool overwriteOutput)
        {
            return overwriteOutput ? "-y " : "-n ";
        }

        private string BuildFilters()
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
                        string map = $"[{configuration.StreamNumber}]";
                        string value = string.IsNullOrEmpty(filter.Value) ? $"{filter.Key} " : $"{filter.Key}={filter.Value}";
                        values.Add($"{map} {value} ");
                    }
                    builder.Append(string.Join(";", values));
                }
                builder.Append("\" ");
            }
            return builder.ToString();
        }

        /// <summary>
        ///     Create argument for ffmpeg with thread configuration
        /// </summary>
        /// <param name="multiThread">Use multi thread</param>
        /// <returns>Build parameter argument</returns>
        private string BuildThreadsArgument(bool multiThread)
        {
            string threadCount = "";
            if (_threadsCount == null)
            {
                threadCount = multiThread
                    ? Environment.ProcessorCount.ToString()
                    : "1";
            }
            else
            {
                threadCount = _threadsCount.ToString();
            }

            return $"-threads {threadCount} ";
        }

        /// <summary>
        ///     Create map for included streams
        /// </summary>
        /// <returns>Map argument</returns>
        private string BuildMap()
        {
            var builder = new StringBuilder();
            foreach (IStream stream in _streams)
            {
                foreach (var source in stream.GetSource())
                {
                    builder.Append($"-map {_inputFileMap[source]}:{stream.Index} ");
                }
            }
            return builder.ToString();
        }

        /// <summary>
        ///     Create parameters string
        /// </summary>
        /// <param name="forPosition">Position for parameters</param>
        /// <returns>Parameters</returns>
        private string BuildParameters(ParameterPosition forPosition)
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
        private string BuildInput()
        {
            var builder = new StringBuilder();
            var index = 0;
            foreach (var source in _streams.SelectMany(x => x.GetSource()).Distinct())
            {
                _inputFileMap[source] = index++;
                builder.Append($"-i \"{source}\" ");
            }
            return builder.ToString();
        }

        private bool HasH264Stream()
        {
            foreach (IStream stream in _streams)
            {
                if (stream is IVideoStream)
                {
                    IVideoStream s = (IVideoStream)stream;
                    if (s.Codec == "libx264" ||
                        s.Codec == VideoCodec.h264.ToString())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        ///     Get new instance of Conversion
        /// </summary>
        /// <returns>IConversion object</returns>
        public static IConversion New()
        {
            return new Conversion();
        }

        /// <inheritdoc />
        public IConversion UseHardwareAcceleration(HardwareAccelerator hardwareAccelerator, VideoCodec decoder, VideoCodec encoder, int device = 0)
        {
            return UseHardwareAcceleration($"{hardwareAccelerator}", decoder.ToString(), encoder.ToString(), device);
        }

        /// <inheritdoc />
        public IConversion UseHardwareAcceleration(string hardwareAccelerator, string decoder, string encoder, int device = 0)
        {
            _hardwareAcceleration = $"-hwaccel {hardwareAccelerator} -c:v {decoder} ";
            AddParameter($"-c:v {encoder?.ToString()} ");

            if (device != 0)
            {
                _hardwareAcceleration += $"-hwaccel_device {device} ";
            }
            UseMultiThread(false);
            return this;
        }


        /// <inheritdoc />
        public IConversion SetOverwriteOutput(bool overwrite)
        {
            _overwriteOutput = overwrite;
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
            }
            return SetInputFormat(format);
        }

        /// <inheritdoc />
        public IConversion SetInputFormat(string format)
        {
            if (format != null)
                _inputFormat = $"-f {format} ";
            return this;
        }

        /// <inheritdoc />
        public IConversion SetOutputFormat(Format outputFormat)
        {
            var format = outputFormat.ToString();
            switch(outputFormat)
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
            return SetOutputFormat(format);
        }

        /// <inheritdoc />
        public IConversion SetOutputFormat(string format)
        {
            if (format != null)
                _outputFormat = $"-f {format} ";
            return this;
        }

        /// <inheritdoc />
        public IConversion SetPixelFormat(PixelFormat pixelFormat)
        {
            string format = pixelFormat.ToString();
            switch(pixelFormat)
            {
                case PixelFormat._0bgr:
                    format = "0bgr";
                    break;
                case PixelFormat._0rgb:
                    format = "0rgb";
                    break;
            }
            return SetPixelFormat(format);
        }

        /// <inheritdoc />
        public IConversion SetPixelFormat(string pixelFormat)
        {
            if (pixelFormat != null)
                _outputPixelFormat = $"-pix_fmt {pixelFormat} ";
            return this;
        }
    }
}
