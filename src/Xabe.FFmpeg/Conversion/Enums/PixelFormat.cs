namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Pixel Format ("ffmpeg -pix_fmts")
    /// </summary>
    public enum PixelFormat
    {
        ///<summary>
        /// yuv420p
        ///</summary>
        yuv420p,

        ///<summary>
        /// yuyv422
        ///</summary>
        yuyv422,

        ///<summary>
        /// rgb24
        ///</summary>
        rgb24,

        ///<summary>
        /// bgr24
        ///</summary>
        bgr24,

        ///<summary>
        /// yuv422p
        ///</summary>
        yuv422p,

        ///<summary>
        /// yuv444p
        ///</summary>
        yuv444p,

        ///<summary>
        /// yuv410p
        ///</summary>
        yuv410p,

        ///<summary>
        /// yuv411p
        ///</summary>
        yuv411p,

        ///<summary>
        /// gray
        ///</summary>
        gray,

        ///<summary>
        /// monow
        ///</summary>
        monow,

        ///<summary>
        /// monob
        ///</summary>
        monob,

        ///<summary>
        /// pal8
        ///</summary>
        pal8,

        ///<summary>
        /// yuvj420p
        ///</summary>
        yuvj420p,

        ///<summary>
        /// yuvj422p
        ///</summary>
        yuvj422p,

        ///<summary>
        /// yuvj444p
        ///</summary>
        yuvj444p,

        ///<summary>
        /// uyvy422
        ///</summary>
        uyvy422,

        ///<summary>
        /// uyyvyy411
        ///</summary>
        uyyvyy411,

        ///<summary>
        /// bgr8
        ///</summary>
        bgr8,

        ///<summary>
        /// bgr4
        ///</summary>
        bgr4,

        ///<summary>
        /// bgr4_byte
        ///</summary>
        bgr4_byte,

        ///<summary>
        /// rgb8
        ///</summary>
        rgb8,

        ///<summary>
        /// rgb4
        ///</summary>
        rgb4,

        ///<summary>
        /// rgb4_byte
        ///</summary>
        rgb4_byte,

        ///<summary>
        /// nv12
        ///</summary>
        nv12,

        ///<summary>
        /// nv21
        ///</summary>
        nv21,

        ///<summary>
        /// argb
        ///</summary>
        argb,

        ///<summary>
        /// rgba
        ///</summary>
        rgba,

        ///<summary>
        /// abgr
        ///</summary>
        abgr,

        ///<summary>
        /// bgra
        ///</summary>
        bgra,

        ///<summary>
        /// gray16be
        ///</summary>
        gray16be,

        ///<summary>
        /// gray16le
        ///</summary>
        gray16le,

        ///<summary>
        /// yuv440p
        ///</summary>
        yuv440p,

        ///<summary>
        /// yuvj440p
        ///</summary>
        yuvj440p,

        ///<summary>
        /// yuva420p
        ///</summary>
        yuva420p,

        ///<summary>
        /// rgb48be
        ///</summary>
        rgb48be,

        ///<summary>
        /// rgb48le
        ///</summary>
        rgb48le,

        ///<summary>
        /// rgb565be
        ///</summary>
        rgb565be,

        ///<summary>
        /// rgb565le
        ///</summary>
        rgb565le,

        ///<summary>
        /// rgb555be
        ///</summary>
        rgb555be,

        ///<summary>
        /// rgb555le
        ///</summary>
        rgb555le,

        ///<summary>
        /// bgr565be
        ///</summary>
        bgr565be,

        ///<summary>
        /// bgr565le
        ///</summary>
        bgr565le,

        ///<summary>
        /// bgr555be
        ///</summary>
        bgr555be,

        ///<summary>
        /// bgr555le
        ///</summary>
        bgr555le,

        ///<summary>
        /// vaapi_moco
        ///</summary>
        vaapi_moco,

        ///<summary>
        /// vaapi_idct
        ///</summary>
        vaapi_idct,

        ///<summary>
        /// vaapi_vld
        ///</summary>
        vaapi_vld,

        ///<summary>
        /// yuv420p16le
        ///</summary>
        yuv420p16le,

        ///<summary>
        /// yuv420p16be
        ///</summary>
        yuv420p16be,

        ///<summary>
        /// yuv422p16le
        ///</summary>
        yuv422p16le,

        ///<summary>
        /// yuv422p16be
        ///</summary>
        yuv422p16be,

        ///<summary>
        /// yuv444p16le
        ///</summary>
        yuv444p16le,

        ///<summary>
        /// yuv444p16be
        ///</summary>
        yuv444p16be,

        ///<summary>
        /// dxva2_vld
        ///</summary>
        dxva2_vld,

        ///<summary>
        /// rgb444le
        ///</summary>
        rgb444le,

        ///<summary>
        /// rgb444be
        ///</summary>
        rgb444be,

        ///<summary>
        /// bgr444le
        ///</summary>
        bgr444le,

        ///<summary>
        /// bgr444be
        ///</summary>
        bgr444be,

        ///<summary>
        /// ya8
        ///</summary>
        ya8,

        ///<summary>
        /// bgr48be
        ///</summary>
        bgr48be,

        ///<summary>
        /// bgr48le
        ///</summary>
        bgr48le,

        ///<summary>
        /// yuv420p9be
        ///</summary>
        yuv420p9be,

        ///<summary>
        /// yuv420p9le
        ///</summary>
        yuv420p9le,

        ///<summary>
        /// yuv420p10be
        ///</summary>
        yuv420p10be,

        ///<summary>
        /// yuv420p10le
        ///</summary>
        yuv420p10le,

        ///<summary>
        /// yuv422p10be
        ///</summary>
        yuv422p10be,

        ///<summary>
        /// yuv422p10le
        ///</summary>
        yuv422p10le,

        ///<summary>
        /// yuv444p9be
        ///</summary>
        yuv444p9be,

        ///<summary>
        /// yuv444p9le
        ///</summary>
        yuv444p9le,

        ///<summary>
        /// yuv444p10be
        ///</summary>
        yuv444p10be,

        ///<summary>
        /// yuv444p10le
        ///</summary>
        yuv444p10le,

        ///<summary>
        /// yuv422p9be
        ///</summary>
        yuv422p9be,

        ///<summary>
        /// yuv422p9le
        ///</summary>
        yuv422p9le,

        ///<summary>
        /// gbrp
        ///</summary>
        gbrp,

        ///<summary>
        /// gbrp9be
        ///</summary>
        gbrp9be,

        ///<summary>
        /// gbrp9le
        ///</summary>
        gbrp9le,

        ///<summary>
        /// gbrp10be
        ///</summary>
        gbrp10be,

        ///<summary>
        /// gbrp10le
        ///</summary>
        gbrp10le,

        ///<summary>
        /// gbrp16be
        ///</summary>
        gbrp16be,

        ///<summary>
        /// gbrp16le
        ///</summary>
        gbrp16le,

        ///<summary>
        /// yuva422p
        ///</summary>
        yuva422p,

        ///<summary>
        /// yuva444p
        ///</summary>
        yuva444p,

        ///<summary>
        /// yuva420p9be
        ///</summary>
        yuva420p9be,

        ///<summary>
        /// yuva420p9le
        ///</summary>
        yuva420p9le,

        ///<summary>
        /// yuva422p9be
        ///</summary>
        yuva422p9be,

        ///<summary>
        /// yuva422p9le
        ///</summary>
        yuva422p9le,

        ///<summary>
        /// yuva444p9be
        ///</summary>
        yuva444p9be,

        ///<summary>
        /// yuva444p9le
        ///</summary>
        yuva444p9le,

        ///<summary>
        /// yuva420p10be
        ///</summary>
        yuva420p10be,

        ///<summary>
        /// yuva420p10le
        ///</summary>
        yuva420p10le,

        ///<summary>
        /// yuva422p10be
        ///</summary>
        yuva422p10be,

        ///<summary>
        /// yuva422p10le
        ///</summary>
        yuva422p10le,

        ///<summary>
        /// yuva444p10be
        ///</summary>
        yuva444p10be,

        ///<summary>
        /// yuva444p10le
        ///</summary>
        yuva444p10le,

        ///<summary>
        /// yuva420p16be
        ///</summary>
        yuva420p16be,

        ///<summary>
        /// yuva420p16le
        ///</summary>
        yuva420p16le,

        ///<summary>
        /// yuva422p16be
        ///</summary>
        yuva422p16be,

        ///<summary>
        /// yuva422p16le
        ///</summary>
        yuva422p16le,

        ///<summary>
        /// yuva444p16be
        ///</summary>
        yuva444p16be,

        ///<summary>
        /// yuva444p16le
        ///</summary>
        yuva444p16le,

        ///<summary>
        /// vdpau
        ///</summary>
        vdpau,

        ///<summary>
        /// xyz12le
        ///</summary>
        xyz12le,

        ///<summary>
        /// xyz12be
        ///</summary>
        xyz12be,

        ///<summary>
        /// nv16
        ///</summary>
        nv16,

        ///<summary>
        /// nv20le
        ///</summary>
        nv20le,

        ///<summary>
        /// nv20be
        ///</summary>
        nv20be,

        ///<summary>
        /// rgba64be
        ///</summary>
        rgba64be,

        ///<summary>
        /// rgba64le
        ///</summary>
        rgba64le,

        ///<summary>
        /// bgra64be
        ///</summary>
        bgra64be,

        ///<summary>
        /// bgra64le
        ///</summary>
        bgra64le,

        ///<summary>
        /// yvyu422
        ///</summary>
        yvyu422,

        ///<summary>
        /// ya16be
        ///</summary>
        ya16be,

        ///<summary>
        /// ya16le
        ///</summary>
        ya16le,

        ///<summary>
        /// gbrap
        ///</summary>
        gbrap,

        ///<summary>
        /// gbrap16be
        ///</summary>
        gbrap16be,

        ///<summary>
        /// gbrap16le
        ///</summary>
        gbrap16le,

        ///<summary>
        /// qsv
        ///</summary>
        qsv,

        ///<summary>
        /// mmal
        ///</summary>
        mmal,

        ///<summary>
        /// d3d11va_vld
        ///</summary>
        d3d11va_vld,

        ///<summary>
        /// cuda
        ///</summary>
        cuda,

        ///<summary>
        /// _0rgb
        ///</summary>
        _0rgb,

        ///<summary>
        /// rgb0
        ///</summary>
        rgb0,

        ///<summary>
        /// _0bgr
        ///</summary>
        _0bgr,

        ///<summary>
        /// bgr0
        ///</summary>
        bgr0,

        ///<summary>
        /// yuv420p12be
        ///</summary>
        yuv420p12be,

        ///<summary>
        /// yuv420p12le
        ///</summary>
        yuv420p12le,

        ///<summary>
        /// yuv420p14be
        ///</summary>
        yuv420p14be,

        ///<summary>
        /// yuv420p14le
        ///</summary>
        yuv420p14le,

        ///<summary>
        /// yuv422p12be
        ///</summary>
        yuv422p12be,

        ///<summary>
        /// yuv422p12le
        ///</summary>
        yuv422p12le,

        ///<summary>
        /// yuv422p14be
        ///</summary>
        yuv422p14be,

        ///<summary>
        /// yuv422p14le
        ///</summary>
        yuv422p14le,

        ///<summary>
        /// yuv444p12be
        ///</summary>
        yuv444p12be,

        ///<summary>
        /// yuv444p12le
        ///</summary>
        yuv444p12le,

        ///<summary>
        /// yuv444p14be
        ///</summary>
        yuv444p14be,

        ///<summary>
        /// yuv444p14le
        ///</summary>
        yuv444p14le,

        ///<summary>
        /// gbrp12be
        ///</summary>
        gbrp12be,

        ///<summary>
        /// gbrp12le
        ///</summary>
        gbrp12le,

        ///<summary>
        /// gbrp14be
        ///</summary>
        gbrp14be,

        ///<summary>
        /// gbrp14le
        ///</summary>
        gbrp14le,

        ///<summary>
        /// yuvj411p
        ///</summary>
        yuvj411p,

        ///<summary>
        /// bayer_bggr8
        ///</summary>
        bayer_bggr8,

        ///<summary>
        /// bayer_rggb8
        ///</summary>
        bayer_rggb8,

        ///<summary>
        /// bayer_gbrg8
        ///</summary>
        bayer_gbrg8,

        ///<summary>
        /// bayer_grbg8
        ///</summary>
        bayer_grbg8,

        ///<summary>
        /// bayer_bggr16le
        ///</summary>
        bayer_bggr16le,

        ///<summary>
        /// bayer_bggr16be
        ///</summary>
        bayer_bggr16be,

        ///<summary>
        /// bayer_rggb16le
        ///</summary>
        bayer_rggb16le,

        ///<summary>
        /// bayer_rggb16be
        ///</summary>
        bayer_rggb16be,

        ///<summary>
        /// bayer_gbrg16le
        ///</summary>
        bayer_gbrg16le,

        ///<summary>
        /// bayer_gbrg16be
        ///</summary>
        bayer_gbrg16be,

        ///<summary>
        /// bayer_grbg16le
        ///</summary>
        bayer_grbg16le,

        ///<summary>
        /// bayer_grbg16be
        ///</summary>
        bayer_grbg16be,

        ///<summary>
        /// xvmc
        ///</summary>
        xvmc,

        ///<summary>
        /// yuv440p10le
        ///</summary>
        yuv440p10le,

        ///<summary>
        /// yuv440p10be
        ///</summary>
        yuv440p10be,

        ///<summary>
        /// yuv440p12le
        ///</summary>
        yuv440p12le,

        ///<summary>
        /// yuv440p12be
        ///</summary>
        yuv440p12be,

        ///<summary>
        /// ayuv64le
        ///</summary>
        ayuv64le,

        ///<summary>
        /// ayuv64be
        ///</summary>
        ayuv64be,

        ///<summary>
        /// videotoolbox_vld
        ///</summary>
        videotoolbox_vld,

        ///<summary>
        /// p010le
        ///</summary>
        p010le,

        ///<summary>
        /// p010be
        ///</summary>
        p010be,

        ///<summary>
        /// gbrap12be
        ///</summary>
        gbrap12be,

        ///<summary>
        /// gbrap12le
        ///</summary>
        gbrap12le,

        ///<summary>
        /// gbrap10be
        ///</summary>
        gbrap10be,

        ///<summary>
        /// gbrap10le
        ///</summary>
        gbrap10le,

        ///<summary>
        /// mediacodec
        ///</summary>
        mediacodec,

        ///<summary>
        /// gray12be
        ///</summary>
        gray12be,

        ///<summary>
        /// gray12le
        ///</summary>
        gray12le,

        ///<summary>
        /// gray10be
        ///</summary>
        gray10be,

        ///<summary>
        /// gray10le
        ///</summary>
        gray10le,

        ///<summary>
        /// p016le
        ///</summary>
        p016le,

        ///<summary>
        /// p016be
        ///</summary>
        p016be,

        ///<summary>
        /// d3d11
        ///</summary>
        d3d11,

        ///<summary>
        /// gray9be
        ///</summary>
        gray9be,

        ///<summary>
        /// gray9le
        ///</summary>
        gray9le,

        ///<summary>
        /// gbrpf32be
        ///</summary>
        gbrpf32be,

        ///<summary>
        /// gbrpf32le
        ///</summary>
        gbrpf32le,

        ///<summary>
        /// gbrapf32be
        ///</summary>
        gbrapf32be,

        ///<summary>
        /// gbrapf32le
        ///</summary>
        gbrapf32le,

        ///<summary>
        /// drm_prime
        ///</summary>
        drm_prime,

        ///<summary>
        /// opencl
        ///</summary>
        opencl,

        ///<summary>
        /// gray14be
        ///</summary>
        gray14be,

        ///<summary>
        /// gray14le
        ///</summary>
        gray14le,

        ///<summary>
        /// grayf32be
        ///</summary>
        grayf32be,

        ///<summary>
        /// grayf32le
        ///</summary>
        grayf32le,

        ///<summary>
        /// yuva422p12be
        ///</summary>
        yuva422p12be,

        ///<summary>
        /// yuva422p12le
        ///</summary>
        yuva422p12le,

        ///<summary>
        /// yuva444p12be
        ///</summary>
        yuva444p12be,

        ///<summary>
        /// yuva444p12le
        ///</summary>
        yuva444p12le,

        ///<summary>
        /// nv24
        ///</summary>
        nv24,

        ///<summary>
        /// nv42
        ///</summary>
        nv42
    }
}
