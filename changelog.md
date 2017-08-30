Change Log / Release Notes
==========================

## Xabe.FFMpeg 1.5.1

* Getting input video duration from FFMpeg header
* Add ability to perform dynamic ffmpeg commands
* Rotate video
* Force user to use VideoSize in case to change video size
* More unit tests

## Xabe.FFMpeg 1.5.0

* Code refactor
* Some new unit tests
* Change way of working VideoInfo conversions
* Properly calculate output media file for joining multiple files
* Dispose ffmpeg process now kill process not send stop signal

## Xabe.FFMpeg 1.4.2

* Even more unit tests
* Optimize creating VideoInfo and Conversion object. FFMpeg executables pathes is saved after found
* Better result of conversion state
* Remove Kill method from FFBase but move functionality to Dispose
* Add Resharper annotations and configuration
* Add Conversion.Stop method which send exit signal to FFMpeg process

## Xabe.FFMpeg 1.4.1

* More unit tests
* Fix issue when trying to process media with disabled video channel
* Change obsolete alias "vframes" to "frames:v"
* Change input parameter type to Conversion.Concat method from IEnumerable<string> to params string[]

## Xabe.FFMpeg 1.4.0

* Change name of SetChannels method to StreamCopy and extend documentation
* Fix changing scale of output video
* Add ffmpeg predefined video size
* Refactor VideoSize class