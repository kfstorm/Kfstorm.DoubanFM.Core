using System;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// OAuth authentication method
    /// </summary>
    public class OAuthAuthentication : AuthenticationBase
    {
        /// <summary>
        /// Gets or sets the redirect URI.
        /// </summary>
        /// <value>
        /// The redirect URI.
        /// </value>
        public Uri RedirectUri => ServerConnection.RedirectUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthAuthentication" /> class.
        /// </summary>
        /// <param name="serverConnection">The server connection.</param>
        public OAuthAuthentication(IServerConnection serverConnection) : base(serverConnection)
        {
        }

        /// <summary>
        /// Gets or sets the delegate of getting the redirected URI.
        /// </summary>
        /// <value>
        /// The delegate of getting the redirected URI.
        /// </value>
        public Func<Uri, Task<Uri>> GetRedirectedUri { get; set; }

        /// <summary>
        /// Authenticates and returns user info.
        /// </summary>
        /// <returns>
        /// The user info, including username and token.
        /// </returns>
        public override async Task<UserInfo> Authenticate()
        {
            Logger.Info("Start OAuth authentication.");
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
