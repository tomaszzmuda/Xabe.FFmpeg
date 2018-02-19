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
using Xabe.FFmpeg.Model;
using Xabe.FFmpeg.Streams;

namespace Xabe.FFmpeg
{
    /// <inheritdoc />
    public partial class Conversion : IConversion
    {
        private readonly object _builderLock = new object();
        private readonly IList<string> _parameters = new List<string>();
        private readonly Dictionary<FileInfo, int> _inputFileMap = new Dictionary<FileInfo, int>();
        private readonly List<IStream> _streams = new List<IStream>();

        private IEnumerable<FieldInfo> _fields;
        private string _output;
        private string _shortestInput;
        private string _preset;
        private bool _useMultiThreads = true;

        /// <inheritdoc />
        public string Build()
        {
            lock(_builderLock)
            {
                var builder = new StringBuilder();
                builder.Append(BuildInput());
                builder.Append("-n ");
                builder.Append(BuildThreadsArgument(_useMultiThreads));
                builder.Append(_preset);
                builder.Append(_shortestInput);
                builder.Append(BuildStreamParameters());
                builder.Append(BuildFilters());
                builder.Append(BuildMap());
                builder.Append(string.Join("", _parameters));
                builder.Append(_output);
                return builder.ToString();
            }
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

        private string BuildFilters()
        {
            var builder = new StringBuilder();
            var configurations = new List<FilterConfiguration>();
            foreach(IStream stream in _streams)
            {
                if(stream is IFilterable filterable)
                {
                    configurations.AddRange(filterable.GetFilters());
                }
            }
            IEnumerable<IGrouping<string, FilterConfiguration>> filterGroups = configurations.GroupBy(configuration => configuration.FilterType);
            foreach(IGrouping<string, FilterConfiguration> filterGroup in filterGroups)
            {
                builder.Append($"{filterGroup.Key} \"");
                foreach (FilterConfiguration configuration in configurations.Where(x=>x.FilterType == filterGroup.Key))
                {
                    var values = new List<string>();
                    foreach(KeyValuePair<string, string> filter in configuration.Filters)
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

        /// <inheritdoc />
        public event ConversionProgressEventHandler OnProgress;

        /// <inheritdoc/>
        public event DataReceivedEventHandler OnDataReceived;

        /// <inheritdoc />
        public async Task<IConversionResult> Start()
        {
            return await Start(Build());
        }

        /// <inheritdoc />
        public async Task<IConversionResult> Start(CancellationToken cancellationToken)
        {
            return await Start(Build(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IConversionResult> Start(string parameters)
        {
            return await Start(parameters, new CancellationToken());
        }

        /// <inheritdoc />
        public async Task<IConversionResult> Start(string parameters, CancellationToken cancellationToken)
        {
            var ffmpeg = new FFmpeg();
            ffmpeg.OnProgress += OnProgress;
            ffmpeg.OnDataReceived += OnDataReceived;
            var result = new ConversionResult
            {
                StartTime = DateTime.Now,
                Success = await ffmpeg.RunProcess(parameters, cancellationToken),
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
            if(_fields == null)
                _fields = GetType()
                    .GetFields(BindingFlags.NonPublic |
                               BindingFlags.Instance)
                    .Where(x => x.FieldType == typeof(string));
            foreach(FieldInfo fieldinfo in _fields)
                fieldinfo.SetValue(this, null);
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
            foreach(T stream in streams)
                if(stream != null)
                    _streams.Add(stream);
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
        public IConversion UseMultiThread(bool multiThread)
        {
            _useMultiThreads = multiThread;
            return this;
        }

        /// <summary>
        /// Create arhument for ffmpeg with thread configuration
        /// </summary>
        /// <param name="multiThread">Use multi thread</param>
        /// <returns>Build parameter argument</returns>
        private static string BuildThreadsArgument(bool multiThread)
        {
            string threadCount = multiThread
                ? Environment.ProcessorCount.ToString()
                : "1";

            return $"-threads {threadCount} ";
        }

        /// <inheritdoc />
        public IConversion SetOutput(string outputPath)
        {
            OutputFilePath = outputPath;
            _output = $"\"{outputPath}\"";
            return this;
        }

        /// <inheritdoc />
        public IConversion UseShortest(bool useShortest)
        {
            _shortestInput = !useShortest ? string.Empty : "-shortest ";
            return this;
        }

        /// <summary>
        ///     Create map for included streams
        /// </summary>
        /// <returns>Map argument</returns>
        private string BuildMap()
        {
            var builder = new StringBuilder();
            foreach(IStream stream in _streams)
            {
                builder.Append($"-map {_inputFileMap[stream.Source]}:{stream.Index} ");
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
            foreach(FileInfo source in _streams.Select(x => x.Source)
                                               .Distinct())
            {
                _inputFileMap[source] = index++;
                builder.Append($"-i \"{source.FullName}\" ");
            }
            return builder.ToString();
        }

        /// <summary>
        ///     Get new instance of Conversion
        /// </summary>
        /// <returns>IConversion object</returns>
        public static IConversion New()
        {
            return new Conversion();
        }
    }
}
