using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using WatchingArchiver.Archiver;
using WatchingArchiver.Events;
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
        IHandle<FileRemoved>
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
        public string ActionButtonText => Archiving ? "Stop archiving" : "Start archiving";

        #endregion

        #region Methods

        public MainWindowViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);

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

        public void Handle(FileCompressed evt)
        {
            var item = Processing.First(entry => entry.File == evt.File);
            Processing.Remove(item);

            item.FileStatus = FileStatus.Compressed;

            Processed.Add(item);
        }

        public void Handle(FileDoesNotExist evt)
        {
            var item = Processing.First(entry => entry.File == evt.File);
            Processing.Remove(item);

            item.FileStatus = FileStatus.DoesNotExist;

            Processed.Add(item);
        }

        public void Handle(FileInProgress evt)
        {
            Processing.First(entry => entry.File == evt.File).FileStatus = FileStatus.Compressing;
        }

        public void Handle(FileInUse evt)
        {
            Processing.First(entry => entry.File == evt.File).FileStatus = FileStatus.Waiting;
        }

        public void Handle(FileMoved evt)
        {
            var item = Processing.First(entry => entry.File == evt.File);
            Processing.Remove(item);

            item.FileStatus = FileStatus.Moved;

            Processed.Add(item);
        }

        public void Handle(FileRemoved evt)
        {
            var item = Processing.First(entry => entry.File == evt.File);
            Processing.Remove(item);

            item.FileStatus = FileStatus.Aborted;

            Processed.Add(item);
        }

        #endregion
    }
}