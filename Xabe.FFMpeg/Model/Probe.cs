namespace Xabe.FFMpeg.Model
{
    internal class ProbeModel
    {
        public Stream[] Streams { get; set; }

        public class Stream
        {
            public int Index { get; set; }
            public string CodecName { get; set; }
            public string CodecLongName { get; set; }
            public string Profile { get; set; }
            public string CodecType { get; set; }
            public string CodecTimeBase { get; set; }
            public string CodecTagString { get; set; }
            public string CodecTag { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int CodedWidth { get; set; }
            public int CodedHeight { get; set; }
            public int HasBFrames { get; set; }
            public string SampleAspectRatio { get; set; }
            public string DisplayAspectRatio { get; set; }
            public string PixFmt { get; set; }
            public int Level { get; set; }
            public string ChromaLocation { get; set; }
            public int Refs { get; set; }
            public string QuarterSample { get; set; }
            public string DivxPacked { get; set; }
            public string RFrameRate { get; set; }
            public string AvgFrameRate { get; set; }
            public string TimeBase { get; set; }
            public int StartPts { get; set; }
            public string StartTime { get; set; }
            public int DurationTs { get; set; }
            public double Duration { get; set; }
            public double BitRate { get; set; }
            public string NbFrames { get; set; }
            public Disposition Disposition { get; set; }
        }

        public class Disposition
        {
            public int Default { get; set; }
            public int Dub { get; set; }
            public int Original { get; set; }
            public int Comment { get; set; }
            public int Lyrics { get; set; }
            public int Karaoke { get; set; }
            public int Forced { get; set; }
            public int HearingImpaired { get; set; }
            public int VisualImpaired { get; set; }
            public int CleanEffects { get; set; }
            public int AttachedPic { get; set; }
            public int TimedThumbnails { get; set; }
        }
    }
}
