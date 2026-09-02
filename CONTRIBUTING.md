# Contributing to Xabe.FFmpeg

Thanks for considering a contribution. This repository wraps the `ffmpeg` and `ffprobe` command-line tools; it does not contain FFmpeg source.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (the version used by CI — currently 10.0.x — or a compatible one).
- `ffmpeg` and `ffprobe` executables reachable on `PATH` (or in your application directory). CI uses a SHA-256-pinned FFmpeg build, installed by the `.github/actions/ffmpeg-static` composite action.

## Building and testing

```console
dotnet build Xabe.FFmpeg.sln -c Release
dotnet test test/Xabe.FFmpeg.Test -c Release
dotnet test test/Xabe.FFmpeg.Downloader.Test -c Release
```

The test suites use xUnit. Some tests (screen capture, hardware acceleration) skip automatically on machines without the required hardware; RTSP tests use a local Docker daemon.

## Guidelines

- **Tests are expected.** The maintainer merges tested changes; please extend an existing test suite or add a focused one alongside the behaviour you change.
- Keep changes focused; do not mix refactors with behaviour changes.
- Follow the existing code style (the repository ships an `.editorconfig` that the build enforces).
- Call out public API changes in your PR description — release notes are generated from the commit log.
- Documentation in `README.md` must stay truthful to the code: the examples there are byte-compared against test sources and executed by the test suite — update them together with any API change they use.
- For anything user-facing, prefer fixing the example over adding a workaround in the wrapper.

## Reporting problems

Bug reports go to [GitHub Issues](https://github.com/tomaszzmuda/Xabe.FFmpeg/issues). Please include the output of `ffmpeg -version`, the platform, and — for conversion failures — the `Arguments` string from `IConversionResult`.

## License note

The wrapper is released under CC BY-NC-SA 3.0 for non-commercial use. Commercial licensing is described on the [license page](https://ffmpeg.xabe.net/license.html). By contributing you agree that your contribution follows the same terms.
