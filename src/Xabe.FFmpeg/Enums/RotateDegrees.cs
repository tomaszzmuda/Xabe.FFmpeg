namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Defines types of available rotation
    /// </summary>
    public enum RotateDegrees
    {
        /// <summary>
        ///     90 degrees counter clockwise and vertical flip
        /// </summary>
        CounterClockwiseAndFlip = 0,

        /// <summary>
        ///     90 degress clockwise
        /// </summary>
        Clockwise = 1,

        /// <summary>
        ///     90 degrees counter clockwise
        /// </summary>
        CounterClockwise = 2,

        /// <summary>
        ///     90 degrees counter clockwise and vertical flip
        /// </summary>
        ClockwiseAndFlip = 3,

        /// <summary>
        ///     Rotate video by 180 degrees
        /// </summary>
        Invert = 5
    }
}
