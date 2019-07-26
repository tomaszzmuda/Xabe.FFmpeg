using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Xabe.FFmpeg
{
    /// <summary>
    /// Default Implementation of the IInputBuilder Interface
    /// </summary>
    public class InputBuilder : IInputBuilder
    {
        /// <inheritdoc />
        public List<FileInfo> FileList { get; }

        /// <inheritdoc />
        public InputBuilder()
        {
            FileList = new List<FileInfo>();
        }

        /// <inheritdoc />
        public Func<string, string> PrepareInputFiles(List<string> files, out string directory)
        {
            Guid directoryGuid = Guid.NewGuid();

            for(int i = 0; i < files.Count; i++)
            {
                string destinationPath = Path.Combine(Path.GetTempPath(), directoryGuid.ToString(), BuildFileName(i + 1, Path.GetExtension(files[i])));

                if (!Directory.Exists(Path.Combine(Path.GetTempPath(), directoryGuid.ToString())))
                    Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), directoryGuid.ToString()));

                File.Copy(files[i], destinationPath);
                FileList.Add(new FileInfo(destinationPath));
            }

            directory = Path.Combine(Path.GetTempPath(), directoryGuid.ToString());
            return (number) => { return $" -i {Path.Combine(FileList[0].DirectoryName, "img" + number + FileList[0].Extension)} "; };
        }


        private string BuildFileName(int fileIndex, string extension)
        {
            string name = $"img_";

            if (fileIndex < 10)
                name += $"00{fileIndex}" + extension;
            else if (fileIndex < 100)
                name += $"0{fileIndex}" + extension;
            else
                name += $"{fileIndex}" + extension;

            return name;
        }
    }
}
