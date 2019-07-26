using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xabe.FFmpeg.Model;
using Xabe.FFmpeg.Streams;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Get information about media file
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    internal sealed class FFprobeWrapper : FFmpeg
    {
        private async Task<ProbeModel.Stream[]> GetStream(string videoPath)
        {
            ProbeModel probe = null;
            string stringResult = await Start($"-v quiet -print_format json -show_streams \"{videoPath}\"").ConfigureAwait(false);
            if (string.IsNullOrEmpty(stringResult))
            {
                return new ProbeModel.Stream[0];
            }
            probe = JsonConvert.DeserializeObject<ProbeModel>(stringResult);
            return probe.streams ?? new ProbeModel.Stream[0];
        }

        private double GetVideoFramerate(ProbeModel.Stream vid)
        {
            string[] fr = vid.r_frame_rate.Split('/');
            return Math.Round(double.Parse(fr[0]) / double.Parse(fr[1]), 3);
        }

        private string GetVideoAspectRatio(int width, int height)
        {
            int cd = GetGcd(width, height);
            if (cd <= 0)
            {
                return "0:0";
            }
            return width / cd + ":" + height / cd;
        }

        private async Task<FormatModel.Format> GetFormat(string videoPath)
        {
            string stringResult = await Start($"-v quiet -print_format json -show_format \"{videoPath}\"").ConfigureAwait(false);
            var root = JsonConvert.DeserializeObject<FormatModel.Root>(stringResult);
            return root.format;
        }

        private TimeSpan GetAudioDuration(ProbeModel.Stream audio)
        {
            double duration = audio.duration;
            TimeSpan audioDuration = TimeSpan.FromSeconds(duration);
            audioDuration = audioDuration.Subtract(TimeSpan.FromMilliseconds(audioDuration.Milliseconds));
            return audioDuration;
        }

        private TimeSpan GetVideoDuration(ProbeModel.Stream video, FormatModel.Format format)
        {
            double duration = video.duration > 0.01 ? video.duration : format.duration;
            TimeSpan videoDuration = TimeSpan.FromSeconds(duration);
            videoDuration = videoDuration.Subtract(TimeSpan.FromMilliseconds(videoDuration.Milliseconds));
            return videoDuration;
        }

        private int GetGcd(int width, int height)
        {
            while (width != 0 &&
                  height != 0)
            {
                if (width > height)
                {
                    width -= height;
                }
                else
                {
                    height -= width;
                }
            }
            return width == 0 ? height : width;
        }

        public Task<string> Start(string args)
        {
            return RunProcess(args);
        }

        private Task<string> RunProcess(string args)
        {
            return Task.Factory.StartNew(() =>
            {
                using (Process process = RunProcess(args, FFprobePath, Priority, standardOutput: true))
                {
                    while (!process.HasExited)
                    {
                        process.WaitForExit(10);
                        int toRead = process.StandardOutput.Peek();
                        if (toRead > 0)
                        {
                            break;
                        }
                    }

                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return output;
                }
            },
            CancellationToken.None,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
        }

        /// <summary>
        ///     Get proporties prom media file
        /// </summary>
        /// <param name="fileInfo">Media file info</param>
        /// <param name="mediaInfo">Empty media info</param>
        /// <returns>Properties</returns>
        public async Task<MediaInfo> GetProperties(System.IO.FileInfo fileInfo, MediaInfo mediaInfo)
        {
            ProbeModel.Stream[] streams = await GetStream(fileInfo.FullName).ConfigureAwait(false);
            if (!streams.Any())
            {
                throw new ArgumentException($"Invalid file. Cannot load file {fileInfo.Name}");
            }

            FormatModel.Format format = await GetFormat(fileInfo.FullName).ConfigureAwait(false);
            mediaInfo.Size = long.Parse(format.size);

            mediaInfo.VideoStreams = PrepareVideoStreams(fileInfo, streams.Where(x => x.codec_type == "video"), format);
            mediaInfo.AudioStreams = PrepareAudioStreams(fileInfo, streams.Where(x => x.codec_type == "audio"));
            mediaInfo.SubtitleStreams = PrepareSubtitleStreams(fileInfo, streams.Where(x => x.codec_type == "subtitle"));

            mediaInfo.Duration = CalculateDuration(mediaInfo.VideoStreams, mediaInfo.AudioStreams);
            return mediaInfo;
        }

        private static TimeSpan CalculateDuration(IEnumerable<IVideoStream> videoStreams, IEnumerable<IAudioStream> audioStreams)
        {
            double audioMax = audioStreams.Any() ? audioStreams.Max(x => x.Duration.TotalSeconds) : 0;
            double videoMax = videoStreams.Any() ? videoStreams.Max(x => x.Duration.TotalSeconds) : 0;

            return TimeSpan.FromSeconds(Math.Max(audioMax, videoMax));
        }

        private IEnumerable<IAudioStream> PrepareAudioStreams(System.IO.FileInfo fileInfo, IEnumerable<ProbeModel.Stream> audioStreamModels)
        {
            foreach (ProbeModel.Stream model in audioStreamModels)
            {
                var stream = new AudioStream
                {
                    Format = model.codec_name,
                    Duration = GetAudioDuration(model),
                    Source = fileInfo,
                    Index = model.index,
                    Bitrate = Math.Abs(model.bit_rate),
                    Channels = model.channels,
                    SampleRate = model.sample_rate,
                    Language = model.tags?.language,
                    Default = model.disposition?._default,
                    Forced = model.disposition?.forced,
                };
                yield return stream;
            }
        }

        private static IEnumerable<ISubtitleStream> PrepareSubtitleStreams(System.IO.FileInfo fileInfo, IEnumerable<ProbeModel.Stream> subtitleStreamModels)
        {
            foreach (ProbeModel.Stream model in subtitleStreamModels)
            {
                var stream = new SubtitleStream
                {
                    Format = model.codec_name,
                    Source = fileInfo,
                    Index = model.index,
                    Language = model.tags?.language,
                    Title = model.tags?.title,
                    Default = model.disposition?._default,
                    Forced = model.disposition?.forced,
                };
                yield return stream;
            }
        }

        private IEnumerable<IVideoStream> PrepareVideoStreams(System.IO.FileInfo fileInfo, IEnumerable<ProbeModel.Stream> videoStreamModels, FormatModel.Format format)
        {
            foreach (ProbeModel.Stream model in videoStreamModels)
            {
                var stream = new VideoStream
                {
                    Format = model.codec_name,
                    Duration = GetVideoDuration(model, format),
                    Width = model.width,
                    Height = model.height,
                    FrameRate = GetVideoFramerate(model),
                    Ratio = GetVideoAspectRatio(model.width, model.height),
                    Source = fileInfo,
                    Index = model.index,
                    Bitrate = Math.Abs(model.bit_rate) > 0.01 ? model.bit_rate : format.bit_Rate,
                    Default = model.disposition?._default,
                    Forced = model.disposition?.forced,
                };
                yield return stream;
            }
        }
    }
}
