# Xabe.FFMpeg  [![Build Status](https://travis-ci.org/tomaszzmuda/Xabe.FFMpeg.svg?branch=master)](https://travis-ci.org/tomaszzmuda/Xabe.FFMpeg)

.NET Core wrapper for FFMpeg.
Require installed ffmpeg and added in your PATH variable.

## Using ##

Install the [Xabe.FFMpeg NuGet package](https://www.nuget.org/packages/Xabe.FFMpeg "") via nuget:

	PM> Install-Package Xabe.FFMpeg
	
Creating video info:

	var videoInfo = new VideoInfo("videofile.mp");
	videoInfo.ConvertTo(VideoType.Ts, new FileInfo("output.ts"));
	
This will video info object contains information about video. 
You can use wrapper for FFMpeg too:

	var ffmpeg = new FFMpeg();
	ffmpeg.ToTs(new VideoInfo("video.mp4"), new FileInfo("output.ts"));

Last parameter is optional and defines if lock should be automatically refreshing before expired.

If file already has lock file, and it time haven't expired, method returns null.

## Features ##

	* Convert media (.ts, .mp4, .ogv, .webm)
	* Create snapshot
	* Add audio to vidoe
	* Extract audio
	* Extract video
	* Concatenate multiple videos
	* Get information about video by FFProbe
	
## Lincence ## 

Xabe.FFMpeg is licensed under MIT - see [License](License.md) for details.
