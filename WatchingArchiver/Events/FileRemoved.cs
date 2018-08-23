using System.IO;

namespace WatchingArchiver.Events
{
    internal class FileRemoved
    {
        public FileRemoved(string sourcePath)
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