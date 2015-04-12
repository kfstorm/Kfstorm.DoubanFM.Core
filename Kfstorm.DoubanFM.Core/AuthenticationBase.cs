using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json.Linq;

namespace Kfstorm.DoubanFM.Core
{
    public abstract class AuthenticationBase : IAuthentication
    {
        protected ILog Logger { get; }

        protected AuthenticationBase(IServerConnection serverConnection)
        {
            ServerConnection = serverConnection;
            Logger = LogManager.GetLogger(GetType());
        }

        protected IServerConnection ServerConnection { get; }

        protected UserInfo ParseLogOnResult(string jsonContent)
        {
            var obj = JObject.Parse(jsonContent);
            return new UserInfo
            {
                AccessToken = (string)obj[StringTable.AccessToken],
                Username = (string)obj[StringTable.DoubanUsername],
                UserId = (long)obj[StringTable.DoubanUserId],
                ExpiresIn = (long)obj[StringTable.ExpiresIn],
                RefreshToken = (string)obj[StringTable.RefreshToken],
            };
        }

        public abstract Task<UserInfo> Authenticate();
    }
}
