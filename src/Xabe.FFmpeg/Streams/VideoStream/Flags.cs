using System;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Flags for Vidoes (-flags option)
    /// </summary>
    public enum Flag
    {
        ///<summary>
        /// Use four motion vector by macroblock (mpeg4).
        ///</summary>
        mv4,

        ///<summary>
        /// Use 1/4 pel motion compensation.
        ///</summary>
        qpel,

        ///<summary>
        /// Use loop filter.
        ///</summary>
        loop,

        ///<summary>
        /// Use fixed qscale.
        ///</summary>
        qscale,

        ///<summary>
        /// Use internal 2pass ratecontrol in first pass mode.
        ///</summary>
        pass1,

        ///<summary>
        /// Use internal 2pass ratecontrol in second pass mode.
        ///</summary>
        pass2,

        ///<summary>
        /// Only decode/encode grayscale.
        ///</summary>
        gray,

        ///<summary>
        /// Do not draw edges.
        ///</summary>
        emu_edge,

        ///<summary>
        /// Set error[?] variables during encoding.
        ///</summary>
        psnr,

        ///<summary>
        /// Input bitstream might be randomly truncated.
        ///</summary>
        truncated,

        ///<summary>
        /// Dont output frames whose parameters differ from first decoded frame in stream. Error AVERROR_INPUT_CHANGED is returned when a frame is dropped.
        ///</summary>
        drop_changed,

        ///<summary>
        /// Use interlaced DCT.
        ///</summary>
        ildct,

        ///<summary>
        /// Force low delay.
        ///</summary>
        low_delay,

        ///<summary>
        /// Place global headers in extradata instead of every keyframe.
        ///</summary>
        global_header,

        ///<summary>
        /// Only write platform-, build- and time-independent data. (except (I)DCT). This ensures that file and data checksums are reproducible and match between platforms. Its primary use is for regression testing.
        ///</summary>
        bitexact,

        ///<summary>
        /// Apply H263 advanced intra coding / mpeg4 ac prediction.
        ///</summary>
        aic,

        ///<summary>
        /// Deprecated, use mpegvideo private options instead.
        ///</summary>
        [Obsolete]
        cbp,

        ///<summary>
        /// Deprecated, use mpegvideo private options instead.
        ///</summary>
        [Obsolete]
        qprd,

        ///<summary>
        /// Apply interlaced motion estimation.
        ///</summary>
        ilme,

        ///<summary>
        /// Use closed gop.
        ///</summary>
        cgop,

        ///<summary>
        /// Output even potentially corrupted frames.output_corrupt
        ///</summary>
        output_corrupt
    }
}