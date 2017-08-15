using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Xabe.FFMpeg.Enums;

namespace Xabe.FFMpeg
{
    /// <summary>
    ///     Info about conversion progress
    /// </summary>
    /// <param name="duration">Current processing time</param>
    /// <param name="totalLength">Movie length</param>
    public delegate void ConversionHandler(TimeSpan duration, TimeSpan totalLength);

    // ReSharper disable once InconsistentNaming
    /// <summary>
    ///     Wrapper for FFMpeg
    /// </summary>
    internal class FFMpeg: FFBase
    {
        private volatile string _errorData = string.Empty;
        private TimeSpan _totalTime;

        /// <summary>
        ///     Fires when ffmpeg progress changes
        /// </summary>
        public event ConversionHandler OnProgress;

        /// <summary>
        ///     Saves snapshot of video
        /// </summary>
        /// <param name="source">Video</param>
        /// <param name="outputPath">Output file</param>
        /// <param name="size">Dimension of snapshot</param>
        /// <param name="captureTime"></param>
        /// <returns>Conversion result</returns>
        public bool Snapshot(VideoInfo source, string outputPath, Size? size = null, TimeSpan? captureTime = null)
        {
            if(captureTime == null)
                captureTime = TimeSpan.FromSeconds(source.Duration.TotalSeconds / 3);

            size = GetSize(source, size);

            CheckIfFilesExists(source, outputPath);

            string arguments = new ArgumentBuilder()
                .SetInput(source)
                .SetVideo(VideoCodec.Png)
                .SetOutputFramesCount(1)
                .SetSeek(captureTime)
                .SetSize(size)
                .SetOutput(outputPath)
                .Build();

            return RunProcess(arguments, outputPath);
        }

        private static Size? GetSize(VideoInfo source, Size? size)
        {
            if(size == null ||
               size.Value.Height == 0 && size.Value.Width == 0)
                size = new Size(source.Width, source.Height);

            if(size.Value.Width != size.Value.Height)
            {
                if(size.Value.Width == 0)
                {
                    double ratio = source.Width / (double) size.Value.Width;

                    size = new Size((int) (source.Width * ratio), (int) (source.Height * ratio));
                }

                if(size.Value.Height == 0)
                {
                    double ratio = source.Height / (double) size.Value.Height;

                    size = new Size((int) (source.Width * ratio), (int) (source.Height * ratio));
                }
            }

            return size;
        }

        /// <summary>
        ///     Converts a source video to MP4 format.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="outputPath">Output video file.</param>
        /// <param name="speed">Conversion speed preset.</param>
        /// <param name="size">Output video size.</param>
        /// <param name="aQuality">Output audio quality.</param>
        /// <param name="multithread">Use multithreading for conversion.</param>
        /// <returns>Conversion result</returns>
        public bool ToMp4(VideoInfo source, string outputPath, Speed speed = Speed.SuperFast,
            VideoSize size = VideoSize.Original, AudioQuality aQuality = AudioQuality.Normal, bool multithread = false)
        {
            _totalTime = source.Duration;

            CheckIfFilesExists(source, outputPath);
            CheckExtension(outputPath, Extensions.Mp4);

            string arguments = new ArgumentBuilder()
                .SetInput(source)
                .UseMultiThread(multithread)
                .SetScale(size)
                .SetVideo(VideoCodec.LibX264, 2400)
                .SetSpeed(speed)
                .SetAudio(AudioCodec.Aac, aQuality)
                .SetOutput(outputPath)
                .Build();

            return RunProcess(arguments, outputPath);
        }

        /// <summary>
        ///     Converts a source video to WebM format.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="outputPath">Output video file.</param>
        /// <param name="size">Output video size.</param>
        /// <param name="aQuality">Output audio quality.</param>
        /// <returns>Conversion result</returns>
        public bool ToWebM(VideoInfo source, string outputPath, VideoSize size = VideoSize.Original,
            AudioQuality aQuality = AudioQuality.Normal)
        {
            _totalTime = source.Duration;

            CheckIfFilesExists(source, outputPath);
            CheckExtension(outputPath, Extensions.WebM);

            string arguments = new ArgumentBuilder()
                .SetInput(source)
                .SetScale(size)
                .SetVideo(VideoCodec.LibVpx, 2400)
                .SetSpeed(16)
                .SetAudio(AudioCodec.LibVorbis, aQuality)
                .SetOutput(outputPath)
                .Build();

            return RunProcess(arguments, outputPath);
        }

