# Xabe.FFMpeg  [![Build Status](https://travis-ci.org/tomaszzmuda/Xabe.FFMpeg.svg?branch=master)](https://travis-ci.org/tomaszzmuda/Xabe.FFMpeg)

.NET Core wrapper for FFMpeg.
Require installed ffmpeg and added in your PATH variable.

## Using ##

You have to have installed FFMpeg and added it to your PATH variable or specify directory where FFMpeg executables are before run conversion in variable FFBase.FFMpegDir.

Install the [Xabe.FFMpeg NuGet package](https://www.nuget.org/packages/Xabe.FFMpeg "") via nuget:

	PM> Install-Package Xabe.FFMpeg
	
Creating video info:

	string output = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");
	IVideoInfo videoInfo = new VideoInfo("videofile.mp");
	IVideoInfo outputVideo = videoInfo.ToMp4(output);
	
Video info contains information about video like: duration, audio format, video format, radio, frame rate, height, width, size.

## Features ##

	* Convert media (.ts, .mp4, .ogv, .webm)
	* Create snapshot
	* Add audio to vidoe
	* Extract audio
	* Extract video
	* Concatenate multiple videos
	* Get information about video by FFProbe
	
## Lincence ## 

Xabe.FFMpeg is licensed under MIT - see [License](LICENSE.md) for details.
