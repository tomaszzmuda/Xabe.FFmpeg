using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Xabe.FFMpeg.Enums;

namespace Xabe.FFMpeg
{
    /// <summary>
    ///     Derives preconfigurated Configuration objects
    /// </summary>
    public abstract class ConversionHelper
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
        [UsedImplicitly]
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
        /// <returns>Conversion result</returns>
        [UsedImplicitly]
        public static IConversion ToWebM(string inputPath, string outputPath, VideoSize size = null, AudioQuality audioQuality = AudioQuality.Normal)
        {
            return new Conversion()
                .SetInput(inputPath)
                .SetScale(size)
                .SetVideo(VideoCodec.LibVpx, 2400)
                .SetSpeed(16)
                .SetAudio(AudioCodec.LibVorbis, audioQuality)
                .SetOutput(outputPath);
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
            return new Conversion()
                .SetInput(inputPath)
                .SetScale(size)
                .SetVideo(VideoCodec.LibTheora, 2400)
                .SetSpeed(16)
                .SetAudio(AudioCodec.LibVorbis, audioQuality)
                .SetOutput(outputPath);
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
            return new Conversion()
                      .SetInput(inputPath)
                      .StreamCopy(Channel.Both)
                      .DisableChannel(Channel.Audio)
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
            return new Conversion()
                      .SetInput(inputPath)
                      .DisableChannel(Channel.Video)
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
        /// <param name="size">Dimension of snapshot</param>
        /// <param name="captureTime"></param>
        /// <returns>Conversion result</returns>
        [UsedImplicitly]
        public static IConversion Snapshot(string inputPath, string outputPath, Size? size = null, TimeSpan? captureTime = null)
        {
            IVideoInfo source = new VideoInfo(inputPath);
            if (captureTime == null)
                captureTime = TimeSpan.FromSeconds(source.VideoDuration.TotalSeconds / 3);

            size = GetSize(source, size);

            return new Conversion()
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
            if (uri.Scheme != "http" ||
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
        [UsedImplicitly]
        public static async Task<bool> JoinWith(string output, params string[] inputVideos)
        {
            var pathList = new List<string>();

            foreach (string path in inputVideos)
            {
                string tempFileName = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Ts);
                pathList.Add(tempFileName);
                await ToTs(path, tempFileName).Start();
            }

            return await new Conversion().
                Concat(pathList.ToArray())
                      .StreamCopy(Channel.Both)
                      .SetBitstreamFilter(Channel.Audio, Filter.Aac_AdtstoAsc)
                      .SetOutput(output)
                      .Start();
        }

        private static Size? GetSize(IVideoInfo source, Size? size)
        {
            if (size == null ||
                size.Value.Height == 0 && size.Value.Width == 0)
                size = new Size(source.Width, source.Height);

            if (size.Value.Width != size.Value.Height)
            {
                if (size.Value.Width == 0)
                {
                    double ratio = source.Width / (double)size.Value.Width;

                    size = new Size((int)(source.Width * ratio), (int)(source.Height * ratio));
                }

                if (size.Value.Height == 0)
                {
                    double ratio = source.Height / (double)size.Value.Height;

                    size = new Size((int)(source.Width * ratio), (int)(source.Height * ratio));
                }
            }

            return size;
        }
    }
}
