﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xabe.FFmpeg
{
    /// <inheritdoc />
    public partial class Conversion
    {
        /// <summary>
        ///     Loop file infinitely to rtsp server with some default parameters like: -re, -preset ultrafast
        /// </summary>
        /// <param name="inputFilePath">Path to file</param>
        /// <param name="rtspServerUri">Uri of RTSP Server in format: rtsp://127.0.0.1:8554/name</param>
        /// <returns>IConversion object</returns>
        internal static async Task<IConversion> SendToRtspServer(string inputFilePath, Uri rtspServerUri)
        {
            IMediaInfo info = await FFmpeg.GetMediaInfo(inputFilePath);

            var streams = new List<IStream>();
            foreach (var stream in info.VideoStreams)
            {
                stream.SetStreamLoop(-1);
                stream.UseNativeInputRead(true);
                stream.SetCodec(VideoCodec.libx264);
                stream.SetFramerate(23.976);
                stream.SetBitrate(1024000, 1024000, 1024000);
                streams.Add(stream);
            }

            foreach (var stream in info.AudioStreams)
            {
                stream.SetStreamLoop(-1);
                stream.UseNativeInputRead(true);
                stream.SetCodec(AudioCodec.aac);
                stream.SetBitrate(192000);
                stream.SetBitrate(1024000, 1024000, 1024000);
                streams.Add(stream);
            }

            var conversion = New();
            conversion.AddStream(streams);
            conversion.SetPixelFormat(PixelFormat.yuv420p);
            conversion.SetPreset(ConversionPreset.UltraFast);
            conversion.SetOutputFormat(Format.rtsp);
            conversion.SetOutput(rtspServerUri.OriginalString);

            return conversion;
        }

        /// <summary>
        ///     Send your dekstop to rtsp server with some default parameters like: -re, -preset ultrafast
        /// </summary>
        /// <param name="inputFilePath">Path to file</param>
        /// <param name="rtspServerUri">Uri of RTSP Server in format: rtsp://127.0.0.1:8554/name</param>
        /// <returns>IConversion object</returns>
        internal static IConversion SendDesktopToRtspServer(Uri rtspServerUri)
        {
            var conversion = FFmpeg.Conversions.New()
                                               .AddDesktopStream("800x600", 30, 0, 0)
                                               .AddParameter("-tune zerolatency")
                                               .SetOutputFormat(Format.rtsp)
                                               .SetOutput(rtspServerUri.OriginalString);

            return conversion;
        }
    }
}
