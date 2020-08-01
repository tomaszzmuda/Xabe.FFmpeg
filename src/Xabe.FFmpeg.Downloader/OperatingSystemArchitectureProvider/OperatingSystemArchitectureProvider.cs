using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Xabe.FFmpeg.Downloader
{
    internal class OperatingSystemArchitectureProvider : IOperatingSystemArchitectureProvider
    {
        public OperatingSystemArchitecture GetArchitecture()
        {
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.Arm:
                    return OperatingSystemArchitecture.Arm;
                case Architecture.Arm64:
                    return OperatingSystemArchitecture.Arm64;
                case Architecture.X64:
                    return OperatingSystemArchitecture.X64;
                case Architecture.X86:
                    return OperatingSystemArchitecture.X86;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