        /// <summary>
        ///     Converts a source video to OGV format.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="outputPath">Output video file.</param>
        /// <param name="size">Output video size.</param>
        /// <param name="aQuality">Output audio quality.</param>
        /// <param name="multithread">Use multithreading for conversion.</param>
        /// <returns>Conversion result</returns>
        public bool ToOgv(VideoInfo source, string outputPath, VideoSize size = VideoSize.Original,
            AudioQuality aQuality = AudioQuality.Normal, bool multithread = false)
        {
            _totalTime = source.Duration;

            CheckIfFilesExists(source, outputPath);
            CheckExtension(outputPath, Extensions.Ogv);

            string arguments = new ArgumentBuilder()
                .SetInput(source)
                .SetScale(size)
                .SetVideo(VideoCodec.LibTheora, 2400)
                .SetSpeed(16)
                .SetAudio(AudioCodec.LibVorbis, aQuality)
                .SetOutput(outputPath)
                .Build();

            return RunProcess(arguments, outputPath);
        }

        /// <summary>
        ///     Converts a source video to TS format.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="outputPath">Output video file.</param>
        /// <returns>Conversion result</returns>
        public bool ToTs(VideoInfo source, string outputPath)
        {
            _totalTime = source.Duration;

            CheckIfFilesExists(source, outputPath);
            CheckExtension(outputPath, Extensions.Ts);

            string arguments = new ArgumentBuilder()
                .SetInput(source)
                .SetChannels(Channel.Both)
                .SetFilter(Channel.Video, Filter.H264_Mp4ToAnnexB)
                .SetCodec(VideoCodec.MpegTs)
                .SetOutput(outputPath)
                .Build();

            return RunProcess(arguments, outputPath);
        }

        /// <summary>
        ///     Adds a poster image to an audio file.
        /// </summary>
        /// <param name="image">Source image file.</param>
        /// <param name="audio">Source audio file.</param>
        /// <param name="outputPath">Output video file.</param>
        /// <returns>Conversion result</returns>
        public bool PosterWithAudio(FileInfo image, FileInfo audio, string outputPath)
        {
            CheckIfFilesExists(image, audio);
            CheckExtension(outputPath, Extensions.Mp4);

            string arguments = new ArgumentBuilder()
                .SetInput(image, audio)
                .SetLoop(1)
                .SetVideo(VideoCodec.LibX264, 2400)
                .SetAudio(AudioCodec.Aac, AudioQuality.Normal)
                .UseShortest(true)
                .SetOutput(outputPath)
                .Build();

            return RunProcess(arguments, outputPath);
        }

        /// <summary>
        ///     Joins a list of video files.
        /// </summary>
        /// <param name="outputPath">Output video file.</param>
        /// <param name="videos">List of vides that need to be joined together.</param>
        /// <returns>Conversion result</returns>
        public bool Join(string outputPath, params VideoInfo[] videos)
        {
            var pathList = new List<string>();

            foreach(VideoInfo video in videos)
            {
                string tempFileName = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Ts);
                pathList.Add(tempFileName);
                ToTs(video, tempFileName);
            }

            string arguments = new ArgumentBuilder().Concat(pathList)
                                                    .SetChannels(Channel.Both)
                                                    .SetFilter(Channel.Audio, Filter.Aac_AdtstoAsc)
                                                    .SetOutput(outputPath)
                                                    .Build();

            return RunProcess(arguments, outputPath);
        }

        /// <summary>
        ///     Records M3U8 streams to the specified output.
        /// </summary>
        /// <param name="uri">URI to stream.</param>
        /// <param name="outputPath">Output file</param>
        /// <returns>Conversion result</returns>
        public bool SaveM3U8Stream(Uri uri, string outputPath)
        {
            CheckExtension(outputPath, Extensions.Mp4);

            if(uri.Scheme != "http" ||
               uri.Scheme != "https")
                throw new ArgumentException($"Invalid uri {uri.AbsolutePath}");

            string arguments = new ArgumentBuilder().SetInput(uri)
                                                    .SetOutput(outputPath)
                                                    .Build();

            return RunProcess(arguments, outputPath);
        }

