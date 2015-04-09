using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public interface IServerConnection
    {
        Task<string> Get(string url);
    }
}