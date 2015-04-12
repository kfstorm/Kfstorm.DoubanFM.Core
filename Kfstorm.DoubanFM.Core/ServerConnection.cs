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
    public class ServerConnection : IServerConnection
    {
        protected ILog Logger = LogManager.GetLogger(typeof(ServerConnection));

        public ServerConnection(string clientId, string clientSecret, string appName, string appVersion, Uri redirectUri)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            AppName = appName;
            AppVersion = appVersion;
            RedirectUri = redirectUri;
        }

        public ServerConnection()
        {
        }

        public IDictionary<string, string> Context { get; } = new Dictionary<string, string>();

        public string ClientId
        {
            get { return GetContextOptional(StringTable.ClientId); }
            set { Context[StringTable.ClientId] = value; }
        }

        public string ClientSecret
        {
            get { return GetContextOptional(StringTable.ClientSecret); }
            set { Context[StringTable.ClientSecret] = value; }
        }

        public string AppName
        {
            get { return GetContextOptional(StringTable.AppName); }
            set { Context[StringTable.AppName] = value; }
        }

        public string AppVersion
        {
            get { return GetContextOptional(StringTable.Version); }
            set { Context[StringTable.Version] = value; }
        }

        public Uri RedirectUri
        {
            get
            {
                var uri = GetContextOptional(StringTable.RedirectUri);
                return uri == null ? null : new Uri(uri);
            }
            set { Context[StringTable.RedirectUri] = value?.AbsoluteUri; }
        }

        public string AccessToken
        {
            get { return GetContextOptional(StringTable.AccessToken); }
            set { Context[StringTable.AccessToken] = value; }
        }

        protected virtual string GetContextOptional(string name)
        {
            string temp;
            if (Context.TryGetValue(name, out temp))
            {
                return temp;
            }
            return null;
        }

        public virtual async Task<string> Get(Uri uri)
        {
            return await Get(uri, null);
        }

        public async Task<string> Get(Uri uri, Action<HttpWebRequest> modifier)
        {
            Logger.Debug($"GET: {uri}");
            return await LogExceptionIfAny(Logger, () => ServerException.TryThrow(async () =>
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = WebRequestMethods.Http.Get;
                modifier?.Invoke(request);
                var response = await request.GetResponseAsync();
                var responseStream = response.GetResponseStream();
                var reader = new StreamReader(responseStream, Encoding.UTF8);
                var content = await reader.ReadToEndAsync();
                Logger.Debug($"Response: {content}");
                ServerException.TryThrow(content);
                return content;
            }));
        }

        public virtual async Task<string> Post(Uri uri, byte[] data)
        {
            return await Post(uri, data, null);
        }

        public async Task<string> Post(Uri uri, byte[] data, Action<HttpWebRequest> modifier)
        {
            if (data == null)
            {
                data = new byte[0];
            }
            Logger.Debug($"POST: {uri}. Data length: {data.Length}");
            return await LogExceptionIfAny(Logger, () => ServerException.TryThrow(async () =>
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = "application/x-www-form-urlencoded";
                modifier?.Invoke(request);
                if (data.Length > 0)
                {
                    var requestStream = await request.GetRequestStreamAsync();
                    requestStream.Write(data, 0, data.Length);
                }
                var response = await request.GetResponseAsync();
                var responseStream = response.GetResponseStream();
                var reader = new StreamReader(responseStream, Encoding.UTF8);
                var content = await reader.ReadToEndAsync();
                Logger.Debug($"Response: {content}");
                ServerException.TryThrow(content);
                return content;
            }));
        }
    }
}