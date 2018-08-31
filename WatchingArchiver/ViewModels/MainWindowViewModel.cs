using System;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using WatchingArchiver.Archiver;
using WatchingArchiver.Events;
using WatchingArchiver.Logger;
using WatchingArchiver.Models;
using WatchingArchiver.Utils;

namespace WatchingArchiver.ViewModels
{
    internal class MainWindowViewModel : Screen,
        IHandle<FileCompressed>,
        IHandle<FileDoesNotExist>,
        IHandle<FileInProgress>,
        IHandle<FileInUse>,
        IHandle<FileMoved>,
        IHandle<FileRemoved>,
        ILogger
    {
        #region Fields

        /// <summary>
        ///     FileSystemWatcher instance
        /// </summary>
        private readonly FileSystemWatcher _watcher;

        /// <summary>
        ///     Archiver instance
        /// </summary>
        private readonly FileArchiver _archiver;

        /// <summary>
        ///     Logger instance
        /// </summary>
        private readonly CompositeLogger _logger;

        #endregion

        #region Properties

        /// <summary>
        ///     Collection of currently processed entries
        /// </summary>
        public BindableCollection<FileEntry> Processing { get; set; } = new BindableCollection<FileEntry>();

        /// <summary>
        ///     Collection of archived entries
        /// </summary>
        public BindableCollection<FileEntry> Processed { get; set; } = new BindableCollection<FileEntry>();

        /// <summary>
        ///     Path to the folder that needs to be watched
        /// </summary>
        public string WatchPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "watching";

        /// <summary>
        ///     Path to the archive
        /// </summary>
        public string ArchivePath { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "archiving";

        /// <summary>
        ///     Is archiving currently active
        /// </summary>
        public bool Archiving { get; set; }

        /// <summary>
        ///     Text on button
        /// </summary>
        public string ActionButtonText => Archiving ? Properties.Strings.StopMonitoring : Properties.Strings.StartMonitoring;

        #endregion

        #region Methods

        public MainWindowViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);

            _logger = new CompositeLogger();
            _logger.Add(this);
            _logger.Add(new TextLogger());

            _archiver = new FileArchiver(eventAggregator);
            _watcher = new FileSystemWatcher();
            _watcher.Created += Archive;
        }

        public void ToggleAction()
        {
            Archiving = !Archiving;

            if (Archiving)
            {
                _watcher.Path = WatchPath;
                _archiver.DestinationPath = ArchivePath;
            }

            _watcher.EnableRaisingEvents = Archiving;
        }

        private void Archive(object sender, FileSystemEventArgs e)
        {
            if (FileUtils.IsFile(e.FullPath))
            {
                Processing.Add(new FileEntry
                {
                    File = e.Name
                });

                _archiver.Process(e.FullPath);
            }
        }

        #endregion

        #region IHandle

        /// <inheritdoc />
        public void Handle(FileCompressed e)
        {
            _logger.Compressed(e.File);
        }

        /// <inheritdoc />
        public void Handle(FileDoesNotExist e)
        {
            _logger.DoesNotExists(e.File);
        }

        /// <inheritdoc />
        public void Handle(FileInProgress e)
        {
            _logger.InProgress(e.File);
        }

        /// <inheritdoc />
        public void Handle(FileInUse e)
        {
            _logger.InUse(e.File);
        }

        /// <inheritdoc />
        public void Handle(FileMoved e)
        {
            _logger.Moved(e.File);
        }

        /// <inheritdoc />
        public void Handle(FileRemoved e)
        {
            _logger.Removed(e.File);
        }

        #endregion

        #region ILogger

        /// <inheritdoc />
        public void Compressed(string file)
        {
            var item = Processing.First(entry => entry.File == file);
            Processing.Remove(item);

            item.FileStatus = FileStatus.Compressed;

            Processed.Add(item);
        }

        /// <inheritdoc />
        public void DoesNotExists(string file)
        {
            var item = Processing.First(entry => entry.File == file);
            Processing.Remove(item);

            item.FileStatus = FileStatus.DoesNotExist;

            Processed.Add(item);
        }

        /// <inheritdoc />
        public void InProgress(string file)
        {
            Processing.First(entry => entry.File == file).FileStatus = FileStatus.Compressing;
        }

        /// <inheritdoc />
        public void InUse(string file)
        {
            Processing.First(entry => entry.File == file).FileStatus = FileStatus.Waiting;
        }

        /// <inheritdoc />
        public void Moved(string file)
        {
            var item = Processing.First(entry => entry.File == file);
            Processing.Remove(item);

            item.FileStatus = FileStatus.Moved;

            Processed.Add(item);
        }

        /// <inheritdoc />
        public void Removed(string file)
        {
            var item = Processing.First(entry => entry.File == file);
            Processing.Remove(item);

            item.FileStatus = FileStatus.Aborted;

            Processed.Add(item);
        }

        #endregion
    }
}