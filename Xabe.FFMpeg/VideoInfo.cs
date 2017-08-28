using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Xabe.FFMpeg.Enums;

namespace Xabe.FFMpeg
{
    /// <inheritdoc cref="IVideoInfo" />
    public class VideoInfo: IDisposable, IVideoInfo
    {
        private IConversion _conversion;

        /// <inheritdoc />
        public VideoInfo(FileInfo sourceFileInfo): this(sourceFileInfo.FullName)
        {
        }

        /// <summary>
        ///     Get VideoInfo from file
        /// </summary>
        /// <param name="filePath">FilePath to file</param>
        [UsedImplicitly]
        public VideoInfo(string filePath)
        {
            if(!File.Exists(filePath))
                throw new ArgumentException($"Input file {filePath} doesn't exists.");
            FilePath = filePath;
            new FFProbe().ProbeDetails(this);
        }

        /// <inheritdoc cref="IVideoInfo.Dispose" />
        [UsedImplicitly]
        public void Dispose()
        {
            _conversion.Dispose();
        }

        /// <inheritdoc />
        public string FilePath { get; }

        /// <inheritdoc />
        public ConversionHandler OnConversionProgress { get; set; }


        /// <inheritdoc />
        public string Extension => Path.GetExtension(FilePath);


        /// <inheritdoc />
        public TimeSpan Duration { get; internal set; }

        /// <inheritdoc />
        public string AudioFormat { get; internal set; }


        /// <inheritdoc />
        public string VideoFormat { get; internal set; }


        /// <inheritdoc />
        public string Ratio { get; internal set; }

        /// <inheritdoc />
        public double FrameRate { get; internal set; }

        /// <inheritdoc />
        public int Height { get; internal set; }

        /// <inheritdoc />
        public int Width { get; internal set; }

        /// <inheritdoc />
        public double Size { get; internal set; }

        /// <inheritdoc />
        public bool IsRunning => _conversion?.IsRunning == true;


        /// <inheritdoc cref="IVideoInfo.ToString" />
        [UsedImplicitly]
        public override string ToString()
        {
            return "Video FilePath : " + FilePath + Environment.NewLine +
                   "Video Root : " + Path.GetDirectoryName(FilePath) + Environment.NewLine +
                   "Video Name: " + Path.GetFileName(FilePath) + Environment.NewLine +
                   "Video Extension : " + Extension + Environment.NewLine +
                   "Video duration : " + Duration + Environment.NewLine +
                   "Audio format : " + AudioFormat + Environment.NewLine +
                   "Video format : " + VideoFormat + Environment.NewLine +
                   "Aspect Ratio : " + Ratio + Environment.NewLine +
                   "Framerate : " + FrameRate + "fps" + Environment.NewLine +
                   "Resolution : " + Width + "x" + Height + Environment.NewLine +
                   "Size : " + Size + " MB";
        }


        /// <inheritdoc />
        [UsedImplicitly]
        public bool ToTs(string outputPath)
        {
            _conversion = new Conversion()
                .SetInput(FilePath)
                .StreamCopy(Channel.Both)
                .SetBitstreamFilter(Channel.Video, Filter.H264_Mp4ToAnnexB)
                .SetCodec(VideoCodec.MpegTs)
                .SetOutput(outputPath);

            _conversion.OnProgress += OnConversionProgress;
            return _conversion.Start();
        }


        /// <inheritdoc />
        [UsedImplicitly]
        public bool ToWebM(string outputPath, string size = "", AudioQuality audioQuality = AudioQuality.Normal)
        {
            _conversion = new Conversion()
                .SetInput(FilePath)
                .SetScale(size)
                .SetVideo(VideoCodec.LibVpx, 2400)
                .SetSpeed(16)
                .SetAudio(AudioCodec.LibVorbis, audioQuality)
                .SetOutput(outputPath);

            _conversion.OnProgress += OnConversionProgress;
            return _conversion.Start();
        }


        /// <inheritdoc />
        [UsedImplicitly]
        public bool ToOgv(string outputPath, string size = "", AudioQuality audioQuality = AudioQuality.Normal, bool multithread = false)
        {
            _conversion = new Conversion()
                .SetInput(FilePath)
                .SetScale(size)
                .SetVideo(VideoCodec.LibTheora, 2400)
                .SetSpeed(16)
                .SetAudio(AudioCodec.LibVorbis, audioQuality)
                .SetOutput(outputPath);

            _conversion.OnProgress += OnConversionProgress;
            return _conversion.Start();
        }


