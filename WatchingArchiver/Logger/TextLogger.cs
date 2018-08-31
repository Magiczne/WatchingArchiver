using System;
using System.IO;

namespace WatchingArchiver.Logger
{
    public class TextLogger : ILogger
    {
        /// <summary>
        ///     Logs directory
        /// </summary>
        private static readonly string LogsDirectory = $"logs/{DateTime.Now:yyyy}";

        /// <summary>
        ///     Log file
        /// </summary>
        private static string LogFile => $"{LogsDirectory}/{DateTime.Now:yyyy-MM-dd}.log";

        /// <summary>
        ///     Check if log directory exists
        /// </summary>
        private static void CheckDirectoryExistence()
        {
            if (!Directory.Exists(LogsDirectory))
            {
                Directory.CreateDirectory(LogsDirectory);
            }
        }

        /// <summary>
        ///     Log data to the file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="message"></param>
        private static void Log(string file, string message)
        {
            File.AppendAllLines(LogFile, new[]
            {
                $"{DateTime.Now:HH:mm:ss}\t{file}\t{message}"
            });
        }

        #region ILogger

        /// <inheritdoc />
        public void Compressed(string file)
        {
            CheckDirectoryExistence();
            Log(file, Properties.Logger.Compressed);
        }

        /// <inheritdoc />
        public void DoesNotExists(string file)
        {
            CheckDirectoryExistence();
            Log(file, Properties.Logger.DoesNotExists);
        }

        /// <inheritdoc />
        public void InProgress(string file)
        {
            CheckDirectoryExistence();
            Log(file, Properties.Logger.InProgress);
        }

        /// <inheritdoc />
        public void InUse(string file)
        {
            CheckDirectoryExistence();
            Log(file, Properties.Logger.InUse);
        }

        /// <inheritdoc />
        public void Moved(string file)
        {
            CheckDirectoryExistence();
            Log(file, Properties.Logger.Moved);
        }

        /// <inheritdoc />
        public void Removed(string file)
        {
            CheckDirectoryExistence();
            Log(file, Properties.Logger.Removed);
        }

        #endregion
    }
}