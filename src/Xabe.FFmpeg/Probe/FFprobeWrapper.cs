using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Get information about media file
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    internal sealed class FFprobeWrapper : FFmpeg
    {
        private readonly JsonSerializerOptions _defaultSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            WriteIndented = true
        };

        private async Task<ProbeModel.Stream[]> GetStreams(string videoPath, CancellationToken cancellationToken)
        {
            var stringResult = await Start($"-v panic -print_format json=c=1 -show_streams {videoPath}", cancellationToken);
            if (string.IsNullOrEmpty(stringResult))
            {
                return Array.Empty<ProbeModel.Stream>();
            }

            var probe = JsonSerializer.Deserialize<ProbeModel>(stringResult, _defaultSerializerOptions);
            return probe.Streams ?? Array.Empty<ProbeModel.Stream>();
        }

        private double GetVideoFramerate(ProbeModel.Stream vid)
        {
            var frameCount = GetFrameCount(vid);
            var duration = vid.Duration;
            var fr = vid.RFrameRate.Split('/');

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
            return long.TryParse(vid.NbFrames, out var frameCount) ? frameCount : 0;
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

            return JsonSerializer.Deserialize<FormatModel.Root>(stringResult, _defaultSerializerOptions);
        }

        private TimeSpan GetAudioDuration(ProbeModel.Stream audio)
        {
            var duration = audio.Duration;
            var audioDuration = TimeSpan.FromSeconds(duration);
            return audioDuration;
        }

        private TimeSpan GetVideoDuration(ProbeModel.Stream video, FormatModel.Format format)
        {
            var duration = video.Duration > 0.01 ? video.Duration : format.duration;
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

            if (!string.IsNullOrWhiteSpace(infos.format.tags?.creation_time) && DateTimeOffset.TryParse(infos.format.tags.creation_time, out var creationdate))
            {
                mediaInfo.CreationTime = creationdate.UtcDateTime;
            }

            mediaInfo.VideoStreams = PrepareVideoStreams(path, streams.Where(x => x.CodecType == "video"), infos.format);
            mediaInfo.AudioStreams = PrepareAudioStreams(path, streams.Where(x => x.CodecType == "audio"));
            mediaInfo.SubtitleStreams = PrepareSubtitleStreams(path, streams.Where(x => x.CodecType == "subtitle"));

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
                Codec = model.CodecName,
                Duration = GetAudioDuration(model),
                Path = path,
                Index = model.Index,
                Bitrate = Math.Abs(model.BitRate),
                Channels = model.Channels,
                SampleRate = model.SampleRate,
                Language = model.Tags?.Language,
                Default = model.Disposition?.Default,
                Title = model.Tags?.Title,
                Forced = model.Disposition?.Forced,
            });
        }

        private static IEnumerable<ISubtitleStream> PrepareSubtitleStreams(string path, IEnumerable<ProbeModel.Stream> subtitleStreamModels)
        {
            return subtitleStreamModels.Select(model => new SubtitleStream()
            {
                Codec = model.CodecName,
                Path = path,
                Index = model.Index,
                Language = model.Tags?.Language,
                Title = model.Tags?.Title,
                Default = model.Disposition?.Default,
                Forced = model.Disposition?.Forced,
            });
        }

        private IEnumerable<IVideoStream> PrepareVideoStreams(string path, IEnumerable<ProbeModel.Stream> videoStreamModels, FormatModel.Format format)
        {
            return videoStreamModels.Select(model => new VideoStream()
            {
                Codec = model.CodecName,
                Duration = GetVideoDuration(model, format),
                Width = model.Width,
                Height = model.Height,
                Framerate = GetVideoFramerate(model),
                Ratio = GetVideoAspectRatio(model.Width, model.Height),
                Path = path,
                Index = model.Index,
                Bitrate = Math.Abs(model.BitRate) > 0.01 ? model.BitRate : format.bit_Rate,
                PixelFormat = model.PixFmt,
                Default = model.Disposition?.Default,
                Forced = model.Disposition?.Forced,
                Rotation = model.Tags?.Rotate
            });
        }
    }
}
