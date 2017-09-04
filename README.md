# Xabe.FFMpeg  [![Build Status](https://travis-ci.org/tomaszzmuda/Xabe.FFMpeg.svg?branch=master)](https://travis-ci.org/tomaszzmuda/Xabe.FFMpeg)

.NET Core wrapper for FFMpeg.
Require installed ffmpeg and added in your PATH variable.

## Using ##

You have to have installed FFMpeg and added it to your PATH variable or specify directory where FFMpeg executables are before run conversion in variable FFBase.FFMpegDir.

Install the [Xabe.FFMpeg NuGet package](https://www.nuget.org/packages/Xabe.FFMpeg "") via nuget:

	PM> Install-Package Xabe.FFMpeg
	
Creating video info:

	IVideoInfo videoInfo = new VideoInfo("videofile.mkv");
	
Video info contains information about video like: duration, audio format, video format, radio, frame rate, height, width, size in VideoProperties property.

It is possible to have more elastic way to convert media. **Conversion** class is builder for FFMpeg command. You can specify all implemented FFMpeg options and run process:
	
	string output = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");
	bool result = await ConversionHelper.ToMp4("videofile.mkv", output)
                                    .Start();

or

	string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
	bool conversionResult = await new Conversion()
		.SetInput("videofile.mkv")
		.Rotate(rotateDegrees)
		.SetOutput(outputPath)
		.Start();

it is possible to give your own arguments

	bool conversionResult = await new Conversion().Start(-i "C:\Xabe.FFMpeg.Test\bin\Debug\netcoreapp2.0\Resources\SampleVideo_360x240_1mb.mkv" "C:\Users\tzmuda\AppData\Local\Temp\tmp9B8A.mp4");


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

## Lincence ## 

Xabe.FFMpeg is licensed under MIT - see [License](LICENSE.md) for details.
