using System;
using System.Collections.Generic;
using System.Text;

namespace Xabe.FFmpeg
{
    /// <summary>
    /// Hash Formats ("ffmpeg -i INPUT -f hash")
    /// </summary>
    public enum Hash
    {
        /// <summary>
        ///     MD5 Hashing Algorithm
        /// </summary>
        MD5,

        /// <summary>
        ///     murmur3 Hashing Algorithm
        /// </summary>
        murmur3,

        /// <summary>
        ///     RIPEMD 128-bit Hashing Algorithm
        /// </summary>
        RIPEMD128,

        /// <summary>
        ///     RIPEMD 160-bit Hashing Algorithm
        /// </summary>
        RIPEMD160,

        /// <summary>
        ///     RIPEMD 256-bit Hashing Algorithm
        /// </summary>
        RIPEMD256,

        /// <summary>
        ///     RIPEMD 320-bit Hashing Algorithm
        /// </summary>
        RIPEMD320,

        /// <summary>
        ///     SHA 160-bit Hashing Algorithm
        /// </summary>
        SHA160,

        /// <summary>
        ///     SHA 224-bit Hashing Algorithm
        /// </summary>
        SHA224,

        /// <summary>
        ///     SHA 256-bit Hashing Algorithm
        ///     This is the default algorithm if no -hash argument is specified
        /// </summary>
        SHA256,

        /// <summary>
        ///     SHA 512-bit/224-bit Hashing Algorithm
        /// </summary>
        SHA512_224,

        /// <summary>
        ///     SHA 512-bit/256-bit Hashing Algorithm
        /// </summary>
        SHA512_256,

        /// <summary>
        ///     SHA 384-bit Hashing Algorithm
        /// </summary>
        SHA384,

        /// <summary>
        ///     SHA 512-bit Hashing Algorithm
        /// </summary>
        SHA512,

        /// <summary>
        ///     CRC32 Hashing Algorithm
        /// </summary>
        CRC32,

        /// <summary>
        ///     Adler32 Hashing Algorithm
        /// </summary>
        adler32,
    }
}
