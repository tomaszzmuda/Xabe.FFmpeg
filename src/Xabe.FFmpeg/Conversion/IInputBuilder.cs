using System;
using System.Collections.Generic;
using System.IO;

namespace Xabe.FFmpeg
{
    /// <summary>
    /// Interface for building a list of input files intended to be used with the BuildVideoFromImages Function
    /// </summary>
    public interface IInputBuilder
    {
        /// <summary>
        /// List of File Paths to be used as an input
        /// </summary>
        List<FileInfo> FileList { get; }

        /// <summary>
        /// Prepares a list of files to be used as input by renaming them to have a consistent file name and copying them to the temp directory
        /// </summary>
        /// <param name="files">A list of file paths to prepare</param>
        /// <param name="directory">The Path to the temporary directory containing the prepared files</param>
        /// <returns>Delegate function to generate input argument from file List</returns>
        Func<string, string> PrepareInputFiles(List<string> files, out string directory);
    }
}
