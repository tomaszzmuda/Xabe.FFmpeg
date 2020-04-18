using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace MyVideosConverter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Run().GetAwaiter().GetResult();
        }

        private static async Task Run()
        {
            await BasicConversion.Run();
            await ModifyStreams.Run();
            Console.In.ReadLine();
        }
    }
}
