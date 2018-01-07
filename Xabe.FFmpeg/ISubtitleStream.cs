using Xabe.FFmpeg.Enums;

namespace Xabe.FFmpeg
{
    public interface ISubtitleStream: IStream
    {
        string Language { get; }
        ISubtitleStream SetFormat(SubtitleFormat format);

        ISubtitleStream SetLanguage(string lang);
    }
}
