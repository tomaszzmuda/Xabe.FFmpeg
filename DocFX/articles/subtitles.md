# Subtitles

## Converting subtitles

Subtitles are typical streams so can be converted like other streams:

    string outputPath = Path.ChangeExtension(Path.GetTempFileName(), "ass");
    
    IMediaInfo info = await MediaInfo.Get(Resources.SubtitleSrt);
    
    ISubtitleStream subtitleStream = info.SubtitleStreams.FirstOrDefault()
    .SetFormat(new SubtitleFormat(format));
    
    IConversionResult result = await Conversion.New()
    .AddStream(subtitleStream)
    .SetOutput(outputPath)
    .Start();
	
## Adding subtitles

There are two ways to add subtitles into video. The first one is to burn it into video. The next one is to add new stream with subtitles, as in .mkv format.

### Burning subtitles

IVideoStream allows to burn subtitle into video:

    IMediaInfo inputFile = await MediaInfo.Get(Resources.MkvWithAudio);
    string outputPath = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
    
    IConversionResult conversionResult = await Conversion.New()
    .AddStream(inputFile.VideoStreams.First().AddSubtitles(Resources.SubtitleSrt))
    .SetOutput(outputPath)
    .Start();

### Add subtitles

Subtitles are streams too so could be added to conversion like other streams:

    IMediaInfo mediaInfo = AsyncHelper.RunSync(() => MediaInfo.Get(inputPath));
    IMediaInfo subtitleInfo = AsyncHelper.RunSync(() => MediaInfo.Get(subtitlePath));
    
    ISubtitleStream subtitleStream = subtitleInfo.SubtitleStreams.First()
    .SetLanguage(language);
    
    return New()
    .AddStream(mediaInfo.VideoStreams.ToArray())
    .AddStream(mediaInfo.AudioStreams.ToArray())
    .AddStream(subtitleStream)
    .SetOutput(outputPath);

or easier using Conversion.Helpers

    string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp4);
    
    IConversionResult result = await Conversion.AddSubtitles(Resources.Mp4, output, Resources.SubtitleSrt)
    .Start();