using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xabe.FFmpeg.Model;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Get information about media file
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public sealed class FFprobe: FFbase
    {
        private async Task<ProbeModel.Stream[]> GetStream(string videoPath)
        {
            string jsonStreams = await RunProcess($"-v quiet -print_format json -show_streams \"{videoPath}\"");
            var probe = JsonConvert.DeserializeObject<ProbeModel>(jsonStreams, new JsonSerializerSettings());
            return probe.streams ?? new ProbeModel.Stream[0];
        }

        private double GetVideoFramerate(ProbeModel.Stream vid)
        {
            string[] fr = vid.r_frame_rate.Split('/');
            return Math.Round(double.Parse(fr[0]) / double.Parse(fr[1]), 3);
        }

        private string GetVideoAspectRatio(int width, int heigght)
        {
            int cd = GetGcd(width, heigght);
            return width / cd + ":" + heigght / cd;
        }

        private TimeSpan GetVideoDuration(FormatModel.Format format, ProbeModel.Stream video)
        {
            video.duration = format.duration;
            video.bit_rate = Math.Abs(format.bitRate) < 0.01 ? video.bit_rate : format.bitRate;

            double duration = video.duration;
            TimeSpan videoDuration = TimeSpan.FromSeconds(duration);
            videoDuration = videoDuration.Subtract(TimeSpan.FromMilliseconds(videoDuration.Milliseconds));

            return videoDuration;
        }

        private async Task<FormatModel.Format> GetFormat(string videoPath)
        {
            string jsonFormat =
                await RunProcess($"-v quiet -print_format json -show_format \"{videoPath}\"");
            FormatModel.Format format = JsonConvert.DeserializeObject<FormatModel.Root>(jsonFormat)
                                                   .format;
            return format;
        }

        private TimeSpan GetAudioDuration(ProbeModel.Stream audio)
        {
            double duration = audio.duration;
            TimeSpan audioDuration = TimeSpan.FromSeconds(duration);
            audioDuration = audioDuration.Subtract(TimeSpan.FromMilliseconds(audioDuration.Milliseconds));
            return audioDuration;
        }

        private int GetGcd(int width, int height)
        {
            while(width != 0 &&
                  height != 0)
                if(width > height)
                    width -= height;
                else
                    height -= width;
            return width == 0 ? height : width;
        }


        private async Task<string> RunProcess(string args)
        {
            return await Task.Run(() =>
            {
                RunProcess(args, FFprobePath, rStandardOutput: true);

                string output;

                try
                {
                    output = Process.StandardOutput.ReadToEnd();
                }
                catch(Exception)
                {
                    output = "";
                }
                finally
                {
                    Process.WaitForExit();
                    Process.Close();
                }

                return output;
            });
        }

        /// <summary>
        /// Get proporties prom media file
        /// </summary>
        /// <param name="fileInfo">Media file info</param>
        /// <param name="mediaInfo">Empty media info</param>
        /// <returns>Properties</returns>
        public async Task<MediaInfo> GetProperties(FileInfo fileInfo, MediaInfo mediaInfo)
        {
            ProbeModel.Stream[] streams = await GetStream(fileInfo.FullName);
            if(!streams.Any())
            {
                throw new ArgumentException($"Invalid file. Cannot load file {fileInfo.Name}");
            }

            FormatModel.Format format = await GetFormat(fileInfo.FullName);
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

        private IEnumerable<IAudioStream> PrepareAudioStreams(FileInfo fileInfo, IEnumerable<ProbeModel.Stream> audioStreamModels)
        {
            foreach (ProbeModel.Stream model in audioStreamModels)
            {
                var stream = new AudioStream
                {
                    Format = model.codec_name,
                    Duration = GetAudioDuration(model),
                    Source = fileInfo,
                    Index = model.index
                };
                yield return stream;
            }
        }

        private static IEnumerable<ISubtitleStream> PrepareSubtitleStreams(FileInfo fileInfo, IEnumerable<ProbeModel.Stream> audioStreamModels)
        {
            foreach (ProbeModel.Stream model in audioStreamModels)
            {
                var stream = new SubtitleStream
                {
                    Format = model.codec_name,
                    Source = fileInfo,
                    Index = model.index,
                    Language = model.tags?.language
                };
                yield return stream;
            }
        }

        private IEnumerable<IVideoStream> PrepareVideoStreams(FileInfo fileInfo, IEnumerable<ProbeModel.Stream> videoStreamModels, FormatModel.Format format)
        {
            foreach(ProbeModel.Stream model in videoStreamModels)
            {
                var stream = new VideoStream
                {
                    Format = model.codec_name,
                    Duration = GetVideoDuration(format, model),
                    Width = model.width,
                    Height = model.height,
                    FrameRate = GetVideoFramerate(model),
                    Ratio = GetVideoAspectRatio(model.width, model.height),
                    Source = fileInfo,
                    Index = model.index
                };
                yield return stream;
            }
        }
    }
}
