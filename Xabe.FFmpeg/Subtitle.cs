using System.Collections.Generic;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Reference to subtitle file
    /// </summary>
    public class Subtitle
    {
        private static readonly Dictionary<SubtitleFormat, string> _descriptions = new Dictionary<SubtitleFormat, string>();
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
            if(!_descriptions.TryGetValue(subtitleFormat, out string description))
            {
                description = Extensions.GetDescription(subtitleFormat);
                _descriptions.Add(subtitleFormat, description);
            }

            return await new Conversion().SetInput(_path)
                                         .SetOutput(outputPath)
                                         .SetCodec(description)
                                         .Start();
        }
    }
}
