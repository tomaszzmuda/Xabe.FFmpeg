using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Xabe.FFmpeg
{
    public enum VsyncParams
    {
        passthrough,
        cfr,
        vfr,
        drop,
        auto
    }
}
