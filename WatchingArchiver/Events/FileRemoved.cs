using System.IO;

namespace WatchingArchiver.Events
{
    internal class FileRemoved
    {
        public FileRemoved(string path)
        {
            File = Path.GetFileName(path);
        }

        public string File { get; }
    }
}