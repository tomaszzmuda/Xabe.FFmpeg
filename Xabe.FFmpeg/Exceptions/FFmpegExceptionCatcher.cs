using System;
using System.Collections.Generic;

namespace Xabe.FFmpeg.Exceptions
{
    internal class ExceptionCheck
    {
        private string _searchPhrase;
        private bool _ContainsFileIsEmptyMessage;
        public ExceptionCheck(string searchPhrase, bool ContainsFileIsEmptyMessage = false)
        {
            _searchPhrase = searchPhrase;
            _ContainsFileIsEmptyMessage = ContainsFileIsEmptyMessage;
        }

        /// <summary>
        /// Checks outputlog and throws Exception - some errors are only fatal if the text "Output file is empty" is found in the log
        /// </summary>
        /// <param name="log"></param>
        internal bool CheckLog(string log)
        {
            if (log.Contains(this._searchPhrase) && (!this._ContainsFileIsEmptyMessage || log.Contains("Output file is empty")))
            {
                return true;
            }
            return false;
        }

    }

    internal class FFmpegExceptionCatcher
    {
        private static Dictionary<ExceptionCheck, Action<string, string>> Checks = new Dictionary<ExceptionCheck, Action<string, string>>();

        internal FFmpegExceptionCatcher()
        {
            Checks.Add(new ExceptionCheck("Invalid NAL unit size"), (output, args) => throw new ConversionException(output, args));
            Checks.Add(new ExceptionCheck("Packet mismatch", true), (output, args) => throw new ConversionException(output, args));

            Checks.Add(new ExceptionCheck("asf_read_pts failed", true), (output, args) => throw new UnknownDecoderException(output, args));
            Checks.Add(new ExceptionCheck("Missing key frame while searching for timestamp", true), (output, args) => throw new UnknownDecoderException(output, args));
            Checks.Add(new ExceptionCheck("Old interlaced mode is not supported", true), (output, args) => throw new UnknownDecoderException(output, args));
            Checks.Add(new ExceptionCheck("mpeg1video", true), (output, args) => throw new UnknownDecoderException(output, args));
            Checks.Add(new ExceptionCheck("Frame rate very high for a muxer not efficiently supporting it", true), (output, args) => throw new UnknownDecoderException(output, args));
            Checks.Add(new ExceptionCheck("multiple fourcc not supported"), (output, args) => throw new UnknownDecoderException(output, args));
            Checks.Add(new ExceptionCheck("Unknown decoder"), (output, args) => throw new UnknownDecoderException(output, args));
            Checks.Add(new ExceptionCheck("Failed to open codec in avformat_find_stream_info"), (output, args) => throw new UnknownDecoderException(output, args));

            Checks.Add(new ExceptionCheck("Unrecognized hwaccel: "), (output, args) => throw new HardwareAcceleratorNotFoundException(output, args));

            Checks.Add(new ExceptionCheck("Unable to find a suitable output format"), (output, args) => throw new FFmpegNoSuitableOutputFormatFoundException(output, args));
        }

        internal void CatchFFmpegErrors(string output, string args)
        {
            foreach(var check in Checks)
            {
                try
                {
                    if (check.Key.CheckLog(output))
                        check.Value(output, args);
                }
                catch(FFmpegNoSuitableOutputFormatFoundException e)
                {
                    throw new ConversionException(e.Message, e, e.InputParameters);
                }
            }
        }
    }
}
