# Xabe.FFmpeg

Fluent .NET Standard wrapper that drives the `ffmpeg` and `ffprobe` command-line tools for you: describe a conversion in code and Xabe.FFmpeg assembles and runs the correct FFmpeg process, reporting progress and results through events and results objects.

This package is **Xabe.FFmpeg** (the core wrapper). To also obtain the FFmpeg/FFprobe binaries automatically, add **[Xabe.FFmpeg.Downloader](https://www.nuget.org/packages/Xabe.FFmpeg.Downloader)**.

## Requirements

Xabe.FFmpeg does **not** bundle FFmpeg or FFprobe. The `ffmpeg` and `ffprobe` executables must be available on the `PATH`, or placed in a directory you register with `FFmpeg.SetExecutablesPath(...)` (see *Getting started*).

## Installation

PowerShell (Package Manager Console):

    PM> Install-Package Xabe.FFmpeg

dotnet CLI:

    dotnet add package Xabe.FFmpeg

## Getting started

If FFmpeg lives in a dedicated directory (for example one prepared by Xabe.FFmpeg.Downloader), point Xabe.FFmpeg at it once, before first use:

    FFmpeg.SetExecutablesPath(@"C:\ffmpeg-binaries");

Probe a media file:

    using Xabe.FFmpeg;

    IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(@"movie.mkv");
    Console.WriteLine(mediaInfo.Duration);

Run a conversion (MKV to MP4):

    IConversion conversion = await FFmpeg.Conversions.FromSnippet.ToMp4(@"movie.mkv", @"movie.mp4");
    IConversionResult result = await conversion.Start(CancellationToken.None);

More operations are available as ready-made snippets (`ExtractAudio`, `Snapshot`, `ChangeSize`, `BurnSubtitle`, ...) via `FFmpeg.Conversions.FromSnippet`, or as fully fluent pipelines via `FFmpeg.Conversions.New()`.

## Primary entry points

| Member | Purpose |
| --- | --- |
| `FFmpeg.Conversions.New()` | Fluent builder for a custom conversion |
| `FFmpeg.Conversions.FromSnippet.*` | Ready-made conversions (ToMp4, ExtractAudio, Snapshot, ...) |
| `FFmpeg.GetMediaInfo(path)` | Probe a file with FFprobe |
| `FFmpeg.SetExecutablesPath(dir, ...)` | Register the directory holding `ffmpeg`/`ffprobe` |

## Compatibility and limitations

- Targets **.NET Standard 2.0**; works from any .NET (Core) 2.0+ or .NET Framework 4.6.1+ application.
- Xabe.FFmpeg shells out to the installed `ffmpeg`/`ffprobe` processes. Features, encoders and behaviour follow the version of FFmpeg you provide — the wrapper does not bundle or pin it.
- Conversions run asynchronously and report progress through `IConversion` events; long-running jobs should be cancellable through the token accepted by `Start`.

## Licensing

Xabe.FFmpeg (this library) is licensed under the **Xabe License**: CC BY-NC-SA 3.0 for non-commercial projects, and the full Xabe License Agreement for commercial use — see `LICENSE.md` inside this package and <https://ffmpeg.xabe.net/license.html>.

The `ffmpeg`/`ffprobe` executables you provide are **licensed separately** (GPL/LGPL depending on the build) and remain subject to their own terms, which are independent of this package's license.

## Links

- Repository and contributor guide: <https://github.com/tomaszzmuda/Xabe.FFmpeg>
- Canonical documentation: <https://ffmpeg.xabe.net/docs.html>
