Getting streams from media file
===============================

Basic properties of media file can be read by MediaInfo class:

    string filePath = Path.Combine("C:", "samples", "SampleVideo.mp4");
    IMediaInfo mediaInfo = await MediaInfo.Get(Resources.Mp3);

IMediaInfo contains basic properties about media:

![Xabe.FFmpeg MediaInfo diagram](https://xabe.net/wp-content/uploads/2018/03/IMediaInfo.png)

More properties can be found in specific stream.