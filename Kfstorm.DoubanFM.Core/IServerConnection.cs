using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public interface IServerConnection
    {
        IDictionary<string, string> Context { get; }
        Task<string> Get(Uri uri);
        Task<string> Post(Uri uri, byte[] data);
    }
}