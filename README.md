# Xabe.FFmpeg  
[![Build Status](https://travis-ci.org/tomaszzmuda/Xabe.FFmpeg.svg?branch=master)](https://travis-ci.org/tomaszzmuda/Xabe.FFmpeg)
[![NuGet version](https://badge.fury.io/nu/Xabe.FFmpeg.svg)](https://badge.fury.io/nu/Xabe.FFmpeg)
[![GitHub issues](https://img.shields.io/github/issues/tomaszzmuda/Xabe.FFmpeg.svg)](https://github.com/tomaszzmuda/Xabe.FFmpeg/issues)
[![GitHub stars](https://img.shields.io/github/stars/tomaszzmuda/Xabe.FFmpeg.svg)](https://github.com/tomaszzmuda/Xabe.FFmpeg/stargazers)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/tomaszzmuda/Xabe.FFmpeg/master/LICENSE.md)
[![HitCount](http://hits.dwyl.io/tomaszzmuda/Xabe.FFmpeg.svg)](http://hits.dwyl.io/tomaszzmuda/Xabe.FFmpeg)
[![Join the chat at https://gitter.im/Xabe-FFmpeg/Lobby](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Xabe-FFmpeg/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)


.NET Standard wrapper for FFmpeg. It's allow to process media without know how FFmpeg works, and can be used to pass customized arguments to FFmpeg from dotnet core application.


[List of features and wiki](https://github.com/tomaszzmuda/Xabe.FFmpeg/wiki)

## Using ##

You have to have installed FFmpeg and added it to your PATH variable or specify directory where FFmpeg executables are before run conversion in variable FFbase.FFmpegDir.
Default the library is trying to find FFmpeg executables names contains "ffprobe", "ffmpeg". This function is case insensitive. Those names can be changed in FFbase.FFmpegExecutableName and FFbase.FFprobeExecutableName.

Install the [Xabe.FFmpeg NuGet package](https://www.nuget.org/packages/Xabe.FFmpeg) via nuget:

	PM> Install-Package Xabe.FFmpeg
	
Creating media info:

	IMediaInfo mediaInfo = new MediaInfo("videofile.mkv");
	
Media info contains information about video like: duration, audio format, video format, radio, frame rate, height, width, size in Properties property.

It is possible to have more elastic way to convert media. **Conversion** class is builder for FFmpeg command. You can specify all implemented FFmpeg options and run process:
	
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

	bool conversionResult = await new Conversion().Start("-i \"C:\Xabe.FFmpeg.Test\bin\Debug\netcoreapp2.0\Resources\SampleVideo_360x240_1mb.mkv\" \"C:\Users\tzmuda\AppData\Local\Temp\tmp9B8A.mp4\"");

## Lincence ##

Xabe.FFmpeg is licensed under MIT - see [License](LICENSE.md) for details.

## Contact ##

Feel free to ask any questions on [Gitter](https://gitter.im/Xabe-FFmpeg/Lobby# "Gitter") or write e-mail  to **tomasz.zmuda@xabe.xyz**