using System;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    internal partial class Player : IPlayer
    {
        private readonly IServerConnection _serverConnection;
        private PlayerState _state;

        public Song CurerntSong { get; protected set; }

        public Song[] NextSongs { get; protected set; }

        public ChannelList ChannelList { get; protected set; }

        public Channel CurrentChannel { get; set; }

        public PlayerState State
        {
            get { return _state; }
            protected set
            {
                if (_state != value)
                {
                    _state = value;
                    OnStateChanged(new EventArgs<PlayerState>(value));
                }
            }
        }

        public event EventHandler<EventArgs<Song>> SongChanged;
        public event EventHandler<EventArgs<Channel>> ChannelChanged;
        public event EventHandler<EventArgs<PlayerState>> StateChanged;

        public Player(IServerConnection serverConnection)
        {
            _serverConnection = serverConnection;
        }

        public async Task StartInitialize()
        {
            if (State != PlayerState.NotInitialized)
            {
                throw new InvalidOperationException("Player already initialized");
            }
            State = PlayerState.Initializing;
            var channelList = await GetChannelList();
            if (channelList != null)
            {
                ChannelList = channelList;
                State = PlayerState.Stoped;
            }
            else
            {
                State = PlayerState.NotInitialized;
            }
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

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnStateChanged(EventArgs<PlayerState> e)
        {
            StateChanged?.Invoke(this, e);
        }
    }
}