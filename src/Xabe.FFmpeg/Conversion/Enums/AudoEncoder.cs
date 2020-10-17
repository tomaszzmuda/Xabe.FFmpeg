using System;
using System.Collections.Generic;
using System.Text;

namespace Xabe.FFmpeg
{
    public enum AudioEncoder
    {
        copy,
        lib_aac,
        libmp3lame,
        libvorbis,
        libwavpack,
        libtwolame,
        flac
    }
}
