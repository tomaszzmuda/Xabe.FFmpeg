using System;
using System.Linq;
using Newtonsoft.Json;
using Xabe.FFMpeg.Model;

namespace Xabe.FFMpeg
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    ///     Get info about media file
    /// </summary>
    internal sealed class FFProbe: FFBase
    {
        /// <summary>
        ///     Retrieve details from video file
        /// </summary>
        /// <param name="source">Source video file.</param>
        public void ProbeDetails(string source)
        {
            ProbeDetails(new VideoInfo(source));
        }

        /// <summary>
        ///     Retrieve details from video file
        /// </summary>
        /// <param name="info">Source video file.</param>
        /// <returns>VideoInfo object with details</returns>
        public void ProbeDetails(VideoInfo info)
        {
            string jsonOutput =
                RunProcess($"-v quiet -print_format json -show_streams \"{info.Path}\"");

            var probe =
                JsonConvert.DeserializeObject<ProbeModel>(jsonOutput, new JsonSerializerSettings());

            ProbeModel.Stream vid = probe.streams.FirstOrDefault(x => x.codec_type == "video");
            ProbeModel.Stream aud = probe.streams.FirstOrDefault(x => x.codec_type == "audio");

            double duration = GetDuration(info, vid);
            double videoSize = GetSize(info, vid, duration);
            double audioSize = GetAudioSize(info, aud);

            if(vid != null)
            {
                info.Width = vid.width;
                info.Height = vid.height;
                info.FrameRate = GetVideoFramerate(vid);
                info.Ratio = GetVideoAspectRatio(info);
            }
            info.Size = Math.Round(videoSize + audioSize, 2);
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

        private double GetAudioSize(VideoInfo info, ProbeModel.Stream aud)
        {
            if(aud == null)
            {
                info.AudioFormat = "none";
                return 0;
            }

            info.AudioFormat = aud.codec_name;
            return aud.bit_rate * aud.duration / 8388608;
        }

        private double GetSize(VideoInfo info, ProbeModel.Stream vid, double duration)
        {
            if(vid == null)
            {
                info.VideoFormat = "none";
                return 0;
            }

            info.VideoFormat = vid.codec_name;
            return vid.bit_rate * duration / 8388608;
        }

        private double GetDuration(VideoInfo info, ProbeModel.Stream video)
        {
            if(info.Extension == ".mkv")
            {
                string jsonOutput =
                    RunProcess($"-v quiet -print_format json -show_format \"{info.Path}\"");
                FormatModel.Format format = JsonConvert.DeserializeObject<FormatModel.Root>(jsonOutput)
                                                       .format;

                video.duration = format.duration;
                video.bit_rate = format.bitRate;
            }

            double duration = video.duration;
            info.Duration = TimeSpan.FromSeconds(duration);
            info.Duration = info.Duration.Subtract(TimeSpan.FromMilliseconds(info.Duration.Milliseconds));

            return duration;
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
