using System;
using System.Linq;
using Newtonsoft.Json;
using Xabe.FFMpeg.Model;

namespace Xabe.FFMpeg
{
    /// <summary>
    ///     Get info about media file
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    internal sealed class FFProbe: FFBase
    {
        private ProbeModel.Stream[] GetStream(VideoInfo videoInfo)
        {
            string jsonStreams =
                RunProcess($"-v quiet -print_format json -show_streams \"{videoInfo.FullName}\"");

            var probe =
                JsonConvert.DeserializeObject<ProbeModel>(jsonStreams, new JsonSerializerSettings());

            return new[] {probe.streams.FirstOrDefault(x => x.codec_type == "video") ?? null, probe.streams.FirstOrDefault(x => x.codec_type == "audio") ?? null};
        }

        /// <summary>
        ///     Retrieve details from video file
        /// </summary>
        /// <param name="info">Source video file.</param>
        /// <returns>VideoInfo object with details</returns>
        public void ProbeDetails(VideoInfo info)
        {
            ProbeModel.Stream[] streams = GetStream(info);
            ProbeModel.Stream videoStream = streams[0];
            ProbeModel.Stream audioStream = streams[1];
            FormatModel.Format format = GetFormat(info);
            info.Size = long.Parse(format.size);

            if(videoStream != null)
            {
                info.VideoFormat = videoStream.codec_name;
                info.VideoDuration = GetVideoDuration(format, videoStream);
                info.Width = videoStream.width;
                info.Height = videoStream.height;
                info.FrameRate = GetVideoFramerate(videoStream);
                info.Ratio = GetVideoAspectRatio(info);
            }
            if(audioStream != null)
            {
                info.AudioFormat = audioStream.codec_name;
                info.AudioDuration = GetAudioDuration(audioStream);
            }

            info.Duration = TimeSpan.FromSeconds(Math.Max(info.VideoDuration.TotalSeconds, info.AudioDuration.TotalSeconds));
        }

        private double GetVideoFramerate(ProbeModel.Stream vid)
        {
            string[] fr = vid.r_frame_rate.Split('/');
            return Math.Round(double.Parse(fr[0]) / double.Parse(fr[1]), 3);
        }

        private string GetVideoAspectRatio(VideoInfo info)
        {
            int cd = GetGcd(info.Width, info.Height);
            return info.Width / cd + ":" + info.Height / cd;
        }

        private TimeSpan GetVideoDuration(FormatModel.Format format, ProbeModel.Stream video)
        {
            video.duration = format.duration;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            video.bit_rate = format.bitRate == 0 ? video.bit_rate : format.bitRate;

            double duration = video.duration;
            TimeSpan videoDuration = TimeSpan.FromSeconds(duration);
            videoDuration = videoDuration.Subtract(TimeSpan.FromMilliseconds(videoDuration.Milliseconds));

            return videoDuration;
        }

        private FormatModel.Format GetFormat(VideoInfo info)
        {
            string jsonFormat =
                RunProcess($"-v quiet -print_format json -show_format \"{info.FullName}\"");
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


        private string RunProcess(string args)
        {
            RunProcess(args, FFProbePath, rStandardOutput: true);

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
        }
    }
}
