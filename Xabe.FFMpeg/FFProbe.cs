using System;
using Newtonsoft.Json;
using Xabe.FFMpeg.Model;

namespace Xabe.FFMpeg
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    ///     Get info about media file
    /// </summary>
    public sealed class FFProbe: FFBase
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
                RunProcess($"-v quiet -print_format json -show_streams \"{info.FullName}\"");

            var probe =
                JsonConvert.DeserializeObject<ProbeModel>(jsonOutput, new JsonSerializerSettings());
            int vid = probe.streams[0].codec_type == "video" ? 0 : 1, aud = 1 - vid;

            double duration = GetDuration(info, probe, vid);
            double videoSize = GetSize(info, probe, vid, duration);
            double audioSize = GetAudioSize(info, probe, aud);

            if(probe.streams.Length > vid)
            {
                info.Width = probe.streams[vid].width;
                info.Height = probe.streams[vid].height;
                info.FrameRate = GetVideoFramerate(probe, vid);
                info.Ratio = GetVideoAspectRatio(info);
            }
            info.Size = Math.Round(videoSize + audioSize, 2);
        }

        private double GetVideoFramerate(ProbeModel probe, int vid)
        {
            string[] fr = probe.streams[vid].r_frame_rate.Split('/');
            return Math.Round(double.Parse(fr[0]) / double.Parse(fr[1]), 3);
        }

        private string GetVideoAspectRatio(VideoInfo info)
        {
            int cd = GetGcd(info.Width, info.Height);
            return info.Width / cd + ":" + info.Height / cd;
        }

        private double GetAudioSize(VideoInfo info, ProbeModel probe, int aud)
        {
            if(probe.streams.Length <= aud)
            {
                info.AudioFormat = "none";
                return 0;
            }

            info.AudioFormat = probe.streams[aud].codec_name;
            return probe.streams[aud].bit_rate * probe.streams[aud].duration / 8388608;
        }

        private double GetSize(VideoInfo info, ProbeModel probe, int vid, double duration)
        {
            if(probe.streams.Length <= vid)
            {
                info.VideoFormat = "none";
                return 0;
            }

            info.VideoFormat = probe.streams[vid].codec_name;
            return probe.streams[vid].bit_rate * duration / 8388608;
        }

        private double GetDuration(VideoInfo info, ProbeModel probe, int vid)
        {
            if(info.Extension == ".mkv")
            {
                string jsonOutput =
                    RunProcess($"-v quiet -print_format json -show_format \"{info.FullName}\"");
                FormatModel.Format format = JsonConvert.DeserializeObject<FormatModel.Root>(jsonOutput)
                                                       .format;

                probe.streams[vid].duration = format.duration;
                probe.streams[vid].bit_rate = format.bitRate;
            }

            double duration = probe.streams[vid].duration;
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
