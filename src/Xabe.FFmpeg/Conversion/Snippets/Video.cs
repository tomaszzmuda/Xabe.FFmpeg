using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Xabe.FFmpeg
{
    /// <inheritdoc />
    public partial class Conversion
    {
        /// <summary>
        ///     Melt watermark into video
        /// </summary>
        /// <param name="inputPath">Input video path</param>
        /// <param name="outputPath">Output file</param>
        /// <param name="inputImage">Watermark</param>
        /// <param name="position">Position of watermark</param>
        /// <returns>Conversion result</returns>
        [Obsolete("This will be deleted in next major version. Please use FFmpeg.Conversions.FromSnippet instead of that.")]
        public static IConversion SetWatermark(string inputPath, string outputPath, string inputImage, Position position)
        {
            IMediaInfo info = FFmpeg.GetMediaInfo(inputPath).GetAwaiter().GetResult();

            IVideoStream videoStream = info.VideoStreams.FirstOrDefault()
                                           .SetWatermark(inputImage, position);

            return New()
                .AddStream(videoStream)
                .AddStream(info.AudioStreams.ToArray())
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Extract video from file
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Output audio stream</param>
        /// <returns>Conversion result</returns>
        [Obsolete("This will be deleted in next major version. Please use FFmpeg.Conversions.FromSnippet instead of that.")]
        public static IConversion ExtractVideo(string inputPath, string outputPath)
        {
            IMediaInfo info = FFmpeg.GetMediaInfo(inputPath).GetAwaiter().GetResult();

            IVideoStream videoStream = info.VideoStreams.FirstOrDefault();

            return New()
                .AddStream(videoStream)
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Saves snapshot of video
        /// </summary>
        /// <param name="inputPath">Video</param>
        /// <param name="outputPath">Output file</param>
        /// <param name="captureTime">TimeSpan of snapshot</param>
        /// <returns>Conversion result</returns>
        [Obsolete("This will be deleted in next major version. Please use FFmpeg.Conversions.FromSnippet instead of that.")]
        public static IConversion Snapshot(string inputPath, string outputPath, TimeSpan captureTime)
        {
            IMediaInfo info = FFmpeg.GetMediaInfo(inputPath).GetAwaiter().GetResult();

            IVideoStream videoStream = info.VideoStreams.FirstOrDefault()
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
        /// <param name="width">Expected width</param>
        /// <param name="height">Expected height</param>
        /// <returns>Conversion result</returns>
        [Obsolete("This will be deleted in next major version. Please use FFmpeg.Conversions.FromSnippet instead of that.")]
        public static IConversion ChangeSize(string inputPath, string outputPath, int width, int height)
        {
            IMediaInfo info = FFmpeg.GetMediaInfo(inputPath).GetAwaiter().GetResult();

            IVideoStream videoStream = info.VideoStreams.FirstOrDefault()
                                           .SetSize(width, height);
            return New()
                .AddStream(videoStream)
                .AddStream(info.AudioStreams.ToArray())
                .AddStream(info.SubtitleStreams.ToArray())
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Change video size
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Output path</param>
        /// <param name="size">Expected size</param>
        /// <returns>Conversion result</returns>
        [Obsolete("This will be deleted in next major version. Please use FFmpeg.Conversions.FromSnippet instead of that.")]
        public static IConversion ChangeSize(string inputPath, string outputPath, VideoSize size)
        {
            IMediaInfo info = FFmpeg.GetMediaInfo(inputPath).GetAwaiter().GetResult();

            IVideoStream videoStream = info.VideoStreams.FirstOrDefault()
                                           .SetSize(size);
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
        [Obsolete("This will be deleted in next major version. Please use FFmpeg.Conversions.FromSnippet instead of that.")]
        public static IConversion Split(string inputPath, string outputPath, TimeSpan startTime, TimeSpan duration)
        {
            IMediaInfo info = FFmpeg.GetMediaInfo(inputPath).GetAwaiter().GetResult();

            var streams = new List<IStream>();
            foreach (IVideoStream stream in info.VideoStreams)
            {
                streams.Add(stream.Split(startTime, duration));
            }
            foreach (IAudioStream stream in info.AudioStreams)
            {
                streams.Add(stream.Split(startTime, duration));
            }

            return New()
                .AddStream(streams)
                .SetOutput(outputPath);
        }

        /// <summary>
        /// Save M3U8 stream
        /// </summary>
        /// <param name="uri">Uri to stream</param>
        /// <param name="outputPath">Output path</param>
        /// <param name="duration">Duration of stream</param>
        /// <returns>Conversion result</returns>
        [Obsolete("This will be deleted in next major version. Please use FFmpeg.Conversions.FromSnippet instead of that.")]
        public static IConversion SaveM3U8Stream(Uri uri, string outputPath, TimeSpan? duration = null)
        {
            var mediaInfo = FFmpeg.GetMediaInfo(uri.ToString()).GetAwaiter().GetResult();
            return New()
                .AddStream(mediaInfo.Streams)
                .SetInputTime(duration)
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Concat multiple inputVideos.
        /// </summary>
        /// <param name="output">Concatenated inputVideos</param>
        /// <param name="inputVideos">Videos to add</param>
        /// <returns>Conversion result</returns>
        [Obsolete("This will be deleted in next major version. Please use FFmpeg.Conversions.FromSnippet instead of that.")]
        public static async Task<IConversion> Concatenate(string output, params string[] inputVideos)
        {
            if (inputVideos.Length <= 1)
            {
                throw new ArgumentException("You must provide at least 2 files for the concatenation to work", "inputVideos");
            }

            var mediaInfos = new List<IMediaInfo>();

            IConversion conversion = New();
            foreach (string inputVideo in inputVideos)
            {
                IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(inputVideo);

                mediaInfos.Add(mediaInfo);
                conversion.AddParameter($"-i \"{inputVideo}\" ");
            }
            conversion.AddParameter($"-t 1 -f lavfi -i anullsrc=r=48000:cl=stereo");
            conversion.AddParameter($"-filter_complex \"");

            IVideoStream maxResolutionMedia = mediaInfos.Select(x => x.VideoStreams.OrderByDescending(z => z.Width)
                                                                      .First())
                                                        .OrderByDescending(x => x.Width)
                                                        .First();
            for (var i = 0; i < mediaInfos.Count; i++)
            {
                conversion.AddParameter(
                    $"[{i}:v]scale={maxResolutionMedia.Width}:{maxResolutionMedia.Height},setdar=dar={maxResolutionMedia.Ratio},setpts=PTS-STARTPTS[v{i}]; ");
            }
            for (var i = 0; i < mediaInfos.Count; i++)
            {
                conversion.AddParameter(!mediaInfos[i].AudioStreams.Any() ? $"[v{i}]" : $"[v{i}][{i}:a]");
            }

            conversion.AddParameter($"concat=n={inputVideos.Length}:v=1:a=1 [v] [a]\" -map \"[v]\" -map \"[a]\"");
            conversion.AddParameter($"-aspect {maxResolutionMedia.Ratio}");
            return conversion.SetOutput(output);
        }


        /// <summary>
        ///     Convert one file to another with destination format.
        /// </summary>
        /// <param name="inputFilePath">Path to file</param>
        /// <param name="outputFilePath">Path to file</param>
        /// <returns>IConversion object</returns>
        [Obsolete("This will be deleted in next major version. Please use FFmpeg.Conversions.FromSnippet instead of that.")]
        public static IConversion Convert(string inputFilePath, string outputFilePath)
        {
            IMediaInfo info = FFmpeg.GetMediaInfo(inputFilePath).GetAwaiter().GetResult();

            var conversion = New().SetOutput(outputFilePath);

            foreach (var stream in info.Streams)
            {
                if(!typeof(ISubtitleStream).IsAssignableFrom(stream.GetType()))
                    conversion.AddStream(stream);
            }

            return conversion;
        }
    }
}
