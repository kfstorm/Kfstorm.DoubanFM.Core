using System;

namespace Kfstorm.DoubanFM.Core
{
    internal class Player : IPlayer
    {
        private readonly IServerConnection _serverConnection;
        public Song CurerntSong { get; protected set; }

        public Song[] NextSongs { get; protected set; }

        public ChannelList ChannelList { get; protected set; }

        public Player(IServerConnection serverConnection)
        {
            _serverConnection = serverConnection;
        }

        public void Skip()
        {
            throw new NotImplementedException();
        }

        public void Ban()
        {
            throw new NotImplementedException();
        }

        public void Next()
        {
            throw new NotImplementedException();
        }

        public void SetRedHeart(bool redHeart)
        {
            throw new NotImplementedException();
        }
    }
}