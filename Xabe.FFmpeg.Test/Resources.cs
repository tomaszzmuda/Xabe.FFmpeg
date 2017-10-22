using System;
using System.IO;

namespace Xabe.FFmpeg.Test
{
    internal static class Resources
    {
        internal static readonly FileInfo PngSample = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Resources", "watermark.png"));
        internal static readonly FileInfo Mp4WithAudio = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Resources", "input.mp4"));
        internal static readonly FileInfo Mp3 = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Resources", "audio.mp3"));
        internal static readonly FileInfo Mp4 = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Resources", "mute.mp4"));
        internal static readonly FileInfo MkvWithAudio = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Resources", "SampleVideo_360x240_1mb.mkv"));
        internal static readonly FileInfo TsWithAudio = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Resources", "sample.ts"));
        internal static readonly FileInfo FlvWithAudio = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Resources", "sample.flv"));
    }
}
