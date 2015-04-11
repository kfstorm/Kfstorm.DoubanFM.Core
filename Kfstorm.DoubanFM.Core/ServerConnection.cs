using System;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public class ServerConnection : IServerConnection
    {
        public ServerConnection(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        public string ClientId { get; }
        public string ClientSecret { get; }

        public Task<string> Get(string url)
        {
            throw new NotImplementedException();
        }

        public Task<string> Post(string url, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}