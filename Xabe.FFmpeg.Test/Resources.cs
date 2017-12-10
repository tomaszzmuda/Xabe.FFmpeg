using System.IO;

namespace Xabe.FFmpeg.Test
{
    internal static class Resources
    {
        internal static readonly string PngSample = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "watermark.png");
        internal static readonly string Mp4WithAudio = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "input.mp4");
        internal static readonly string Mp3 = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "audio.mp3");
        internal static readonly string Mp4 = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "mute.mp4");
        internal static readonly string MkvWithAudio = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "SampleVideo_360x240_1mb.mkv");
        internal static readonly string TsWithAudio = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "sample.ts");
        internal static readonly string FlvWithAudio = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "sample.flv");
        internal static readonly string Dll = Path.Combine(Directory.GetCurrentDirectory(), "Xabe.FFmpeg.Test.dll");

        internal static readonly string SubtitleSrt = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "sampleSrt.srt");
    }
}
