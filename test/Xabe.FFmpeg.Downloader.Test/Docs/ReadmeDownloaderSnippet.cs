using System;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace Xabe.FFmpeg.Downloader.Test.Docs
{
    /// <summary>
    /// Compilation guard for the downloader snippet shown in README.md. Keeping the exact
    /// statements here means the example is checked against the current API on every build.
    /// </summary>
    public static class ReadmeDownloaderSnippet
    {
        internal static async Task DownloadIntoDirectory()
        {
            string executableDirectory = "./ffmpeg";

            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, executableDirectory);
            FFmpeg.SetExecutablesPath(executableDirectory);
        }
    }
}
