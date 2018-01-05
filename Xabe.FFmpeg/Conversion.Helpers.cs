using System;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg
{
    public partial class Conversion
    {
        /// <summary>
        ///     Convert file to MP4
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Destination file</param>
        /// <returns>Conversion result</returns>
        public static async Task<IConversion> ToMp4(string inputPath, string outputPath)
        {
            IMediaInfo info = await MediaInfo.Get(inputPath);

            IStream videoStream = info.VideoStreams.FirstOrDefault()
                                           ?.SetCodec(VideoCodec.h264, 2400);
            IStream audioStream = info.AudioStreams.FirstOrDefault()
                                           ?.SetCodec(AudioCodec.aac, AudioQuality.Normal);

            return New()
                .AddStream(videoStream, audioStream)
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Convert file to TS
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Destination file</param>
        /// <returns>Conversion result</returns>
        public static async Task<IConversion> ToTs(string inputPath, string outputPath)
        {
            IMediaInfo info = await MediaInfo.Get(inputPath);

            IStream videoStream = info.VideoStreams.FirstOrDefault()
                                           ?.CopyStream()
                                           ?.SetBitstreamFilter(BitstreamFilter.H264_Mp4ToAnnexB);
            IStream audioStream = info.AudioStreams.FirstOrDefault();

            return New()
                .AddStream(videoStream, audioStream)
                .SetFormat(MediaFormat.mpegts)
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Convert file to OGV
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Destination file</param>
        /// <returns>Conversion result</returns>
        public static async Task<IConversion> ToOgv(string inputPath, string outputPath)
        {
            IMediaInfo info = await MediaInfo.Get(inputPath);

            IStream videoStream = info.VideoStreams.FirstOrDefault()
                                           ?.SetCodec(VideoCodec.theora, 2400);
            IStream audioStream = info.AudioStreams.FirstOrDefault()
                ?.SetCodec(AudioCodec.libvorbis, AudioQuality.Normal);

            return New()
                .AddStream(videoStream, audioStream)
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Convert file to WebM
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Destination file</param>
        /// <returns>Conversion result</returns>
        public static async Task<IConversion> ToWebM(string inputPath, string outputPath)
        {
            IMediaInfo info = await MediaInfo.Get(inputPath);

            IStream videoStream = info.VideoStreams.FirstOrDefault()
                                           ?.SetCodec(VideoCodec.vp8, 2400);
            IStream audioStream = info.AudioStreams.FirstOrDefault()
                                           ?.SetCodec(AudioCodec.libvorbis, AudioQuality.Normal);

            return New()
                .AddStream(videoStream, audioStream)
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Convert image video stream to gif
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Output path</param>
        /// <param name="loop">Number of repeats</param>
        /// <param name="delay">Delay between repeats (in seconds)</param>
        /// <returns>Conversion result</returns>
        public static async Task<IConversion> ToGif(string inputPath, string outputPath, int loop, int delay = 0)
        {
            IMediaInfo info = await MediaInfo.Get(inputPath);

            IVideoStream videoStream = info.VideoStreams.FirstOrDefault()
                                           ?.SetLoop(loop, delay);

            return New()
                .AddStream(videoStream)
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Melt watermark into video
        /// </summary>
        /// <param name="inputPath">Input video path</param>
        /// <param name="position">Position of watermark</param>
        /// <param name="outputPath">Output file</param>
        /// <param name="inputImage">Watermark</param>
        /// <returns>Conversion result</returns>
        public static async Task<IConversion> SetWatermark(string inputPath, string outputPath, string inputImage, Position position)
        {
            IMediaInfo info = await MediaInfo.Get(inputPath);

            IVideoStream videoStream = info.VideoStreams.FirstOrDefault()
                                           ?.SetWatermark(inputImage, position);

            return New()
                .AddStream(videoStream)
                .SetOutput(outputPath);

        }

        /// <summary>
        ///     Extract video from file
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Output audio stream</param>
        /// <returns>Conversion result</returns>
        public static async Task<IConversion> ExtractVideo(string inputPath, string outputPath)
        {
            IMediaInfo info = await MediaInfo.Get(inputPath);

            IVideoStream videoStream = info.VideoStreams.FirstOrDefault();

            return New()
                .AddStream(videoStream)
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Extract audio from file
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Output video stream</param>
        /// <returns>Conversion result</returns>
        public static async Task<IConversion> ExtractAudio(string inputPath, string outputPath)
        {
            IMediaInfo info = await MediaInfo.Get(inputPath);

            IAudioStream videoStream = info.AudioStreams.FirstOrDefault();

            return New()
                .AddStream(videoStream)
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Saves snapshot of video
        /// </summary>
        /// <param name="inputPath">Video</param>
        /// <param name="outputPath">Output file</param>
        /// <param name="captureTime"></param>
        /// <returns>Conversion result</returns>
        public static async Task<IConversion> Snapshot(string inputPath, string outputPath, TimeSpan captureTime)
        {
            IMediaInfo info = await MediaInfo.Get(inputPath);

            IVideoStream videoStream = info.VideoStreams.FirstOrDefault()
                .SetCodec(VideoCodec.png)
                .SetOutputFramesCount(1)
                .SetSeek(captureTime);

            return New()
                .AddStream(videoStream)
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Change video size
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Output path</param>
        /// <param name="size">Expected size</param>
        /// <returns>Conversion result</returns>
        public static async Task<IConversion> ChangeSize(string inputPath, string outputPath, VideoSize size)
        {
            IMediaInfo info = await MediaInfo.Get(inputPath);

            IVideoStream videoStream = info.VideoStreams.FirstOrDefault()
                                           .SetScale(size);
            return New()
                .AddStream(videoStream)
                .AddStream(info.AudioStreams.ToArray())
                .AddStream(info.SubtitleStreams.ToArray())
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Get part of video
        /// </summary>
        /// <param name="inputPath">Video</param>
        /// <param name="outputPath">Output file</param>
        /// <param name="startTime">Start point</param>
        /// <param name="duration">Duration of new video</param>
        /// <returns>Conversion result</returns>
        public static async Task<IConversion> Split(string inputPath, string outputPath, TimeSpan startTime, TimeSpan duration)
        {
            IMediaInfo info = await MediaInfo.Get(inputPath);

            return New()
                .AddStream(info.VideoStreams.ToArray())
                .AddStream(info.AudioStreams.ToArray())
                .AddStream(info.SubtitleStreams.ToArray())
                .Split(startTime, duration)
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Add audio stream to video file
        /// </summary>
        /// <param name="videoPath">Video</param>
        /// <param name="audioPath">Audio</param>
        /// <param name="outputPath">Output file</param>
        /// <returns>Conversion result</returns>
        public static async Task<IConversion> AddAudio(string videoPath, string audioPath, string outputPath)
        {
            IMediaInfo videoInfo = await MediaInfo.Get(videoPath);
            IMediaInfo audioInfo = await MediaInfo.Get(audioPath);

            return New()
                .AddStream(videoInfo.VideoStreams.FirstOrDefault())
                .AddStream(audioInfo.AudioStreams.FirstOrDefault())
                .AddStream(videoInfo.SubtitleStreams.ToArray())
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Add subtitles to video stream
        /// </summary>
        /// <param name="inputPath">Video</param>
        /// <param name="subtitlesPath">Subtitles</param>
        /// <param name="outputPath">Output file</param>
        /// <returns>Conversion result</returns>
        public static async Task<IConversion> AddSubtitles(string inputPath, string outputPath, string subtitlesPath)
        {
            IMediaInfo info = await MediaInfo.Get(inputPath);
            var videoStream = info.VideoStreams.FirstOrDefault()
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
        public static async Task<IConversion> AddSubtitle(string inputPath, string outputPath, string subtitlePath, string language = null)
        {
            IMediaInfo mediaInfo = await MediaInfo.Get(inputPath);
            IMediaInfo subtitleInfo = await MediaInfo.Get(subtitlePath);

            var subtitleStream = subtitleInfo.SubtitleStreams.First().SetLanguage(language);

            return New()
                .AddStream(mediaInfo.VideoStreams.ToArray())
                .AddStream(mediaInfo.AudioStreams.ToArray())
                .AddStream(subtitleStream)
                .SetOutput(outputPath);
        }
    }
}
