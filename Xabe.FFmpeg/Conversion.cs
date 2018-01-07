using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg
{
    /// <inheritdoc />
    public partial class Conversion: IConversion
    {
        private readonly object _builderLock = new object();
        private readonly IList<string> _parameters = new List<string>();
        private string _audio;
        private string _audioSpeed;
        private string _bitsreamFilter;
        private string _copy;
        private IEnumerable<FieldInfo> _fields;
        private string _format;
        private string _frameCount;
        private string _input;

        private readonly Dictionary<FileInfo, int> _inputFileMap = new Dictionary<FileInfo, int>();
        private string _loop;
        private string _output;
        private string _reverse;
        private string _rotate;
        private string _seek;
        private string _shortestInput;
        private string _speed;
        private string _split;

        private readonly List<IStream> _streams = new List<IStream>();
        private string _threads;
        private string _videoSpeed;

        private Conversion()
        {
        }

        /// <inheritdoc />
        public string Build()
        {
            lock(_builderLock)
            {
                var builder = new StringBuilder();
                builder.Append(BuildInput());
                builder.Append("-n ");
                builder.Append(_threads);
                builder.Append(_format);
                builder.Append(_split);
                foreach(IStream stream in _streams)
                    builder.Append(stream.Build());
                builder.Append(BuildMap());
                builder.Append(string.Join("", _parameters));
                builder.Append(_output);
                string command = builder.ToString();
                return command;
            }
        }

        /// <inheritdoc cref="IConversion.OnProgress" />
        public event ConversionProgressEventHandler OnProgress;

        /// <inheritdoc cref="IConversion.OnDataReceived" />
        public event DataReceivedEventHandler OnDataReceived;

        /// <inheritdoc />
        public async Task<bool> Start()
        {
            return await Start(Build());
        }

        /// <inheritdoc />
        public async Task<bool> Start(CancellationToken cancellationToken)
        {
            return await Start(Build(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> Start(string parameters)
        {
            return await Start(parameters, new CancellationToken());
        }

        /// <inheritdoc />
        public async Task<bool> Start(string parameters, CancellationToken cancellationToken)
        {
            var ffmpeg = new FFmpeg();
            ffmpeg.OnProgress += OnProgress;
            ffmpeg.OnDataReceived += OnDataReceived;
            bool processing = await ffmpeg.RunProcess(parameters, cancellationToken);
            return processing;
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
        public IConversion Rotate(RotateDegrees rotateDegrees)
        {
            if(rotateDegrees == RotateDegrees.Invert)
                _rotate = "-vf \"transpose=2,transpose=2\" ";
            else
                _rotate = $"-vf \"transpose={(int) rotateDegrees}\" ";
            return this;
        }

        /// <inheritdoc />
        public IConversion Split(TimeSpan startTime, TimeSpan duration)
        {
            _split = $"-ss {startTime} -t {duration} ";
            return this;
        }

        /// <inheritdoc />
        public IConversion AddSubtitle(string subtitlePath, string language)
        {
            //_subtitles.Add(language, subtitlePath);
            //todo: subtitles
            return this;
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
        public IConversion UseMultiThread(bool multiThread)
        {
            string threadCount = multiThread
                ? Environment.ProcessorCount.ToString()
                : "1";

            _threads = $"-threads {threadCount} ";
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
        public IConversion SetFormat(MediaFormat format)
        {
            _format = $"-f {format} ";
            return this;
        }

        /// <inheritdoc />
        public IConversion StreamCopy(Channel type)
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
        public IConversion ChangeSpeed(double multiplication)
        {
            _audioSpeed = $"atempo={string.Format(CultureInfo.GetCultureInfo("en-US"), "{0:N1}", MediaSpeedHelper.GetAudioSpeed(multiplication))} ";
            _videoSpeed = $"setpts={string.Format(CultureInfo.GetCultureInfo("en-US"), "{0:N1}", MediaSpeedHelper.GetVideoSpeed(multiplication))}*PTS ";
            return this;
        }

        /// <inheritdoc />
        public IConversion Reverse()
        {
            _reverse = "-vf reverse -af areverse ";
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
        public IConversion Concatenate(params string[] paths)
        {
            string tmpFile = Path.GetTempFileName();
            File.WriteAllLines(tmpFile, paths.Select(x => $"file '{x}'"));

            _input = $"-f concat -safe 0 -i \"{tmpFile}\" -c copy ";
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
                builder.Append($"-map {_inputFileMap[stream.Source]}:{stream.Index} ");
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
