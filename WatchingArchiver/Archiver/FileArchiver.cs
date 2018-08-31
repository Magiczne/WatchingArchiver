using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using Caliburn.Micro;
using WatchingArchiver.Events;
using WatchingArchiver.Utils;

namespace WatchingArchiver.Archiver
{
    public class FileArchiver : IArchiver
    {
        #region Fields

        /// <summary>
        ///     Event aggregator instance
        /// </summary>
        private readonly IEventAggregator _eventAggregator;

        #endregion

        public FileArchiver(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        #region IArchiver

        #region Properties

        /// <inheritdoc />
        public string DestinationPath { get; set; }

        #endregion

        /// <inheritdoc />
        public void Process(string sourcePath)
        {
            new Thread(() =>
            {
                // If file does not exists join thread and finish
                if (!File.Exists(sourcePath))
                {
                    _eventAggregator.PublishOnUIThread(new FileDoesNotExist(sourcePath));
                    Thread.CurrentThread.Join();
                    return;
                }

                // If file is used by another process wait one second
                // If file has been removed join thread and finish
                try
                {
                    while (FileUtils.IsFileLocked(sourcePath))
                    {
                        _eventAggregator.PublishOnUIThread(new FileInUse(sourcePath));
                        Thread.Sleep(1000);
                    }
                }
                catch (FileNotFoundException ex)
                {
                    _eventAggregator.PublishOnUIThread(new FileRemoved(ex.FileName));

                    Thread.CurrentThread.Join();
                    return;
                }

                // If file is already an archive just move it
                // In other case compress it
                if (FileUtils.IsArchived(sourcePath))
                {
                    Move(sourcePath);
                    _eventAggregator.PublishOnUIThread(new FileMoved(sourcePath));
                }
                else
                {
                    _eventAggregator.PublishOnUIThread(new FileInProgress(sourcePath));
                    Compress(sourcePath);
                    Remove(sourcePath);
                    _eventAggregator.PublishOnUIThread(new FileCompressed(sourcePath));
                }

                Thread.CurrentThread.Join();
            }).Start();
        }

        /// <inheritdoc />
        public void Compress(string sourcePath)
        {
            var fileName = Path.GetFileName(sourcePath);
            var archiveName = $"{DestinationPath}/{DateTime.Now:dd/MM/yyyy HH.mm.ss-fff} - {fileName}.zip";

            using (var zip = ZipFile.Open(archiveName, ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(sourcePath, fileName);
            }
        }

        /// <inheritdoc />
        public void Move(string sourcePath)
        {
            var fileName = Path.GetFileName(sourcePath);
            var archiveName = $"{DestinationPath}/{DateTime.Now:dd/MM/yyyy HH.mm.ss-fff} - {fileName}.zip";

            File.Move(sourcePath, archiveName);
        }

        /// <inheritdoc />
        public void Remove(string sourcePath)
        {
            File.Delete(sourcePath);
        }

        #endregion
    }
}