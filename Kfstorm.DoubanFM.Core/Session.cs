using System;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public class Session : ISession
    {
        private SessionState _state;

        public SessionState State
        {
            get { return _state; }
            protected set
            {
                if (_state != value)
                {
                    _state = value;
                    OnStateChanged(new EventArgs<SessionState>(_state));
                }
            }
        }

        public event EventHandler<EventArgs<SessionState>> StateChanged;

        public UserInfo UserInfo { get; protected set; }

        public async Task<bool> LogOn(IAuthentication authentication)
        {
            if (State == SessionState.LoggedOn)
            {
                throw new InvalidOperationException("Already logged on");
            }
            State = SessionState.LoggingOn;
            var result = await authentication.Authenticate();
            if (result.UserInfo == null)
            {
                State = SessionState.LoggedOff;
                return false;
            }
            UserInfo = result.UserInfo;
            State = SessionState.LoggedOn;
            return true;
        }

        public void LogOff()
        {
            UserInfo = null;
            State = SessionState.LoggingOff;
            State = SessionState.LoggedOff;
        }

        protected virtual void OnStateChanged(EventArgs<SessionState> e)
        {
            StateChanged?.Invoke(this, e);
        }
    }
}
