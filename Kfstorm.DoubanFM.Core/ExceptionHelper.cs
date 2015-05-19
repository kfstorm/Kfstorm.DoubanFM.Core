using System;
using System.Threading.Tasks;
using log4net;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// A helper class of exception
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        /// Logs the exception if the action throws any.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="action">The action.</param>
        /// <param name="message">The log message.</param>
        /// <returns></returns>
        public static async Task LogExceptionIfAny(ILog logger, Func<Task> action, string message = null)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                logger.Warn(message, ex);
                throw;
            }
        }

        /// <summary>
        /// Logs the exception if the action throws any.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger">The logger.</param>
        /// <param name="action">The action.</param>
        /// <param name="message">The log message.</param>
        /// <returns></returns>
        public static async Task<T> LogExceptionIfAny<T>(ILog logger, Func<Task<T>> action, string message = null)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                logger.Warn(message, ex);
                throw;
            }
        }
    }
}
