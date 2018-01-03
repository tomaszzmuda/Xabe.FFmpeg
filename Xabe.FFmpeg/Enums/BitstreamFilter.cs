namespace Xabe.FFmpeg.Enums
{
    /// <summary>
    ///     Bistream filter
    /// </summary>
    public class BitstreamFilter
    {
        /// <summary>
        /// Bistream filter
        /// </summary>
        public string Filter { get; private set; }

        /// <summary>
        /// Create filter
        /// </summary>
        /// <param name="filter">Filter string</param>
        public BitstreamFilter(string filter)
        {
            Filter = filter;
        }

        /// <summary>
        ///     H264_Mp4ToAnnexB
        /// </summary>
        public static BitstreamFilter H264_Mp4ToAnnexB => new BitstreamFilter("H264_Mp4ToAnnexB");

        /// <summary>
        ///     Aac_AdtstoAsc
        /// </summary>
        public static BitstreamFilter Aac_AdtstoAsc => new BitstreamFilter("Aac_AdtstoAsc");

        public override string ToString()
        {
            return Filter.ToLower();
        }
    }
}
