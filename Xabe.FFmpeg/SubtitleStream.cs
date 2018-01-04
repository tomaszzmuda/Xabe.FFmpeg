using System;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Reference to subtitle file
    /// </summary>
    public class SubtitleStream : ISubtitleStream
    {
        public SubtitleStream()
        {
            
        }

        private readonly string _path;

        /// <inheritdoc />
        public SubtitleStream(string path)
        {
            _path = path;
        }

        /// <summary>
        ///     Convert subtitle to specific format
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="subtitleFormat"></param>
        /// <returns>Conversion result</returns>
        public async Task<bool> Convert(string outputPath, SubtitleFormat subtitleFormat)
        {
            //var description = subtitleFormat.GetDescription();
            //return await Conversion.New().SetInput(_path)
            //                             .SetOutput(outputPath)
            //                             .SetFormat(new MediaFormat(description))
            //                             .Start();

            //todo: subtitles
            throw  new NotImplementedException();
        }

        /// <inheritdoc />
        public string Format { get; internal set; }

        /// <inheritdoc />
        public FileInfo Source { get; internal set; }

        /// <inheritdoc />
        public string Build()
        {
            return null;
        }

        /// <inheritdoc />
        public CodecType CodecType { get; internal set; }

        /// <inheritdoc />
        public int Index { get; internal set; }
    }
}
