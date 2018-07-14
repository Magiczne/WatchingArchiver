using System;
using System.IO;

namespace WatchingArchiver.Utils
{
    internal class FileUtils
    {
        /// <summary>
        ///     Determine if path is file or directory
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static bool IsFile(string fullPath) => !File.GetAttributes(fullPath).HasFlag(FileAttributes.Directory);

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