using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xabe.FFmpeg
{
    public partial class Conversion
    {
        /// <summary>
        ///     Extract audio from file
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Output video stream</param>
        /// <returns>Conversion result</returns>
        [Obsolete("This will be deleted in next major version. Please use FFmpeg.Conversions.FromSnippet instead of that.")]
        public static IConversion ExtractAudio(string inputPath, string outputPath)
        {
            IMediaInfo info = FFmpeg.GetMediaInfo(inputPath).GetAwaiter().GetResult();

            IAudioStream audioStream = info.AudioStreams.FirstOrDefault();

            return New()
                .AddStream(audioStream)
                .SetAudioBitrate(audioStream.Bitrate)
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Add audio stream to video file
        /// </summary>
        /// <param name="videoPath">Video</param>
        /// <param name="audioPath">Audio</param>
        /// <param name="outputPath">Output file</param>
        /// <returns>Conversion result</returns>
        [Obsolete("This will be deleted in next major version. Please use FFmpeg.Conversions.FromSnippet instead of that.")]
        public static IConversion AddAudio(string videoPath, string audioPath, string outputPath)
        {
            IMediaInfo videoInfo = FFmpeg.GetMediaInfo(videoPath).GetAwaiter().GetResult();

            IMediaInfo audioInfo = FFmpeg.GetMediaInfo(audioPath).GetAwaiter().GetResult();

            return New()
                .AddStream(videoInfo.VideoStreams.FirstOrDefault())
                .AddStream(audioInfo.AudioStreams.FirstOrDefault())
                .AddStream(videoInfo.SubtitleStreams.ToArray())
                .SetOutput(outputPath);
        }
    }
}
