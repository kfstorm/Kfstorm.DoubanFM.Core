using System;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public class UsernamePasswordAuthentication : IAuthentication
    {
        private readonly IServerConnection _serverConnection;
        const string LogOnUrl = "https://www.douban.com/service/auth2/token";

        public UsernamePasswordAuthentication(IServerConnection serverConnection)
        {
            _serverConnection = serverConnection;
        }

        public string Username { get; set; }

        public string Password { get; set; }

        protected virtual string GetLogOnPostData(string clientId, string clientSecret)
        {
            return $"%3F_v=12674&client_id={clientId}&client_secret={clientSecret}&grant_type=password&password={Password}&redirect_uri=http%3A%2F%2Fwww.douban.com%2Fmobile%2Ffm&username={Password}";
        }

        public async Task<LogOnResult> Authenticate()
        {
            throw new NotImplementedException();
        }
    }
}
