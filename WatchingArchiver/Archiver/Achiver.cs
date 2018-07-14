using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using Caliburn.Micro;
using WatchingArchiver.Events;
using WatchingArchiver.Utils;

namespace WatchingArchiver.Archiver
{
    internal class Archiver
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        #endregion

        public Archiver(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        #region Properties

        /// <summary>
        ///     Destiation archive path
        /// </summary>
        public string DestinationPath { get; set; }

        #endregion

        public void Archive(string fullPath)
        {
            if (FileUtils.IsFile(fullPath))
            {
                ProcessFile(fullPath);
            }
        }

        private void ProcessFile(string fullPath)
        {
            Console.WriteLine($"[DEBUG] {fullPath}");
            new Thread(() =>
            {
                try
                {
                    while (FileUtils.IsFileLocked(fullPath))
                    {
                        Thread.Sleep(1000);
                        Console.WriteLine($"Waiting for file: {fullPath}");
                    }
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    _eventAggregator.PublishOnUIThread(new FileRemoved(e.FileName));
                    Thread.CurrentThread.Join();
                }

                Console.WriteLine($"Compressing file: {fullPath}");
                CompressFile(fullPath);
                _eventAggregator.PublishOnUIThread(new FileCompressed(fullPath));

                Thread.CurrentThread.Join();
            }).Start();
        }

        #region Compressor

        public void CompressFile(string path)
        {
            var fileName = Path.GetFileName(path);
            var zipName = $"{DestinationPath}/{DateTime.Now:dd/MM/yyyy hh.mm.ss} - {Path.GetFileName(path)}.zip";

            using (var zip = ZipFile.Open(zipName, ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(path, fileName);
            }
        }

        public void CompressDirectory(string path)
        {
        }

        #endregion
    }
}