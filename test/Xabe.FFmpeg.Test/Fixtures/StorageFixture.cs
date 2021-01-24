using System;
using System.IO;

namespace Xabe.FFmpeg.Test.Fixtures
{
    public class StorageFixture : IDisposable
    {
        public string TempDirPath { get; private set; }

        public StorageFixture()
        {
            TempDirPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(TempDirPath);
        }

        public string GetTempFileName(string extension = null)
        {
            if (extension != null)
            {
                return Path.Combine(TempDirPath, $"{Guid.NewGuid()}{extension}");
            }
            return Path.Combine(TempDirPath, $"{Guid.NewGuid()}");
        }

        public void Dispose()
        {
            new DirectoryInfo(TempDirPath).Delete(true);
        }
    }
}
