using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Xabe.FFmpeg.Streams;

namespace Xabe.FFmpeg
{
    /// <inheritdoc cref="IAudioStream" />
    public class AudioStream : IAudioStream, IFilterable
    {
        private readonly ParametersList<ConversionParameter> _parameters = new ParametersList<ConversionParameter>();
        private readonly Dictionary<string, string> _audioFilters = new Dictionary<string, string>();

        internal AudioStream()
        {

        }

        /// <inheritdoc />
        public IAudioStream Reverse()
        {
            _parameters.Add(new ConversionParameter("-af areverse"));
            return this;
        }

        /// <inheritdoc />
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
        public IAudioStream Split(TimeSpan startTime, TimeSpan duration)
        {
            _parameters.Add(new ConversionParameter($"-ss {startTime.ToFFmpeg()}"));
            _parameters.Add(new ConversionParameter($"-t {duration.ToFFmpeg()}"));
            return this;
        }

        /// <inheritdoc />
        public IAudioStream CopyStream()
        {
            return this.SetCodec(AudioCodec.copy);
        }

        /// <inheritdoc />
        public StreamType StreamType => StreamType.Audio;

        /// <inheritdoc />
        public IAudioStream SetChannels(int channels)
        {
            _parameters.Add(new ConversionParameter($"-ac:{Index} {channels}"));
            return this;
        }

        /// <inheritdoc />
        public IAudioStream SetBitstreamFilter(BitstreamFilter filter)
        {
            return SetBitstreamFilter($"{filter}");
        }

        /// <inheritdoc />
        public IAudioStream SetBitstreamFilter(string filter)
        {
            _parameters.Add(new ConversionParameter($"-bsf:a {filter}"));
            return this;
        }

        /// <inheritdoc />
        public IAudioStream SetBitrate(long bitRate)
        {
            _parameters.Add(new ConversionParameter($"-b:a:{Index} {bitRate}"));
            return this;
        }

        /// <inheritdoc />
        public IAudioStream SetBitrate(long minBitrate, long maxBitrate, long bufferSize)
        {
            _parameters.Add(new ConversionParameter($"-b:a:{Index} {minBitrate}"));
            _parameters.Add(new ConversionParameter($"-maxrate {maxBitrate}"));
            _parameters.Add(new ConversionParameter($"-bufsize {bufferSize}"));
            return this;
        }

        /// <inheritdoc />
        public IAudioStream SetSampleRate(int sampleRate)
        {
            _parameters.Add(new ConversionParameter($"-ar:{Index} {sampleRate}"));
            return this;
        }

        /// <inheritdoc />
        public IAudioStream ChangeSpeed(double multiplication)
        {
            _audioFilters["atempo"] = $"{GetAudioSpeed(multiplication)}";
            return this;
        }

        private string GetAudioSpeed(double multiplication)
        {
            if (multiplication < 0.5 || multiplication > 2.0)
            {
                throw new ArgumentOutOfRangeException(nameof(multiplication), "Value has to be greater than 0.5 and less than 2.0.");
            }

            return $"{multiplication.ToFFmpegFormat()} ";
        }

        /// <inheritdoc />
        public IAudioStream SetCodec(AudioCodec codec)
        {
            string input = codec.ToString();
            if (codec == AudioCodec._4gv)
            {
                input = "4gv";
            }
            else if (codec == AudioCodec._8svx_exp)
            {
                input = "8svx_exp";
            }
            else if (codec == AudioCodec._8svx_fib)
            {
                input = "8svx_fib";
            }
            return SetCodec($"{input}");
        }

        /// <inheritdoc />
        public IAudioStream SetCodec(string codec)
        {
            _parameters.Add(new ConversionParameter($"-c:a {codec.ToString()} "));
            return this;
        }

        /// <inheritdoc />
        public int Index { get; internal set; }

        /// <inheritdoc />
        public TimeSpan Duration { get; internal set; }

        /// <inheritdoc />
        public string Codec { get; internal set; }

        /// <inheritdoc />
        public long Bitrate { get; internal set; }

        /// <inheritdoc />
        public int Channels { get; internal set; }

        /// <inheritdoc />
        public int SampleRate { get; internal set; }

        /// <inheritdoc />
        public string Language { get; internal set; }

        /// <inheritdoc />
        public string Title { get; internal set; }

        /// <inheritdoc />
        public int? Default { get; internal set; }

        /// <inheritdoc />
        public int? Forced { get; internal set; }

        /// <inheritdoc />
        public IEnumerable<string> GetSource()
        {
            return new[] { Path };
        }

        /// <inheritdoc />
        public string Path { get; set; }

        /// <inheritdoc />
        public IAudioStream SetSeek(TimeSpan? seek)
        {
            _parameters.Add(new ConversionParameter($"-ss {seek.Value.ToFFmpeg()}", ParameterPosition.PreInput));
            return this;
        }

        /// <inheritdoc />
        public IEnumerable<IFilterConfiguration> GetFilters()
        {
            if (_audioFilters.Any())
            {
                yield return new FilterConfiguration
                {
                    FilterType = "-filter:a",
                    StreamNumber = Index,
                    Filters = _audioFilters
                };
            }
        }

        /// <inheritdoc />
        public IAudioStream SetInputFormat(string inputFormat)
        {
            _parameters.Add(new ConversionParameter($"-f {inputFormat}", ParameterPosition.PreInput));
            return this;
        }

        /// <inheritdoc />
        public IAudioStream SetInputFormat(Format inputFormat)
        {
            return this.SetInputFormat(inputFormat.ToString());
        }

        /// <inheritdoc />
        public IAudioStream UseNativeInputRead(bool readInputAtNativeFrameRate)
        {
            _parameters.Add(new ConversionParameter($"-re", ParameterPosition.PreInput));
            return this;
        }

        /// <inheritdoc />
        public IAudioStream SetStreamLoop(int loopCount)
        {
            _parameters.Add(new ConversionParameter($"-stream_loop {loopCount}", ParameterPosition.PreInput));
            return this;
        }
    }
}
