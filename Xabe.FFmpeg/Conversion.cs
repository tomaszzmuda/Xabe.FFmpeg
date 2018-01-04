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
        private string _format;
        private string _copy;
        private IEnumerable<FieldInfo> _fields;
        private string _frameCount;
        private string _input;
        private string _loop;
        private string _output;
        private string _reverse;
        private string _rotate;
        private string _seek;
        private string _shortestInput;
        private string _speed;
        private string _split;
        private string _threads;
        private string _codec;

        private List<IStream> _streams = new List<IStream>();
        private List<ISubtitleStream> _subtitles = new List<ISubtitleStream>();
        private string _videoSpeed;

        private Conversion()
        {
        }

        /// <inheritdoc />
        public string Build()
        {
            lock(_builderLock)
            {
                if(_streams.Count < 1)
                {
                    return "";
                }
                string inputPath = _streams.First()
                                           .Source.FullName;
                inputPath = $"-i \"{inputPath}\" ";

                //todo: refactor this

                var builder = new StringBuilder();
                builder.Append(inputPath);
                //AddSubtitles(builder);
                builder.Append("-n ");
                builder.Append(_threads);
                builder.Append(_format);
                foreach(IStream stream in _streams)
                {
                    builder.Append(stream.Build());
                }
                builder.Append(BuildMap());
                builder.Append(_output);
                return builder.ToString();

                throw new NotImplementedException("conversion with few streams or stream from another files");
            }
        }

        /// <summary>
        /// Create map for selected streams
        /// </summary>
        /// <returns>map argument</returns>
        private string BuildMap()
        {
            var builder = new StringBuilder();
            foreach(IStream stream in _streams)
            {
                builder.Append($"-map 0:{stream.Index} ");
            }
            return builder.ToString();
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
            _subtitles.Clear();
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
        public IConversion AddStream(params IStream[] streams)
        {
            _streams.AddRange(streams);
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
            ThrowIfSubtitles();
            _audioSpeed = $"atempo={String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:N1}", MediaSpeedHelper.GetAudioSpeed(multiplication))} ";
            _videoSpeed = $"setpts={String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:N1}", MediaSpeedHelper.GetVideoSpeed(multiplication))}*PTS ";
            return this;
        }

        /// <inheritdoc />
        public IConversion Reverse()
        {
            ThrowIfSubtitles();
            _reverse = "-vf reverse -af areverse ";
            return this;
        }

        private void ThrowIfSubtitles()
        {
            if (_subtitles.Any())
            {
                throw new InvalidOperationException("Can not reverse media with subtitles");
            }
        }

        /// <inheritdoc />
        public IConversion SetSeek(TimeSpan? seek)
        {
            if(seek.HasValue)
                _seek = $"-ss {seek} ";

            return this;
        }

        /// <inheritdoc />
        public IConversion SetOutputFramesCount(int number)
        {
            _frameCount = $"-frames:v {number} ";
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
        ///     Get new instance of Conversion
        /// </summary>
        /// <returns>IConversion object</returns>
        public static IConversion New()
        {
            return new Conversion();
        }

        private void AddSubtitles(StringBuilder builder)
        {
            //if(!_subtitles.Any())
            //    return;

            //foreach(KeyValuePair<string, string> item in _subtitles)
            //    builder.Append($"-i \"{item.Value}\" ");
            //builder.Append("-map 0 ");
            //for(var i = 0; i < _subtitles.Count; i++)
            //    builder.Append($"-map {i + 1} ");
            //for(var i = 0; i < _subtitles.Count; i++)
            //    builder.Append($"-metadata:s:s:{i} language={_subtitles.ElementAt(i) .Key} ");

            //todo: subtitles
        }
    }
}
