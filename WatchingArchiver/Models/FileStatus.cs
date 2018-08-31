namespace WatchingArchiver.Models
{
    public enum FileStatus
    {
        Waiting,
        Compressing,

        Compressed,
        DoesNotExist,
        Moved,
        Aborted
    }
}