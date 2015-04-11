using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace Kfstorm.DoubanFM.Core
{
    public class Session : ISession
    {
        protected ILog Logger = LogManager.GetLogger(typeof(Session));

        public UserInfo UserInfo { get; protected set; }

        protected int IsWorking;

        public async Task<string> LogOn(IAuthentication authentication)
        {

            if (UserInfo != null)
            {
                throw new InvalidOperationException("Already logged on");
            }
            if (Interlocked.CompareExchange(ref IsWorking, 1, 0) == 0)
            {
                try
                {
                    var result = await authentication.Authenticate();
                    Logger.Info($"Authentication result: {result}");
                    if (result.UserInfo == null)
                    {
                        return result.ErrorMessage;
                    }
                    UserInfo = result.UserInfo;
                    return null;
                }
                catch (Exception ex)
                {
                    Logger.Error("Exception occurred when trying to log on.", ex);
                    return ex.Message;
                }
                finally
                {
                    Interlocked.CompareExchange(ref IsWorking, 0, 1);
                }
            }
            throw new InvalidOperationException("Another unfinished log on/off request exists.");
        }

        public async Task<string> LogOff(IAuthentication authentication)
        {
            if (UserInfo == null)
            {
                throw new InvalidOperationException("Already logged off");
            }
            if (Interlocked.CompareExchange(ref IsWorking, 1, 0) == 0)
            {
                try
                {
                    var error = await authentication.UnAuthenticate();
                    if (error != null)
                    {
                        return error;
                    }
                    UserInfo = null;
                    Logger.Info("Logged off.");
                    return null;
                }
                catch (Exception ex)
                {
                    Logger.Error("Exception occurred when trying to log off.", ex);
                    return ex.Message;
                }
                finally
                {
                    Interlocked.CompareExchange(ref IsWorking, 0, 1);
                }
            }
            throw new InvalidOperationException("Another unfinished log on/off request exists.");
        }
    }
}
