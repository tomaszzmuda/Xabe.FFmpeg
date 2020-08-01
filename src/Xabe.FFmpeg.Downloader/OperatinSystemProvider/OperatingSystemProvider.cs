using System;
using System.Runtime.InteropServices;

namespace Xabe.FFmpeg.Downloader
{
    internal class OperatingSystemProvider : IOperatingSystemProvider
    {
        public OperatingSystem GetOperatingSystem()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (RuntimeInformation.OSArchitecture == Architecture.X64)
                {
                    return OperatingSystem.Windows64;
                }
                else if (RuntimeInformation.OSArchitecture == Architecture.X64)
                {
                    return OperatingSystem.Windows64;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OperatingSystem.Osx64;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                switch (RuntimeInformation.OSArchitecture)
                {
                    case Architecture.X64:
                        return OperatingSystem.Linux64;
                    case Architecture.X86:
                        return OperatingSystem.Linux32;
                    case Architecture.Arm:
                        return OperatingSystem.LinuxArmhf;
                    case Architecture.Arm64:
                        return OperatingSystem.LinuxArm64;
                }

                // TODO : How to distinct Tizen / Raspberry architecture
                // Linux (Armet) (Tizen)
                // Linux (LinuxArmhf) (for glibc based OS) -> Raspberry Pi
            }

            throw new InvalidOperationException("Missing system type and architecture.");
        }
    }
}
