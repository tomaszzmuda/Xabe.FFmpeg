using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg
{
    public interface ISubtitleStream : IStream
    {
        ISubtitleStream SetFormat(SubtitleFormat format);

        string Language { get; }

        ISubtitleStream SetLanguage(string lang);
    }
}
