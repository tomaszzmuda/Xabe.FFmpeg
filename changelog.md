Change Log / Release Notes
==========================

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