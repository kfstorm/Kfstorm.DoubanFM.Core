using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public interface ISession
    {
        UserInfo UserInfo { get; }

        IServerConnection ServerConnection { get; }

        Task LogOn(IAuthentication authentication);
        void LogOff();
    }
}