using System;
using System.Collections.Generic;
using System.Text;

namespace System
{

    public class Type_libfdk_aac_Class
    {
        public int bitrate;
        public Type_libfdk_aac_Class(int _bitrate)
        {
            bitrate = _bitrate;
        }

        public override string ToString()
        {
            return $" -c:a libfdk_aac  -b {bitrate}";
        }
    }

    public class Type_libmp3lame_Class
    {
        public int bitrate;
        public Type_libmp3lame_Class(int _bitrate)
        {
            bitrate = _bitrate;
        }

        public override string ToString()
        {
            return $" -c:a libmp3lame  -b {bitrate}";
        }
    }
    public class Type_libvorbis_Class
    {
        public int bitrate, VBR, minrate, maxbitrate;
        public float iblock, cutoff = 0;
        public Type_libvorbis_Class(int _bitrate, int _VBR, int _minrate, int _maxbitrate, float _iblock, float _cutoff = 0)
        {
            (bitrate, VBR, minrate, maxbitrate, iblock, cutoff) = (_bitrate, _VBR, _minrate, _maxbitrate, _iblock, _cutoff);
        }

        public override string ToString()
        {
            return $" -c:a libvorbis -b {bitrate} -q {VBR} -m {minrate} -M {maxbitrate}" + $" --advanced-encode-option impulse_noisetune={iblock}"
                + $" --advanced-encode-option lowpass_frequency={cutoff}";
        }
    }

    public class Type_libtwolame_Class
    {// - b Bitrate  - q quality VBR 0- 10  - m minrate - M maxrate
        public int bitrate, VBR, psymodel;
        public string Mode = "auto";
        public Type_libtwolame_Class(int _bitrate, int _VBR, string _mode, int _psymodel)
        {
            (bitrate, VBR, Mode, psymodel) = (_bitrate, _VBR, _mode, _psymodel);
        }

        public override string ToString()
        {
            return $" -c:a libtwolame  -b {bitrate} -V{VBR} --mode {Mode} -M {psymodel}";
        }
    }

    public class Type_libwavpack_Class
    {
        public int frame_size, compression_level;
        public Type_libwavpack_Class(int _frame_size, int _compression_level)
        {
            (frame_size, compression_level) = (_frame_size, _compression_level);
        }

        public override string ToString()
        {
            return $" -c:a libwavpack --blocksize {frame_size} compression_level {compression_level}";
        }
    }
}
