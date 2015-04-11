using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public interface ISession
    {
        UserInfo UserInfo { get; }

        Task<bool> LogOn(IAuthentication authentication);
        Task<bool> LogOff(IAuthentication authentication);
    }
}