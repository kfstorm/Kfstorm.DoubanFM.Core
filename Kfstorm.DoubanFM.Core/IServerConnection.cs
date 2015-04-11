using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public interface IServerConnection
    {
        string ClientId { get; }
        string ClientSecret { get; }

        Task<string> Get(string url);
        Task<string> Post(string url, byte[] data);
    }
}