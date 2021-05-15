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
        internal static IConversion ExtractAudio(string inputPath, string outputPath)
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
        internal static IConversion AddAudio(string videoPath, string audioPath, string outputPath)
        {
            IMediaInfo videoInfo = FFmpeg.GetMediaInfo(videoPath).GetAwaiter().GetResult();

            IMediaInfo audioInfo = FFmpeg.GetMediaInfo(audioPath).GetAwaiter().GetResult();

            return New()
                .AddStream(videoInfo.VideoStreams.FirstOrDefault())
                .AddStream(audioInfo.AudioStreams.FirstOrDefault())
                .AddStream(videoInfo.SubtitleStreams.ToArray())
                .SetOutput(outputPath);
        }

        /// <summary>
        /// Generates a visualisation of an audio stream using the 'showfreqs' filter
        /// </summary>
        /// <param name="inputPath">Path to the input file containing the audio stream to visualise</param>
        /// <param name="outputPath">Path to output the visualised audio stream to</param>
        /// <param name="size">The Size of the outputted video stream</param>
        /// <param name="pixelFormat">The output pixel format (default is yuv420p)</param>
        /// <param name="mode">The visualisation mode (default is bar)</param>
        /// <param name="amplitudeScale">The frequency scale (default is lin)</param>
        /// <param name="frequencyScale">The amplitude scale (default is log)</param>
        /// <returns>IConversion object</returns>
        internal static IConversion VisualiseAudio(string inputPath, string outputPath, VideoSize size,
            PixelFormat pixelFormat = PixelFormat.yuv420p,
            VisualisationMode mode = VisualisationMode.bar,
            AmplitudeScale amplitudeScale = AmplitudeScale.lin,
            FrequencyScale frequencyScale = FrequencyScale.log)
        {
            IMediaInfo inputInfo = FFmpeg.GetMediaInfo(inputPath).GetAwaiter().GetResult();
            IAudioStream audioStream = inputInfo.AudioStreams.FirstOrDefault();
            IVideoStream videoStream = inputInfo.VideoStreams.FirstOrDefault();

            string filter = $"\"[0:a]showfreqs=mode={mode}:fscale={frequencyScale}:ascale={amplitudeScale},format={pixelFormat},scale={size.ToFFmpegFormat()} [v]\"";

            return New()
                .AddStream(audioStream)
                .AddParameter($"-filter_complex {filter}")
                .AddParameter("-map [v]")
                .SetFrameRate(videoStream != null ? videoStream.Framerate : 30) // Pin framerate at the original rate or 30 fps to stop dropped or duplicated frames
                .SetOutput(outputPath);
        }
    }
}
