using System;
using System.IO;
using System.Windows;
using Caliburn.Micro;
using WatchingArchiver.Events;

namespace WatchingArchiver.ViewModels
{
    internal class MainWindowViewModel : Screen, 
        IHandle<FileRemoved>,
        IHandle<FileCompressed>
    {
        #region Fields

        /// <summary>
        ///     FileSystemWatcher instance
        /// </summary>
        private readonly FileSystemWatcher _watcher;

        /// <summary>
        ///     Archiver instance
        /// </summary>
        private readonly Archiver.Archiver _archiver;

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

            _archiver = new Archiver.Archiver(eventAggregator);
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
            Processing.Add(e.Name);
            _archiver.Archive(e.FullPath);
        }

        #endregion

        #region IHandle

        public void Handle(FileRemoved fileRemovedEvent)
        {
            Processing.Remove(fileRemovedEvent.File);
            Archived.Add($"[ABORTED] {fileRemovedEvent.File}");
        }

        public void Handle(FileCompressed fileCompressedEvent)
        {
            Processing.Remove(fileCompressedEvent.File);
            Archived.Add($"[SUCESS] {fileCompressedEvent.File}");
        }

        #endregion
    }
}