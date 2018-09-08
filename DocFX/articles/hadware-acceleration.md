Using hardware acceleration
===========================

Since version 3.1.0 there is a possibility to use hardware acceleration.

    ///<summary>
     ///Use hardware acceleration. This option set -threads to 1 for compatibility reasons. This should be use with proper codec (e.g. -c:v h164_nvencorh164_cuvid)
     /// </summary>
     /// <param name="hardwareAccelerator">Hardware accelerator. List of all accelerators available for your system - "ffmpeg -hwaccels"</param>
     /// <param name="decoder">Codec using to decode input video.</param>
     /// <param name="encoder">Codec using to encode output video.</param>
     /// <param name="device">Number of device (0 = default video card) if more than one video card.</param>
     /// <returns>IConversion object</returns>
     IConversion UseHardwareAcceleration(HardwareAccelerator hardwareAccelerator, VideoCodec decoder, VideoCodec encoder, int device = 0);

![gpu vs cpu conversion](https://xabe.net/wp-content/uploads/2018/02/gpu-vs-cpu-conversion.png)You can read more at [official ffmpeg wiki](https://trac.ffmpeg.org/wiki/HWAccelIntro).
