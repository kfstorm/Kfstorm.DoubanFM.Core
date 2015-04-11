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

        public ServerConnection(string clientId, string clientSecret)
        {
            Context[StringTable.ClientId] = clientId;
            Context[StringTable.ClientSecret] = clientSecret;
        }

        public IDictionary<string, string> Context { get; } = new Dictionary<string, string>();

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
                Logger.Info($"Response: {content}");
                return content;
            });
        }
    }
}