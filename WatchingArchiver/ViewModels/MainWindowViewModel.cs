using System;
using System.IO;
using Caliburn.Micro;
using WatchingArchiver.Archiver;
using WatchingArchiver.Events;
using WatchingArchiver.Utils;

namespace WatchingArchiver.ViewModels
{
    internal class MainWindowViewModel : Screen,
        IHandle<FileCompressed>,
        IHandle<FileDoesNotExist>,
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
        public BindableCollection<string> Processing { get; set; } = new BindableCollection<string>();

        /// <summary>
        ///     Collection of archived entries
        /// </summary>
        public BindableCollection<string> Archived { get; set; } = new BindableCollection<string>();

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
                Processing.Add(e.Name);
                _archiver.Process(e.FullPath);
            }
        }

        #endregion

        #region IHandle

        public void Handle(FileCompressed e)
        {
            Processing.Remove(e.File);
            Archived.Add($"[SUCESS] {e.File}");
        }

        public void Handle(FileDoesNotExist e)
        {
            Processing.Remove(e.File);
            Archived.Add($"[DOES NOT EXISTS] {e.File}");
        }

        public void Handle(FileMoved e)
        {
            Processing.Remove(e.File);
            Archived.Add($"[SUCCESS][MOVED] {e.File}");
        }

        public void Handle(FileRemoved e)
        {
            Processing.Remove(e.File);
            Archived.Add($"[ABORTED] {e.File}");
        }

        #endregion
    }
}