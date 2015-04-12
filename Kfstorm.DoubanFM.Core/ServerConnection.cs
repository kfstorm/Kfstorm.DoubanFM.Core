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

        public ServerConnection(string clientId, string clientSecret, Uri redirectUri)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            RedirectUri = redirectUri;
        }

        public ServerConnection()
        {
        }

        public IDictionary<string, string> Context { get; } = new Dictionary<string, string>();

        public string ClientId
        {
            get { return Context[StringTable.ClientId]; }
            set { Context[StringTable.ClientId] = value; }
        }

        public string ClientSecret
        {
            get { return Context[StringTable.ClientSecret]; }
            set { Context[StringTable.ClientSecret] = value; }
        }

        public Uri RedirectUri
        {
            get { return new Uri(Context[StringTable.RedirectUri]); }
            set { Context[StringTable.RedirectUri] = value?.AbsoluteUri; }
        }

        public virtual Task<string> Get(Uri uri)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<string> Post(Uri uri, byte[] data)
        {
            if (data == null)
            {
                data = new byte[0];
            }
            Logger.Debug($"POST: {uri}. Data length: {data.Length}");
            return await LogExceptionIfAny(Logger, async () =>
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = "application/x-www-form-urlencoded";
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
                return content;
            });
        }
    }
}