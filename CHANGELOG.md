# Changelog
## 6.0.0 - 10.02.2025
- [Use internal System.Text.Json > 9.0.0](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/485)
- [Eol Repair](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/467)
- [Fix for Issue #456](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/459)

## 5.2.6 - 14.03.2023
- [Default to Win32 and Win64 for Arm and Arm64 Respectively on Windows Systems](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/450)

## 5.2.5 - 09.02.2022
- [Fix of date time offset that was using the local time zone of the computer](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/443)

## 5.2.4 - 25.01.2023
- [Remove get awaiter everywhere](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/441)

## 5.2.3 - 22.12.2022
- [Solve problems with chineese dates](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/439)
- [Remove obsolete tasks](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/438)

## 5.2.2 - 20.12.2022
- [Rethrow downloader exception instead of silently continue](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/436)

## 5.2.1 - 11.12.2022
- [Fix exception when only file name was given as a path](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/433)

## 5.2.0 - 05.04.2022
- [Add creation time to MediaInfo](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/411)
- [Properly calculate TotalTime during conversion](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/400)
- [Fix problem with wrong timespan](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/405/commits)
- [Add Title to AudioStream](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/406)
- [Introduce editor.config and yml pipeline](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/404)

## 5.1.0 - 17.01.2022
- [Support for piping video output from FFmpeg](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/365)
- [Correctly dispose cancellation token source in MediaInfo](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/367)
- [Adjust SendDesktopToRtspServer method comment](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/367)
- [Make tests more stable](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/367)

## 5.0.2 - 2021-05-28
- [Fix bug with unhandled InvalidOperationException](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/362/)
- [Fix bug when user cannot pass duplicated parameters in AddParameter method](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/363)

## 5.0.1 - 2021-05-22
- [Fix bug when conversion duration cannot be found](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/359)

## [5.0.0](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/339) - 2021-05-15
- A lot of code improvements and simplifications
- Automated tests are using docker now to set up RTSP server
- Change GetScreenCapture to AddDesktopStream
- Added more tests related to RTSP
- Remove obsolete methods
- Add SetInputFormat ("-f"), SetStreamLoop ("-stream_loop"), and UseNativeInputRead ("-re" flag) to IAudioStream
- Add SetInputFormat ("-f"), SetStreamLoop ("-stream_loop"), and UseNativeInputRead ("-re" flag) to IVideoStream
- Add SetStreamLoop ("-stream_loop"), and UseNativeInputRead ("-re" flag) to ISubtitleStream
- UseMultiThread(true) is using max 16 threads due to compatibility reasons
- Fix bug with RTSP streams when FFmpeg stucks

## 4.4.1 - 2021-05-11

- [Upgrade test project .NET Core version to 3.1](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/353)
- Handle special characters in paths [PullRequest#350](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/350) [PullRequest#351](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/351)
- [Create directory for output file if it not exists](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/351/files)
- [Extend GetScreenCapture by offset and size](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/325)
- [Tests clean up their temp files from now](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/326)
- [License moved to official Xabe.FFmpeg site](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/342/files)
- [Change few missleading comments](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/348/files)
- [Added a few more tests](https://github.com/tomaszzmuda/Xabe.FFmpeg/pull/349/files)

## 4.4.0 - 2020-12-09