using System;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public interface ISession
    {
        SessionState State { get; }
        UserInfo UserInfo { get; }

        event EventHandler<EventArgs<SessionState>> StateChanged;

        Task<bool> LogOn(IAuthentication authentication);
        void LogOff();
    }
}