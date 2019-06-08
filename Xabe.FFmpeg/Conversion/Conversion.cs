using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;
using Xabe.FFmpeg.Events;
using Xabe.FFmpeg.Model;
using Xabe.FFmpeg.Streams;

namespace Xabe.FFmpeg
{
    /// <inheritdoc />
    public partial class Conversion : IConversion
    {
        private readonly object _builderLock = new object();
        private readonly Dictionary<string, int> _inputFileMap = new Dictionary<string, int>();
        private readonly IList<string> _parameters = new List<string>();
        private readonly List<IStream> _streams = new List<IStream>();

        private IEnumerable<FieldInfo> _fields;
        private string _output;
        private string _preset;
        private string _hardwareAcceleration;
        private bool _overwriteOutput;
        private string _shortestInput;
        private string _seek;
        private bool _useMultiThreads = true;
        private int? _threadsCount;
        private ProcessPriorityClass? _priority = null;
        private FFmpegWrapper _ffmpeg;

        private Func<string, string> _buildOutputFileName = null; 
        /// <inheritdoc />
        public string Build()
        {
            lock (_builderLock)
            {
                var builder = new StringBuilder();

                if (_buildOutputFileName == null)
                    _buildOutputFileName = (number) => { return _output; };

                builder.Append(_hardwareAcceleration);
                builder.Append(BuildInputParameters());
                builder.Append(BuildInput());
                builder.Append(BuildOverwriteOutputParameter(_overwriteOutput));
                builder.Append(BuildThreadsArgument(_useMultiThreads));
                builder.Append(BuildConversionParameters());
                builder.Append(BuildStreamParameters());
                builder.Append(BuildFilters());
                builder.Append(BuildMap());
                builder.Append(string.Join(string.Empty, _parameters));
                builder.Append(_buildOutputFileName("_%03d"));

                return builder.ToString();
            }
        }

        /// <inheritdoc />
        public event ConversionProgressEventHandler OnProgress;

        /// <inheritdoc />
        public event DataReceivedEventHandler OnDataReceived;

        /// <inheritdoc />
        public int? FFmpegProcessId => _ffmpeg?.FFmpegProcessId;

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
            _ffmpeg.Priority = _priority;
            _ffmpeg.OnProgress += OnProgress;
            _ffmpeg.OnDataReceived += OnDataReceived;
            var result = new ConversionResult
            {
                StartTime = DateTime.Now,
                Success = await _ffmpeg.RunProcess(parameters, cancellationToken).ConfigureAwait(false),
                EndTime = DateTime.Now,
                MediaInfo = new Lazy<IMediaInfo>(() => MediaInfo.Get(OutputFilePath)
                                                                .Result),
                Arguments = parameters
            };
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
        public IConversion AddParameter(string parameter)
        {
            _parameters.Add($"{parameter.Trim()} ");
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
        public string OutputFilePath { get; private set; }

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
        public IConversion SetVideoBitrate(string bitrate)
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
        public IConversion SetAudioBitrate(string bitrate)
        {
            AddParameter(string.Format("-b:a {0}", bitrate));
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
            AddParameter("-vsync vfr");
            
            return this;
        }

        /// <inheritdoc />
        public IConversion ExtractNthFrame(int frameNo, Func<string, string> buildOutputFileName)
        {
            _buildOutputFileName = buildOutputFileName;
            AddParameter(string.Format("-vf select='eq(n\\,{0})'", frameNo));
            AddParameter("-vsync 0");
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

        /// <inheritdoc />
        public IConversion SetOverwriteOutput(bool overwrite)
        {
            _overwriteOutput = overwrite;
            return this;
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
                    VideoCodec codec = ((VideoStream)s).Codec;
                    if (codec.ToString() == VideoCodec.Libx264.ToString() ||
                        codec.ToString() == VideoCodec.H264.ToString())
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
            _hardwareAcceleration = $"-hwaccel {hardwareAccelerator.ToString()} -c:v {decoder} ";
            AddParameter($"-c:v {encoder?.ToString()} ");

            if (device != 0)
            {
                _hardwareAcceleration += $"-hwaccel_device {device} ";
            }
            UseMultiThread(false);
            return this;
        }
    }
}
