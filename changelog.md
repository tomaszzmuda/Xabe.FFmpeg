Change Log / Release Notes
==========================

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