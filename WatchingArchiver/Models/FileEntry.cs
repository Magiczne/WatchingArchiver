using System.ComponentModel;

namespace WatchingArchiver.Models
{
    public class FileEntry : INotifyPropertyChanged
    {
        /// <summary>
        ///     File name
        /// </summary>
        public string File { get; set; }

        /// <summary>
        ///     File status
        /// </summary>
        public FileStatus FileStatus { get; set; } = FileStatus.Compressing;

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}