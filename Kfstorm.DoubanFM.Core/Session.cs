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

        public async Task<bool> LogOn(IAuthentication authentication)
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
                        return false;
                    }
                    UserInfo = result.UserInfo;
                    return true;
                }
                finally
                {
                    Interlocked.CompareExchange(ref IsWorking, 0, 1);
                }
            }
            throw new InvalidOperationException("Another unfinished log on/off request exists.");
        }

        public async Task<bool> LogOff(IAuthentication authentication)
        {
            if (UserInfo == null)
            {
                throw new InvalidOperationException("Already logged off");
            }
            if (Interlocked.CompareExchange(ref IsWorking, 1, 0) == 0)
            {
                try
                {
                    if (await authentication.UnAuthenticate())
                    {
                        UserInfo = null;
                        Logger.Info("Logged off.");
                        return true;
                    }
                    Logger.Warn("Failed to log off.");
                    return false;
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
