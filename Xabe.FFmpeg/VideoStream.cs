using System;
using System.IO;
using System.Text;
using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg
{
    public class VideoStream : IVideoStream
    {
        private string _burnSubtitles;
        private string _videoSpeed;
        private string _watermark;
        private string _reverse;
        private string _scale;
        private string _size;
        private string _codec;

        /// <inheritdoc />
        public int Width { get; internal set; }

        /// <inheritdoc />
        public int Height { get; internal set; }

        /// <inheritdoc />
        public double FrameRate { get; internal set; }

        /// <inheritdoc />
        public string Ratio { get; internal set; }

        /// <inheritdoc />
        public FileInfo Source { get; internal set; }

        /// <inheritdoc />
        public string Build()
        {
            //todo: all params
            var builder = new StringBuilder();
            builder.Append(_watermark);
            builder.Append(_scale);
            builder.Append(_codec);
            //builder.Append(_speed);
            //builder.Append(_audio);
            //builder.Append(_format);
            //builder.Append(_bitsreamFilter);
            //builder.Append(_copy);
            //builder.Append(_seek);
            //builder.Append(_frameCount);
            //builder.Append(_loop);
            builder.Append(_reverse);
            //builder.Append(_rotate);
            //builder.Append(_shortestInput);
            builder.Append(BuildFilter());
            //builder.Append(_split);
            //builder.Append(_output);

            return builder.ToString();
        }

        private string BuildFilter()
        {
            var builder = new StringBuilder();
            builder.Append("-filter:v ");
            builder.Append(_videoSpeed);
            builder.Append(_burnSubtitles);

            string filter = builder.ToString();
            if (filter == "-filter:v ")
                return "";
            return filter;
        }

        /// <summary>
        ///     Burn subtitle into file
        /// </summary>
        /// <param name="subtitlePath">Path to subtitle file in .srt format</param>
        /// <returns>Conversion result</returns>
        public void SetSubtitle(string subtitlePath)
        {
            SetSubtitle(subtitlePath, "", "", null);
        }

        /// <summary>
        ///     Burn subtitle into file
        /// </summary>
        /// <param name="subtitlePath">Path to subtitle file in .srt format</param>
        /// <param name="encode">Set subtitles input character encoding. Only useful if not UTF-8.</param>
        /// <returns>Conversion result</returns>
        public void SetSubtitle(string subtitlePath, string encode)
        {
            SetSubtitle(subtitlePath, encode, "", null);
        }

        /// <summary>
        ///     Burn subtitle into file
        /// </summary>
        /// <param name="subtitlePath">Path to subtitle file in .srt format</param>
        /// <param name="style">Override default style or script info parameters of the subtitles. It accepts a string containing ASS style format KEY=VALUE couples separated by ","</param>
        /// <param name="originalSize">Specify the size of the original video, the video for which the ASS style was composed. This is necessary to correctly scale the fonts if the aspect ratio has been changed.</param>
        /// <returns>Conversion result</returns>
        public void SetSubtitle(string subtitlePath, string style, VideoSize originalSize)
        {
            SetSubtitle(subtitlePath, "", style, originalSize);
        }

        /// <summary>
        ///     Burn subtitle into file
        /// </summary>
        /// <param name="subtitlePath">Path to subtitle file in .srt format</param>
        /// <param name="encode">Set subtitles input character encoding. Only useful if not UTF-8.</param>
        /// <param name="style">Override default style or script info parameters of the subtitles. It accepts a string containing ASS style format KEY=VALUE couples separated by ","</param>
        /// <param name="originalSize">Specify the size of the original video, the video for which the ASS style was composed. This is necessary to correctly scale the fonts if the aspect ratio has been changed.</param>
        /// <returns>Conversion result</returns>
        public void SetSubtitle(string subtitlePath, string encode, string style, VideoSize originalSize)
        {
            _burnSubtitles = $"\"subtitles='{subtitlePath}'".Replace("\\", "\\\\")
                                                            .Replace(":", "\\:");

            if (!string.IsNullOrEmpty(encode))
                _burnSubtitles += $":charenc={encode}";
            if (!string.IsNullOrEmpty(style))
                _burnSubtitles += $":force_style=\'{style}\'";
            if (originalSize != null)
                _burnSubtitles += $":original_size={originalSize}";
            _burnSubtitles += "\" ";
        }

        /// <inheritdoc />
        public IVideoStream SetWatermark(string imagePath, Position position)
        {
            string argument = $"-i \"{imagePath}\" -filter_complex ";
            switch (position)
            {
                case Position.Bottom:
                    argument += "\"overlay=(main_w-overlay_w)/2:main_h-overlay_h\" ";
                    break;
                case Position.Center:
                    argument += "\"overlay=x=(main_w-overlay_w)/2:y=(main_h-overlay_h)/2\" ";
                    break;
                case Position.BottomLeft:
                    argument += "\"overlay=5:main_h-overlay_h\" ";
                    break;
                case Position.UpperLeft:
                    argument += "\"overlay=5:5\" ";
                    break;
                case Position.BottomRight:
                    argument += "\"overlay=(main_w-overlay_w):main_h-overlay_h\" ";
                    break;
                case Position.UpperRight:
                    argument += "\"overlay=(main_w-overlay_w):5\" ";
                    break;
                case Position.Left:
                    argument += "\"overlay=5:(main_h-overlay_h)/2\" ";
                    break;
                case Position.Right:
                    argument += "\"overlay=(main_w-overlay_w-5):(main_h-overlay_h)/2\" ";
                    break;
                case Position.Up:
                    argument += "\"overlay=(main_w-overlay_w)/2:5\" ";
                    break;
            }
            _watermark = argument;
            return this;
        }

        /// <inheritdoc />
        public IVideoStream Reverse()
        {
            _reverse = "-vf reverse ";
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetScale(VideoSize size)
        {
            if(size != null)
            {
                _scale = $"-vf scale={size} ";
            }
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetSize(VideoSize size)
        {
            if(size != null)
            {
                _size = $"-s {size} ";
            }
            return this;
        }

        /// <inheritdoc />
        public IVideoStream SetCodec(VideoCodec codec, int bitrate = 0)
        {
            _codec = $"-codec:v {codec} ";

            if (bitrate > 0)
                _codec += $"-b:v {bitrate}k ";
            return this;
        }

        /// <inheritdoc />
        public TimeSpan Duration { get; internal set; }

        /// <inheritdoc />
        public string Format { get; internal set; }
    }
}
