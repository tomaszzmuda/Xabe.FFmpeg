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
        ///     Convert file to MP4
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Destination file</param>
        /// <returns>Conversion result</returns>
        public static IConversion ToMp4(string inputPath, string outputPath)
        {
            IMediaInfo info = AsyncHelper.RunSync(() => MediaInfo.Get(inputPath));

            IStream videoStream = info.VideoStreams.FirstOrDefault()
                                      ?.SetCodec(VideoCodec.h264);
            IStream audioStream = info.AudioStreams.FirstOrDefault()
                                      ?.SetCodec(AudioCodec.aac);

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
        public static IConversion ToTs(string inputPath, string outputPath)
        {
            IMediaInfo info = AsyncHelper.RunSync(() => MediaInfo.Get(inputPath));

            IStream videoStream = info.VideoStreams.FirstOrDefault()
                                      ?.SetCodec(VideoCodec.mpeg2video);
            IStream audioStream = info.AudioStreams.FirstOrDefault()
                                      ?.SetCodec(AudioCodec.mp2);

            return New()
                .AddStream(videoStream, audioStream)
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Convert file to OGV
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Destination file</param>
        /// <returns>Conversion result</returns>
        public static IConversion ToOgv(string inputPath, string outputPath)
        {
            IMediaInfo info = AsyncHelper.RunSync(() => MediaInfo.Get(inputPath));

            IStream videoStream = info.VideoStreams.FirstOrDefault()
                                      ?.SetCodec(VideoCodec.theora);
            IStream audioStream = info.AudioStreams.FirstOrDefault()
                                      ?.SetCodec(AudioCodec.libvorbis);

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
                                      ?.SetCodec(VideoCodec.vp8);
            IStream audioStream = info.AudioStreams.FirstOrDefault()
                                      ?.SetCodec(AudioCodec.libvorbis);

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
        public static IConversion ToGif(string inputPath, string outputPath, int loop, int delay = 0)
        {
            IMediaInfo info = AsyncHelper.RunSync(() => MediaInfo.Get(inputPath));

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
        /// <param name="outputPath">Output file</param>
        /// <param name="inputImage">Watermark</param>
        /// <param name="position">Position of watermark</param>
        /// <returns>Conversion result</returns>
        public static IConversion SetWatermark(string inputPath, string outputPath, string inputImage, Position position)
        {
            IMediaInfo info = AsyncHelper.RunSync(() => MediaInfo.Get(inputPath));

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
        public static IConversion ExtractVideo(string inputPath, string outputPath)
        {
            IMediaInfo info = AsyncHelper.RunSync(() => MediaInfo.Get(inputPath));

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
        public static IConversion ExtractAudio(string inputPath, string outputPath)
        {
            IMediaInfo info = AsyncHelper.RunSync(() => MediaInfo.Get(inputPath));

            IAudioStream audioStream = info.AudioStreams.FirstOrDefault();

            return New()
                .AddStream(audioStream)
                .SetAudioBitrate(audioStream.Bitrate)
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Saves snapshot of video
        /// </summary>
        /// <param name="inputPath">Video</param>
        /// <param name="outputPath">Output file</param>
        /// <param name="captureTime">TimeSpan of snapshot</param>
        /// <returns>Conversion result</returns>
        public static IConversion Snapshot(string inputPath, string outputPath, TimeSpan captureTime)
        {
            IMediaInfo info = AsyncHelper.RunSync(() => MediaInfo.Get(inputPath));

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
        /// <param name="size">Expected size</param>
        /// <returns>Conversion result</returns>
        public static IConversion ChangeSize(string inputPath, string outputPath, VideoSize size)
        {
            IMediaInfo info = AsyncHelper.RunSync(() => MediaInfo.Get(inputPath));

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
        public static IConversion Split(string inputPath, string outputPath, TimeSpan startTime, TimeSpan duration)
        {
            IMediaInfo info = AsyncHelper.RunSync(() => MediaInfo.Get(inputPath));

            IStream[] streams = info.Streams.ToArray();
            foreach (IStream stream in streams)
            {
                if (stream is ILocalStream localStream)
                {
                    localStream.Split(startTime, duration);
                }
            }

            return New()
                .AddStream(streams)
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Add audio stream to video file
        /// </summary>
        /// <param name="videoPath">Video</param>
        /// <param name="audioPath">Audio</param>
        /// <param name="outputPath">Output file</param>
        /// <returns>Conversion result</returns>
        public static IConversion AddAudio(string videoPath, string audioPath, string outputPath)
        {
            IMediaInfo videoInfo = AsyncHelper.RunSync(() => MediaInfo.Get(videoPath));

            IMediaInfo audioInfo = AsyncHelper.RunSync(() => MediaInfo.Get(audioPath));

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
        /// <param name="outputPath">Output file</param>
        /// <param name="subtitlesPath">Subtitles</param>
        /// <returns>Conversion result</returns>
        public static IConversion AddSubtitles(string inputPath, string outputPath, string subtitlesPath)
        {
            IMediaInfo info = AsyncHelper.RunSync(() => MediaInfo.Get(inputPath));

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
            IMediaInfo mediaInfo = AsyncHelper.RunSync(() => MediaInfo.Get(inputPath));
            IMediaInfo subtitleInfo = AsyncHelper.RunSync(() => MediaInfo.Get(subtitlePath));

            ISubtitleStream subtitleStream = subtitleInfo.SubtitleStreams.First()
                                                         .SetLanguage(language);

            return New()
                .AddStream(mediaInfo.VideoStreams.ToArray())
                .AddStream(mediaInfo.AudioStreams.ToArray())
                .AddStream(subtitleStream)
                .SetOutput(outputPath);
        }

        /// <summary>
        /// Save M3U8 stream
        /// </summary>
        /// <param name="uri">Uri to stream</param>
        /// <param name="outputPath">Output path</param>
        /// <param name="duration">Duration of stream</param>
        /// <returns>Conversion result</returns>
        public static IConversion SaveM3U8Stream(Uri uri, string outputPath, TimeSpan? duration = null)
        {
            var stream = new WebStream(uri, "M3U8", duration);
            return New()
                .AddStream(stream)
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Concat multiple inputVideos.
        /// </summary>
        /// <param name="output">Concatenated inputVideos</param>
        /// <param name="inputVideos">Videos to add</param>
        /// <returns>Conversion result</returns>
        public static Task<IConversionResult> Concatenate(string output, params string[] inputVideos)
        {
            if (inputVideos.Length <= 1)
            {
                throw new ArgumentException("You must provide at least 2 files for the concatenation to work", "inputVideos");
            }

            var mediaInfos = new List<IMediaInfo>();

            IConversion conversion = New();
            foreach (string inputVideo in inputVideos)
            {
                IMediaInfo mediaInfo = AsyncHelper.RunSync(() => MediaInfo.Get(inputVideo));

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
            conversion.SetOutput(output);
            return conversion.Start();
        }


        /// <summary>
        ///     Convert one file to another with destination format.
        /// </summary>
        /// <param name="inputFilePath">Path to file</param>
        /// <param name="outputFilePath">Path to file</param>
        /// <returns>IConversion object</returns>
        public static IConversion Convert(string inputFilePath, string outputFilePath)
        {
            IMediaInfo info = AsyncHelper.RunSync(() => MediaInfo.Get(inputFilePath));

            var conversion = New().SetOutput(outputFilePath);

            foreach (var stream in info.Streams)
            {
                if(!typeof(ISubtitleStream).IsAssignableFrom(stream.GetType()))
                    conversion.AddStream(stream);
            }

            return conversion;
        }

        /// <summary>
        ///     Convert one file to another with destination format using hardware acceleration (if possible). Using cuvid. Works only on Windows/Linux with NVidia GPU.
        /// </summary>
        /// <param name="inputFilePath">Path to file</param>
        /// <param name="outputFilePath">Path to file</param>
        /// <param name="hardwareAccelerator">Hardware accelerator. List of all acceclerators available for your system - "ffmpeg -hwaccels"</param>
        /// <param name="decoder">Codec using to decoding input video (e.g. h264_cuvid)</param>
        /// <param name="encoder">Codec using to encode output video (e.g. h264_nvenc)</param>
        /// <param name="device">Number of device (0 = default video card) if more than one video card.</param>
        /// <returns>IConversion object</returns>
        public static IConversion ConvertWithHardware(string inputFilePath, string outputFilePath, HardwareAccelerator hardwareAccelerator, VideoCodec decoder, VideoCodec encoder, int device = 0)
        {
            var conversion = Convert(inputFilePath, outputFilePath);
            conversion.UseHardwareAcceleration(hardwareAccelerator, decoder, encoder, device);
            return conversion;
        }
    }
}
