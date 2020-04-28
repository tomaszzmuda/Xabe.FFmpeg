using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace MyVideosConverter
{
    internal class ModifyStreams
    {
        public static async Task Run()
        {
            Console.Out.WriteLine("[Start] Modify streams");

            Queue<FileInfo> filesToConvert = new Queue<FileInfo>(GetFilesToConvert("."));
            await Console.Out.WriteLineAsync($"Find {filesToConvert.Count()} files to convert.");

            //Set directory where app should look for FFmpeg executables.
            FFmpeg.SetExecutablesPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FFmpeg"));
            //Get latest version of FFmpeg. It's great idea if you don't know if you had installed FFmpe1g.
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official);

            //Run conversion
            await RunConversion(filesToConvert);

            Console.Out.WriteLine("[End] Modify streams");
        }

        private static async Task RunConversion(Queue<FileInfo> filesToConvert)
        {
            while (filesToConvert.TryDequeue(out FileInfo fileToConvert))
            {
                //Save file to the same location with changed extension
                string outputFileName = Path.ChangeExtension(fileToConvert.FullName, ".mp4");

                var mediaInfo = await MediaInfo.Get(fileToConvert);
                var videoStream = mediaInfo.VideoStreams.First();
                var audioStream = mediaInfo.AudioStreams.First();

                //Change some parameters of video stream
                videoStream
                    //Rotate video counter clockwise
                    .Rotate(RotateDegrees.CounterClockwise)
                    //Set size to 480p
                    .SetSize(VideoSize.Hd480)
                    //Set codec which will be used to encode file. If not set it's set automatically according to output file extension
                    .SetCodec(VideoCodec.h264);

                var conversion = await FFmpeg.Conversions.FromSnippet.Convert(fileToConvert.FullName, outputFileName);

                //Set output file path
                conversion.SetOutput(outputFileName)
                    //SetOverwriteOutput to overwrite files. It's useful when we already run application before
                    .SetOverwriteOutput(true)
                    //Disable multithreading
                    .UseMultiThread(false)
                    //Set conversion preset. You have to chose between file size and quality of video and duration of conversion
                    .SetPreset(ConversionPreset.UltraFast);

                //Add log to OnProgress
                conversion.OnProgress += async (sender, args) =>
                {
                    //Show all output from FFmpeg to console
                    await Console.Out.WriteLineAsync($"[{args.Duration}/{args.TotalLength}][{args.Percent}%] {fileToConvert.Name}");
                };
                //Start conversion
                await conversion.Start();

                await Console.Out.WriteLineAsync($"Finished converion file [{fileToConvert.Name}]");
            }
        }

        private static IEnumerable<FileInfo> GetFilesToConvert(string directoryPath)
        {
            //Return all files excluding mp4 because I want convert it to mp4
            return new DirectoryInfo(directoryPath).GetFiles().Where(x => x.Extension == ".mkv");
        }
    }
}
