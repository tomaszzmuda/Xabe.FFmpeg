using System;
using System.Collections.Generic;
using System.Text;

namespace Xabe.FFmpeg.Streams.SubtitleStream
{
    ///<summary>
    ///      Subtitle codec ("ffmpeg -codecs")
    ///</summary>
    public enum SubtitleCodec
    {
        ///<summary>
        ///      copy
        ///</summary>
        copy,

        ///<summary>
        ///      dvdsub
        ///</summary>
        dvdsub,

        ///<summary>
        ///      srt
        ///</summary>
        srt,

        ///<summary>
        ///      mov_text
        ///</summary>
        mov_text,
    }
}
