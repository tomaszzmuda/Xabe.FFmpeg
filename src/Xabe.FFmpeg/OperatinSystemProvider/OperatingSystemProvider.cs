using System;
using System.Runtime.InteropServices;

namespace Xabe.FFmpeg
{
    internal class OperatingSystemProvider : IOperatingSystemProvider
    {
        public OperatingSystem GetOperatingSystem()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OperatingSystem.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OperatingSystem.Osx;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OperatingSystem.Linux;

                // TODO : How to distinct Tizen / Raspberry architecture
                // Linux (Armet) (Tizen)
                // Linux (LinuxArmhf) (for glibc based OS) -> Raspberry Pi
            }

            throw new InvalidOperationException("Missing system type and architecture.");
        }
    }
}
