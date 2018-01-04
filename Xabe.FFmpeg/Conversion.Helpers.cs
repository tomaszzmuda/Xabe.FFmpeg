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

            IVideoStream videoStream = info.VideoStreams.FirstOrDefault()
                                           ?.SetCodec(VideoCodec.h264, 2400);
            IAudioStream audioStream = info.AudioStreams.FirstOrDefault()
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

            IVideoStream videoStream = info.VideoStreams.FirstOrDefault()
                                           ?.CopyStream()
                                           ?.SetBitstreamFilter(BitstreamFilter.H264_Mp4ToAnnexB);
            IAudioStream audioStream = info.AudioStreams.FirstOrDefault();

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

            IVideoStream videoStream = info.VideoStreams.FirstOrDefault()
                                           ?.SetCodec(VideoCodec.theora, 2400);
            IAudioStream audioStream = info.AudioStreams.FirstOrDefault()
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

            IVideoStream videoStream = info.VideoStreams.FirstOrDefault()
                                           ?.SetCodec(VideoCodec.vp8, 2400);
            IAudioStream audioStream = info.AudioStreams.FirstOrDefault()
                                           ?.SetCodec(AudioCodec.libvorbis, AudioQuality.Normal);

            return New()
                .AddStream(videoStream, audioStream)
                .SetOutput(outputPath);
        }
    }
}
