using System;
using System.Collections.Generic;
using System.Text;

namespace Xabe.FFmpeg
{
    public static class AudioModeCodec
    {
        public static string TooParameter(this object AudioObject)
        {
            string result = "";
            if (AudioObject is Type_libvorbis_Class @class)  result = @class.ToString();
            if (AudioObject is Type_libtwolame_Class @class2) result = @class2.ToString();
            if (AudioObject is Type_libwavpack_Class @class3) result = @class3.ToString();
            if (AudioObject is Type_libfdk_aac_Class @class4) result = @class4.ToString();
            if (AudioObject is Type_libmp3lame_Class @class5) result = @class5.ToString();
            return result;
        }
    }
}
