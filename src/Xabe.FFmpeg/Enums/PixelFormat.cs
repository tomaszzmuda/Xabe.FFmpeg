using System;
using System.Collections.Generic;
using System.Text;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Pixel Format ("ffmpeg -pix_fmts")
    /// </summary>
    public class PixelFormat
    {
        /// <summary>
        /// YUV420P Pixel Format
        /// </summary>
        public static readonly PixelFormat Yuv420P = new PixelFormat("yuv420p");

        /// <summary>
        /// YUV422 Pixel Format
        /// </summary>
        public static readonly PixelFormat Yuv422 = new PixelFormat("yuv422");

        /// <summary>
        /// RGB24 Pixel Format
        /// </summary>
        public static readonly PixelFormat Rgb24 = new PixelFormat("rgb24");

        /// <summary>
        /// BGR24 Pixel Format
        /// </summary>
        public static readonly PixelFormat Bgr24 = new PixelFormat("bgr24");

        /// <summary>
        /// YUV422P Pixel Format
        /// </summary>
        public static readonly PixelFormat Yuv422P = new PixelFormat("yuv422p");

        /// <inheritdoc />
        public PixelFormat(string pixelFormat)
        {
            Format = pixelFormat;
        }

        /// <summary>
        ///     Pixel Format
        /// </summary>
        public string Format { get; }

        /// <summary>
        ///     Format to string
        /// </summary>
        /// <returns>Pixel Format as string</returns>
        public override string ToString()
        {
            return Format;
        }
    }
}
