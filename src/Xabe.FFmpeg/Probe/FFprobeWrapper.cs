﻿using System;
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
            var stringResult = await Start($"-v panic -print_format json=c=1 -show_streams {videoPath}", cancellationToken);
            if (string.IsNullOrEmpty(stringResult))
            {
                return new ProbeModel.Stream[0];
            }

            ProbeModel probe = JsonConvert.DeserializeObject<ProbeModel>(stringResult);
            return probe.streams ?? new ProbeModel.Stream[0];
        }

        private double GetVideoFramerate(ProbeModel.Stream vid)
        {
            var frameCount = GetFrameCount(vid);
            var duration = vid.duration;
            var fr = vid.r_frame_rate.Split('/');

            if (frameCount > 0)
            {
                return Math.Round(frameCount / duration, 3);
            }
            else
            {
                return Math.Round(double.Parse(fr[0]) / double.Parse(fr[1]), 3);
            }
        }

        private long GetFrameCount(ProbeModel.Stream vid)
        {
            return long.TryParse(vid.nb_frames, out var frameCount) ? frameCount : 0;
        }

        private string GetVideoAspectRatio(int width, int height)
        {
            var cd = GetGcd(width, height);
            if (cd <= 0)
            {
                return "0:0";
            }

            return (width / cd) + ":" + (height / cd);
        }

        private async Task<FormatModel.Root> GetInfos(string videoPath, CancellationToken cancellationToken)
        {
            var stringResult = await Start($"-v panic -print_format json=c=1 -show_entries format=size,duration,bit_rate:format_tags=creation_time {videoPath}", cancellationToken);
            var root = JsonConvert.DeserializeObject<FormatModel.Root>(stringResult);
            return root;
        }

        private TimeSpan GetAudioDuration(ProbeModel.Stream audio)
        {
            var duration = audio.duration;
            var audioDuration = TimeSpan.FromSeconds(duration);
            return audioDuration;
        }

        private TimeSpan GetVideoDuration(ProbeModel.Stream video, FormatModel.Format format)
        {
            var duration = video.duration > 0.01 ? video.duration : format.duration;
            var videoDuration = TimeSpan.FromSeconds(duration);
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
            return await Task.Factory.StartNew(() =>
            {
                using (Process process = RunProcess(args, FFprobePath, null, standardOutput: true))
                {
                    var processExited = false;
                    cancellationToken.Register(() =>
                    {
                        try
                        {
                            if (!processExited && !process.HasExited)
                            {
                                process.CloseMainWindow();
                                process.Kill();
                            }
                        }
                        catch
                        {

                        }
                    });
                    var text = new List<string>();
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    processExited = true;
                    return output;
                }
            },
            cancellationToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
        }

        /// <summary>
        ///     Get proporties from media file
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="mediaInfo">Empty media info</param>
        /// <returns>Properties</returns>
        public async Task<MediaInfo> SetProperties(MediaInfo mediaInfo, CancellationToken cancellationToken)
        {
            var path = mediaInfo.Path.Escape();
            ProbeModel.Stream[] streams = await GetStreams(path, cancellationToken);
            if (!streams.Any())
            {
                throw new ArgumentException($"Invalid file. Cannot load file {path}");
            }

            var infos = await GetInfos(path, cancellationToken);
            if (infos.format.size != null)
            {
                mediaInfo.Size = long.Parse(infos.format.size);
            }

            if (!string.IsNullOrWhiteSpace(infos.format.tags?.creation_time) && DateTime.TryParse(infos.format.tags.creation_time, out var creationdate))
            {
                mediaInfo.CreationTime = creationdate;
            }

            mediaInfo.VideoStreams = PrepareVideoStreams(path, streams.Where(x => x.codec_type == "video"), infos.format);
            mediaInfo.AudioStreams = PrepareAudioStreams(path, streams.Where(x => x.codec_type == "audio"));
            mediaInfo.SubtitleStreams = PrepareSubtitleStreams(path, streams.Where(x => x.codec_type == "subtitle"));

            mediaInfo.Duration = CalculateDuration(mediaInfo.VideoStreams, mediaInfo.AudioStreams);
            return mediaInfo;
        }

        private static TimeSpan CalculateDuration(IEnumerable<IVideoStream> videoStreams, IEnumerable<IAudioStream> audioStreams)
        {
            var audioMax = audioStreams.Any() ? audioStreams.Max(x => x.Duration.TotalSeconds) : 0;
            var videoMax = videoStreams.Any() ? videoStreams.Max(x => x.Duration.TotalSeconds) : 0;

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
                Default = model.disposition?.@default,
                Title = model.tags?.title,
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
                Default = model.disposition?.@default,
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
                Default = model.disposition?.@default,
                Forced = model.disposition?.forced,
                Rotation = model.tags?.rotate
            });
        }
    }
}
