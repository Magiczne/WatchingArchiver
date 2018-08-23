using System.Collections.Generic;
using System.IO;

namespace WatchingArchiver.Utils
{
    internal class FileUtils
    {
        /// <summary>
        ///     List of archives extenstions
        /// </summary>
        private static readonly List<string> ArchiveExtensions = new List<string>
        {
            ".zip",
            ".rar",
            ".7z",
            ".tar",
            ".gz",
            ".tar.gz",
            ".tgz"
        };

        /// <summary>
        ///     Determine if path is file or directory
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static bool IsFile(string fullPath)
        {
            return !File.GetAttributes(fullPath).HasFlag(FileAttributes.Directory);
        }

        /// <summary>
        ///     Determine
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        public static bool IsArchived(string sourcePath)
        {
            if (!IsFile(sourcePath)) return false;

            var extension = Path.GetExtension(sourcePath);

            return extension != null && ArchiveExtensions.Contains(extension.ToLower());
        }

        /// <summary>
        ///     Determine if file is available to process
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static bool IsFileLocked(string fullPath)
        {
            FileStream stream = null;

            try
            {
                stream = File.Open(fullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (IOException)
            {
                // The file is unavailable because:
                // - Still being written to
                // - Being processed by another thread
                return true;
            }
            finally
            {
                stream?.Close();
            }

            // File is not locked
            return false;
        }
    }
}