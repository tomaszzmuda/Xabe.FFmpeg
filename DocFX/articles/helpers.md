Conversion helpers
==================

Xabe.FFmpeg.Conversion.Helpers is a part of Conversion class with conversion snippets. Most simple conversions will be done with helpers or little modification. That was designed to simplify typical conversions and teach how to create own solutions.

Xabe.FFmpeg.Conversion.Helpers is a good point to start using Xabe.FFmpeg library. Every method may be used as template for a more complicated conversions. If you think that your new conversion method is really useful, do not worry about include them to Xabe.FFmpeg.Conversion.Helpers by pull request.

Every method is only a snippet and uses only IConversion with specific configuration. Let's see the source to know how chosen conversion works.

Extracting audio
----------------

The simplest way to extract audio from media file is by Conversion.Helpers:

    string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp3);
    IConversionResult result = await Conversion.ExtractAudio(Resources.Mp4WithAudio, output)
    .Start();

Extracting video
----------------

The simplest way to extract video from media file is by Conversion.Helpers:

    string output = Path.ChangeExtension(Path.GetTempFileName(), Path.GetExtension(Resources.Mp4WithAudio));
    IConversionResult result = await Conversion.ExtractVideo(Resources.Mp4WithAudio, output)
    .Start();

Reversing media
---------------

Reverse is possible by operating on streams using Reverse() method:

    IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio);
    string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
    
    IConversionResult conversionResult = await Conversion.New()
    .AddStream(inputFile.VideoStreams.First()
    .SetCodec(VideoCodec.h164)
    .Reverse())
    .SetOutput(outputPath)
    .Start();

In given example output video file will have only one stream - reversed first video stream from source file.

Use Reverse() methods is possible on IAudioStream and IVideoStream.

Adding audio
------------

The simplest way to add audio to video file is by Conversion.Helpers:

    string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
    IConversionResult result = await Conversion.AddAudio(Resources.Mp4, Resources.Mp3, output)
    .Start();

Changing speed
--------------

IVideoStream and IAudioStream allow to change media speed:

    IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio);
    string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
    
    IConversionResult conversionResult = await Conversion.New()
    .AddStream(inputFile.VideoStreams.First().SetCodec(VideoCodec.h164)
    .ChangeSpeed(1.5))
    .SetOutput(outputPath)
    .Start();

ChangeSpeed() method accepting 1 argument - multiplayer. Multiplayer has to be between 0.5 and 2.0. If you want to speed up streams, use values greater than 1, if not, less than 1.

Changing size
-------------

The simplest way to change video size is by Conversion.Helpers:

    string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mkv);
    string input = Resources.MkvWithAudio;
    
    IConversionResult result = await Conversion.ChangeSize(input, output, new VideoSize(640, 360))
    .Start();

Changing video format
---------------------

Conversion.Helpers contains few predefined methods to change video format e.g.:

    await Conversion.ToOgv(inputVideoPath, outputPathOgv).Start();
    await Conversion.ToTs(inputVideoPath, outputPathTs).Start();
    await Conversion.ToWebM(inputVideoPath, outputPathWebm).Start();

More conversion types are possible by Conversion:

    string inputVideoPath = Path.Combine("C:", "Temp", "input.mkv");
    string outputPathMp4 = Path.Combine("C:", "Temp", "result.mp4");
    
    IMediaInfo info = AsyncHelper.RunSync(() => MediaInfo.Get(inputVideoPath));
    
    IStream videoStream = info.VideoStreams.FirstOrDefault()
    ?.SetCodec(VideoCodec.h164);
    IStream audioStream = info.AudioStreams.FirstOrDefault()
    ?.SetCodec(AudioCodec.Aac);
    
    return New()
    .AddStream(videoStream, audioStream)
    .SetOutput(outputPathMp4);

Concatenate videos
------------------

The simplest way to concatenate video files is by Conversion.Helpers:

    string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
    IConversionResult result = await Conversion.Concatenate(output,  Resources.MkvWithAudio, Resources.Mp4WithAudio);

Files list is params so it is possible to concatenate more than two files.

Concatenate is a complicated operation so look at helper implementation to understand how it works.

Split
-----

IVideoStream and IAudioStream allows split media, but the fastest way is by Conversion.Helpers:

    string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
    IConversionResult result = await Conversion.Split(Resources.Mp4WithAudio, output, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(8))
    .Start();

Helper splits all streams in media file and copies them (splitted) to output. In example on output will be media file with duration of 6 seconds contains both streams: audio and video.

Watermarks
----------

Example of use watermarks:

    string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
    IConversionResult result = await Conversion.SetWatermark(Resources.Mp4WithAudio, output, Resources.PngSample, Position.Center)
    .Start();

Watermark can be set in different position in a video:

*   Position.UpperRight
*   Position.BottomRight
*   Position.Right
*   Position.BottomLeft
*   Position.UpperLeft
*   Position.Left
*   Position.Center
*   Position.Bottom
*   Position.Up

Snapshot
--------

The simplest way to get snapshot is by Conversion.Helpers:

    string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Png);
    IConversionResult result = await Conversion.Snapshot(Resources.Mp4WithAudio, output, TimeSpan.FromSeconds(0))
    .Start();

Conversion always returns snapshot in png file format so outputPath should be with correct extension. Image has exactly the same size as a video.

Gifs
----

FFmpeg allows to create gif file from video. Number of loops (one to infinity) and delay between repeats can be specified in parameters. The easiest way to get gif from video is to use Conversion.Helpers.ToGif() method:

    string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Gif);
    
    IConversionResult result = await Conversion.ToGif(Resources.Mp4, output, 1, 1)
    .Start();





