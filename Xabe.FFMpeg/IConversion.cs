using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFMpeg.Enums;

namespace Xabe.FFMpeg
{
    /// <summary>
    ///     Allows to prepare and start IConversion. Only for advanced users.
    /// </summary>
    public interface IConversion
    {
        /// <summary>
        ///     Reverse media
        /// </summary>
        /// <param name="type">Channel type</param>
        /// <returns>IConversion object</returns>
        IConversion Reverse(Channel type);

        /// <summary>
        ///     Set speed of IConversion. Slower speed equals better compression and quality.
        /// </summary>
        /// <param name="speed">Speed</param>
        /// <returns>IConversion object</returns>
        IConversion SetSpeed(Speed speed);

        /// <summary>
        ///     Set max cpu threads
        /// </summary>
        /// <param name="cpu">Threads</param>
        /// <returns>IConversion object</returns>
        IConversion SetSpeed(int cpu);

        /// <summary>
        ///     Set audio codec and bitrate
        /// </summary>
        /// <param name="codec">Audio odec</param>
        /// <param name="bitrate">Audio bitrade</param>
        /// <returns>IConversion object</returns>
        IConversion SetAudio(AudioCodec codec, AudioQuality bitrate);

        /// <summary>
        ///     Set audio codec and bitrate
        /// </summary>
        /// <param name="codec">Audio odec</param>
        /// <param name="bitrate">Audio bitrade</param>
        /// <returns>IConversion object</returns>
        IConversion SetAudio(string codec, AudioQuality bitrate);

        /// <summary>
        ///     Set video codec and bitrate
        /// </summary>
        /// <param name="codec">Video codec</param>
        /// <param name="bitrate">Video bitrate</param>
        /// <returns>IConversion object</returns>
        IConversion SetVideo(VideoCodec codec, int bitrate = 0);

        /// <summary>
        ///     Set video codec and bitrate
        /// </summary>
        /// <param name="codec">Video codec</param>
        /// <param name="bitrate">Video bitrate</param>
        /// <returns>IConversion object</returns>
        IConversion SetVideo(string codec, int bitrate = 0);

        /// <summary>
        ///     Defines if converter should use all CPU cores
        /// </summary>
        /// <param name="multiThread">Use all CPU cores</param>
        /// <returns>IConversion object</returns>
        IConversion UseMultiThread(bool multiThread);

        /// <summary>
        ///     Set URI of stream
        /// </summary>
        /// <param name="uri">URI</param>
        /// <returns>IConversion object</returns>
        IConversion SetInput(Uri uri);

        /// <summary>
        ///     Disable channel. Can remove audio or video from media file.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>IConversion object</returns>
        IConversion DisableChannel(Channel type);

        /// <summary>
        ///     Set input file
        /// </summary>
        /// <param name="input">Media file to convert</param>
        /// <returns>IConversion object</returns>
        IConversion SetInput(VideoInfo input);

        /// <summary>
        ///     Set input files
        /// </summary>
        /// <param name="inputs">Media files to convert</param>
        /// <returns>IConversion object</returns>
        IConversion SetInput(params FileInfo[] inputs);

        /// <summary>
        ///     Set output path
        /// </summary>
        /// <param name="outputPath">Output media file</param>
        /// <returns>IConversion object</returns>
        IConversion SetOutput(string outputPath);

        /// <summary>
        ///     Set size of video
        /// </summary>
        /// <param name="size">VideoSize</param>
        /// <returns>IConversion object</returns>
        IConversion SetScale(VideoSize size);

        /// <summary>
        ///     Set size of video
        /// </summary>
        /// <param name="size">Size in ffmpeg format</param>
        /// <returns>IConversion object</returns>
        IConversion SetScale(string size);

        /// <summary>
        ///     Set size of video
        /// </summary>
        /// <param name="size">VideoSize</param>
        /// <returns>IConversion object</returns>
        IConversion SetSize(Size? size);

        /// <summary>
        ///     Set video codec
        /// </summary>
        /// <param name="codec">Video codec</param>
        /// <returns>IConversion object</returns>
        IConversion SetCodec(VideoCodec codec);

        /// <summary>
        ///     Set video codec
        /// </summary>
        /// <param name="codec">Video codec</param>
        /// <returns>IConversion object</returns>
        IConversion SetCodec(string codec);

        /// <summary>
        ///     Set filter
        /// </summary>
        /// <param name="type">Channel type</param>
        /// <param name="filter">Filter</param>
        /// <returns>IConversion object</returns>
        IConversion SetBitstreamFilter(Channel type, Filter filter);

        /// <summary>
        ///     Set filter
        /// </summary>
        /// <param name="type">Channel type</param>
        /// <param name="filter">Filter</param>
        /// <returns>IConversion object</returns>
        IConversion SetBitstreamFilter(Channel type, string filter);

        /// <summary>
        ///     It makes ffmpeg omit the decoding and encoding step for the specified stream, so it does only demuxing and muxing.
        ///     It is useful for changing the container format or modifying container-level metadata.
        ///     Cannot be used with operations which require encoding and decoding video (scaling, changing codecs etc.)
        /// </summary>
        /// <param name="type">Channel type</param>
        /// <returns>IConversion object</returns>
        IConversion StreamCopy(Channel type);

        /// <summary>
        ///     Change video speed
        /// </summary>
        /// <param name="multiplication">Speed value. To double the speed of the video set this to 0.5.</param>
        /// <returns>IConversion object</returns>
        IConversion ChangeVideoSpeed(double multiplication);

        /// <summary>
        ///     Change video speed
        /// </summary>
        /// <param name="multiplication">Speed value. (0.5 - 2.0). To double the speed of the audio set this to 2.0</param>
        /// <returns>IConversion object</returns>
        IConversion ChangeAudioSpeed(double multiplication);

        /// <summary>
        ///     Seeks in input file to position. (-ss argument)
        /// </summary>
        /// <param name="seek">Position</param>
        /// <returns>IConversion object</returns>
        IConversion SetSeek(TimeSpan? seek);

        /// <summary>
        ///     Set output frames count
        /// </summary>
        /// <param name="number">Number of frames</param>
        /// <returns>IConversion object</returns>
        IConversion SetOutputFramesCount(int number);

        /// <summary>
        ///     Loop over the input stream. Currently it works only for image streams. (-loop)
        /// </summary>
        /// <param name="count">Number of repeats</param>
        /// <returns>IConversion object</returns>
        IConversion SetLoop(int count);

        /// <summary>
        ///     Finish encoding when the shortest input stream ends. (-shortest)
        /// </summary>
        /// <param name="useShortest"></param>
        /// <returns>IConversion object</returns>
        IConversion UseShortest(bool useShortest);

        /// <summary>
        ///     Concat multiple media files
        /// </summary>
        /// <param name="paths">Media files</param>
        /// <returns>IConversion object</returns>
        IConversion Concat(params string[] paths);

        /// <summary>
        ///     Build command
        /// </summary>
        /// <returns>Command</returns>
        string Build();

        /// <summary>
        ///     Start conversion
        /// </summary>
        /// <returns>Conversion result</returns>
        bool Start();

        /// <summary>
        ///     Returns true if the associated process is still alive/running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        ///     Dispose process
        /// </summary>
        void Dispose();

        /// <summary>
        ///     Kill ffmpeg process.
        /// </summary>
        void Kill();
    }
}
