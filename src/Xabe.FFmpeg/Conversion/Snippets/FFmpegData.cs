using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Xabe.FFmpeg
{
    /// <inheritdoc />
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
        internal static async Task<Device[]> GetAvailableDevices()
        {
            Format format = Format.dshow;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                format = Format.v4l2;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                format = Format.avfoundation;

            }

            var conversion = New().AddParameter($"-list_devices true -f {format} -i dummy");
            var text = new StringBuilder();
            conversion.OnDataReceived += (sender, e) => text.AppendLine(e.Data);
            await conversion.Start();

            var result = text.ToString();

            var devices = new List<Device>();
            var matches = Regex.Matches(result, "\"([^\"]*)\"");
            for (var i = 0; i < matches.Count; i += 2)
            {
                devices.Add(new Device()
                {
                    Name = matches[i].Value.Substring(1, matches[i].Value.Length - 2),
                    AlternativeName = matches[i + 1].Value.Substring(1, matches[i + 1].Value.Length - 2)
                });
            }

            return devices.ToArray();
        }
    }
}
