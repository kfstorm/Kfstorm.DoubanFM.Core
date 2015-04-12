using System;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public class OAuthAuthentication : AuthenticationBase
    {
        public Uri RedirectUri => ServerConnection.RedirectUri;

        public OAuthAuthentication(IServerConnection serverConnection) : base(serverConnection)
        {
        }

        public Func<Uri, Task<Uri>> GetRedirectedUri { get; set; }

        public override async Task<UserInfo> Authenticate()
        {

            var uriBuilder = new UriBuilder("https://www.douban.com/service/auth2/auth");
            uriBuilder.AppendAuthenticationCommonFields(ServerConnection);
            uriBuilder.AppendQuery(StringTable.ResponseType, StringTable.Code);
            var redirectedUri = await GetRedirectedUri(uriBuilder.Uri);
            if (redirectedUri != null)
            {
                var queries = redirectedUri.GetQueries();
                string code;
                if (queries.TryGetValue(StringTable.Code, out code) && !string.IsNullOrEmpty(code))
                {
                    var tokenUrl = new UriBuilder("https://www.douban.com/service/auth2/token");
                    tokenUrl.AppendAuthenticationCommonFields(ServerConnection);
                    tokenUrl.AppendQuery(StringTable.GrantType, StringTable.AuthorizationCode);
                    tokenUrl.AppendQuery(StringTable.Code, code);
                    var jsonContent = await ServerConnection.Post(tokenUrl.Uri, null);
                    return ParseLogOnResult(jsonContent);
                }
                string error;
                if (queries.TryGetValue(StringTable.Error, out error))
                {
                    throw new ServerException(-1, error);
                }
            }
            throw new OperationCanceledException("User cancelled OAuth.");
        }
    }
}
