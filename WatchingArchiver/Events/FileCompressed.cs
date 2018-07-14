using System.IO;

namespace WatchingArchiver.Events
{
    internal class FileCompressed
    {
        public FileCompressed(string path)
        {
            File = Path.GetFileName(path);
        }

        public string File { get; }
    }
}