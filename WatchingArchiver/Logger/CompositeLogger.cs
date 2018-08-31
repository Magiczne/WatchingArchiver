using System.Collections.Generic;

namespace WatchingArchiver.Logger
{
    public class CompositeLogger : ILogger
    {
        /// <summary>
        ///     Loggers instances
        /// </summary>
        private readonly List<ILogger> _innerLoggers = new List<ILogger>();

        /// <summary>
        ///     Add logger
        /// </summary>
        /// <param name="logger"></param>
        public void Add(ILogger logger)
        {
            _innerLoggers.Add(logger);
        }

        /// <summary>
        ///     Remove logger
        /// </summary>
        /// <param name="logger"></param>
        public void Remove(ILogger logger)
        {
            _innerLoggers.Remove(logger);
        }

        #region ILogger

        /// <inheritdoc />
        public void Compressed(string file)
        {
            foreach (var logger in _innerLoggers)
            {
                logger.Compressed(file);
            }
        }

        /// <inheritdoc />
        public void DoesNotExists(string file)
        {
            foreach (var logger in _innerLoggers)
            {
                logger.DoesNotExists(file);
            }
        }

        /// <inheritdoc />
        public void InProgress(string file)
        {
            foreach (var logger in _innerLoggers)
            {
                logger.InProgress(file);
            }
        }

        /// <inheritdoc />
        public void InUse(string file)
        {
            foreach (var logger in _innerLoggers)
            {
                logger.InUse(file);
            }
        }

        /// <inheritdoc />
        public void Moved(string file)
        {
            foreach (var logger in _innerLoggers)
            {
                logger.Moved(file);
            }
        }
        
        /// <inheritdoc />
        public void Removed(string file)
        {
            foreach (var logger in _innerLoggers)
            {
                logger.Removed(file);
            }
        }

        #endregion
    }
}