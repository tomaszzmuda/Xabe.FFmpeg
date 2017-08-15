# Xabe.FFMpeg  [![Build Status](https://travis-ci.org/tomaszzmuda/Xabe.FFMpeg.svg?branch=master)](https://travis-ci.org/tomaszzmuda/Xabe.FFMpeg)

.NET Core wrapper for FFMpeg.
Require installed ffmpeg and added in your PATH variable.

## Using ##

Install the [Xabe.FFMpeg NuGet package](https://www.nuget.org/packages/Xabe.FFMpeg "") via nuget:

	PM> Install-Package Xabe.FFMpeg
	
Creating video info:

	string output = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");
	var videoInfo = new VideoInfo("videofile.mp");
	VideoInfo outputVideo = videoInfo.ToMp4(output);
	
Video info contains information about video like 
You can use wrapper for FFMpeg too: duration, audio format, video format, radio, frame rate, height, width, size.

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
