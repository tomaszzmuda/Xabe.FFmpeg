using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xabe.FFmpeg
{
    public partial class Conversion
    {
        /// <summary>
        ///     Add subtitles to video stream
        /// </summary>
        /// <param name="inputPath">Video</param>
        /// <param name="outputPath">Output file</param>
        /// <param name="subtitlesPath">Subtitles</param>
        /// <returns>Conversion result</returns>
        public static IConversion AddSubtitles(string inputPath, string outputPath, string subtitlesPath)
        {
            IMediaInfo info = MediaInfo.Get(inputPath).GetAwaiter().GetResult();

            IVideoStream videoStream = info.VideoStreams.FirstOrDefault()
                                           ?.AddSubtitles(subtitlesPath);

            return New()
                .AddStream(videoStream)
                .AddStream(info.AudioStreams.FirstOrDefault())
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Add subtitle to file. It will be added as new stream so if you want to burn subtitles into video you should use
        ///     SetSubtitles method.
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Output path</param>
        /// <param name="subtitlePath">Path to subtitle file in .srt format</param>
        /// <param name="language">Language code in ISO 639. Example: "eng", "pol", "pl", "de", "ger"</param>
        /// <returns>Conversion result</returns>
        public static IConversion AddSubtitle(string inputPath, string outputPath, string subtitlePath, string language = null)
        {
            IMediaInfo mediaInfo = MediaInfo.Get(inputPath).GetAwaiter().GetResult();
            IMediaInfo subtitleInfo = MediaInfo.Get(subtitlePath).GetAwaiter().GetResult();

            ISubtitleStream subtitleStream = subtitleInfo.SubtitleStreams.First()
                                                         .SetLanguage(language);

            return New()
                .AddStream(mediaInfo.VideoStreams)
                .AddStream(mediaInfo.AudioStreams)
                .AddStream(subtitleStream)
                .SetOutput(outputPath);
        }
    }
}
