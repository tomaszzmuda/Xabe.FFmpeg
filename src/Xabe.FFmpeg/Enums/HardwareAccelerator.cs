namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    ///     Hardware accelerators ("ffmpeg -hwaccels")
    /// </summary>
    public class HardwareAccelerator
    {
        /// <summary>
        ///     d3d11va
        /// </summary>
        public static readonly HardwareAccelerator d3d11va = new HardwareAccelerator("d3d11va");

        /// <summary>
        ///     Use the first available option
        /// </summary>
        public static readonly HardwareAccelerator Auto = new HardwareAccelerator("auto");

        /// <summary>
        ///     dxva2
        /// </summary>
        public static readonly HardwareAccelerator dxva2 = new HardwareAccelerator("dxva2");

        /// <summary>
        ///     qsv
        /// </summary>
        public static readonly HardwareAccelerator qsv = new HardwareAccelerator("qsv");

        /// <summary>
        ///     qsv
        /// </summary>
        public static readonly HardwareAccelerator cuvid = new HardwareAccelerator("cuvid");

        /// <summary>
        ///     qsv
        /// </summary>
        public static readonly HardwareAccelerator vdpau = new HardwareAccelerator("vdpau");

        /// <inheritdoc />
        public HardwareAccelerator(string accelerator)
        {
            Accelerator = accelerator;
        }

        /// <summary>
        ///     Hardware accelerator
        /// </summary>
        public string Accelerator { get; }

        /// <summary>
        ///     Convert to string format
        /// </summary>
        /// <returns>Accelerator string</returns>
        public override string ToString()
        {
            return Accelerator;
        }
    }
}
