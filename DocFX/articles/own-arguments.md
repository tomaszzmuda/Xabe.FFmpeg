Own arguments
=============

It is impossible to wrap all functionality of FFmpeg in C#. To perform more complex tasks it will be necessary to pass your own arguments directly to FFmpeg.

If you want to add additional parameter to IConversion, use AddParameter method. All parameters added in this way will go near the end of ffmpeg arguments, just before output parameter. This code adds 3 parameters to conversion: -ss (start position), -t (duration) and -s (size).

    bool conversionResult = await new Conversion().SetInput(Resources.MkvWithAudio)
    .AddParameter($"-ss {TimeSpan.FromSeconds(1)} -t {TimeSpan.FromSeconds(1)}")
    .AddParameter("-s 1920x1080")
    .SetOutput(outputPath)
    .Start();
    
    //Output ffmpeg arguments should look like "ffmpeg.exe -i sample.mkv -ss 1 -t 1 -s 1920x1080 output.mp4"

Also user can pass only his own arguments, without using IConversion class. Simplest conversion, from one format to another, can be obtained in this way:

    string inputFile = Path.Combine(Environment.CurrentDirectory, "Resources", "SampleVideo_360x240_1mb.mkv");
    string outputPath = Path.ChangeExtension(Path.GetTempFileName(), Extensions.Mp4);
    string arguments = $"-i "{inputFile}" "{outputPath}"";
    
    bool conversionResult = await new Conversion().Start(arguments);

In a result, Arguments variable should look like this (depends on OS and directories):

    -i "C:Xabe.FFmpegXabe.FFmpeg.TestbinDebugnetcoreapp2.0ResourcesSampleVideo_360x240_1mb.mkv" "C:TemptmpA1AA.mp4"

That string will be passed directly to FFmpeg so final command running in console will look like:

    ffmpeg.exe -i "C:Xabe.FFmpegXabe.FFmpeg.TestbinDebugnetcoreapp2.0ResourcesSampleVideo_360x240_1mb.mkv" "C:TemptmpA1AA.mp4"

