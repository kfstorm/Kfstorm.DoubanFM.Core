using System;
using System.Threading.Tasks;
using log4net;

namespace Kfstorm.DoubanFM.Core
{
    public static class ExceptionHelper
    {
        public static async Task IgnoreExceptionIfAny(ILog logger, Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                logger.Warn("Exception ignored.", ex);
            }
        }

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
