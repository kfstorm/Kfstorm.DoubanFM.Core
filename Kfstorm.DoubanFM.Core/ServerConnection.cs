using System;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public class ServerConnection : IServerConnection
    {
        public Task<string> Get(string url)
        {
            throw new NotImplementedException();
        }
    }
}