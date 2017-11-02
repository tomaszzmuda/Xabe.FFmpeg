using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Reference to subtitle file
    /// </summary>
    public class Subtitle
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
        /// <returns></returns>
        public async Task<bool> Convert(string outputPath, SubtitleFormat subtitleFormat)
        {
            var codec = "";
            switch(subtitleFormat)
            {
                case SubtitleFormat.SRT:
                    codec = "srt";
                    break;
                case SubtitleFormat.WebVTT:
                    codec = "webvtt";
                    break;
                case SubtitleFormat.ASS:
                    codec = "ass";
                    break;
            }

            return await new Conversion().SetInput(_path)
                                  .SetOutput(outputPath)
                                  .SetCodec(codec)
                                  .Start();
        }
    }
}
