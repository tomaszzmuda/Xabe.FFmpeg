using System;
using System.IO;
using JetBrains.Annotations;

namespace Xabe.FFMpeg
{
    /// <inheritdoc cref="IVideoInfo" />
    public class VideoInfo: IVideoInfo
    {
        private TimeSpan _videoDuration;
        private bool _wasInitalized;
        private TimeSpan _duration;
        private string _audioFormat;
        private TimeSpan _audioDuration;
        private string _videoFormat;
        private string _ratio;
        private long _size;
        private int _width;
        private int _height;
        private double _frameRate;

        /// <inheritdoc />
        public VideoInfo(FileInfo sourceFileInfo): this(sourceFileInfo.FullName)
        {
        }

        /// <summary>
        ///     Get VideoInfo from file
        /// </summary>
        /// <param name="fullName">FullName to file</param>
        [UsedImplicitly]
        public VideoInfo(string fullName)
        {
            if(!File.Exists(fullName))
                throw new ArgumentException($"Input file {fullName} doesn't exists.");
            FullName = fullName;
        }

        /// <inheritdoc />
        public string FullName { get; }

        /// <inheritdoc />
        public TimeSpan VideoDuration
        {
            get
            {
                if(!_wasInitalized)
                    // ReSharper disable once ObjectCreationAsStatement
                    { _wasInitalized = true; new FFProbe(this); }
                return _videoDuration;
            }
            internal set => _videoDuration = value;
        }

        /// <inheritdoc />
        public string Extension => Path.GetExtension(FullName);

        /// <inheritdoc />
        public TimeSpan Duration
        {
            get
            {
                if (!_wasInitalized)
                    // ReSharper disable once ObjectCreationAsStatement
                    { _wasInitalized = true; new FFProbe(this); }
                return _duration;
            }
            internal set => _duration = value;
        }
        /// <inheritdoc />
        public string AudioFormat
        {
            get
            {
                if (!_wasInitalized)
                    // ReSharper disable once ObjectCreationAsStatement
                    { _wasInitalized = true; new FFProbe(this); }
                return _audioFormat;
            }
            internal set => _audioFormat = value;
        }

        /// <inheritdoc />
        public TimeSpan AudioDuration
        {
            get
            {
                if (!_wasInitalized)
                    // ReSharper disable once ObjectCreationAsStatement
                    { _wasInitalized = true; new FFProbe(this); }
                return _audioDuration;
            }
            internal set => _audioDuration = value;
        }

        /// <inheritdoc />
        public string VideoFormat
        {
            get
            {
                if (!_wasInitalized)
                    // ReSharper disable once ObjectCreationAsStatement
                    { _wasInitalized = true; new FFProbe(this); }
                return _videoFormat;
            }
            internal set => _videoFormat = value;
        }

        /// <inheritdoc />
        public string Ratio
        {
            get
            {
                if (!_wasInitalized)
                    // ReSharper disable once ObjectCreationAsStatement
                    { _wasInitalized = true; new FFProbe(this); }
                return _ratio;
            }
            internal set => _ratio = value;
        }

        /// <inheritdoc />
        public double FrameRate
        {
            get
            {
                if (!_wasInitalized)
                    // ReSharper disable once ObjectCreationAsStatement
                    { _wasInitalized = true; new FFProbe(this); }
                return _frameRate;
            }
            internal set => _frameRate = value;
        }

        /// <inheritdoc />
        public int Height
        {
            get
            {
                if (!_wasInitalized)
                    // ReSharper disable once ObjectCreationAsStatement
                    { _wasInitalized = true; new FFProbe(this); }
                return _height;
            }
            internal set => _height = value;
        }

        /// <inheritdoc />
        public int Width
        {
            get
            {
                if (!_wasInitalized)
                    // ReSharper disable once ObjectCreationAsStatement
                    { _wasInitalized = true; new FFProbe(this); }
                return _width;
            }
            internal set => _width = value;
        }

        /// <inheritdoc />
        public long Size
        {
            get
            {
                if (!_wasInitalized)
                    // ReSharper disable once ObjectCreationAsStatement
                    { _wasInitalized = true; new FFProbe(this); }
                return _size;
            }
            internal set => _size = value;
        }

        /// <inheritdoc cref="IVideoInfo.ToString" />
        [UsedImplicitly]
        public override string ToString()
        {
            if(!_wasInitalized)
                { _wasInitalized = true; new FFProbe(this); }
            return $"Video fullName : {FullName}{Environment.NewLine}" +
                   $"Video root : {Path.GetDirectoryName(FullName)}{Environment.NewLine}" +
                   $"Video name: {Path.GetFileName(FullName)}{Environment.NewLine}" +
                   $"Video extension : {Extension}{Environment.NewLine}" +
                   $"Video duration : {VideoDuration}{Environment.NewLine}" +
                   $"Video format : {VideoFormat}{Environment.NewLine}" +
                   $"Audio format : {AudioFormat}{Environment.NewLine}" +
                   $"Audio duration : {AudioDuration}{Environment.NewLine}" +
                   $"Aspect Ratio : {Ratio}{Environment.NewLine}" +
                   $"Framerate : {Ratio} fps{Environment.NewLine}" +
                   $"Resolution : {Width} x {Height}{Environment.NewLine}" +
                   $"Size : {Size} b";
        }
    }
}
