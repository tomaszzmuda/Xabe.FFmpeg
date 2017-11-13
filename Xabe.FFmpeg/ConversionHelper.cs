using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Derives preconfigurated Configuration objects
    /// </summary>
    public static class ConversionHelper
    {
        /// <summary>
        ///     Convert file to MP4
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Destination file</param>
        /// <param name="speed">Conversion speed</param>
        /// <param name="size">Dimension</param>
        /// <param name="audioQuality">Audio quality</param>
        /// <param name="multithread">Use multithread</param>
        /// <returns>Conversion result</returns>
        [UsedImplicitly]
        public static IConversion ToMp4(string inputPath, string outputPath, Speed speed = Speed.SuperFast,
            VideoSize size = null, AudioQuality audioQuality = AudioQuality.Normal, bool multithread = false)
        {
            return Conversion.New()
                             .SetInput(inputPath)
                             .UseMultiThread(multithread)
                             .SetScale(size)
                             .SetVideo(VideoCodec.LibX264, 2400)
                             .SetSpeed(speed)
                             .SetAudio(AudioCodec.Aac, audioQuality)
                             .SetOutput(outputPath);
        }

        /// <summary>
        ///     Convert file to TS
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Destination file</param>
        /// <returns>Conversion result</returns>
        [UsedImplicitly]
        public static IConversion ToTs(string inputPath, string outputPath)
        {
            return Conversion.New()
                             .SetInput(inputPath)
                             .StreamCopy(Channel.Both)
                             .SetBitstreamFilter(Channel.Video, Filter.H264_Mp4ToAnnexB)
                             .SetCodec(VideoCodec.MpegTs)
                             .SetOutput(outputPath);
        }


        /// <summary>
        ///     Convert file to WebM
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Destination file</param>
        /// <param name="size">Dimension</param>
        /// <param name="audioQuality">Audio quality</param>
        /// <param name="multithread">Use multithread</param>
        /// <returns>Conversion result</returns>
        [UsedImplicitly]
        public static IConversion ToWebM(string inputPath, string outputPath, VideoSize size = null, AudioQuality audioQuality = AudioQuality.Normal, bool multithread = false)
        {
            return Conversion.New()
                .SetInput(inputPath)
                .SetScale(size)
                .SetVideo(VideoCodec.LibVpx, 2400)
                .SetAudio(AudioCodec.LibVorbis, audioQuality)
                .SetOutput(outputPath)
                .UseMultiThread(multithread);
        }


        /// <summary>
        ///     Convert file to OGV
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="outputPath">Destination file</param>
        /// <param name="size">Dimension</param>
        /// <param name="audioQuality">Audio quality</param>
        /// <param name="multithread">Use multithread</param>
        /// <returns>Conversion result</returns>
        [UsedImplicitly]
        public static IConversion ToOgv(string inputPath, string outputPath, VideoSize size = null, AudioQuality audioQuality = AudioQuality.Normal, bool multithread = false)
        {
            return Conversion.New()
                .SetInput(inputPath)
                .SetScale(size)
                .SetVideo(VideoCodec.LibTheora, 2400)
                .SetSpeed(16)
                .SetAudio(AudioCodec.LibVorbis, audioQuality)
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Change video size
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="output">Output path</param>
        /// <param name="size">Expected size</param>
        /// <returns>Conversion result</returns>
        public static IConversion ChangeSize(string inputPath, string output, VideoSize size)
        {
            return Conversion.New()
                             .SetInput(inputPath)
                             .SetScale(size)
                             .SetOutput(output);
        }

        /// <summary>
        ///     Add subtitle to file. It will be added as new stream so if you want to burn subtitles into video you should use
        ///     SetSubtitles method.
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="output">Output path</param>
        /// <param name="subtitlePath">Path to subtitle file in .srt format</param>
        /// <param name="language">Language code in ISO 639. Example: "eng", "pol", "pl", "de", "ger"</param>
        /// <returns>Conversion result</returns>
        public static IConversion AddSubtitle(string inputPath, string output, string subtitlePath, string language)
        {
            return Conversion.New()
                             .SetInput(inputPath)
                             .AddSubtitle(subtitlePath, language)
                             .StreamCopy(Channel.Both)
                             .SetOutput(output);
        }

        /// <summary>
        ///     Burn subtitle into video. If you want to add subtitle as new stream (like in .mkv) you should use AddSubtitle
        ///     method.
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="output">Output path</param>
        /// <param name="subtitlePath">Path to subtitle file in .srt format</param>
        /// <returns>Conversion result</returns>
        public static IConversion BurnSubtitle(string inputPath, string output, string subtitlePath)
        {
            return Conversion.New()
                             .SetInput(inputPath)
                             .SetSubtitle(subtitlePath)
                             .SetOutput(output);
        }

        /// <summary>
        ///     Extract video from file
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="output">Output audio stream</param>
        /// <returns>Conversion result</returns>
        [UsedImplicitly]
        public static IConversion ExtractVideo(string inputPath, string output)
        {
            return Conversion.New()
                             .SetInput(inputPath)
                             .StreamCopy(Channel.Both)
                             .DisableChannel(Channel.Audio)
                             .SetOutput(output);
        }


        /// <summary>
        ///     Melt watermark into video
        /// </summary>
        /// <param name="inputPath">Input video path</param>
        /// <param name="position">Position of watermark</param>
        /// <param name="output">Output file</param>
        /// <param name="inputImage">Watermark</param>
        /// <returns>Conversion result</returns>
        [UsedImplicitly]
        public static IConversion SetWatermark(string inputPath, string inputImage, Position position, string output)
        {
            return Conversion.New()
                             .SetInput(inputPath)
                             .SetWatermark(inputImage, position)
                             .SetOutput(output);
        }

        /// <summary>
        ///     Extract audio from file
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="output">Output video stream</param>
        /// <returns>Conversion result</returns>
        [UsedImplicitly]
        public static IConversion ExtractAudio(string inputPath, string output)
        {
            return Conversion.New()
                             .SetInput(inputPath)
                             .DisableChannel(Channel.Video)
                             .SetOutput(output);
        }

        /// <summary>
        ///     Convert image video stream to gif
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="output">Output path</param>
        /// <param name="loop">Number of repeats</param>
        /// <param name="delay">Delay between repeats (in seconds)</param>
        /// <returns>Conversion result</returns>
        [UsedImplicitly]
        public static IConversion ToGif(string inputPath, string output, int loop, int delay = 0)
        {
            return Conversion.New()
                             .SetInput(inputPath)
                             .SetLoop(1, delay)
                             .SetOutput(output);
        }

        /// <summary>
        ///     Add audio to file
        /// </summary>
        /// <param name="inputPath">Input path</param>
        /// <param name="audioFilePath">Audio file</param>
        /// <param name="output">Output file</param>
        /// <returns>Conversion result</returns>
        [UsedImplicitly]
        public static IConversion AddAudio(string inputPath, string audioFilePath, string output)
        {
            return Conversion.New()
                             .SetInput(inputPath, audioFilePath)
                             .StreamCopy(Channel.Video)
                             .SetAudio(AudioCodec.Aac, AudioQuality.Hd)
                             .SetOutput(output);
        }

        /// <summary>
        ///     Saves snapshot of video
        /// </summary>
        /// <param name="inputPath">Video</param>
        /// <param name="outputPath">Output file</param>
        /// <param name="size">Dimension of snapshot</param>
        /// <param name="captureTime"></param>
        /// <returns>Conversion result</returns>
        [UsedImplicitly]
        public static IConversion Snapshot(string inputPath, string outputPath, Size? size = null, TimeSpan? captureTime = null)
        {
            IMediaInfo source = new MediaInfo(inputPath);
            if(captureTime == null)
                captureTime = TimeSpan.FromSeconds(source.Properties.VideoDuration.TotalSeconds / 3);

            size = GetSize(source, size);

            return Conversion.New()
                             .SetInput(inputPath)
                             .SetVideo(VideoCodec.Png)
                             .SetOutputFramesCount(1)
                             .SetSeek(captureTime)
                             .SetSize(size)
                             .SetOutput(outputPath);
        }

        /// <summary>
        ///     Records M3U8 streams to the specified output.
        /// </summary>
        /// <param name="uri">URI to stream.</param>
        /// <param name="outputPath">Output file</param>
        /// <returns>Conversion result</returns>
        [UsedImplicitly]
        public static IConversion SaveM3U8Stream(Uri uri, string outputPath)
        {
            if(uri.Scheme != "http" ||
               uri.Scheme != "https")
                throw new ArgumentException($"Invalid uri {uri.AbsolutePath}");

            return Conversion.New()
                             .SetInput(uri)
                             .SetOutput(outputPath);
        }

        /// <summary>
        ///     Concat multiple inputVideos.
        /// </summary>
        /// <param name="output">Concatenated inputVideos</param>
        /// <param name="inputVideos">Videos to add</param>
        /// <returns>Conversion result</returns>
        [UsedImplicitly]
        public static async Task<bool> Concatenate(string output, params string[] inputVideos)
        {
            var pathList = new List<string>();

            foreach(string path in inputVideos)
            {
                string tempFileName = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Ts);
                pathList.Add(tempFileName);
                await ToTs(path, tempFileName)
                    .Start();
            }

            return await Conversion.New()
                                   .
                                    Concatenate(pathList.ToArray())
                                   .StreamCopy(Channel.Both)
                                   .SetBitstreamFilter(Channel.Audio, Filter.Aac_AdtstoAsc)
                                   .SetOutput(output)
                                   .Start();
        }

        private static Size? GetSize(IMediaInfo source, Size? size)
        {
            if(size == null ||
               size.Value.Height == 0 && size.Value.Width == 0)
                size = new Size(source.Properties.Width, source.Properties.Height);

            if(size.Value.Width != size.Value.Height)
            {
                if(size.Value.Width == 0)
                {
                    double ratio = source.Properties.Width / (double) size.Value.Width;

                    size = new Size((int) (source.Properties.Width * ratio), (int) (source.Properties.Height * ratio));
                }

                if(size.Value.Height == 0)
                {
                    double ratio = source.Properties.Height / (double) size.Value.Height;

                    size = new Size((int) (source.Properties.Width * ratio), (int) (source.Properties.Height * ratio));
                }
            }

            return size;
        }

        /// <summary>
        ///     Get part of video
        /// </summary>
        /// <param name="inputPath">Video</param>
        /// <param name="outputPath">Output file</param>
        /// <param name="startTime">Start point</param>
        /// <param name="duration">Duration of new video</param>
        /// <returns></returns>
        public static IConversion Split(string inputPath, TimeSpan startTime, TimeSpan duration, string outputPath)
        {
            return Conversion.New()
                             .SetInput(inputPath)
                             .Split(startTime, duration)
                             .SetOutput(outputPath);
        }
    }
}
