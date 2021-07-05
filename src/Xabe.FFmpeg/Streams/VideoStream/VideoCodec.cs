namespace Xabe.FFmpeg
{
    ///<summary>
    ///      Video codec ("ffmpeg -codecs")
    ///</summary>
    public enum VideoCodec
    {
        ///<summary>
        ///      Uncompressed 4:2:2 10-bit
        ///</summary>
        _012v,

        ///<summary>
        ///      4X Movie
        ///</summary>
        _4xm,

        ///<summary>
        ///      QuickTime 8BPS video
        ///</summary>
        _8bps,

        ///<summary>
        ///      Multicolor charset for Commodore 64 (encoders: a64multi )
        ///</summary>
        a64_multi,

        ///<summary>
        ///      Multicolor charset for Commodore 64, extended with 5th color (colram) (encoders: a64multi5 )
        ///</summary>
        a64_multi5,

        ///<summary>
        ///      Autodesk RLE
        ///</summary>
        aasc,

        ///<summary>
        ///      Amuse Graphics Movie
        ///</summary>
        agm,

        ///<summary>
        ///      Apple Intermediate Codec
        ///</summary>
        aic,

        ///<summary>
        ///      Alias/Wavefront PIX image
        ///</summary>
        alias_pix,

        ///<summary>
        ///      AMV Video
        ///</summary>
        amv,

        ///<summary>
        ///      Deluxe Paint Animation
        ///</summary>
        anm,

        ///<summary>
        ///      ASCII/ANSI art
        ///</summary>
        ansi,

        ///<summary>
        ///      APNG (Animated Portable Network Graphics) image
        ///</summary>
        apng,

        ///<summary>
        ///      Gryphon's Anim Compressor
        ///</summary>
        arbc,

        ///<summary>
        ///      ASUS V1
        ///</summary>
        asv1,

        ///<summary>
        ///      ASUS V2
        ///</summary>
        asv2,

        ///<summary>
        ///      Auravision AURA
        ///</summary>
        aura,

        ///<summary>
        ///      Auravision Aura 2
        ///</summary>
        aura2,

        ///<summary>
        ///      Alliance for Open Media AV1 (decoders: libaom-av1 libdav1d ) (encoders: libaom-av1 )
        ///</summary>
        av1,

        ///<summary>
        ///      Avid AVI Codec
        ///</summary>
        avrn,

        ///<summary>
        ///      Avid 1:1 10-bit RGB Packer
        ///</summary>
        avrp,

        ///<summary>
        ///      AVS (Audio Video Standard) video
        ///</summary>
        avs,

        ///<summary>
        ///      AVS2-P2/IEEE1857.4
        ///</summary>
        avs2,

        ///<summary>
        ///      Avid Meridien Uncompressed
        ///</summary>
        avui,

        ///<summary>
        ///      Uncompressed packed MS 4:4:4:4
        ///</summary>
        ayuv,

        ///<summary>
        ///      Bethesda VID video
        ///</summary>
        bethsoftvid,

        ///<summary>
        ///      Brute Force and Ignorance
        ///</summary>
        bfi,

        ///<summary>
        ///      Bink video
        ///</summary>
        binkvideo,

        ///<summary>
        ///      Binary text
        ///</summary>
        bintext,

        ///<summary>
        ///      Bitpacked
        ///</summary>
        bitpacked,

        ///<summary>
        ///      BMP (Windows and OS/2 bitmap)
        ///</summary>
        bmp,

        ///<summary>
        ///      Discworld II BMV video
        ///</summary>
        bmv_video,

        ///<summary>
        ///      BRender PIX image
        ///</summary>
        brender_pix,

        ///<summary>
        ///      Interplay C93
        ///</summary>
        c93,

        ///<summary>
        ///      Chinese AVS (Audio Video Standard) (AVS1-P2, JiZhun profile)
        ///</summary>
        cavs,

        ///<summary>
        ///      CD Graphics video
        ///</summary>
        cdgraphics,

        ///<summary>
        ///      Commodore CDXL video
        ///</summary>
        cdxl,

        ///<summary>
        ///      Cineform HD
        ///</summary>
        cfhd,

        ///<summary>
        ///      Cinepak
        ///</summary>
        cinepak,

        ///<summary>
        ///      Iterated Systems ClearVideo
        ///</summary>
        clearvideo,

        ///<summary>
        ///      Cirrus Logic AccuPak
        ///</summary>
        cljr,

        ///<summary>
        ///      Canopus Lossless Codec
        ///</summary>
        cllc,

        ///<summary>
        ///      Electronic Arts CMV video (decoders: eacmv )
        ///</summary>
        cmv,

        ///<summary>
        ///      CPiA video format
        ///</summary>
        cpia,

        ///<summary>
        ///      CamStudio (decoders: camstudio )
        ///</summary>
        cscd,

        ///<summary>
        ///      Creative YUV (CYUV)
        ///</summary>
        cyuv,

        ///<summary>
        ///      Daala
        ///</summary>
        daala,

        ///<summary>
        ///      DirectDraw Surface image decoder
        ///</summary>
        dds,

        ///<summary>
        ///      Chronomaster DFA
        ///</summary>
        dfa,

        ///<summary>
        ///      Dirac (encoders: vc2 )
        ///</summary>
        dirac,

        ///<summary>
        ///      VC3/DNxHD
        ///</summary>
        dnxhd,

        ///<summary>
        ///      DPX (Digital Picture Exchange) image
        ///</summary>
        dpx,

        ///<summary>
        ///      Delphine Software International CIN video
        ///</summary>
        dsicinvideo,

        ///<summary>
        ///      DV (Digital Video)
        ///</summary>
        dvvideo,

        ///<summary>
        ///      Feeble Files/ScummVM DXA
        ///</summary>
        dxa,

        ///<summary>
        ///      Dxtory
        ///</summary>
        dxtory,

        ///<summary>
        ///      Resolume DXV
        ///</summary>
        dxv,

        ///<summary>
        ///      Escape 124
        ///</summary>
        escape124,

        ///<summary>
        ///      Escape 130
        ///</summary>
        escape130,

        ///<summary>
        ///      OpenEXR image
        ///</summary>
        exr,

        ///<summary>
        ///      FFmpeg video codec #1
        ///</summary>
        ffv1,

        ///<summary>
        ///      Huffyuv FFmpeg variant
        ///</summary>
        ffvhuff,

        ///<summary>
        ///      Mirillis FIC
        ///</summary>
        fic,

        ///<summary>
        ///      FITS (Flexible Image Transport System)
        ///</summary>
        fits,

        ///<summary>
        ///      Flash Screen Video v1
        ///</summary>
        flashsv,

        ///<summary>
        ///      Flash Screen Video v2
        ///</summary>
        flashsv2,

        ///<summary>
        ///      Autodesk Animator Flic video
        ///</summary>
        flic,

        ///<summary>
        ///      FLV / Sorenson Spark / Sorenson H.263 (Flash Video) (decoders: flv ) (encoders: flv )
        ///</summary>
        flv1,

        ///<summary>
        ///      FM Screen Capture Codec
        ///</summary>
        fmvc,

        ///<summary>
        ///      Fraps
        ///</summary>
        fraps,

        ///<summary>
        ///      Forward Uncompressed
        ///</summary>
        frwu,

        ///<summary>
        ///      Go2Meeting
        ///</summary>
        g2m,

        ///<summary>
        ///      Gremlin Digital Video
        ///</summary>
        gdv,

        ///<summary>
        ///      CompuServe GIF (Graphics Interchange Format)
        ///</summary>
        gif,

        ///<summary>
        ///      H.261
        ///</summary>
        h261,

        ///<summary>
        ///      H.263 / H.263-1996, H.263+ / H.263-1998 / H.263 version 2
        ///</summary>
        h263,

        ///<summary>
        ///      Intel H.263
        ///</summary>
        h263i,

        ///<summary>
        ///      H.263+ / H.263-1998 / H.263 version 2
        ///</summary>
        h263p,

        ///<summary>
        ///      H.264 / AVC / MPEG-4 AVC / MPEG-4 part 10 (decoders: h264 h264_qsv h264_cuvid ) (encoders: libx264 libx264rgb h264_amf h264_nvenc h264_qsv nvenc nvenc_h264 )
        ///</summary>
        h264,

        ///<summary>
        ///      Vidvox Hap
        ///</summary>
        hap,

        ///<summary>
        ///      H.265 / HEVC (High Efficiency Video Coding) (decoders: hevc hevc_qsv hevc_cuvid ) (encoders: libx265 nvenc_hevc hevc_amf hevc_nvenc hevc_qsv )
        ///</summary>
        hevc,

        ///<summary>
        ///      HNM 4 video
        ///</summary>
        hnm4video,

        ///<summary>
        ///      Canopus HQ/HQA
        ///</summary>
        hq_hqa,

        ///<summary>
        ///      Canopus HQX
        ///</summary>
        hqx,

        ///<summary>
        ///      HuffYUV
        ///</summary>
        huffyuv,

        ///<summary>
        ///      HuffYUV MT
        ///</summary>
        hymt,

        ///<summary>
        ///      id Quake II CIN video (decoders: idcinvideo )
        ///</summary>
        idcin,

        ///<summary>
        ///      iCEDraw text
        ///</summary>
        idf,

        ///<summary>
        ///      IFF ACBM/ANIM/DEEP/ILBM/PBM/RGB8/RGBN (decoders: iff )
        ///</summary>
        iff_ilbm,

        ///<summary>
        ///      Infinity IMM4
        ///</summary>
        imm4,

        ///<summary>
        ///      Intel Indeo 2
        ///</summary>
        indeo2,

        ///<summary>
        ///      Intel Indeo 3
        ///</summary>
        indeo3,

        ///<summary>
        ///      Intel Indeo Video Interactive 4
        ///</summary>
        indeo4,

        ///<summary>
        ///      Intel Indeo Video Interactive 5
        ///</summary>
        indeo5,

        ///<summary>
        ///      Interplay MVE video
        ///</summary>
        interplayvideo,

        ///<summary>
        ///      JPEG 2000 (decoders: jpeg2000 libopenjpeg ) (encoders: jpeg2000 libopenjpeg )
        ///</summary>
        jpeg2000,

        ///<summary>
        ///      JPEG-LS
        ///</summary>
        jpegls,

        ///<summary>
        ///      Bitmap Brothers JV video
        ///</summary>
        jv,

        ///<summary>
        ///      Kega Game Video
        ///</summary>
        kgv1,

        ///<summary>
        ///      Karl Morton's video codec
        ///</summary>
        kmvc,

        ///<summary>
        ///      Lagarith lossless
        ///</summary>
        lagarith,

        ///<summary>
        ///      Lossless JPEG
        ///</summary>
        ljpeg,

        ///<summary>
        ///      LOCO
        ///</summary>
        loco,

        ///<summary>
        ///      LEAD Screen Capture
        ///</summary>
        lscr,

        ///<summary>
        ///      Matrox Uncompressed SD
        ///</summary>
        m101,

        ///<summary>
        ///      Electronic Arts Madcow Video (decoders: eamad )
        ///</summary>
        mad,

        ///<summary>
        ///      MagicYUV video
        ///</summary>
        magicyuv,

        ///<summary>
        ///      Sony PlayStation MDEC (Motion DECoder)
        ///</summary>
        mdec,

        ///<summary>
        ///      Mimic
        ///</summary>
        mimic,

        ///<summary>
        ///      Motion JPEG (decoders: mjpeg mjpeg_cuvid ) (encoders: mjpeg mjpeg_qsv )
        ///</summary>
        mjpeg,

        ///<summary>
        ///      Apple MJPEG-B
        ///</summary>
        mjpegb,

        ///<summary>
        ///      American Laser Games MM Video
        ///</summary>
        mmvideo,

        ///<summary>
        ///      Motion Pixels video
        ///</summary>
        motionpixels,

        ///<summary>
        ///      MPEG-1 video (decoders: mpeg1video mpeg1_cuvid )
        ///</summary>
        mpeg1video,

        ///<summary>
        ///      MPEG-2 video (decoders: mpeg2video mpegvideo mpeg2_qsv mpeg2_cuvid ) (encoders: mpeg2video mpeg2_qsv )
        ///</summary>
        mpeg2video,

        ///<summary>
        ///      MPEG-4 part 2 (decoders: mpeg4 mpeg4_cuvid ) (encoders: mpeg4 libxvid )
        ///</summary>
        mpeg4,

        ///<summary>
        ///      MS ATC Screen
        ///</summary>
        msa1,

        ///<summary>
        ///      Mandsoft Screen Capture Codec
        ///</summary>
        mscc,

        ///<summary>
        ///      MPEG-4 part 2 Microsoft variant version 1
        ///</summary>
        msmpeg4v1,

        ///<summary>
        ///      MPEG-4 part 2 Microsoft variant version 2
        ///</summary>
        msmpeg4v2,

        ///<summary>
        ///      MPEG-4 part 2 Microsoft variant version 3 (decoders: msmpeg4 ) (encoders: msmpeg4 )
        ///</summary>
        msmpeg4v3,

        ///<summary>
        ///      Microsoft RLE
        ///</summary>
        msrle,

        ///<summary>
        ///      MS Screen 1
        ///</summary>
        mss1,

        ///<summary>
        ///      MS Windows Media Video V9 Screen
        ///</summary>
        mss2,

        ///<summary>
        ///      Microsoft Video 1
        ///</summary>
        msvideo1,

        ///<summary>
        ///      LCL (LossLess Codec Library) MSZH
        ///</summary>
        mszh,

        ///<summary>
        ///      MS Expression Encoder Screen
        ///</summary>
        mts2,

        ///<summary>
        ///      Silicon Graphics Motion Video Compressor 1
        ///</summary>
        mvc1,

        ///<summary>
        ///      Silicon Graphics Motion Video Compressor 2
        ///</summary>
        mvc2,

        ///<summary>
        ///      MatchWare Screen Capture Codec
        ///</summary>
        mwsc,

        ///<summary>
        ///      Mobotix MxPEG video
        ///</summary>
        mxpeg,

        ///<summary>
        ///      NuppelVideo/RTJPEG
        ///</summary>
        nuv,

        ///<summary>
        ///      Amazing Studio Packed Animation File Video
        ///</summary>
        paf_video,

        ///<summary>
        ///      PAM (Portable AnyMap) image
        ///</summary>
        pam,

        ///<summary>
        ///      PBM (Portable BitMap) image
        ///</summary>
        pbm,

        ///<summary>
        ///      PC Paintbrush PCX image
        ///</summary>
        pcx,

        ///<summary>
        ///      PGM (Portable GrayMap) image
        ///</summary>
        pgm,

        ///<summary>
        ///      PGMYUV (Portable GrayMap YUV) image
        ///</summary>
        pgmyuv,

        ///<summary>
        ///      Pictor/PC Paint
        ///</summary>
        pictor,

        ///<summary>
        ///      Apple Pixlet
        ///</summary>
        pixlet,

        ///<summary>
        ///      PNG (Portable Network Graphics) image
        ///</summary>
        png,

        ///<summary>
        ///      PPM (Portable PixelMap) image
        ///</summary>
        ppm,

        ///<summary>
        ///      Apple ProRes (iCodec Pro) (encoders: prores prores_aw prores_ks )
        ///</summary>
        prores,

        ///<summary>
        ///      Brooktree ProSumer Video
        ///</summary>
        prosumer,

        ///<summary>
        ///      Photoshop PSD file
        ///</summary>
        psd,

        ///<summary>
        ///      V.Flash PTX image
        ///</summary>
        ptx,

        ///<summary>
        ///      Apple QuickDraw
        ///</summary>
        qdraw,

        ///<summary>
        ///      Q-team QPEG
        ///</summary>
        qpeg,

        ///<summary>
        ///      QuickTime Animation (RLE) video
        ///</summary>
        qtrle,

        ///<summary>
        ///      AJA Kona 10-bit RGB Codec
        ///</summary>
        r10k,

        ///<summary>
        ///      Uncompressed RGB 10-bit
        ///</summary>
        r210,

        ///<summary>
        ///      RemotelyAnywhere Screen Capture
        ///</summary>
        rasc,

        ///<summary>
        ///      raw video
        ///</summary>
        rawvideo,

        ///<summary>
        ///      RL2 video
        ///</summary>
        rl2,

        ///<summary>
        ///      id RoQ video (decoders: roqvideo ) (encoders: roqvideo )
        ///</summary>
        roq,

        ///<summary>
        ///      QuickTime video (RPZA)
        ///</summary>
        rpza,

        ///<summary>
        ///      innoHeim/Rsupport Screen Capture Codec
        ///</summary>
        rscc,

        ///<summary>
        ///      RealVideo 1.0
        ///</summary>
        rv10,

        ///<summary>
        ///      RealVideo 2.0
        ///</summary>
        rv20,

        ///<summary>
        ///      RealVideo 3.0
        ///</summary>
        rv30,

        ///<summary>
        ///      RealVideo 4.0
        ///</summary>
        rv40,

        ///<summary>
        ///      LucasArts SANM/SMUSH video
        ///</summary>
        sanm,

        ///<summary>
        ///      ScreenPressor
        ///</summary>
        scpr,

        ///<summary>
        ///      Screenpresso
        ///</summary>
        screenpresso,

        ///<summary>
        ///      SGI image
        ///</summary>
        sgi,

        ///<summary>
        ///      SGI RLE 8-bit
        ///</summary>
        sgirle,

        ///<summary>
        ///      BitJazz SheerVideo
        ///</summary>
        sheervideo,

        ///<summary>
        ///      Smacker video (decoders: smackvid )
        ///</summary>
        smackvideo,

        ///<summary>
        ///      QuickTime Graphics (SMC)
        ///</summary>
        smc,

        ///<summary>
        ///      Sigmatel Motion Video
        ///</summary>
        smvjpeg,

        ///<summary>
        ///      Snow
        ///</summary>
        snow,

        ///<summary>
        ///      Sunplus JPEG (SP5X)
        ///</summary>
        sp5x,

        ///<summary>
        ///      NewTek SpeedHQ
        ///</summary>
        speedhq,

        ///<summary>
        ///      Screen Recorder Gold Codec
        ///</summary>
        srgc,

        ///<summary>
        ///      Sun Rasterfile image
        ///</summary>
        sunrast,

        ///<summary>
        ///      Scalable Vector Graphics
        ///</summary>
        svg,

        ///<summary>
        ///      Sorenson Vector Quantizer 1 / Sorenson Video 1 / SVQ1
        ///</summary>
        svq1,

        ///<summary>
        ///      Sorenson Vector Quantizer 3 / Sorenson Video 3 / SVQ3
        ///</summary>
        svq3,

        ///<summary>
        ///      Truevision Targa image
        ///</summary>
        targa,

        ///<summary>
        ///      Pinnacle TARGA CineWave YUV16
        ///</summary>
        targa_y216,

        ///<summary>
        ///      TDSC
        ///</summary>
        tdsc,

        ///<summary>
        ///      Electronic Arts TGQ video (decoders: eatgq )
        ///</summary>
        tgq,

        ///<summary>
        ///      Electronic Arts TGV video (decoders: eatgv )
        ///</summary>
        tgv,

        ///<summary>
        ///      Theora (encoders: libtheora )
        ///</summary>
        theora,

        ///<summary>
        ///      Nintendo Gamecube THP video
        ///</summary>
        thp,

        ///<summary>
        ///      Tiertex Limited SEQ video
        ///</summary>
        tiertexseqvideo,

        ///<summary>
        ///      TIFF image
        ///</summary>
        tiff,

        ///<summary>
        ///      8088flex TMV
        ///</summary>
        tmv,

        ///<summary>
        ///      Electronic Arts TQI video (decoders: eatqi )
        ///</summary>
        tqi,

        ///<summary>
        ///      Duck TrueMotion 1.0
        ///</summary>
        truemotion1,

        ///<summary>
        ///      Duck TrueMotion 2.0
        ///</summary>
        truemotion2,

        ///<summary>
        ///      Duck TrueMotion 2.0 Real Time
        ///</summary>
        truemotion2rt,

        ///<summary>
        ///      TechSmith Screen Capture Codec (decoders: camtasia )
        ///</summary>
        tscc,

        ///<summary>
        ///      TechSmith Screen Codec 2
        ///</summary>
        tscc2,

        ///<summary>
        ///      Renderware TXD (TeXture Dictionary) image
        ///</summary>
        txd,

        ///<summary>
        ///      IBM UltiMotion (decoders: ultimotion )
        ///</summary>
        ulti,

        ///<summary>
        ///      Ut Video
        ///</summary>
        utvideo,

        ///<summary>
        ///      Uncompressed 4:2:2 10-bit
        ///</summary>
        v210,

        ///<summary>
        ///      Uncompressed 4:2:2 10-bit
        ///</summary>
        v210x,

        ///<summary>
        ///      Uncompressed packed 4:4:4
        ///</summary>
        v308,

        ///<summary>
        ///      Uncompressed packed QT 4:4:4:4
        ///</summary>
        v408,

        ///<summary>
        ///      Uncompressed 4:4:4 10-bit
        ///</summary>
        v410,

        ///<summary>
        ///      Beam Software VB
        ///</summary>
        vb,

        ///<summary>
        ///      VBLE Lossless Codec
        ///</summary>
        vble,

        ///<summary>
        ///      SMPTE VC-1 (decoders: vc1 vc1_qsv vc1_cuvid )
        ///</summary>
        vc1,

        ///<summary>
        ///      Windows Media Video 9 Image v2
        ///</summary>
        vc1image,

        ///<summary>
        ///      ATI VCR1
        ///</summary>
        vcr1,

        ///<summary>
        ///      Miro VideoXL (decoders: xl )
        ///</summary>
        vixl,

        ///<summary>
        ///      Sierra VMD video
        ///</summary>
        vmdvideo,

        ///<summary>
        ///      VMware Screen Codec / VMware Video
        ///</summary>
        vmnc,

        ///<summary>
        ///      On2 VP3
        ///</summary>
        vp3,

        ///<summary>
        ///      On2 VP4
        ///</summary>
        vp4,

        ///<summary>
        ///      On2 VP5
        ///</summary>
        vp5,

        ///<summary>
        ///      On2 VP6
        ///</summary>
        vp6,

        ///<summary>
        ///      On2 VP6 (Flash version, with alpha channel)
        ///</summary>
        vp6a,

        ///<summary>
        ///      On2 VP6 (Flash version)
        ///</summary>
        vp6f,

        ///<summary>
        ///      On2 VP7
        ///</summary>
        vp7,

        ///<summary>
        ///      On2 VP8 (decoders: vp8 libvpx vp8_cuvid vp8_qsv ) (encoders: libvpx )
        ///</summary>
        vp8,

        ///<summary>
        ///      Google VP9 (decoders: vp9 libvpx-vp9 vp9_cuvid ) (encoders: libvpx-vp9 )
        ///</summary>
        vp9,

        ///<summary>
        ///      WinCAM Motion Video
        ///</summary>
        wcmv,

        ///<summary>
        ///      WebP (encoders: libwebp_anim libwebp )
        ///</summary>
        webp,

        ///<summary>
        ///      Windows Media Video 7
        ///</summary>
        wmv1,

        ///<summary>
        ///      Windows Media Video 8
        ///</summary>
        wmv2,

        ///<summary>
        ///      Windows Media Video 9
        ///</summary>
        wmv3,

        ///<summary>
        ///      Windows Media Video 9 Image
        ///</summary>
        wmv3image,

        ///<summary>
        ///      Winnov WNV1
        ///</summary>
        wnv1,

        ///<summary>
        ///      AVFrame to AVPacket passthrough
        ///</summary>
        wrapped_avframe,

        ///<summary>
        ///      Westwood Studios VQA (Vector Quantized Animation) video (decoders: vqavideo )
        ///</summary>
        ws_vqa,

        ///<summary>
        ///      Wing Commander III / Xan
        ///</summary>
        xan_wc3,

        ///<summary>
        ///      Wing Commander IV / Xxan
        ///</summary>
        xan_wc4,

        ///<summary>
        ///      eXtended BINary text
        ///</summary>
        xbin,

        ///<summary>
        ///      XBM (X BitMap) image
        ///</summary>
        xbm,

        ///<summary>
        ///      X-face image
        ///</summary>
        xface,

        ///<summary>
        ///      XPM (X PixMap) image
        ///</summary>
        xpm,

        ///<summary>
        ///      XWD (X Window Dump) image
        ///</summary>
        xwd,

        ///<summary>
        ///      Uncompressed YUV 4:1:1 12-bit
        ///</summary>
        y41p,

        ///<summary>
        ///      YUY2 Lossless Codec
        ///</summary>
        ylc,

        ///<summary>
        ///      Psygnosis YOP Video
        ///</summary>
        yop,

        ///<summary>
        ///      Uncompressed packed 4:2:0
        ///</summary>
        yuv4,

        ///<summary>
        ///      ZeroCodec Lossless Video
        ///</summary>
        zerocodec,

        ///<summary>
        ///      LCL (LossLess Codec Library) ZLIB
        ///</summary>
        zlib,

        ///<summary>
        ///      Zip Motion Blocks Video
        ///</summary>
        zmbv,

        ///<summary>
        ///      copy
        ///</summary>
        copy,

        ///<summary>
        ///      h264_nvenc
        ///</summary>
        h264_nvenc,

        ///<summary>
        ///      h264_cuvid
        ///</summary>
        h264_cuvid,

        ///<summary>
        ///      libx264
        ///</summary>
        libx264,

        ///<summary>
        ///      hevc_qsv (intel quicksync)
        ///</summary>
        hevc_qsv
    }
}
