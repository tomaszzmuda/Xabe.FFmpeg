namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Video codec ("ffmpeg -codecs")
    /// </summary>
    public enum VideoCodec
    {
        ///<summary>
        ///     Uncompressed 4:2:2 10-bit
        ///
        _012v,

        ///<summary>
        ///     4X Movie
        ///
        _4xm,

        ///<summary>
        ///     QuickTime 8BPS video
        ///
        _8bps,

        ///<summary>
        ///     Multicolor charset for Commodore 64 (encoders: a64multi )
        ///
        a64_multi,

        ///<summary>
        ///     Multicolor charset for Commodore 64, extended with 5th color (colram) (encoders: a64multi5 )
        ///
        a64_multi5,

        ///<summary>
        ///     Autodesk RLE
        ///
        aasc,

        ///<summary>
        ///     Amuse Graphics Movie
        ///
        agm,

        ///<summary>
        ///     Apple Intermediate Codec
        ///
        aic,

        ///<summary>
        ///     Alias/Wavefront PIX image
        ///
        alias_pix,

        ///<summary>
        ///     AMV Video
        ///
        amv,

        ///<summary>
        ///     Deluxe Paint Animation
        ///
        anm,

        ///<summary>
        ///     ASCII/ANSI art
        ///
        ansi,

        ///<summary>
        ///     APNG (Animated Portable Network Graphics) image
        ///
        apng,

        ///<summary>
        ///     Gryphon's Anim Compressor
        ///
        arbc,

        ///<summary>
        ///     ASUS V1
        ///
        asv1,

        ///<summary>
        ///     ASUS V2
        ///
        asv2,

        ///<summary>
        ///     Auravision AURA
        ///
        aura,

        ///<summary>
        ///     Auravision Aura 2
        ///
        aura2,

        ///<summary>
        ///     Alliance for Open Media AV1 (decoders: libaom-av1 libdav1d ) (encoders: libaom-av1 )
        ///
        av1,

        ///<summary>
        ///     Avid AVI Codec
        ///
        avrn,

        ///<summary>
        ///     Avid 1:1 10-bit RGB Packer
        ///
        avrp,

        ///<summary>
        ///     AVS (Audio Video Standard) video
        ///
        avs,

        ///<summary>
        ///     AVS2-P2/IEEE1857.4
        ///
        avs2,

        ///<summary>
        ///     Avid Meridien Uncompressed
        ///
        avui,

        ///<summary>
        ///     Uncompressed packed MS 4:4:4:4
        ///
        ayuv,

        ///<summary>
        ///     Bethesda VID video
        ///
        bethsoftvid,

        ///<summary>
        ///     Brute Force & Ignorance
        ///
        bfi,

        ///<summary>
        ///     Bink video
        ///
        binkvideo,

        ///<summary>
        ///     Binary text
        ///
        bintext,

        ///<summary>
        ///     Bitpacked
        ///
        bitpacked,

        ///<summary>
        ///     BMP (Windows and OS/2 bitmap)
        ///
        bmp,

        ///<summary>
        ///     Discworld II BMV video
        ///
        bmv_video,

        ///<summary>
        ///     BRender PIX image
        ///
        brender_pix,

        ///<summary>
        ///     Interplay C93
        ///
        c93,

        ///<summary>
        ///     Chinese AVS (Audio Video Standard) (AVS1-P2, JiZhun profile)
        ///
        cavs,

        ///<summary>
        ///     CD Graphics video
        ///
        cdgraphics,

        ///<summary>
        ///     Commodore CDXL video
        ///
        cdxl,

        ///<summary>
        ///     Cineform HD
        ///
        cfhd,

        ///<summary>
        ///     Cinepak
        ///
        cinepak,

        ///<summary>
        ///     Iterated Systems ClearVideo
        ///
        clearvideo,

        ///<summary>
        ///     Cirrus Logic AccuPak
        ///
        cljr,

        ///<summary>
        ///     Canopus Lossless Codec
        ///
        cllc,

        ///<summary>
        ///     Electronic Arts CMV video (decoders: eacmv )
        ///
        cmv,

        ///<summary>
        ///     CPiA video format
        ///
        cpia,

        ///<summary>
        ///     CamStudio (decoders: camstudio )
        ///
        cscd,

        ///<summary>
        ///     Creative YUV (CYUV)
        ///
        cyuv,

        ///<summary>
        ///     Daala
        ///
        daala,

        ///<summary>
        ///     DirectDraw Surface image decoder
        ///
        dds,

        ///<summary>
        ///     Chronomaster DFA
        ///
        dfa,

        ///<summary>
        ///     Dirac (encoders: vc2 )
        ///
        dirac,

        ///<summary>
        ///     VC3/DNxHD
        ///
        dnxhd,

        ///<summary>
        ///     DPX (Digital Picture Exchange) image
        ///
        dpx,

        ///<summary>
        ///     Delphine Software International CIN video
        ///
        dsicinvideo,

        ///<summary>
        ///     DV (Digital Video)
        ///
        dvvideo,

        ///<summary>
        ///     Feeble Files/ScummVM DXA
        ///
        dxa,

        ///<summary>
        ///     Dxtory
        ///
        dxtory,

        ///<summary>
        ///     Resolume DXV
        ///
        dxv,

        ///<summary>
        ///     Escape 124
        ///
        escape124,

        ///<summary>
        ///     Escape 130
        ///
        escape130,

        ///<summary>
        ///     OpenEXR image
        ///
        exr,

        ///<summary>
        ///     FFmpeg video codec #1
        ///
        ffv1,

        ///<summary>
        ///     Huffyuv FFmpeg variant
        ///
        ffvhuff,

        ///<summary>
        ///     Mirillis FIC
        ///
        fic,

        ///<summary>
        ///     FITS (Flexible Image Transport System)
        ///
        fits,

        ///<summary>
        ///     Flash Screen Video v1
        ///
        flashsv,

        ///<summary>
        ///     Flash Screen Video v2
        ///
        flashsv2,

        ///<summary>
        ///     Autodesk Animator Flic video
        ///
        flic,

        ///<summary>
        ///     FLV / Sorenson Spark / Sorenson H.263 (Flash Video) (decoders: flv ) (encoders: flv )
        ///
        flv1,

        ///<summary>
        ///     FM Screen Capture Codec
        ///
        fmvc,

        ///<summary>
        ///     Fraps
        ///
        fraps,

        ///<summary>
        ///     Forward Uncompressed
        ///
        frwu,

        ///<summary>
        ///     Go2Meeting
        ///
        g2m,

        ///<summary>
        ///     Gremlin Digital Video
        ///
        gdv,

        ///<summary>
        ///     CompuServe GIF (Graphics Interchange Format)
        ///
        gif,

        ///<summary>
        ///     H.261
        ///
        h261,

        ///<summary>
        ///     H.263 / H.263-1996, H.263+ / H.263-1998 / H.263 version 2
        ///
        h263,

        ///<summary>
        ///     Intel H.263
        ///
        h263i,

        ///<summary>
        ///     H.263+ / H.263-1998 / H.263 version 2
        ///
        h263p,

        ///<summary>
        ///     H.264 / AVC / MPEG-4 AVC / MPEG-4 part 10 (decoders: h264 h264_qsv h264_cuvid ) (encoders: libx264 libx264rgb h264_amf h264_nvenc h264_qsv nvenc nvenc_h264 )
        ///
        h264,

        ///<summary>
        ///     Vidvox Hap
        ///
        hap,

        ///<summary>
        ///     H.265 / HEVC (High Efficiency Video Coding) (decoders: hevc hevc_qsv hevc_cuvid ) (encoders: libx265 nvenc_hevc hevc_amf hevc_nvenc hevc_qsv )
        ///
        hevc,

        ///<summary>
        ///     HNM 4 video
        ///
        hnm4video,

        ///<summary>
        ///     Canopus HQ/HQA
        ///
        hq_hqa,

        ///<summary>
        ///     Canopus HQX
        ///
        hqx,

        ///<summary>
        ///     HuffYUV
        ///
        huffyuv,

        ///<summary>
        ///     HuffYUV MT
        ///
        hymt,

        ///<summary>
        ///     id Quake II CIN video (decoders: idcinvideo )
        ///
        idcin,

        ///<summary>
        ///     iCEDraw text
        ///
        idf,

        ///<summary>
        ///     IFF ACBM/ANIM/DEEP/ILBM/PBM/RGB8/RGBN (decoders: iff )
        ///
        iff_ilbm,

        ///<summary>
        ///     Infinity IMM4
        ///
        imm4,

        ///<summary>
        ///     Intel Indeo 2
        ///
        indeo2,

        ///<summary>
        ///     Intel Indeo 3
        ///
        indeo3,

        ///<summary>
        ///     Intel Indeo Video Interactive 4
        ///
        indeo4,

        ///<summary>
        ///     Intel Indeo Video Interactive 5
        ///
        indeo5,

        ///<summary>
        ///     Interplay MVE video
        ///
        interplayvideo,

        ///<summary>
        ///     JPEG 2000 (decoders: jpeg2000 libopenjpeg ) (encoders: jpeg2000 libopenjpeg )
        ///
        jpeg2000,

        ///<summary>
        ///     JPEG-LS
        ///
        jpegls,

        ///<summary>
        ///     Bitmap Brothers JV video
        ///
        jv,

        ///<summary>
        ///     Kega Game Video
        ///
        kgv1,

        ///<summary>
        ///     Karl Morton's video codec
        ///
        kmvc,

        ///<summary>
        ///     Lagarith lossless
        ///
        lagarith,

        ///<summary>
        ///     Lossless JPEG
        ///
        ljpeg,

        ///<summary>
        ///     LOCO
        ///
        loco,

        ///<summary>
        ///     LEAD Screen Capture
        ///
        lscr,

        ///<summary>
        ///     Matrox Uncompressed SD
        ///
        m101,

        ///<summary>
        ///     Electronic Arts Madcow Video (decoders: eamad )
        ///
        mad,

        ///<summary>
        ///     MagicYUV video
        ///
        magicyuv,

        ///<summary>
        ///     Sony PlayStation MDEC (Motion DECoder)
        ///
        mdec,

        ///<summary>
        ///     Mimic
        ///
        mimic,

        ///<summary>
        ///     Motion JPEG (decoders: mjpeg mjpeg_cuvid ) (encoders: mjpeg mjpeg_qsv )
        ///
        mjpeg,

        ///<summary>
        ///     Apple MJPEG-B
        ///
        mjpegb,

        ///<summary>
        ///     American Laser Games MM Video
        ///
        mmvideo,

        ///<summary>
        ///     Motion Pixels video
        ///
        motionpixels,

        ///<summary>
        ///     MPEG-1 video (decoders: mpeg1video mpeg1_cuvid )
        ///
        mpeg1video,

        ///<summary>
        ///     MPEG-2 video (decoders: mpeg2video mpegvideo mpeg2_qsv mpeg2_cuvid ) (encoders: mpeg2video mpeg2_qsv )
        ///
        mpeg2video,

        ///<summary>
        ///     MPEG-4 part 2 (decoders: mpeg4 mpeg4_cuvid ) (encoders: mpeg4 libxvid )
        ///
        mpeg4,

        ///<summary>
        ///     MS ATC Screen
        ///
        msa1,

        ///<summary>
        ///     Mandsoft Screen Capture Codec
        ///
        mscc,

        ///<summary>
        ///     MPEG-4 part 2 Microsoft variant version 1
        ///
        msmpeg4v1,

        ///<summary>
        ///     MPEG-4 part 2 Microsoft variant version 2
        ///
        msmpeg4v2,

        ///<summary>
        ///     MPEG-4 part 2 Microsoft variant version 3 (decoders: msmpeg4 ) (encoders: msmpeg4 )
        ///
        msmpeg4v3,

        ///<summary>
        ///     Microsoft RLE
        ///
        msrle,

        ///<summary>
        ///     MS Screen 1
        ///
        mss1,

        ///<summary>
        ///     MS Windows Media Video V9 Screen
        ///
        mss2,

        ///<summary>
        ///     Microsoft Video 1
        ///
        msvideo1,

        ///<summary>
        ///     LCL (LossLess Codec Library) MSZH
        ///
        mszh,

        ///<summary>
        ///     MS Expression Encoder Screen
        ///
        mts2,

        ///<summary>
        ///     Silicon Graphics Motion Video Compressor 1
        ///
        mvc1,

        ///<summary>
        ///     Silicon Graphics Motion Video Compressor 2
        ///
        mvc2,

        ///<summary>
        ///     MatchWare Screen Capture Codec
        ///
        mwsc,

        ///<summary>
        ///     Mobotix MxPEG video
        ///
        mxpeg,

        ///<summary>
        ///     NuppelVideo/RTJPEG
        ///
        nuv,

        ///<summary>
        ///     Amazing Studio Packed Animation File Video
        ///
        paf_video,

        ///<summary>
        ///     PAM (Portable AnyMap) image
        ///
        pam,

        ///<summary>
        ///     PBM (Portable BitMap) image
        ///
        pbm,

        ///<summary>
        ///     PC Paintbrush PCX image
        ///
        pcx,

        ///<summary>
        ///     PGM (Portable GrayMap) image
        ///
        pgm,

        ///<summary>
        ///     PGMYUV (Portable GrayMap YUV) image
        ///
        pgmyuv,

        ///<summary>
        ///     Pictor/PC Paint
        ///
        pictor,

        ///<summary>
        ///     Apple Pixlet
        ///
        pixlet,

        ///<summary>
        ///     PNG (Portable Network Graphics) image
        ///
        png,

        ///<summary>
        ///     PPM (Portable PixelMap) image
        ///
        ppm,

        ///<summary>
        ///     Apple ProRes (iCodec Pro) (encoders: prores prores_aw prores_ks )
        ///
        prores,

        ///<summary>
        ///     Brooktree ProSumer Video
        ///
        prosumer,

        ///<summary>
        ///     Photoshop PSD file
        ///
        psd,

        ///<summary>
        ///     V.Flash PTX image
        ///
        ptx,

        ///<summary>
        ///     Apple QuickDraw
        ///
        qdraw,

        ///<summary>
        ///     Q-team QPEG
        ///
        qpeg,

        ///<summary>
        ///     QuickTime Animation (RLE) video
        ///
        qtrle,

        ///<summary>
        ///     AJA Kona 10-bit RGB Codec
        ///
        r10k,

        ///<summary>
        ///     Uncompressed RGB 10-bit
        ///
        r210,

        ///<summary>
        ///     RemotelyAnywhere Screen Capture
        ///
        rasc,

        ///<summary>
        ///     raw video
        ///
        rawvideo,

        ///<summary>
        ///     RL2 video
        ///
        rl2,

        ///<summary>
        ///     id RoQ video (decoders: roqvideo ) (encoders: roqvideo )
        ///
        roq,

        ///<summary>
        ///     QuickTime video (RPZA)
        ///
        rpza,

        ///<summary>
        ///     innoHeim/Rsupport Screen Capture Codec
        ///
        rscc,

        ///<summary>
        ///     RealVideo 1.0
        ///
        rv10,

        ///<summary>
        ///     RealVideo 2.0
        ///
        rv20,

        ///<summary>
        ///     RealVideo 3.0
        ///
        rv30,

        ///<summary>
        ///     RealVideo 4.0
        ///
        rv40,

        ///<summary>
        ///     LucasArts SANM/SMUSH video
        ///
        sanm,

        ///<summary>
        ///     ScreenPressor
        ///
        scpr,

        ///<summary>
        ///     Screenpresso
        ///
        screenpresso,

        ///<summary>
        ///     SGI image
        ///
        sgi,

        ///<summary>
        ///     SGI RLE 8-bit
        ///
        sgirle,

        ///<summary>
        ///     BitJazz SheerVideo
        ///
        sheervideo,

        ///<summary>
        ///     Smacker video (decoders: smackvid )
        ///
        smackvideo,

        ///<summary>
        ///     QuickTime Graphics (SMC)
        ///
        smc,

        ///<summary>
        ///     Sigmatel Motion Video
        ///
        smvjpeg,

        ///<summary>
        ///     Snow
        ///
        snow,

        ///<summary>
        ///     Sunplus JPEG (SP5X)
        ///
        sp5x,

        ///<summary>
        ///     NewTek SpeedHQ
        ///
        speedhq,

        ///<summary>
        ///     Screen Recorder Gold Codec
        ///
        srgc,

        ///<summary>
        ///     Sun Rasterfile image
        ///
        sunrast,

        ///<summary>
        ///     Scalable Vector Graphics
        ///
        svg,

        ///<summary>
        ///     Sorenson Vector Quantizer 1 / Sorenson Video 1 / SVQ1
        ///
        svq1,

        ///<summary>
        ///     Sorenson Vector Quantizer 3 / Sorenson Video 3 / SVQ3
        ///
        svq3,

        ///<summary>
        ///     Truevision Targa image
        ///
        targa,

        ///<summary>
        ///     Pinnacle TARGA CineWave YUV16
        ///
        targa_y216,

        ///<summary>
        ///     TDSC
        ///
        tdsc,

        ///<summary>
        ///     Electronic Arts TGQ video (decoders: eatgq )
        ///
        tgq,

        ///<summary>
        ///     Electronic Arts TGV video (decoders: eatgv )
        ///
        tgv,

        ///<summary>
        ///     Theora (encoders: libtheora )
        ///
        theora,

        ///<summary>
        ///     Nintendo Gamecube THP video
        ///
        thp,

        ///<summary>
        ///     Tiertex Limited SEQ video
        ///
        tiertexseqvideo,

        ///<summary>
        ///     TIFF image
        ///
        tiff,

        ///<summary>
        ///     8088flex TMV
        ///
        tmv,

        ///<summary>
        ///     Electronic Arts TQI video (decoders: eatqi )
        ///
        tqi,

        ///<summary>
        ///     Duck TrueMotion 1.0
        ///
        truemotion1,

        ///<summary>
        ///     Duck TrueMotion 2.0
        ///
        truemotion2,

        ///<summary>
        ///     Duck TrueMotion 2.0 Real Time
        ///
        truemotion2rt,

        ///<summary>
        ///     TechSmith Screen Capture Codec (decoders: camtasia )
        ///
        tscc,

        ///<summary>
        ///     TechSmith Screen Codec 2
        ///
        tscc2,

        ///<summary>
        ///     Renderware TXD (TeXture Dictionary) image
        ///
        txd,

        ///<summary>
        ///     IBM UltiMotion (decoders: ultimotion )
        ///
        ulti,

        ///<summary>
        ///     Ut Video
        ///
        utvideo,

        ///<summary>
        ///     Uncompressed 4:2:2 10-bit
        ///
        v210,

        ///<summary>
        ///     Uncompressed 4:2:2 10-bit
        ///
        v210x,

        ///<summary>
        ///     Uncompressed packed 4:4:4
        ///
        v308,

        ///<summary>
        ///     Uncompressed packed QT 4:4:4:4
        ///
        v408,

        ///<summary>
        ///     Uncompressed 4:4:4 10-bit
        ///
        v410,

        ///<summary>
        ///     Beam Software VB
        ///
        vb,

        ///<summary>
        ///     VBLE Lossless Codec
        ///
        vble,

        ///<summary>
        ///     SMPTE VC-1 (decoders: vc1 vc1_qsv vc1_cuvid )
        ///
        vc1,

        ///<summary>
        ///     Windows Media Video 9 Image v2
        ///
        vc1image,

        ///<summary>
        ///     ATI VCR1
        ///
        vcr1,

        ///<summary>
        ///     Miro VideoXL (decoders: xl )
        ///
        vixl,

        ///<summary>
        ///     Sierra VMD video
        ///
        vmdvideo,

        ///<summary>
        ///     VMware Screen Codec / VMware Video
        ///
        vmnc,

        ///<summary>
        ///     On2 VP3
        ///
        vp3,

        ///<summary>
        ///     On2 VP4
        ///
        vp4,

        ///<summary>
        ///     On2 VP5
        ///
        vp5,

        ///<summary>
        ///     On2 VP6
        ///
        vp6,

        ///<summary>
        ///     On2 VP6 (Flash version, with alpha channel)
        ///
        vp6a,

        ///<summary>
        ///     On2 VP6 (Flash version)
        ///
        vp6f,

        ///<summary>
        ///     On2 VP7
        ///
        vp7,

        ///<summary>
        ///     On2 VP8 (decoders: vp8 libvpx vp8_cuvid vp8_qsv ) (encoders: libvpx )
        ///
        vp8,

        ///<summary>
        ///     Google VP9 (decoders: vp9 libvpx-vp9 vp9_cuvid ) (encoders: libvpx-vp9 )
        ///
        vp9,

        ///<summary>
        ///     WinCAM Motion Video
        ///
        wcmv,

        ///<summary>
        ///     WebP (encoders: libwebp_anim libwebp )
        ///
        webp,

        ///<summary>
        ///     Windows Media Video 7
        ///
        wmv1,

        ///<summary>
        ///     Windows Media Video 8
        ///
        wmv2,

        ///<summary>
        ///     Windows Media Video 9
        ///
        wmv3,

        ///<summary>
        ///     Windows Media Video 9 Image
        ///
        wmv3image,

        ///<summary>
        ///     Winnov WNV1
        ///
        wnv1,

        ///<summary>
        ///     AVFrame to AVPacket passthrough
        ///
        wrapped_avframe,

        ///<summary>
        ///     Westwood Studios VQA (Vector Quantized Animation) video (decoders: vqavideo )
        ///
        ws_vqa,

        ///<summary>
        ///     Wing Commander III / Xan
        ///
        xan_wc3,

        ///<summary>
        ///     Wing Commander IV / Xxan
        ///
        xan_wc4,

        ///<summary>
        ///     eXtended BINary text
        ///
        xbin,

        ///<summary>
        ///     XBM (X BitMap) image
        ///
        xbm,

        ///<summary>
        ///     X-face image
        ///
        xface,

        ///<summary>
        ///     XPM (X PixMap) image
        ///
        xpm,

        ///<summary>
        ///     XWD (X Window Dump) image
        ///
        xwd,

        ///<summary>
        ///     Uncompressed YUV 4:1:1 12-bit
        ///
        y41p,

        ///<summary>
        ///     YUY2 Lossless Codec
        ///
        ylc,

        ///<summary>
        ///     Psygnosis YOP Video
        ///
        yop,

        ///<summary>
        ///     Uncompressed packed 4:2:0
        ///
        yuv4,

        ///<summary>
        ///     ZeroCodec Lossless Video
        ///
        zerocodec,

        ///<summary>
        ///     LCL (LossLess Codec Library) ZLIB
        ///
        zlib,

        ///<summary>
        ///     Zip Motion Blocks Video
        ///
        zmbv,

        ///<summary>
        ///     copy
        ///
        copy,
        h264_nvenc,
        h264_cuvid,
        libx264
    }
}
