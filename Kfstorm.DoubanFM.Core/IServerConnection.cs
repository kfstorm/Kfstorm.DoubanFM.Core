using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public interface IServerConnection
    {
        IDictionary<string, string> Context { get; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string AppName { get; set; }
        string AppVersion { get; set; }
        Uri RedirectUri { get; set; }
        string AccessToken { get; set; }

        Task<string> Get(Uri uri);
        Task<string> Get(Uri uri, Action<HttpWebRequest> modifier);
        Task<string> Post(Uri uri, byte[] data);
        Task<string> Post(Uri uri, byte[] data, Action<HttpWebRequest> modifier);
    }
}