using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public interface ISession
    {
        UserInfo UserInfo { get; }

        Task<string> LogOn(IAuthentication authentication);
        Task<string> LogOff(IAuthentication authentication);
    }
}