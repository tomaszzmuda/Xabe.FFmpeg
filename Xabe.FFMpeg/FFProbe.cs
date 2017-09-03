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
        /// <summary>
        ///     Retrieve details from video file
        /// </summary>
        /// <param name="info">Source video file.</param>
        /// <returns>VideoInfo object with details</returns>
        public void ProbeDetails(VideoInfo info)
        {
            string jsonOutput =
                RunProcess($"-v quiet -print_format json -show_streams \"{info.FilePath}\"");

            var probe =
                JsonConvert.DeserializeObject<ProbeModel>(jsonOutput, new JsonSerializerSettings());

            ProbeModel.Stream videoStream = probe.streams.FirstOrDefault(x => x.codec_type == "video");
            ProbeModel.Stream audioStream = probe.streams.FirstOrDefault(x => x.codec_type == "audio");


            if (videoStream != null)
            {
                info.VideoFormat = videoStream.codec_name;
                info.VideoDuration = GetVideoDuration(info, videoStream);
                info.VideoSize = GetVideoSize(videoStream, info.VideoDuration);
                info.Width = videoStream.width;
                info.Height = videoStream.height;
                info.FrameRate = GetVideoFramerate(videoStream);
                info.Ratio = GetVideoAspectRatio(info);
            }
            if(audioStream != null)
            {
                info.AudioFormat = audioStream.codec_name;
                info.AudioSize = GetAudioSize(audioStream);
                info.AudioDuration = GetAudioDuration(audioStream);
            }


            info.Size = Math.Round(info.AudioSize + info.VideoSize, 2);
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

        private double GetAudioSize(ProbeModel.Stream aud)
        {
            return aud.bit_rate * aud.duration / 8388608;
        }

        private double GetVideoSize(ProbeModel.Stream vid, TimeSpan duration)
        {
            return vid.bit_rate * duration.TotalSeconds / 8388608;
        }

        private TimeSpan GetVideoDuration(VideoInfo info, ProbeModel.Stream video)
        {
            if(info.Extension == ".mkv" ||
               info.Extension == ".webm")
            {
                string jsonOutput =
                    RunProcess($"-v quiet -print_format json -show_format \"{info.FilePath}\"");
                FormatModel.Format format = JsonConvert.DeserializeObject<FormatModel.Root>(jsonOutput)
                                                       .format;

                video.duration = format.duration;
                video.bit_rate = format.bitRate;
            }

            double duration = video.duration;
            TimeSpan videoDuration = TimeSpan.FromSeconds(duration);
            videoDuration = videoDuration.Subtract(TimeSpan.FromMilliseconds(videoDuration.Milliseconds));

            return videoDuration;
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
