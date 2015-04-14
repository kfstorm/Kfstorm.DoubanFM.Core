using System;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public class PasswordAuthentication : AuthenticationBase
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public override async Task<UserInfo> Authenticate()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                throw new InvalidOperationException("Username is empty");
            }
            if (string.IsNullOrEmpty(Password))
            {
                throw new InvalidOperationException("Password is empty");
            }

            Logger.Info("Start password authentication.");
            var uriBuilder = new UriBuilder("https://www.douban.com/service/auth2/token");
            uriBuilder.AppendAuthenticationCommonFields(ServerConnection);
            uriBuilder.AppendQuery(StringTable.GrantType, StringTable.Password);
            uriBuilder.AppendQuery(StringTable.Username, Username);
            uriBuilder.AppendQuery(StringTable.Password, Password);
            var data = uriBuilder.RemoveQuery();
            var jsonContent = await ServerConnection.Post(uriBuilder.Uri, data);
            return ParseLogOnResult(jsonContent);
        }

        public PasswordAuthentication(IServerConnection serverConnection) : base(serverConnection)
        {
        }
    }
}
