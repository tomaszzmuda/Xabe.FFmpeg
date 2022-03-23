using System;
using System.Collections.Generic;

namespace Xabe.FFmpeg.Exceptions
{
    internal class ExceptionCheck
    {
        private readonly string _searchPhrase;
        private readonly bool _containsFileIsEmptyMessage;
        public ExceptionCheck(string searchPhrase, bool containsFileIsEmptyMessage = false)
        {
            _searchPhrase = searchPhrase;
            _containsFileIsEmptyMessage = containsFileIsEmptyMessage;
        }

        /// <summary>
        /// Checks outputlog and throws Exception - some errors are only fatal if the text "Output file is empty" is found in the log
        /// </summary>
        /// <param name="log"></param>
        internal bool CheckLog(string log)
        {
            return log.Contains(_searchPhrase) && (!_containsFileIsEmptyMessage || log.Contains("Output file is empty"));
        }
    }

    internal class FFmpegExceptionCatcher
    {
        private static readonly Dictionary<ExceptionCheck, Action<string, string>> _checks = new Dictionary<ExceptionCheck, Action<string, string>>();

        static FFmpegExceptionCatcher()
        {
            _checks.Add(new ExceptionCheck("Invalid NAL unit size"), (output, args) => throw new ConversionException(output, args));
            _checks.Add(new ExceptionCheck("Packet mismatch", true), (output, args) => throw new ConversionException(output, args));

            _checks.Add(new ExceptionCheck("asf_read_pts failed", true), (output, args) => throw new UnknownDecoderException(output, args));
            _checks.Add(new ExceptionCheck("Missing key frame while searching for timestamp", true), (output, args) => throw new UnknownDecoderException(output, args));
            _checks.Add(new ExceptionCheck("Old interlaced mode is not supported", true), (output, args) => throw new UnknownDecoderException(output, args));
            _checks.Add(new ExceptionCheck("mpeg1video", true), (output, args) => throw new UnknownDecoderException(output, args));
            _checks.Add(new ExceptionCheck("Frame rate very high for a muxer not efficiently supporting it", true), (output, args) => throw new UnknownDecoderException(output, args));
            _checks.Add(new ExceptionCheck("multiple fourcc not supported"), (output, args) => throw new UnknownDecoderException(output, args));
            _checks.Add(new ExceptionCheck("Unknown decoder"), (output, args) => throw new UnknownDecoderException(output, args));
            _checks.Add(new ExceptionCheck("Failed to open codec in avformat_find_stream_info"), (output, args) => throw new UnknownDecoderException(output, args));

            _checks.Add(new ExceptionCheck("Unrecognized hwaccel: "), (output, args) => throw new HardwareAcceleratorNotFoundException(output, args));

            _checks.Add(new ExceptionCheck("Unable to find a suitable output format"), (output, args) => throw new FFmpegNoSuitableOutputFormatFoundException(output, args));

            _checks.Add(new ExceptionCheck("is not supported by the bitstream filter"), (output, args) => throw new InvalidBitstreamFilterException(output, args));
        }

        internal void CatchFFmpegErrors(string output, string args)
        {
            foreach (var check in _checks)
            {
                try
                {
                    if (check.Key.CheckLog(output))
                    {
                        check.Value(output, args);
                    }
                }
                catch (ConversionException e)
                {
                    throw new ConversionException(e.Message, e, e.InputParameters);
                }
            }
        }
    }
}