        /// <inheritdoc />
        [UsedImplicitly]
        public bool ToMp4(string outputPath, Speed speed = Speed.SuperFast,
            string size = "", AudioQuality audioQuality = AudioQuality.Normal, bool multithread = false)
        {
            _conversion = new Conversion()
                .SetInput(FilePath)
                .UseMultiThread(multithread)
                .SetScale(size)
                .SetVideo(VideoCodec.LibX264, 2400)
                .SetSpeed(speed)
                .SetAudio(AudioCodec.Aac, audioQuality)
                .SetOutput(outputPath);

            _conversion.OnProgress += OnConversionProgress;
            return _conversion.Start();
        }


        /// <inheritdoc />
        [UsedImplicitly]
        public bool ExtractVideo(string output)
        {
            _conversion = new Conversion().SetInput(FilePath)
                                                     .StreamCopy(Channel.Both)
                                                     .DisableChannel(Channel.Audio)
                                                     .SetOutput(output);

            _conversion.OnProgress += OnConversionProgress;
            return _conversion.Start();
        }

        /// <inheritdoc />
        [UsedImplicitly]
        public bool ExtractAudio(string output)
        {
            _conversion = new Conversion().SetInput(FilePath)
                                                     .DisableChannel(Channel.Video)
                                                     .SetOutput(output);

            _conversion.OnProgress += OnConversionProgress;
            return _conversion.Start();
        }


        /// <inheritdoc />
        [UsedImplicitly]
        public bool AddAudio(FileInfo audio, string output)
        {
            _conversion = new Conversion().SetInput(new FileInfo(FilePath), audio)
                                                     .StreamCopy(Channel.Video)
                                                     .SetAudio(AudioCodec.Aac, AudioQuality.Hd)
                                                     .SetOutput(output);

            _conversion.OnProgress += OnConversionProgress;
            return _conversion.Start();
        }


        /// <inheritdoc />
        [UsedImplicitly]
        public Bitmap Snapshot(Size? size = null, TimeSpan? captureTime = null)
        {
            string output = $"{Environment.TickCount}.png";

            bool success = Snapshot(this, output, size, captureTime);

            if(!success)
                throw new OperationCanceledException("Could not take snapshot!");

            Bitmap result;

            using(Image bmp = Image.FromFile(output))
            {
                using(var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);

                    result = new Bitmap(ms);
                }
            }

            if(File.Exists(output))
                File.Delete(output);

            return result;
        }


        /// <inheritdoc />
        [UsedImplicitly]
        public Bitmap Snapshot(string output, Size? size = null, TimeSpan? captureTime = null)
        {
            bool success = Snapshot(this, output, size, captureTime);

            if(!success)
                throw new OperationCanceledException("Could not take snapshot!");

            Bitmap result;


            using(Image bmp = Image.FromFile(Path.ChangeExtension(output, ".png")))
            {
                result = (Bitmap) bmp.Clone();
            }

            return result;
        }

        /// <inheritdoc />
        [UsedImplicitly]
        public bool Snapshot(VideoInfo source, string outputPath, Size? size = null, TimeSpan? captureTime = null)
        {
            if(captureTime == null)
                captureTime = TimeSpan.FromSeconds(source.Duration.TotalSeconds / 3);

            size = GetSize(source, size);

            _conversion = new Conversion()
                .SetInput(source.FilePath)
                .SetVideo(VideoCodec.Png)
                .SetOutputFramesCount(1)
                .SetSeek(captureTime)
                .SetSize(size)
                .SetOutput(outputPath);

            _conversion.OnProgress += OnConversionProgress;
            return _conversion.Start();
        }

        /// <inheritdoc />
        [UsedImplicitly]
        public bool SaveM3U8Stream(Uri uri, string outputPath)
        {
            if(uri.Scheme != "http" ||
               uri.Scheme != "https")
                throw new ArgumentException($"Invalid uri {uri.AbsolutePath}");

            return new Conversion().SetInput(uri)
                                   .SetOutput(outputPath)
                                   .Start();
        }

        /// <inheritdoc />
        [UsedImplicitly]
        public bool JoinWith(string output, params IVideoInfo[] videos)
        {
            List<IVideoInfo> queuedVideos = videos.ToList();

            queuedVideos.Insert(0, this);

            var pathList = new List<string>();

            foreach(VideoInfo video in videos)
            {
                string tempFileName = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Ts);
                pathList.Add(tempFileName);
                video.ToTs(tempFileName);
            }

            _conversion = new Conversion().
                Concat(pathList.ToArray())
                                                     .StreamCopy(Channel.Both)
                                                     .SetBitstreamFilter(Channel.Audio, Filter.Aac_AdtstoAsc)
                                                     .SetOutput(output);

            _conversion.OnProgress += OnConversionProgress;
            return _conversion.Start();
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
    }
}
