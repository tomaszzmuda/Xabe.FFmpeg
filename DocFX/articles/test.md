### C# FFmpeg wrapper conception

Xabe.FFmpeg uses only streams to operate on every media file. Most common conversions can be done with few simple steps:

1.  Extract streams from input file or create new streams with outside source (e.g. WebStream)
2.  Manipulate with streams with embedded methods
3.  Add selected streams to conversion
4.  Set output
5.  Start conversion

Operating on streams (not on files) allows to work with multi-streams media files and single-stream files with the same way.