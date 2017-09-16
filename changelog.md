Change Log / Release Notes
==========================

## Xabe.FFmpeg 2.0.3

* Remove useless dependencies causing crashes during installing package on .NET Framework target projects

## Xabe.FFmpeg 2.0.2

* More clear exceptions if cannot find FFmpeg executables
* Ability to change name of FFmpeg executables
* Now FFmpeg executables name parser is case insensitive

## Xabe.FFmpeg 2.0.1

* Fix names in solution

## Xabe.FFmpeg 2.0.0

* Time consumptions methods are now asynchronous
* Only one conversion can be in progress at time per Conversion/VideoInfo object
* Lazy load properties for VideoInfo
* Added ConversionHelper class which contains predefined set of parameters for FFmpeg
* VideoInfo now contains only properties, doesn't have any methods to convert media
* Some improvements in unit tests
* Added event on FFmpeg output data received

## Xabe.FFmpeg 1.5.1

* Getting input video duration from FFmpeg header
* Add ability to perform dynamic FFmpeg commands
* Rotate video
* Force user to use VideoSize in case to change video size
* More unit tests

## Xabe.FFmpeg 1.5.0

* Code refactor
* Some new unit tests
* Change way of working VideoInfo conversions
* Properly calculate output media file for joining multiple files
* Dispose FFmpeg process now kill process not send stop signal

## Xabe.FFmpeg 1.4.2

* Even more unit tests
* Optimize creating VideoInfo and Conversion object. FFmpeg executables pathes is saved after found
* Better result of conversion state
* Remove Kill method from FFBase but move functionality to Dispose
* Add Resharper annotations and configuration
* Add Conversion.Stop method which send exit signal to FFmpeg process

## Xabe.FFmpeg 1.4.1

* More unit tests
* Fix issue when trying to process media with disabled video channel
* Change obsolete alias "vframes" to "frames:v"
* Change input parameter type to Conversion.Concat method from IEnumerable<string> to params string[]

## Xabe.FFmpeg 1.4.0

* Change name of SetChannels method to StreamCopy and extend documentation
* Fix changing scale of output video
* Add FFmpeg predefined video size
* Refactor VideoSize class
