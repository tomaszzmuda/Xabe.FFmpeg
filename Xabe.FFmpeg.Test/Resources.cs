using System;
using System.IO;

namespace Xabe.FFmpeg.Test
{
    internal static class Resources
    {
        internal static readonly string PngSample = Path.Combine(Environment.CurrentDirectory, "Resources", "watermark.png");
        internal static readonly string Mp4WithAudio = Path.Combine(Environment.CurrentDirectory, "Resources", "input.mp4");
        internal static readonly string Mp3 = Path.Combine(Environment.CurrentDirectory, "Resources", "audio.mp3");
        internal static readonly string Mp4 = Path.Combine(Environment.CurrentDirectory, "Resources", "mute.mp4");
        internal static readonly string MkvWithAudio = Path.Combine(Environment.CurrentDirectory, "Resources", "SampleVideo_360x240_1mb.mkv");
        internal static readonly string TsWithAudio = Path.Combine(Environment.CurrentDirectory, "Resources", "sample.ts");
        internal static readonly string FlvWithAudio = Path.Combine(Environment.CurrentDirectory, "Resources", "sample.flv");
        internal static readonly string Dll = Path.Combine(Environment.CurrentDirectory, "Xabe.FFmpeg.Test.dll");

        internal static readonly string SubtitleSrt = Path.Combine(Environment.CurrentDirectory, "Resources", "sampleSrt.srt"); 
    }
}
