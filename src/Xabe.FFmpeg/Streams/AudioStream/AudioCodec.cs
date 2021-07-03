namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Audio codec ("ffmpeg -codecs")
    /// </summary>
    public enum AudioCodec
    {
        ///<summary>
        ///     4GV (Fourth Generation Vocoder)
        ///</summary>
        _4gv,

        ///<summary>
        ///     8SVX exponential
        ///</summary>
        _8svx_exp,

        ///<summary>
        ///     8SVX fibonacci
        ///</summary>
        _8svx_fib,

        ///<summary>
        ///     AAC (Advanced Audio Coding) (decoders: aac aac_fixed )
        ///</summary>
        aac,

        ///<summary>
        ///     AAC LATM (Advanced Audio Coding LATM syntax)
        ///</summary>
        aac_latm,

        ///<summary>
        ///     ATSC A/52A (AC-3) (decoders: ac3 ac3_fixed ) (encoders: ac3 ac3_fixed )
        ///</summary>
        ac3,

        ///<summary>
        ///     ADPCM 4X Movie
        ///</summary>
        adpcm_4xm,

        ///<summary>
        ///     SEGA CRI ADX ADPCM
        ///</summary>
        adpcm_adx,

        ///<summary>
        ///     ADPCM Nintendo Gamecube AFC
        ///</summary>
        adpcm_afc,

        ///<summary>
        ///     ADPCM AmuseGraphics Movie AGM
        ///</summary>
        adpcm_agm,

        ///<summary>
        ///     ADPCM Yamaha AICA
        ///</summary>
        adpcm_aica,

        ///<summary>
        ///     ADPCM Creative Technology
        ///</summary>
        adpcm_ct,

        ///<summary>
        ///     ADPCM Nintendo Gamecube DTK
        ///</summary>
        adpcm_dtk,

        ///<summary>
        ///     ADPCM Electronic Arts
        ///</summary>
        adpcm_ea,

        ///<summary>
        ///     ADPCM Electronic Arts Maxis CDROM XA
        ///</summary>
        adpcm_ea_maxis_xa,

        ///<summary>
        ///     ADPCM Electronic Arts R1
        ///</summary>
        adpcm_ea_r1,

        ///<summary>
        ///     ADPCM Electronic Arts R2
        ///</summary>
        adpcm_ea_r2,

        ///<summary>
        ///     ADPCM Electronic Arts R3
        ///</summary>
        adpcm_ea_r3,

        ///<summary>
        ///     ADPCM Electronic Arts XAS
        ///</summary>
        adpcm_ea_xas,

        ///<summary>
        ///     G.722 ADPCM (decoders: g722 ) (encoders: g722 )
        ///</summary>
        adpcm_g722,

        ///<summary>
        ///     G.726 ADPCM (decoders: g726 ) (encoders: g726 )
        ///</summary>
        adpcm_g726,

        ///<summary>
        ///     G.726 ADPCM little-endian (decoders: g726le ) (encoders: g726le )
        ///</summary>
        adpcm_g726le,

        ///<summary>
        ///     ADPCM IMA AMV
        ///</summary>
        adpcm_ima_amv,

        ///<summary>
        ///     ADPCM IMA CRYO APC
        ///</summary>
        adpcm_ima_apc,

        ///<summary>
        ///     ADPCM IMA Eurocom DAT4
        ///</summary>
        adpcm_ima_dat4,

        ///<summary>
        ///     ADPCM IMA Duck DK3
        ///</summary>
        adpcm_ima_dk3,

        ///<summary>
        ///     ADPCM IMA Duck DK4
        ///</summary>
        adpcm_ima_dk4,

        ///<summary>
        ///     ADPCM IMA Electronic Arts EACS
        ///</summary>
        adpcm_ima_ea_eacs,

        ///<summary>
        ///     ADPCM IMA Electronic Arts SEAD
        ///</summary>
        adpcm_ima_ea_sead,

        ///<summary>
        ///     ADPCM IMA Funcom ISS
        ///</summary>
        adpcm_ima_iss,

        ///<summary>
        ///     ADPCM IMA Dialogic OKI
        ///</summary>
        adpcm_ima_oki,

        ///<summary>
        ///     ADPCM IMA QuickTime
        ///</summary>
        adpcm_ima_qt,

        ///<summary>
        ///     ADPCM IMA Radical
        ///</summary>
        adpcm_ima_rad,

        ///<summary>
        ///     ADPCM IMA Loki SDL MJPEG
        ///</summary>
        adpcm_ima_smjpeg,

        ///<summary>
        ///     ADPCM IMA WAV
        ///</summary>
        adpcm_ima_wav,

        ///<summary>
        ///     ADPCM IMA Westwood
        ///</summary>
        adpcm_ima_ws,

        ///<summary>
        ///     ADPCM Microsoft
        ///</summary>
        adpcm_ms,

        ///<summary>
        ///     ADPCM MTAF
        ///</summary>
        adpcm_mtaf,

        ///<summary>
        ///     ADPCM Playstation
        ///</summary>
        adpcm_psx,

        ///<summary>
        ///     ADPCM Sound Blaster Pro 2-bit
        ///</summary>
        adpcm_sbpro_2,

        ///<summary>
        ///     ADPCM Sound Blaster Pro 2.6-bit
        ///</summary>
        adpcm_sbpro_3,

        ///<summary>
        ///     ADPCM Sound Blaster Pro 4-bit
        ///</summary>
        adpcm_sbpro_4,

        ///<summary>
        ///     ADPCM Shockwave Flash
        ///</summary>
        adpcm_swf,

        ///<summary>
        ///     ADPCM Nintendo THP
        ///</summary>
        adpcm_thp,

        ///<summary>
        ///     ADPCM Nintendo THP (Little-Endian)
        ///</summary>
        adpcm_thp_le,

        ///<summary>
        ///     LucasArts VIMA audio
        ///</summary>
        adpcm_vima,

        ///<summary>
        ///     ADPCM CDROM XA
        ///</summary>
        adpcm_xa,

        ///<summary>
        ///     ADPCM Yamaha
        ///</summary>
        adpcm_yamaha,

        ///<summary>
        ///     ALAC (Apple Lossless Audio Codec)
        ///</summary>
        alac,

        ///<summary>
        ///     AMR-NB (Adaptive Multi-Rate NarrowBand) (decoders: amrnb libopencore_amrnb ) (encoders: libopencore_amrnb )
        ///</summary>
        amr_nb,

        ///<summary>
        ///     AMR-WB (Adaptive Multi-Rate WideBand) (decoders: amrwb libopencore_amrwb ) (encoders: libvo_amrwbenc )
        ///</summary>
        amr_wb,

        ///<summary>
        ///     Monkey's Audio
        ///</summary>
        ape,

        ///<summary>
        ///     aptX (Audio Processing Technology for Bluetooth)
        ///</summary>
        aptx,

        ///<summary>
        ///     aptX HD (Audio Processing Technology for Bluetooth)
        ///</summary>
        aptx_hd,

        ///<summary>
        ///     ATRAC1 (Adaptive TRansform Acoustic Coding)
        ///</summary>
        atrac1,

        ///<summary>
        ///     ATRAC3 (Adaptive TRansform Acoustic Coding 3)
        ///</summary>
        atrac3,

        ///<summary>
        ///     ATRAC3 AL (Adaptive TRansform Acoustic Coding 3 Advanced Lossless)
        ///</summary>
        atrac3al,

        ///<summary>
        ///     ATRAC3+ (Adaptive TRansform Acoustic Coding 3+) (decoders: atrac3plus )
        ///</summary>
        atrac3p,

        ///<summary>
        ///     ATRAC3+ AL (Adaptive TRansform Acoustic Coding 3+ Advanced Lossless) (decoders: atrac3plusal )
        ///</summary>
        atrac3pal,

        ///<summary>
        ///     ATRAC9 (Adaptive TRansform Acoustic Coding 9)
        ///</summary>
        atrac9,

        ///<summary>
        ///     On2 Audio for Video Codec (decoders: on2avc )
        ///</summary>
        avc,

        ///<summary>
        ///     Bink Audio (DCT)
        ///</summary>
        binkaudio_dct,

        ///<summary>
        ///     Bink Audio (RDFT)
        ///</summary>
        binkaudio_rdft,

        ///<summary>
        ///     Discworld II BMV audio
        ///</summary>
        bmv_audio,

        ///<summary>
        ///     Constrained Energy Lapped Transform (CELT)
        ///</summary>
        celt,

        ///<summary>
        ///     codec2 (very low bitrate speech codec)
        ///</summary>
        codec2,

        ///<summary>
        ///     RFC 3389 Comfort Noise
        ///</summary>
        comfortnoise,

        ///<summary>
        ///     Cook / Cooker / Gecko (RealAudio G2)
        ///</summary>
        cook,

        ///<summary>
        ///     Dolby E
        ///</summary>
        dolby_e,

        ///<summary>
        ///     DSD (Direct Stream Digital), least significant bit first
        ///</summary>
        dsd_lsbf,

        ///<summary>
        ///     DSD (Direct Stream Digital), least significant bit first, planar
        ///</summary>
        dsd_lsbf_planar,

        ///<summary>
        ///     DSD (Direct Stream Digital), most significant bit first
        ///</summary>
        dsd_msbf,

        ///<summary>
        ///     DSD (Direct Stream Digital), most significant bit first, planar
        ///</summary>
        dsd_msbf_planar,

        ///<summary>
        ///     Delphine Software International CIN audio
        ///</summary>
        dsicinaudio,

        ///<summary>
        ///     Digital Speech Standard - Standard Play mode (DSS SP)
        ///</summary>
        dss_sp,

        ///<summary>
        ///     DST (Direct Stream Transfer)
        ///</summary>
        dst,

        ///<summary>
        ///     DCA (DTS Coherent Acoustics) (decoders: dca ) (encoders: dca )
        ///</summary>
        dts,

        ///<summary>
        ///     DV audio
        ///</summary>
        dvaudio,

        ///<summary>
        ///     ATSC A/52B (AC-3, E-AC-3)
        ///</summary>
        eac3,

        ///<summary>
        ///     EVRC (Enhanced Variable Rate Codec)
        ///</summary>
        evrc,

        ///<summary>
        ///     FLAC (Free Lossless Audio Codec)
        ///</summary>
        flac,

        ///<summary>
        ///     G.723.1
        ///</summary>
        g723_1,

        ///<summary>
        ///     G.729
        ///</summary>
        g729,

        ///<summary>
        ///     DPCM Gremlin
        ///</summary>
        gremlin_dpcm,

        ///<summary>
        ///     GSM
        ///</summary>
        gsm,

        ///<summary>
        ///     GSM Microsoft variant
        ///</summary>
        gsm_ms,

        ///<summary>
        ///     HCOM Audio
        ///</summary>
        hcom,

        ///<summary>
        ///     IAC (Indeo Audio Coder)
        ///</summary>
        iac,

        ///<summary>
        ///     iLBC (Internet Low Bitrate Codec)
        ///</summary>
        ilbc,

        ///<summary>
        ///     IMC (Intel Music Coder)
        ///</summary>
        imc,

        ///<summary>
        ///     DPCM Interplay
        ///</summary>
        interplay_dpcm,

        ///<summary>
        ///     Interplay ACM
        ///</summary>
        interplayacm,

        ///<summary>
        ///     MACE (Macintosh Audio Compression/Expansion) 3:1
        ///</summary>
        mace3,

        ///<summary>
        ///     MACE (Macintosh Audio Compression/Expansion) 6:1
        ///</summary>
        mace6,

        ///<summary>
        ///     Voxware MetaSound
        ///</summary>
        metasound,

        ///<summary>
        ///     MLP (Meridian Lossless Packing)
        ///</summary>
        mlp,

        ///<summary>
        ///     MP1 (MPEG audio layer 1) (decoders: mp1 mp1float )
        ///</summary>
        mp1,

        ///<summary>
        ///     MP2 (MPEG audio layer 2) (decoders: mp2 mp2float ) (encoders: mp2 mp2fixed libtwolame )
        ///</summary>
        mp2,

        ///<summary>
        ///     MP3 (MPEG audio layer 3) (decoders: mp3float mp3 ) (encoders: libmp3lame libshine )
        ///</summary>
        mp3,

        ///<summary>
        ///     ADU (Application Data Unit) MP3 (MPEG audio layer 3) (decoders: mp3adufloat mp3adu )
        ///</summary>
        mp3adu,

        ///<summary>
        ///     MP3onMP4 (decoders: mp3on4float mp3on4 )
        ///</summary>
        mp3on4,

        ///<summary>
        ///     MPEG-4 Audio Lossless Coding (ALS) (decoders: als )
        ///</summary>
        mp4als,

        ///<summary>
        ///     Musepack SV7 (decoders: mpc7 )
        ///</summary>
        musepack7,

        ///<summary>
        ///     Musepack SV8 (decoders: mpc8 )
        ///</summary>
        musepack8,

        ///<summary>
        ///     Nellymoser Asao
        ///</summary>
        nellymoser,

        ///<summary>
        ///     Opus (Opus Interactive Audio Codec) (decoders: opus libopus ) (encoders: opus libopus )
        ///</summary>
        opus,

        ///<summary>
        ///     Amazing Studio Packed Animation File Audio
        ///</summary>
        paf_audio,

        ///<summary>
        ///     PCM A-law / G.711 A-law
        ///</summary>
        pcm_alaw,

        ///<summary>
        ///     PCM signed 16|20|24-bit big-endian for Blu-ray media
        ///</summary>
        pcm_bluray,

        ///<summary>
        ///     PCM signed 20|24-bit big-endian
        ///</summary>
        pcm_dvd,

        ///<summary>
        ///     PCM 16.8 floating point little-endian
        ///</summary>
        pcm_f16le,

        ///<summary>
        ///     PCM 24.0 floating point little-endian
        ///</summary>
        pcm_f24le,

        ///<summary>
        ///     PCM 32-bit floating point big-endian
        ///</summary>
        pcm_f32be,

        ///<summary>
        ///     PCM 32-bit floating point little-endian
        ///</summary>
        pcm_f32le,

        ///<summary>
        ///     PCM 64-bit floating point big-endian
        ///</summary>
        pcm_f64be,

        ///<summary>
        ///     PCM 64-bit floating point little-endian
        ///</summary>
        pcm_f64le,

        ///<summary>
        ///     PCM signed 20-bit little-endian planar
        ///</summary>
        pcm_lxf,

        ///<summary>
        ///     PCM mu-law / G.711 mu-law
        ///</summary>
        pcm_mulaw,

        ///<summary>
        ///     PCM signed 16-bit big-endian
        ///</summary>
        pcm_s16be,

        ///<summary>
        ///     PCM signed 16-bit big-endian planar
        ///</summary>
        pcm_s16be_planar,

        ///<summary>
        ///     PCM signed 16-bit little-endian
        ///</summary>
        pcm_s16le,

        ///<summary>
        ///     PCM signed 16-bit little-endian planar
        ///</summary>
        pcm_s16le_planar,

        ///<summary>
        ///     PCM signed 24-bit big-endian
        ///</summary>
        pcm_s24be,

        ///<summary>
        ///     PCM D-Cinema audio signed 24-bit
        ///</summary>
        pcm_s24daud,

        ///<summary>
        ///     PCM signed 24-bit little-endian
        ///</summary>
        pcm_s24le,

        ///<summary>
        ///     PCM signed 24-bit little-endian planar
        ///</summary>
        pcm_s24le_planar,

        ///<summary>
        ///     PCM signed 32-bit big-endian
        ///</summary>
        pcm_s32be,

        ///<summary>
        ///     PCM signed 32-bit little-endian
        ///</summary>
        pcm_s32le,

        ///<summary>
        ///     PCM signed 32-bit little-endian planar
        ///</summary>
        pcm_s32le_planar,

        ///<summary>
        ///     PCM signed 64-bit big-endian
        ///</summary>
        pcm_s64be,

        ///<summary>
        ///     PCM signed 64-bit little-endian
        ///</summary>
        pcm_s64le,

        ///<summary>
        ///     PCM signed 8-bit
        ///</summary>
        pcm_s8,

        ///<summary>
        ///     PCM signed 8-bit planar
        ///</summary>
        pcm_s8_planar,

        ///<summary>
        ///     PCM unsigned 16-bit big-endian
        ///</summary>
        pcm_u16be,

        ///<summary>
        ///     PCM unsigned 16-bit little-endian
        ///</summary>
        pcm_u16le,

        ///<summary>
        ///     PCM unsigned 24-bit big-endian
        ///</summary>
        pcm_u24be,

        ///<summary>
        ///     PCM unsigned 24-bit little-endian
        ///</summary>
        pcm_u24le,

        ///<summary>
        ///     PCM unsigned 32-bit big-endian
        ///</summary>
        pcm_u32be,

        ///<summary>
        ///     PCM unsigned 32-bit little-endian
        ///</summary>
        pcm_u32le,

        ///<summary>
        ///     PCM unsigned 8-bit
        ///</summary>
        pcm_u8,

        ///<summary>
        ///     PCM Archimedes VIDC
        ///</summary>
        pcm_vidc,

        ///<summary>
        ///     PCM Zork
        ///</summary>
        pcm_zork,

        ///<summary>
        ///     QCELP / PureVoice
        ///</summary>
        qcelp,

        ///<summary>
        ///     QDesign Music Codec 2
        ///</summary>
        qdm2,

        ///<summary>
        ///     QDesign Music
        ///</summary>
        qdmc,

        ///<summary>
        ///     RealAudio 1.0 (14.4K) (decoders: real_144 ) (encoders: real_144 )
        ///</summary>
        ra_144,

        ///<summary>
        ///     RealAudio 2.0 (28.8K) (decoders: real_288 )
        ///</summary>
        ra_288,

        ///<summary>
        ///     RealAudio Lossless
        ///</summary>
        ralf,

        ///<summary>
        ///     DPCM id RoQ
        ///</summary>
        roq_dpcm,

        ///<summary>
        ///     SMPTE 302M
        ///</summary>
        s302m,

        ///<summary>
        ///     SBC (low-complexity subband codec)
        ///</summary>
        sbc,

        ///<summary>
        ///     DPCM Squareroot-Delta-Exact
        ///</summary>
        sdx2_dpcm,

        ///<summary>
        ///     Shorten
        ///</summary>
        shorten,

        ///<summary>
        ///     RealAudio SIPR / ACELP.NET
        ///</summary>
        sipr,

        ///<summary>
        ///     Smacker audio (decoders: smackaud )
        ///</summary>
        smackaudio,

        ///<summary>
        ///     SMV (Selectable Mode Vocoder)
        ///</summary>
        smv,

        ///<summary>
        ///     DPCM Sol
        ///</summary>
        sol_dpcm,

        ///<summary>
        ///     Sonic
        ///</summary>
        sonic,

        ///<summary>
        ///     Sonic lossless
        ///</summary>
        sonicls,

        ///<summary>
        ///     Speex (decoders: libspeex ) (encoders: libspeex )
        ///</summary>
        speex,

        ///<summary>
        ///     TAK (Tom's lossless Audio Kompressor)
        ///</summary>
        tak,

        ///<summary>
        ///     TrueHD
        ///</summary>
        truehd,

        ///<summary>
        ///     DSP Group TrueSpeech
        ///</summary>
        truespeech,

        ///<summary>
        ///     TTA (True Audio)
        ///</summary>
        tta,

        ///<summary>
        ///     VQF TwinVQ
        ///</summary>
        twinvq,

        ///<summary>
        ///     Sierra VMD audio
        ///</summary>
        vmdaudio,

        ///<summary>
        ///     Vorbis (decoders: vorbis libvorbis ) (encoders: vorbis libvorbis )
        ///</summary>
        vorbis,

        ///<summary>
        ///     Wave synthesis pseudo-codec
        ///</summary>
        wavesynth,

        ///<summary>
        ///     WavPack (encoders: wavpack libwavpack )
        ///</summary>
        wavpack,

        ///<summary>
        ///     Westwood Audio (SND1) (decoders: ws_snd1 )
        ///</summary>
        westwood_snd1,

        ///<summary>
        ///     Windows Media Audio Lossless
        ///</summary>
        wmalossless,

        ///<summary>
        ///     Windows Media Audio 9 Professional
        ///</summary>
        wmapro,

        ///<summary>
        ///     Windows Media Audio 1
        ///</summary>
        wmav1,

        ///<summary>
        ///     Windows Media Audio 2
        ///</summary>
        wmav2,

        ///<summary>
        ///     Windows Media Audio Voice
        ///</summary>
        wmavoice,

        ///<summary>
        ///     DPCM Xan
        ///</summary>
        xan_dpcm,

        ///<summary>
        ///     Xbox Media Audio 1
        ///</summary>
        xma1,

        ///<summary>
        ///     Xbox Media Audio 2
        ///</summary>
        xma2,

        ///<summary>
        ///     libvorbis
        ///</summary>
        libvorbis,

        ///<summary>
        ///     copy
        ///</summary>
        copy,

        ///<summary>
        ///     Opus (Opus Interactive Audio Codec) (decoders: opus libopus ) (encoders: opus libopus )
        ///</summary>
        libopus
    }
}
