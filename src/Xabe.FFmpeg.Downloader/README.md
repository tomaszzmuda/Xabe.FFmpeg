# Xabe.FFmpeg.Downloader

Downloads the **latest** FFmpeg and FFprobe binaries for the current operating system and architecture, so a .NET application can obtain a working media toolkit at runtime instead of requiring a pre-installed FFmpeg. Requires **[Xabe.FFmpeg](https://www.nuget.org/packages/Xabe.FFmpeg)** (pulled in automatically).

## What it downloads

| Provider (`FFmpegVersion`) | Source | Supported current platforms |
| --- | --- | --- |
| `Official` (default recommendation) | ffbinaries.com static builds | Windows 32/64, macOS 64, Linux 32/64/ARMHF/ARMEL/ARM64 |
| `Full` | static builds hosted at xabe.net | Windows 32/64, macOS 64 |
| `Shared` | shared (dynamic) builds hosted at xabe.net | Windows 32/64, macOS 64 |
| `Android` | Mobile-FFmpeg builds hosted at xabe.net | Android ARM/ARM64/X86/X64 |

## Installation

PowerShell (Package Manager Console):

    PM> Install-Package Xabe.FFmpeg.Downloader

dotnet CLI:

    dotnet add package Xabe.FFmpeg.Downloader

## Getting started

    using Xabe.FFmpeg;
    using Xabe.FFmpeg.Downloader;

    string ffmpegDir = @"C:\ffmpeg-binaries";                // any writable directory
    await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, ffmpegDir);

    // Required: tell Xabe.FFmpeg where the executables were downloaded.
    FFmpeg.SetExecutablesPath(ffmpegDir);

`GetLatestVersion` skips the download when `ffmpeg`/`ffprobe` (and, for the `Official` provider, the recorded `version.json`) already exist and are up to date. Filenames are matched with the `Contains` filter by default; `FFmpeg.SetExecutablesPath(...)` offers overload parameters to customise executable names and matching.

## Limitations

- Downloads the **latest version published by the chosen provider**; selecting a specific FFmpeg version is not supported by the current API.
- `Full` and `Shared` are only available for Windows and macOS (throwing `NotSupportedException` elsewhere); `Android` targets Android builds.
- Failed downloads are retried with exponential backoff; there is no resume support.
- The downloaded `ffmpeg`/`ffprobe` binaries are **licensed independently** (GPL/LGPL depending on the build) and are not covered by this package's Xabe License.

## Licensing

Xabe.FFmpeg.Downloader (this library) is licensed under the **Xabe License**: CC BY-NC-SA 3.0 for non-commercial projects, and the full Xabe License Agreement for commercial use — see `Xabe-License.txt` inside this package and <https://ffmpeg.xabe.net/license.html>.

## Links

- Core package: [Xabe.FFmpeg](https://www.nuget.org/packages/Xabe.FFmpeg)
- Repository and contributor guide: <https://github.com/tomaszzmuda/Xabe.FFmpeg>
- Canonical documentation: <https://ffmpeg.xabe.net/docs.html>
