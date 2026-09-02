# Xabe.FFmpeg  
[![Build](https://github.com/tomaszzmuda/Xabe.FFmpeg/actions/workflows/ci.yml/badge.svg)](https://github.com/tomaszzmuda/Xabe.FFmpeg/actions/workflows/ci.yml)
[![GitHub release](https://img.shields.io/github/v/release/tomaszzmuda/Xabe.FFmpeg)](https://github.com/tomaszzmuda/Xabe.FFmpeg/releases)
[![NuGet version](https://badge.fury.io/nu/Xabe.FFmpeg.svg)](https://badge.fury.io/nu/Xabe.FFmpeg)
[![GitHub stars](https://img.shields.io/github/stars/tomaszzmuda/Xabe.FFmpeg.svg)](https://github.com/tomaszzmuda/Xabe.FFmpeg/stargazers)
[![Join the chat at https://gitter.im/Xabe-FFmpeg/Lobby](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Xabe-FFmpeg/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)


.NET Standard wrapper for FFmpeg. It allows to process media without know how FFmpeg works, and can be used to pass customized arguments to FFmpeg from dotnet core application.

![Xabe.FFmpeg basic workflow](https://raw.githubusercontent.com/tomaszzmuda/Xabe.FFmpeg/master/Assets/Infographic.png)

[Xabe.FFmpeg Documentation](https://ffmpeg.xabe.net/docs.html)

## Testing ##

- Run all tests (including the RTSP integration tests) with `dotnet test test/Xabe.FFmpeg.Test`.
- The tests need `ffmpeg` and `ffprobe` on `PATH`.
- The RTSP integration tests start a pinned `aler9/rtsp-simple-server` container through [Testcontainers](https://dotnet.testcontainers.org), so they additionally require a running Docker daemon on an x64 machine. No fixed ports or pre-created containers are needed: each test run picks a random free host port.
- If Docker is unavailable, skip only that group with `dotnet test test/Xabe.FFmpeg.Test --filter "FullyQualifiedName!~Rtsp"`.

## License ##

Xabe.FFmpeg is licensed under [Attribution-NonCommercial-ShareAlike 3.0 Unported (CC BY-NC-SA 3.0)](https://creativecommons.org/licenses/by-nc-sa/3.0/) for non commercial use. If you want use Xabe.FFmpeg in commercial project visit our website - [Xabe.FFmpeg](https://ffmpeg.xabe.net/license.html)

## Contact ##

Feel free to ask any questions on [Gitter](https://gitter.im/Xabe-FFmpeg/Lobby# "Gitter") or write e-mail  to **support@xabe.net**

You can check our website too - [Xabe.net](https://ffmpeg.xabe.net/)
