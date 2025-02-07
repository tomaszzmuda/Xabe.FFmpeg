using System;
using System.IO;
using System.Threading;

namespace Xabe.FFmpeg.Test.Common.Fixtures
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

        public string GetTempDirectory()
        {
            var path = Path.Combine(TempDirPath, $"{Guid.NewGuid()}");
            Directory.CreateDirectory(path);
            return path;
        }

        public void Dispose()
        {
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    new DirectoryInfo(TempDirPath).Delete(true);
                    break;
                }
                catch
                {
                    Thread.Sleep(500 * i * i);
                }
            }

            GC.SuppressFinalize(this);
        }
    }
}
