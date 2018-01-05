using System;
using System.Threading.Tasks;

namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Derives preconfigurated Configuration objects
    /// </summary>
    public static class ConversionHelper
    {
        /// <summary>
        ///     Concat multiple inputVideos.
        /// </summary>
        /// <param name="output">Concatenated inputVideos</param>
        /// <param name="inputVideos">Videos to add</param>
        /// <returns>Conversion result</returns>
        public static async Task<bool> Concatenate(string output, params string[] inputVideos)
        {
            //if(inputVideos.Length <= 1)
            //    throw new ArgumentException("You must provide at least 2 files for the concatenation to work", "inputVideos");

            //var mediaInfos = new List<IMediaInfo>();

            //var conversion = Conversion.New();
            //foreach (string inputVideo in inputVideos)
            //{
            //    mediaInfos.Add(await MediaInfo.Get(inputVideo));
            //    conversion.AddParameter($"-i \"{inputVideo}\" ");
            //}
            //conversion.AddParameter($"-t 1 -f lavfi -i anullsrc=r=48000:cl=stereo");
            //conversion.AddParameter($"-filter_complex \"");

            //MediaProperties maxResolutionMedia = mediaInfos.OrderByDescending(x => x.Width)
            //                                         .First().Properties;
            //for(var i = 0; i < mediaInfos.Count; i++)
            //{
            //        conversion.AddParameter(
            //            $"[{i}:v]scale={maxResolutionMedia.Width}:{maxResolutionMedia.Height},setdar=dar={maxResolutionMedia.Ratio},setpts=PTS-STARTPTS[v{i}]; ");
            //}

            //for(var i = 0; i < mediaInfos.Count; i++)
            //{
            //    if(string.IsNullOrEmpty(mediaInfos[i].AudioFormat))
            //        conversion.AddParameter($"[v{i}][{mediaInfos.Count}:a]");
            //    else
            //        conversion.AddParameter($"[v{i}][{i}:a]");
            //}

            //conversion.AddParameter($"concat=n={inputVideos.Length}:v=1:a=1 [v] [a]\" -map \"[v]\" -map \"[a]\"");
            //conversion.SetOutput(output);
            //return await conversion.Start();
            //todo: implementantion

            throw new NotImplementedException();
        }
    }
}
