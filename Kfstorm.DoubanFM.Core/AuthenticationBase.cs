using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Kfstorm.DoubanFM.Core
{
    public abstract class AuthenticationBase : IAuthentication
    {
        protected LogOnResult ParseLogOnResult(string jsonContent)
        {
            var obj = JObject.Parse(jsonContent);
            return new LogOnResult
            {
                UserInfo = new UserInfo
                {
                    AccessToken = (string)obj[StringTable.AccessToken],
                    Username = (string)obj[StringTable.DoubanUsername],
                    UserId = (long)obj[StringTable.DoubanUserId],
                    ExpiresIn = (long)obj[StringTable.ExpiresIn],
                    RefreshToken = (string)obj[StringTable.RefreshToken],
                }
            };
        }

        public abstract Task<LogOnResult> Authenticate();
        public abstract Task<string> UnAuthenticate();
    }
}
