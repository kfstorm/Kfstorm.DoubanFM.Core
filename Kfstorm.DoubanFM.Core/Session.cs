using System;

namespace Kfstorm.DoubanFM.Core
{
    internal class Session : ISession
    {
        private readonly IServerConnection _serverConnection;
        public Session(IServerConnection serverConnection)
        {
            _serverConnection = serverConnection;
        }

        public SessionState State { get; protected set; }

        public void LogOn()
        {
            throw new NotImplementedException();
        }

        public void LogOff()
        {
            throw new NotImplementedException();
        }
    }
}