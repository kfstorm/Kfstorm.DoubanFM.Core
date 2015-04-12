using System;
using System.Threading.Tasks;
using log4net;

namespace Kfstorm.DoubanFM.Core
{
    public class OAuthAuthentication : AuthenticationBase
    {
        protected ILog Logger = LogManager.GetLogger(typeof(OAuthAuthentication));
        private readonly IServerConnection _serverConnection;

        public Uri RedirectUri => _serverConnection.RedirectUri;

        public OAuthAuthentication(IServerConnection serverConnection)
        {
            _serverConnection = serverConnection;
        }

        public Func<Uri, Task<Uri>> GetRedirectedUri { get; set; }

        public override async Task<LogOnResult> Authenticate()
        {
            var uriBuilder = new UriBuilder("https://www.douban.com/service/auth2/auth");
            uriBuilder.AppendQuery(StringTable.ClientId, _serverConnection.ClientId);
            uriBuilder.AppendQuery(StringTable.RedirectUri, _serverConnection.RedirectUri.AbsoluteUri);
            uriBuilder.AppendQuery(StringTable.ResponseType, StringTable.Code);
            try
            {
                var redirectedUri = await GetRedirectedUri(uriBuilder.Uri);
                if (redirectedUri != null)
                {
                    var queries = redirectedUri.GetQueries();
                    string code;
                    if (queries.TryGetValue(StringTable.Code, out code) && !string.IsNullOrEmpty(code))
                    {
                        var tokenUrl = new UriBuilder("https://www.douban.com/service/auth2/token");
                        tokenUrl.AppendQuery(StringTable.ClientId, _serverConnection.ClientId);
                        tokenUrl.AppendQuery(StringTable.ClientSecret, _serverConnection.ClientSecret);
                        tokenUrl.AppendQuery(StringTable.RedirectUri, _serverConnection.RedirectUri.AbsoluteUri);
                        tokenUrl.AppendQuery(StringTable.GrantType, StringTable.AuthorizationCode);
                        tokenUrl.AppendQuery(StringTable.Code, code);
                        var jsonContent = await _serverConnection.Post(tokenUrl.Uri, null);
                        return ParseLogOnResult(jsonContent);
                    }
                    string error;
                    if (queries.TryGetValue(StringTable.Error, out error))
                    {
                        return CreateInternalErrorLogOnResult(error);
                    }
                }
                return CreateInternalErrorLogOnResult("Unknown redirect URL.");
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occurred when trying to authenticate.", ex);
                return CreateInternalErrorLogOnResult(ex.Message);
            }
        }

        private LogOnResult CreateInternalErrorLogOnResult(string message)
        {
            return new LogOnResult { ErrorCode = -1, ErrorMessage = message };
        }

        public override Task<string> UnAuthenticate()
        {
            return Task.FromResult((string)null);
        }
    }
}
