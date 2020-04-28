using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace MyVideosConverter
{
    internal class BasicConversion
    {
        internal static async Task Run()
        {
            Console.Out.WriteLine("[Start] Basic Conversion");
            FileInfo fileToConvert = GetFilesToConvert(".").First();

            //Set directory where app should look for FFmpeg executables.
            FFmpeg.SetExecutablesPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FFmpeg");
            //Get latest version of FFmpeg. It's great idea if you don't know if you had installed FFmpe1g.
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official);

            //Run conversion
            await RunConversion(fileToConvert);

            Console.Out.WriteLine("[End] Basic Conversion");
        }

        private static async Task RunConversion(FileInfo fileToConvert)
        {
            //Save file to the same location with changed extension
            string outputFileName = Path.ChangeExtension(fileToConvert.FullName, ".mp4");

            //Delete file if it already exists
            File.Delete(outputFileName);

            await Conversion.Convert(fileToConvert.FullName, outputFileName).Start();

            await Console.Out.WriteLineAsync($"Finished converion file [{fileToConvert.Name}]");
        }

        private static IEnumerable<FileInfo> GetFilesToConvert(string directoryPath)
        {
            //Return all files excluding mp4 because I want convert it to mp4
            return new DirectoryInfo(directoryPath).GetFiles().Where(x => x.Extension == ".mkv");
        }
    }
}
