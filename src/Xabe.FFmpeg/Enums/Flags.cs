namespace Xabe.FFmpeg
{
    /// <summary>
    ///     Flags for Vidoes (-flags option)
    /// </summary>
    public class Flags
    {
        /// <summary>
        ///     convert a Progressive Video to an Interlaced
        /// </summary>
        public static readonly Flags Interlaced = new Flags("+ilme+ildct");

        /// <inheritdoc />
        public Flags(string flag)
        {
            Flag = flag;
        }

        /// <summary>
        ///     Conversion Flag
        /// </summary>
        public string Flag { get; }

        /// <summary>
        ///     Format to string
        /// </summary>
        /// <returns>Codec as string</returns>
        public override string ToString()
        {
            return Flag;
        }
    }
}