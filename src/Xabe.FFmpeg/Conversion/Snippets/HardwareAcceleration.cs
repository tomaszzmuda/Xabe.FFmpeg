using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xabe.FFmpeg
{
    public partial class Conversion
    {
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
        [Obsolete("This will be deleted in next major version. Please use FFmpeg.Conversions.FromSnippet instead of that.")]
        public static async Task<IConversion> ConvertWithHardware(string inputFilePath, string outputFilePath, HardwareAccelerator hardwareAccelerator, VideoCodec decoder, VideoCodec encoder, int device = 0)
        {
            var conversion = await Convert(inputFilePath, outputFilePath);
            return conversion.UseHardwareAcceleration(hardwareAccelerator, decoder, encoder, device);
        }
    }
}
