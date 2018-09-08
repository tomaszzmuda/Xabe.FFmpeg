Extracting audio
================

The simplest way to extract audio from media file is by Conversion.Helpers:

    string output = Path.ChangeExtension(Path.GetTempFileName(), FileExtensions.Mp3);
    IConversionResult result = await Conversion.ExtractAudio(Resources.Mp4WithAudio, output)
    .Start();