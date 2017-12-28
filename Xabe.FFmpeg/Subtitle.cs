using System;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Reference to subtitle file
    /// </summary>
    public class Subtitle : ISubtitle
    {
        private readonly string _path;

        /// <inheritdoc />
        public Subtitle(string path)
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
            //                             .SetFormat(new VideoFormat(description))
            //                             .Start();

            //todo: subtitles
            throw  new NotImplementedException();
        }
    }
}
