namespace Xabe.FFmpeg
{
    /// <summary>
    ///     A bitstream filter operates on the encoded stream data, and performs bitstream level modifications without performing decoding.
    ///     https://www.ffmpeg.org/ffmpeg-bitstream-filters.html
    /// </summary>
    public enum BitstreamFilter
    {
        /// <summary>
        ///     Convert MPEG-2/4 AAC ADTS to an MPEG-4 Audio Specific Configuration bitstream.
        /// </summary>
        aac_adtstoasc,
        /// <summary>
        ///     Modify metadata embedded in an AV1 stream.
        /// </summary>
        av1_metadata,
        /// <summary>
        ///     Remove zero padding at the end of a packet.
        /// </summary>
        chomp,
        /// <summary>
        ///     Extract the core from a DCA/DTS stream, dropping extensions such as DTS-HD.
        /// </summary>
        dca_core,
        /// <summary>
        ///     Add extradata to the beginning of the filtered packets except when said packets already exactly begin with the extradata that is intended to be added.
        /// </summary>
        dump_extra,
        /// <summary>
        ///     Extract the core from a E-AC-3 stream, dropping extra channels.
        /// </summary>
        eac3_core,
        /// <summary>
        ///     Extract the in-band extradata.
        /// </summary>
        extract_extradata,
        /// <summary>
        ///     Remove units with types in or not in a given set from the stream.
        /// </summary>
        filter_units,
        /// <summary>
        ///     Extract Rgb or Alpha part of an HAPQA file, without recompression, in order to create an HAPQ or an HAPAlphaOnly file.
        /// </summary>
        hapqa_extract,
        /// <summary>
        ///     Modify metadata embedded in an H.264 stream.
        /// </summary>
        h264_metadata,
        /// <summary>
        ///     Convert an H.264 bitstream from length prefixed mode to start code prefixed mode (as defined in the Annex B of the ITU-T H.264 specification).
        /// </summary>
        h264_mp4toannexb,
        /// <summary>
        ///     This applies a specific fixup to some Blu-ray streams which contain redundant PPSs modifying irrelevant parameters of the stream which confuse other transformations which require correct extradata.
        /// </summary>
        h264_redundant_pps,
        /// <summary>
        ///     Modify metadata embedded in an HEVC stream.
        /// </summary>
        hevc_metadata,
        /// <summary>
        ///     Convert an HEVC/H.265 bitstream from length prefixed mode to start code prefixed mode (as defined in the Annex B of the ITU-T H.265 specification).
        /// </summary>
        hevc_mp4toannexb,
        /// <summary>
        ///     Modifies the bitstream to fit in MOV and to be usable by the Final Cut Pro decoder. This filter only applies to the mpeg2video codec, and is likely not needed for Final Cut Pro 7 and newer with the appropriate -tag:v.
        /// </summary>
        imxdump,
        /// <summary>
        ///     Convert MJPEG/AVI1 packets to full JPEG/JFIF packets.
        /// </summary>
        mjpeg2jpeg,
        /// <summary>
        ///     Add an MJPEG A header to the bitstream, to enable decoding by Quicktime.
        /// </summary>
        mjpegadump,
        /// <summary>
        ///     Extract a representable text file from MOV subtitles, stripping the metadata header from each subtitle packet.
        /// </summary>
        mov2textsub,
        /// <summary>
        ///     Decompress non-standard compressed MP3 audio headers.
        /// </summary>
        mp3decomp,
        /// <summary>
        ///     Modify metadata embedded in an MPEG-2 stream.
        /// </summary>
        mpeg2_metadata,
        /// <summary>
        ///     Unpack DivX-style packed B-frames.
        /// </summary>
        mpeg4_unpack_bframes,
        /// <summary>
        ///     Damages the contents of packets or simply drops them without damaging the container. Can be used for fuzzing or testing error resilience/concealment.
        /// </summary>
        noise,
        /// <summary>
        ///     Modify color property metadata embedded in prores stream.
        /// </summary>
        prores_metadata,
        /// <summary>
        ///     Remove extradata from packets.
        /// </summary>
        remove_extra,
        /// <summary>
        ///     Convert text subtitles to MOV subtitles (as used by the mov_text codec) with metadata headers.
        /// </summary>
        text2movsub,
        /// <summary>
        ///     Log trace output containing all syntax elements in the coded stream headers (everything above the level of individual coded blocks). This can be useful for debugging low-level stream issues.
        /// </summary>
        trace_headers,
        /// <summary>
        ///     Extract the core from a TrueHD stream, dropping ATMOS data.
        /// </summary>
        truehd_core,
        /// <summary>
        ///     Modify metadata embedded in a VP9 stream.
        /// </summary>
        vp9_metadata,
        /// <summary>
        ///     Merge VP9 invisible (alt-ref) frames back into VP9 superframes. This fixes merging of split/segmented VP9 streams where the alt-ref frame was split from its visible counterpart.
        /// </summary>
        vp9_superframe,
        /// <summary>
        ///     Split VP9 superframes into single frames.
        /// </summary>
        vp9_superframe_split,
        /// <summary>
        ///     Given a VP9 stream with correct timestamps but possibly out of order, insert additional show-existing-frame packets to correct the ordering.
        /// </summary>
        vp9_raw_reorder
    }
}
