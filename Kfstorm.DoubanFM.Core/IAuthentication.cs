using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public interface IAuthentication
    {
        Task<LogOnResult> Authenticate();
    }
}
