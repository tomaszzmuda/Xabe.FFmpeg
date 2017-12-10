using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFmpeg.Enums;
using Xabe.FFmpeg.Model;

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
        public static IConversion ToMp4(string inputPath, string outputPath, Speed speed = Speed.SuperFast,
            VideoSize size = null, AudioQuality audioQuality = AudioQuality.Normal, bool multithread = false)
        {
            return new Conversion()
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
        public static IConversion ToTs(string inputPath, string outputPath)
        {
            return new Conversion()
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
        public static IConversion ToWebM(string inputPath, string outputPath, VideoSize size = null, AudioQuality audioQuality = AudioQuality.Normal, bool multithread = false)
        {
            return new Conversion()
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
        public static IConversion ToOgv(string inputPath, string outputPath, VideoSize size = null, AudioQuality audioQuality = AudioQuality.Normal, bool multithread = false)
        {
            return new Conversion()
                .SetInput(inputPath)
                .SetScale(size)
                .SetVideo(VideoCodec.LibTheora, 2400)
                .SetAudio(AudioCodec.LibVorbis, audioQuality)
                .SetOutput(outputPath)
                .UseMultiThread(multithread);
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
            return new Conversion()
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
            return new Conversion()
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
            return new Conversion()
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
        public static IConversion ExtractVideo(string inputPath, string output)
        {
            return new Conversion()
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
        public static IConversion SetWatermark(string inputPath, string inputImage, Position position, string output)
        {
            return new Conversion()
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
        public static IConversion ExtractAudio(string inputPath, string output)
        {
            return new Conversion()
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
        public static IConversion ToGif(string inputPath, string output, int loop, int delay = 0)
        {
            return new Conversion()
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
        public static IConversion AddAudio(string inputPath, string audioFilePath, string output)
        {
            return new Conversion()
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
        /// <param name="resolution">Dimension of snapshot</param>
        /// <param name="captureTime"></param>
        /// <returns>Conversion result</returns>
        public static IConversion Snapshot(string inputPath, string outputPath, Resolution resolution = null, TimeSpan? captureTime = null)
        {
            IMediaInfo source = new MediaInfo(inputPath);
            if(captureTime == null)
                captureTime = TimeSpan.FromSeconds(source.Properties.VideoDuration.TotalSeconds / 3);

            resolution = GetSize(source, resolution);

            return new Conversion()
                .SetInput(inputPath)
                .SetVideo(VideoCodec.Png)
                .SetOutputFramesCount(1)
                .SetSeek(captureTime)
                .SetSize(resolution)
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Records M3U8 streams to the specified output.
        /// </summary>
        /// <param name="uri">URI to stream.</param>
        /// <param name="outputPath">Output file</param>
        /// <returns>Conversion result</returns>
        public static IConversion SaveM3U8Stream(Uri uri, string outputPath)
        {
            if(uri.Scheme != "http" ||
               uri.Scheme != "https")
                throw new ArgumentException($"Invalid uri {uri.AbsolutePath}");

            return new Conversion()
                .SetInput(uri)
                .SetOutput(outputPath);
        }

        /// <summary>
        ///     Concat multiple inputVideos
        /// </summary>
        /// <param name="output">Concatenated inputVideos</param>
        /// <param name="inputVideos">Videos to add</param>
        /// <returns>Conversion result</returns>
        [Obsolete("This method will be remove in version 3.0.0. Please use method Xabe.FFmpeg.ConversionHelper.Concatenate instead.")]
        public static async Task<bool> JoinWith(string output, params string[] inputVideos)
        {
            return await Concatenate(output, inputVideos);
        }

        /// <summary>
        ///     Concat multiple inputVideos.
        /// </summary>
        /// <param name="output">Concatenated inputVideos</param>
        /// <param name="inputVideos">Videos to add</param>
        /// <returns>Conversion result</returns>
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

            return await new Conversion().
                Concat(pathList.ToArray())
                                         .StreamCopy(Channel.Both)
                                         .SetBitstreamFilter(Channel.Audio, Filter.Aac_AdtstoAsc)
                                         .SetOutput(output)
                                         .Start();
        }

        private static Resolution GetSize(IMediaInfo source, Resolution resolution)
        {
            if(resolution == null ||
               resolution.Height == 0 && resolution.Width == 0)
                resolution = new Resolution(source.Properties.Width, source.Properties.Height);

            if(resolution.Width != resolution.Height)
            {
                if(resolution.Width == 0)
                {
                    double ratio = source.Properties.Width / (double) resolution.Width;

                    resolution = new Resolution((int) (source.Properties.Width * ratio), (int) (source.Properties.Height * ratio));
                }

                if(resolution.Height == 0)
                {
                    double ratio = source.Properties.Height / (double) resolution.Height;

                    resolution = new Resolution((int) (source.Properties.Width * ratio), (int) (source.Properties.Height * ratio));
                }
            }

            return resolution;
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
            return new Conversion()
                .SetInput(inputPath)
                .Split(startTime, duration)
                .SetOutput(outputPath);
        }
    }
}