        /// <summary>
        ///     Saves a video stream file.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="outputPath">Output video file.</param>
        /// <returns>Conversion result</returns>
        public bool ExtractVideo(VideoInfo source, string outputPath)
        {
            CheckIfFilesExists(source, outputPath);
            CheckExtension(outputPath, source.Extension);

            string arguments = new ArgumentBuilder().SetInput(source)
                                                    .SetChannels(Channel.Both)
                                                    .DisableChannel(Channel.Audio)
                                                    .SetOutput(outputPath)
                                                    .Build();

            return RunProcess(arguments, outputPath);
        }

        /// <summary>
        ///     Saves audio from a video file to disk.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="outputPath">Output audio file.</param>
        /// <returns>Conversion result</returns>
        public bool ExtractAudio(VideoInfo source, string outputPath)
        {
            CheckIfFilesExists(source, outputPath);
            CheckExtension(outputPath, Extensions.Mp3);

            string arguments = new ArgumentBuilder().SetInput(source)
                                                    .DisableChannel(Channel.Video)
                                                    .SetOutput(outputPath)
                                                    .Build();

            return RunProcess(arguments, outputPath);
        }

        private void CheckIfFilesExists(VideoInfo originalVideo, string outputPath)
        {
            if(File.Exists(outputPath))
                throw new IOException($"The output file: {outputPath} already exists!");

            if(!File.Exists(originalVideo.Path))
                throw new IOException($"Input {originalVideo.Path} does not exist!");
        }

        private void CheckIfFilesExists(params FileInfo[] paths)
        {
            foreach(FileInfo path in paths)
                if(!File.Exists(path.FullName))
                    throw new IOException($"Input {path} does not exist!");
        }

        private void CheckExtension(string output, string expected)
        {
//            if(!expected.Equals(new FileInfo(output).Extension, StringComparison.OrdinalIgnoreCase))
//                throw new IOException($"Invalid output file. SourceFile extension should be '{expected}' required.");
        }

        /// <summary>
        ///     Adds audio to a video file.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="audio">Source audio file.</param>
        /// <param name="outputPath">Output video file.</param>
        /// <param name="stopAtShortest">Stop at the shortest input file.</param>
        /// <returns>Conversion result</returns>
        public bool AddAudio(VideoInfo source, FileInfo audio, string outputPath, bool stopAtShortest = false)
        {
            CheckIfFilesExists(source, outputPath);
            CheckIfFilesExists(audio);
            CheckExtension(outputPath, source.Extension);

            string arguments = new ArgumentBuilder().SetInput(new FileInfo(source.Path), audio)
                                                    .SetChannels(Channel.Video)
                                                    .SetAudio(AudioCodec.Aac, AudioQuality.Hd)
                                                    .UseShortest(stopAtShortest)
                                                    .SetOutput(outputPath)
                                                    .Build();

            return RunProcess(arguments, outputPath);
        }

        /// <summary>
        ///     Stops the ffmpeg process.
        /// </summary>
        public void Stop()
        {
            if(IsRunning)
                Process.StandardInput.Write('q');
        }

        private bool RunProcess(string args, string outputPath)
        {
            var result = true;

            RunProcess(args, FFMpegPath, true, rStandardError: true);

            try
            {
                Process.ErrorDataReceived += OutputData;
                Process.BeginErrorReadLine();
                Process.WaitForExit();
            }
            catch(Exception)
            {
                result = false;
            }
            finally
            {
                Process.Close();

                if(!File.Exists(outputPath))
                    throw new InvalidOperationException(_errorData);
                if(new FileInfo(outputPath).Length == 0)
                    throw new InvalidOperationException(_errorData);
            }
            return result;
        }

        private void OutputData(object sender, DataReceivedEventArgs e)
        {
            if(e.Data == null)
                return;

            _errorData = e.Data;

            if(OnProgress == null ||
               !IsRunning)
                return;

            var r = new Regex(@"\w\w:\w\w:\w\w");
            Match m = r.Match(e.Data);

            if(!e.Data.Contains("frame"))
                return;
            if(!m.Success)
                return;

            TimeSpan t = TimeSpan.Parse(m.Value);
            OnProgress(t, _totalTime);
        }
    }
}
