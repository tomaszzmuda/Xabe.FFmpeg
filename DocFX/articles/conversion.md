.NET Video Conversion
=====================

Xabe.FFmpeg.Conversion is the main class to handle FFmpeg conversions. User can manipulate audio, video and subtitle through this class.

Sample below shows basic conversion video file from mkv to mp4 format:

    string output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Mp4);
                IConversionResult result = await Conversion.Convert(Resources.MkvWithAudio, output).Start();

This could be done also by:

    string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
    IMediaInfo mediaInfo = await MediaInfo.Get(Resources.MkvWithAudio);
    
    IStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()
    ?.SetCodec(VideoCodec.h164);
    IStream audioStream = mediaInfo.AudioStreams.FirstOrDefault()
    ?.SetCodec(AudioCodec.Aac);
    
    Conversion.New().AddStream(audioStream, videoStream)
    .SetOutput(outputPath)
    .Start();

Almost all methods in streams return specific stream (IAudioStream, IVideoStream etc.). It allows to create chain of methods. Stream object could be use more than once.

    string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
    IMediaInfo mediaInfo = await MediaInfo.Get(Resources.MkvWithAudio);
    
    IStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()
    ?.SetCodec(VideoCodec.h164)
    ?.Reverse()
    ?.SetSize(VideoSize.Hd480);
    
    Conversion.New().AddStream(videoStream)
    .SetOutput(outputPath)
    .Start();

Method Xabe.FFmpeg.Conversion.Clear() sets IConversion to untouched state. All parameters passed to it are overrided by default values.

IConversion provides events to handle FFmpeg output. OnDataReceived and OnProgress events allow redirect FFmpeg output to user and inform him about progress.

    conversion.OnProgress += (sender, args) =>
    {
    var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
    Debug.WriteLine($"[{args.Duration} / {args.TotalLength}] {percent}%");
    };
    await conversion.Start();

OnDataReceived:

    conversion.OnDataReceived += (sender, args) =>
    {
    Debug.WriteLine($"{args.Data}{Environment.NewLine}") ;
    };
    await conversion.Start();

## Conversions queue


ConversionQueue provides an easy to use interface to queue conversions. If parallel flag is set to true, Queue will process multiple conversions simultaneously. A number of parallel conversions depends on a number of cores, which give the best performance. With parallel flag, multithread in IConversion object should be disabled to gain performance. It is a lot more efficent in conversion small files under few megabytes.

    var queue = new ConversionQueue(parallel: false);
    IConversion conversion = Conversion.ToMp4(Resources.MkvWithAudio, output);
    IConversion conversion2 = Conversion.ToMp4(Resources.MkvWithAudio, output2);
    queue.Add(conversion);
    queue.Add(conversion2);
    
    queue.Start();

## Conversion result

Started conversion returns ConversionResult after it's completed (success or failure). Returned object informs about status of conversion, conversion duration, output file and more.

One of most useful value for debug may be "Arguments" property which have all arguments passed to FFmpeg process for conversion.

## Stop conversion

Started conversion could be stopped. This requires passing CancellationToken to Start method.

    var cancellationTokenSource = new CancellationTokenSource();
    string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
    IMediaInfo mediaInfo = await MediaInfo.Get(Resources.MkvWithAudio);
    
    IStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()
    ?.SetCodec(VideoCodec.h164);
    
    await Conversion.New()
    .AddStream(videoStream)
    .SetOutput(outputPath)
    .Start(cancellationTokenSource.Token);

CancellationTokenSource can be cancelled manually...

    cancellationTokenSource.Cancel();

or automatically after period of time:

    cancellationTokenSource.CancelAfter(500);

## Events

OnException:

        `
queue.OnException += (number, count, conversion) =>
{
System.Console.Out.WriteLine($"Exception when converting file {number}/{count}");
};`

OnConverted:

    queue.OnConverted += (number, count, conversion) =>
    {
    System.Console.Out.WriteLine($"File {number}/{count} converted into {conversion.OutputFilePath}");
    };
