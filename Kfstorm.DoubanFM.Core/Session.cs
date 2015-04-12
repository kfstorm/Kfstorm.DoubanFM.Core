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

        public async Task LogOn(IAuthentication authentication)
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
                    if (result == null)
                    {
                        throw new Exception("Null user info is not allowed");
                    }
                    UserInfo = result;
                    return;
                }
                catch (Exception ex)
                {
                    Logger.Error("Exception occurred when trying to log on.", ex);
                    throw;
                }
                finally
                {
                    Interlocked.CompareExchange(ref IsWorking, 0, 1);
                }
            }
            throw new InvalidOperationException("Another unfinished log on/off request exists.");
        }

        public void LogOff()
        {
            if (UserInfo == null)
            {
                throw new InvalidOperationException("Already logged off");
            }
            if (Interlocked.CompareExchange(ref IsWorking, 1, 0) == 0)
            {
                UserInfo = null;
                Interlocked.CompareExchange(ref IsWorking, 0, 1);
                return;
            }
            throw new InvalidOperationException("Another unfinished log on/off request exists.");
        }
    }
}
