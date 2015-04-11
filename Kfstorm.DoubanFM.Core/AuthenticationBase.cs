using System;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public abstract class AuthenticationBase : IAuthentication
    {
        protected LogOnResult ParseLogOnResult(string jsonContent)
        {
            throw new NotImplementedException();
        }

        public abstract Task<LogOnResult> Authenticate();
        public abstract Task<bool> UnAuthenticate();
    }
}
