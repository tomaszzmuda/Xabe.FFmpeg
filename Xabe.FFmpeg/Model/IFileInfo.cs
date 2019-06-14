
namespace Xabe.FFmpeg.Model
{
    internal interface IFileInfo
    {
        string Name { get; }
        string FullName { get; }
    }

    internal class FileInfo : IFileInfo
    {
        public string Name { get; set; }
        public string FullName { get; set; }
    }
}
