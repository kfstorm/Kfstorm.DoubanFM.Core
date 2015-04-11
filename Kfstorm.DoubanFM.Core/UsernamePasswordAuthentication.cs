using System.Text;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public class UsernamePasswordAuthentication : AuthenticationBase
    {
        private readonly IServerConnection _serverConnection;
        const string LogOnUrl = "https://www.douban.com/service/auth2/token";

        public UsernamePasswordAuthentication(IServerConnection serverConnection)
        {
            _serverConnection = serverConnection;
        }

        public string Username { get; set; }

        public string Password { get; set; }

        protected virtual byte[] GetLogOnPostData()
        {
            var content = $"%3F_v=12674&client_id={_serverConnection.ClientId}&client_secret={_serverConnection.ClientSecret}&grant_type=password&password={Password}&redirect_uri=http%3A%2F%2Fwww.douban.com%2Fmobile%2Ffm&username={Password}";
            return Encoding.ASCII.GetBytes(content);
        }

        public override async Task<LogOnResult> Authenticate()
        {
            var data = GetLogOnPostData();
            var jsonContent = await _serverConnection.Post(LogOnUrl, data);
            return ParseLogOnResult(jsonContent);
        }

        public override Task<bool> UnAuthenticate()
        {
            return Task.FromResult(true);
        }
    }
}
