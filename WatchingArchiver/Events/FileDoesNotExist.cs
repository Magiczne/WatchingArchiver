using System.IO;

namespace WatchingArchiver.Events
{
    public class FileDoesNotExist
    {
        public FileDoesNotExist(string sourcePath)
        {
            SourcePath = sourcePath;
            File = Path.GetFileName(sourcePath);
        }

        /// <summary>
        ///     Filename
        /// </summary>
        public string File { get; }

        /// <summary>
        ///     File source path
        /// </summary>
        public string SourcePath { get; }
    }
}