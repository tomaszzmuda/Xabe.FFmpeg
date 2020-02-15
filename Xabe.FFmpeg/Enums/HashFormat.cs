using System;
using System.Collections.Generic;
using System.Text;

namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    /// Hash Formats ("ffmpeg -i INPUT -f hash")
    /// </summary>
    public class HashFormat
    {
        /// <summary>
        ///     MD5 Hashing Algorithm
        /// </summary>
        public static HashFormat MD5 = new HashFormat("MD5");

        /// <summary>
        ///     murmur3 Hashing Algorithm
        /// </summary>
        public static HashFormat Murmur3 = new HashFormat("Murmur3");

        /// <summary>
        ///     RIPEMD 128-bit Hashing Algorithm
        /// </summary>
        public static HashFormat RIPEMD128 = new HashFormat("RIPEMD128");

        /// <summary>
        ///     RIPEMD 160-bit Hashing Algorithm
        /// </summary>
        public static HashFormat RIPEMD160 = new HashFormat("RIPEMD160");

        /// <summary>
        ///     RIPEMD 256-bit Hashing Algorithm
        /// </summary>
        public static HashFormat RIPEMD256 = new HashFormat("RIPEMD256");

        /// <summary>
        ///     RIPEMD 320-bit Hashing Algorithm
        /// </summary>
        public static HashFormat RIPEMD320 = new HashFormat("RIPEMD320");

        /// <summary>
        ///     SHA 160-bit Hashing Algorithm
        /// </summary>
        public static HashFormat SHA160 = new HashFormat("SHA160");

        /// <summary>
        ///     SHA 224-bit Hashing Algorithm
        /// </summary>
        public static HashFormat SHA224 = new HashFormat("SHA224");

        /// <summary>
        ///     SHA 256-bit Hashing Algorithm
        ///     This is the default algorithm if no -hash argument is specified
        /// </summary>
        public static HashFormat SHA256 = new HashFormat("SHA256");

        /// <summary>
        ///     SHA 512-bit/224-bit Hashing Algorithm
        /// </summary>
        public static HashFormat SHA512_224 = new HashFormat("SHA512/224");

        /// <summary>
        ///     SHA 512-bit/256-bit Hashing Algorithm
        /// </summary>
        public static HashFormat SHA512_256 = new HashFormat("SHA512/256");

        /// <summary>
        ///     SHA 384-bit Hashing Algorithm
        /// </summary>
        public static HashFormat SHA384 = new HashFormat("SHA384");

        /// <summary>
        ///     SHA 512-bit Hashing Algorithm
        /// </summary>
        public static HashFormat SHA512 = new HashFormat("SHA512");

        /// <summary>
        ///     CRC32 Hashing Algorithm
        /// </summary>
        public static HashFormat CRC32 = new HashFormat("CRC32");

        /// <summary>
        ///     Adler32 Hashing Algorithm
        /// </summary>
        public static HashFormat Adler32 = new HashFormat("Adler32");

        /// <summary>
        ///     Create new hash format
        /// </summary>
        /// <param name="format">Media format name</param>
        public HashFormat(string format)
        {
            Format = format;
        }

        /// <summary>
        /// Hash Format
        /// </summary>
        public string Format { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Format;
        }
    }
}
