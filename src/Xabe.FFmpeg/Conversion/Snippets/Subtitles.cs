﻿using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Streams.SubtitleStream;

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
        internal static async Task<IConversion> AddSubtitlesAsync(string inputPath, string outputPath, string subtitlesPath)
        {
            IMediaInfo info = await FFmpeg.GetMediaInfo(inputPath);

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
        internal static async Task<IConversion> AddSubtitleAsync(string inputPath, string outputPath, string subtitlePath, string language = null)
        {
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(inputPath);
            IMediaInfo subtitleInfo = await FFmpeg.GetMediaInfo(subtitlePath);

            ISubtitleStream subtitleStream = subtitleInfo.SubtitleStreams.First()
                                                         .SetLanguage(language);

            return New()
                .AddStream(mediaInfo.VideoStreams)
                .AddStream(mediaInfo.AudioStreams)
                .AddStream(subtitleStream.SetCodec(SubtitleCodec.copy))
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Add subtitle to file. It will be added as new stream so if you want to burn subtitles into video you should use
        ///     SetSubtitles method.
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Output path</param>
        /// <param name="subtitlePath">Path to subtitle file in .srt format</param>
        /// <param name="subtitleCodec">The Subtitle Codec to Use to Encode the Subtitles</param>
        /// <param name="language">Language code in ISO 639. Example: "eng", "pol", "pl", "de", "ger"</param>
        /// <returns>Conversion result</returns>
        internal static async Task<IConversion> AddSubtitleAsync(string inputPath, string outputPath, string subtitlePath, SubtitleCodec subtitleCodec, string language = null)
        {
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(inputPath);
            IMediaInfo subtitleInfo = await FFmpeg.GetMediaInfo(subtitlePath);

            ISubtitleStream subtitleStream = subtitleInfo.SubtitleStreams.First()
                                                         .SetLanguage(language);

            return New()
                .AddStream(mediaInfo.VideoStreams)
                .AddStream(mediaInfo.AudioStreams)
                .AddStream(subtitleStream.SetCodec(subtitleCodec))
                .SetOutput(outputPath);
        }
    }
}
