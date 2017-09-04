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

It is possible to have more elastic way to convert media. **Conversion** class is builder for FFMpeg command. You can specify all implemented FFMpeg options and run process:
	
	bool conversionResult = new Conversion()
		.SetInput(SampleMkvVideo)
		.SetOutput(outputPath)
		.SetCodec(VideoCodec.MpegTs)
		.Start();

## Features ##
* [Getting an information about video](https://github.com/tomaszzmuda/Xabe.FFMpeg/wiki/Getting-an-information-about-video)
* [Extracting audio or video](https://github.com/tomaszzmuda/Xabe.FFMpeg/wiki/Extracting-audio-or-video)
* Convert media
* Create snapshot
* Add audio to video
* Concatenate multiple videos
* Reverse
* Change speed
* Rotate video
* Pass parameters directly to FFMpeg

## Planned features ##
* Split 
* Watermarks
* Chroma key
* Validate ffmpeg parameters
* Make all time consumption operations async
* Make own more precise exceptions
* Event on ffmpeg output

## Lincence ## 

Xabe.FFMpeg is licensed under MIT - see [License](LICENSE.md) for details.
