Download FFmpeg executables
===========================

Getting latest FFmpeg executables is possible on most common operating system or architectures. Currently there are lack support of Tizen and RaspberryPi. Start downloading it is quite easy. Just run this method:

    FFmpeg.GetLatestVersion()

This will acquire latest version for your operating system and saves it on your computer. Default directory is where your assemblies are. This can be changed by **FFmpeg.ExecutablesPath**. Above method create for **version.json** file in specified directory and if don't find it, download everything and save information about version. Next run will get version from file and check if new version is available.
