namespace Xabe.FFMpeg.Enums
{
    /// <summary>
    ///     Quality of audio
    /// </summary>
    internal enum FFMpegStatus
    {
        /// <summary>
        ///     Correct status
        /// </summary>
        Correct = 0,

        /// <summary>
        ///     Status when FFMpeg process was killed
        /// </summary>
        Killed = -1,
    }
}
