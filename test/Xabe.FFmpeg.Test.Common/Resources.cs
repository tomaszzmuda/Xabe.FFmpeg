using System;
using System.IO;

namespace Xabe.FFmpeg.Test.Common
{
    public static class Resources
    {
        public static readonly string FFbinariesInfo = GetResourceFilePath("ffbinaries.json");

        public static string GetResourceFilePath(string fileName) => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", fileName);
    }
}
