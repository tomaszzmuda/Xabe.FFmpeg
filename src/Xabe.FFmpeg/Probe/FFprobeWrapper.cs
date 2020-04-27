using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Get information about media file
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    internal sealed class FFprobeWrapper : FFmpeg
    {
        private async Task<ProbeModel.Stream[]> GetStreams(string videoPath, CancellationToken cancellationToken)
        {
            ProbeModel probe = null;
            string stringResult = await Start($"-v panic -print_format json -show_streams \"{videoPath}\"", cancellationToken);
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

        private async Task<FormatModel.Format> GetFormat(string videoPath, CancellationToken cancellationToken)
        {
            string stringResult = await Start($"-v panic -print_format json -show_entries format=size,duration,bit_rate \"{videoPath}\"", cancellationToken);
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

        public Task<string> Start(string args, CancellationToken cancellationToken)
        {
            return RunProcess(args, cancellationToken);
        }

        private async Task<string> RunProcess(string args, CancellationToken cancellationToken)
        {
            return await await Task.Factory.StartNew(async () =>
            {
                using (Process process = RunProcess(args, FFprobePath, null, standardOutput: true))
                {
                    cancellationToken.Register(() =>
                    {
                        try
                        {
                            if (!process.HasExited)
                            {
                                process.CloseMainWindow();
                                process.Kill();
                            }
                        }
                        catch
                        {

                        }
                    });
                    var output = await process.StandardOutput.ReadToEndAsync();
                    process.WaitForExit();
                    return output;
                }
            },
            cancellationToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
        }

        /// <summary>
        ///     Get proporties prom media file
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="mediaInfo">Empty media info</param>
        /// <returns>Properties</returns>
        public async Task<MediaInfo> SetProperties(MediaInfo mediaInfo, CancellationToken cancellationToken)
        {
            var path = mediaInfo.Path;
            ProbeModel.Stream[] streams = await GetStreams(path, cancellationToken);
            if (!streams.Any())
            {
                throw new ArgumentException($"Invalid file. Cannot load file {path}");
            }

            FormatModel.Format format = await GetFormat(path, cancellationToken);
            if (format.size != null)
            {
                mediaInfo.Size = long.Parse(format.size);
            }

            mediaInfo.VideoStreams = PrepareVideoStreams(path, streams.Where(x => x.codec_type == "video"), format);
            mediaInfo.AudioStreams = PrepareAudioStreams(path, streams.Where(x => x.codec_type == "audio"));
            mediaInfo.SubtitleStreams = PrepareSubtitleStreams(path, streams.Where(x => x.codec_type == "subtitle"));

            mediaInfo.Duration = CalculateDuration(mediaInfo.VideoStreams, mediaInfo.AudioStreams);
            return mediaInfo;
        }

        private static TimeSpan CalculateDuration(IEnumerable<IVideoStream> videoStreams, IEnumerable<IAudioStream> audioStreams)
        {
            double audioMax = audioStreams.Any() ? audioStreams.Max(x => x.Duration.TotalSeconds) : 0;
            double videoMax = videoStreams.Any() ? videoStreams.Max(x => x.Duration.TotalSeconds) : 0;

            return TimeSpan.FromSeconds(Math.Max(audioMax, videoMax));
        }

        private IEnumerable<IAudioStream> PrepareAudioStreams(string path, IEnumerable<ProbeModel.Stream> audioStreamModels)
        {
            return audioStreamModels.Select(model => new AudioStream()
            {
                Codec = model.codec_name,
                Duration = GetAudioDuration(model),
                Path = path,
                Index = model.index,
                Bitrate = Math.Abs(model.bit_rate),
                Channels = model.channels,
                SampleRate = model.sample_rate,
                Language = model.tags?.language,
                Default = model.disposition?._default,
                Forced = model.disposition?.forced,
            });
        }

        private static IEnumerable<ISubtitleStream> PrepareSubtitleStreams(string path, IEnumerable<ProbeModel.Stream> subtitleStreamModels)
        {
            return subtitleStreamModels.Select(model => new SubtitleStream()
            {
                Codec = model.codec_name,
                Path = path,
                Index = model.index,
                Language = model.tags?.language,
                Title = model.tags?.title,
                Default = model.disposition?._default,
                Forced = model.disposition?.forced,
            });
        }

        private IEnumerable<IVideoStream> PrepareVideoStreams(string path, IEnumerable<ProbeModel.Stream> videoStreamModels, FormatModel.Format format)
        {
            return videoStreamModels.Select(model => new VideoStream()
            {
                Codec = model.codec_name,
                Duration = GetVideoDuration(model, format),
                Width = model.width,
                Height = model.height,
                Framerate = GetVideoFramerate(model),
                Ratio = GetVideoAspectRatio(model.width, model.height),
                Path = path,
                Index = model.index,
                Bitrate = Math.Abs(model.bit_rate) > 0.01 ? model.bit_rate : format.bit_Rate,
                PixelFormat = model.pix_fmt,
                Default = model.disposition?._default,
                Forced = model.disposition?.forced
            });
        }
    }
}
