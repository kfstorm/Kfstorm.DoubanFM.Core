using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using log4net;
using static Kfstorm.DoubanFM.Core.ExceptionHelper;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// The default implementation of <see cref="IServerConnection"/>
    /// </summary>
    public class ServerConnection : IServerConnection
    {
        /// <summary>
        /// The logger
        /// </summary>
        protected ILog Logger = LogManager.GetLogger(typeof(ServerConnection));

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConnection"/> class.
        /// </summary>
        /// <param name="clientId">The client ID.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="appName">Name of the application.</param>
        /// <param name="appVersion">The application version.</param>
        /// <param name="redirectUri">The redirect URI.</param>
        /// <param name="udid">The UDID.</param>
        public ServerConnection(string clientId, string clientSecret, string appName, string appVersion, Uri redirectUri, string udid)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            AppName = appName;
            AppVersion = appVersion;
            RedirectUri = redirectUri;
            Udid = udid;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConnection"/> class.
        /// </summary>
        public ServerConnection()
        {
        }

        /// <summary>
        /// Gets the context. The context contains contextual information about server connection, such as client ID and access token.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public IDictionary<string, string> Context { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the client ID.
        /// </summary>
        /// <value>
        /// The client ID.
        /// </value>
        public string ClientId
        {
            get { return GetContextOptional(StringTable.ClientId); }
            set { Context[StringTable.ClientId] = value; }
        }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>
        /// The client secret.
        /// </value>
        public string ClientSecret
        {
            get { return GetContextOptional(StringTable.ClientSecret); }
            set { Context[StringTable.ClientSecret] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        public string AppName
        {
            get { return GetContextOptional(StringTable.AppName); }
            set { Context[StringTable.AppName] = value; }
        }

        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        public string AppVersion
        {
            get { return GetContextOptional(StringTable.Version); }
            set { Context[StringTable.Version] = value; }
        }

        /// <summary>
        /// Gets or sets the redirect URI.
        /// </summary>
        /// <value>
        /// The redirect URI.
        /// </value>
        public Uri RedirectUri
        {
            get
            {
                var uri = GetContextOptional(StringTable.RedirectUri);
                return uri == null ? null : new Uri(uri);
            }
            set { Context[StringTable.RedirectUri] = value?.AbsoluteUri; }
        }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public string AccessToken
        {
            get { return GetContextOptional(StringTable.AccessToken); }
            set { Context[StringTable.AccessToken] = value; }
        }

        /// <summary>
        /// Gets or sets the UDID.
        /// </summary>
        /// <value>
        /// The UDID.
        /// </value>
        public string Udid
        {
            get { return GetContextOptional(StringTable.Udid); }
            set { Context[StringTable.Udid] = value; }
        }

        /// <summary>
        /// Gets the optional context.
        /// </summary>
        /// <param name="name">The name of the context.</param>
        /// <returns>The optional context.</returns>
        protected virtual string GetContextOptional(string name)
        {
            string temp;
            if (Context.TryGetValue(name, out temp))
            {
                return temp;
            }
            return null;
        }

        /// <summary>
        /// Creates an HTTP request.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        protected virtual HttpWebRequest CreateRequest(Uri uri)
        {
            return WebRequest.CreateHttp(uri);
        }

        /// <summary>
        /// Sets the session information to request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void SetSessionInfoToRequest(HttpWebRequest request)
        {
            var uri = request.RequestUri;
            if (!string.IsNullOrEmpty(AccessToken) && uri.Host.Equals("api.douban.com", StringComparison.OrdinalIgnoreCase))
            {
                request.Headers["Authorization"] = "Bearer " + AccessToken;
            }
        }

        /// <summary>
        /// Send an HTTP GET request to the specified URI, and get the response content as string.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="modifier">The modifier to change the request before sending. The modifier can be null.</param>
        /// <returns>The content of response.</returns>
        public async Task<string> Get(Uri uri, Action<HttpWebRequest> modifier)
        {
            Logger.Debug($"GET: {uri}");
            return await LogExceptionIfAny(Logger, () => ServerException.TryThrow(async () =>
            {
                var request = CreateRequest(uri);
                request.Method = WebRequestMethods.Http.Get;
                modifier?.Invoke(request);
                var response = await request.GetResponseAsync();
                var responseStream = response.GetResponseStream();
                // ReSharper disable once AssignNullToNotNullAttribute
                var reader = new StreamReader(responseStream, Encoding.UTF8);
                var content = await reader.ReadToEndAsync();
                Logger.Debug($"Response: {content}");
                ServerException.TryThrow(content);
                return content;
            }));
        }

        /// <summary>
        /// Send an HTTP POST request to the specified URI, and get the response content as string.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="data">The binary data need to be posted. Null or empty array means no data.</param>
        /// <returns>The content of response.</returns>
        public virtual async Task<string> Post(Uri uri, byte[] data)
        {
            return await Post(uri, data, null);
        }

        /// <summary>
        /// Send an HTTP POST request to the specified URI, and get the response content as string.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="data">The binary data need to be posted. Null or empty array means no data.</param>
        /// <param name="modifier">The modifier to change the request before sending. The modifier can be null.</param>
        /// <returns>The content of response.</returns>
        public async Task<string> Post(Uri uri, byte[] data, Action<HttpWebRequest> modifier)
        {
            var dataLength = data?.Length ?? 0;
            Logger.Debug($"POST: {uri}. Data length: {dataLength}");
            return await LogExceptionIfAny(Logger, () => ServerException.TryThrow(async () =>
            {
                var request = CreateRequest(uri);
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = "application/x-www-form-urlencoded";
                modifier?.Invoke(request);
                if (data != null && data.Length > 0)
                {
                    var requestStream = await request.GetRequestStreamAsync();
                    requestStream.Write(data, 0, data.Length);
                }
                var response = await request.GetResponseAsync();
                var responseStream = response.GetResponseStream();
                // ReSharper disable once AssignNullToNotNullAttribute
                var reader = new StreamReader(responseStream, Encoding.UTF8);
                var content = await reader.ReadToEndAsync();
                Logger.Debug($"Response: {content}");
                ServerException.TryThrow(content);
                return content;
            }));
        }
    }
}