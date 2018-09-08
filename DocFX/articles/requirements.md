Xabe.FFmpeg requirements
========================

**FFmpeg has to be installed on end user device. To use this library one of those conditions must be fulfilled:**

*   FFmpeg directory added in PATH (contains ffprobe and ffmpeg executables)
*   FFmpeg.ExecutablePath variable has to be set to FFmpeg directory path
*   FFmpeg executables have to be in the same directory with application executable
*   FFmpeg executables could be downloaded by using **FFmpeg.GetLatestVersion()**

  
Default, the library is trying to find FFmpeg executables names containing "ffprobe", "ffmpeg". This function is case insensitive. Those names can be changed in FFmpeg.FFmpegExecutableName and FFmpeg.FFprobeExecutableName.  
Install the [Xabe.FFmpeg NuGet package](https://www.nuget.org/packages/Xabe.FFmpeg) via NuGet:

    PM> Install-Package Xabe.FFmpeg
    
