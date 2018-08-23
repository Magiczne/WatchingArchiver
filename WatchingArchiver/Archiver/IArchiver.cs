namespace WatchingArchiver.Archiver
{
    public interface IArchiver
    {
        #region Properties

        /// <summary>
        ///     Destination path for the archives
        /// </summary>
        string DestinationPath { get; set; }

        #endregion

        /// <summary>
        ///     Process path
        /// </summary>
        /// <param name="sourcePath">Source path</param>
        void Process(string sourcePath);

        /// <summary>
        ///     Compress path
        /// </summary>
        /// <param name="sourcePath">Source path</param>
        void Compress(string sourcePath);

        /// <summary>
        ///     Move from path to destination path
        /// </summary>
        /// <param name="sourcePath">Source path</param>
        void Move(string sourcePath);

        /// <summary>
        ///     Remove path
        /// </summary>
        /// <param name="sourcePath">Source path</param>
        void Remove(string sourcePath);
    }
}