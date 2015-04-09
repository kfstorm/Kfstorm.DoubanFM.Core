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

        public Client(IServerConnection serverConnection)
        {
            ServerConnection = serverConnection;
            Player = new Player(serverConnection);
            Session = new Session(serverConnection);
        }

        public Client() : this(new ServerConnection())
        {
        }
    }
}