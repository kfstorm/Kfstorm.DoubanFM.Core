using System;

namespace Kfstorm.DoubanFM.Core
{
    public class Client : IClient
    {
        public IPlayer Player { get; }

        public ISession Session { get; }

        public IServerConnection ServerConnection { get; }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public Client(ISession session, IServerConnection serverConnection)
        {
            ServerConnection = serverConnection;
            Player = new Player(serverConnection);
            Session = session;
        }
    }
}