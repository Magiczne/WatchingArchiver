namespace WatchingArchiver.Logger
{
    public interface ILogger
    {
        /// <summary>
        ///     Successful compression
        /// </summary>
        /// <param name="file"></param>
        void Compressed(string file);

        /// <summary>
        ///     File does not exists
        /// </summary>
        /// <param name="file"></param>
        void DoesNotExists(string file);

        /// <summary>
        ///     Started processing
        /// </summary>
        /// <param name="file"></param>
        void InProgress(string file);

        /// <summary>
        ///     File used by another process
        /// </summary>
        /// <param name="file"></param>
        void InUse(string file);
        
        /// <summary>
        ///     File moved to archive
        /// </summary>
        /// <param name="file"></param>
        void Moved(string file);
        
        /// <summary>
        ///     File removed while processing
        /// </summary>
        /// <param name="file"></param>
        void Removed(string file);
    }
}