using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    internal partial class Player : IPlayer
    {
        private readonly IServerConnection _serverConnection;
        private PlayerState _state;
        private Channel _currentChannel;
        private Song _currentSong;

        public Song CurrentSong
        {
            get { return _currentSong; }
            protected set
            {
                if (_currentSong != value)
                {
                    _currentSong = value;
                    OnSongChanged(new EventArgs<Song>(_currentSong));
                    if (_currentSong == null)
                    {
                        State = PlayerState.Stoped;
                    }
                }
            }
        }

        public Queue<Song> NextSongs { get; protected set; }

        public ChannelList ChannelList { get; protected set; }

        public Channel CurrentChannel
        {
            get { return _currentChannel; }
#pragma warning disable 4014
            set { ChangeChannel(value); }
#pragma warning restore 4014
        }

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

        public async Task ChangeChannel(Channel channel)
        {
            if (_currentChannel != channel)
            {
                _currentChannel = channel;
                OnChannelChanged(new EventArgs<Channel>(_currentChannel));
                if (_currentChannel == null)
                {
                    ClearSongs();
                }
                else
                {
                    await Report(ReportType.NewChannel, true);
                }
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

        protected virtual void OnSongChanged(EventArgs<Song> e)
        {
            SongChanged?.Invoke(this, e);
        }

        protected virtual void OnChannelChanged(EventArgs<Channel> e)
        {
            ChannelChanged?.Invoke(this, e);
        }

        private async Task Report(ReportType type, bool changeCurrentSong)
        {
            var newPlayList = await GetPlayList(type);
            if (type == ReportType.SongEnded) return;
            ChangePlayListSongs(newPlayList);
            if (changeCurrentSong)
            {
                await ChangeCurrentSong();
            }
        }

        private async Task ChangeCurrentSong()
        {
            while (NextSongs == null || NextSongs.Count == 0)
            {
                var newPlayList = await GetPlayList(ReportType.PlayListEmpty);
                ChangePlayListSongs(newPlayList);
            }
            CurrentSong = NextSongs.Dequeue();
        }

        private void ChangePlayListSongs(Song[] newPlayList)
        {
            if (NextSongs == null)
            {
                NextSongs = new Queue<Song>();
            }
            NextSongs.Clear();
            foreach (var song in newPlayList)
            {
                NextSongs.Enqueue(song);
            }
        }

        private void ClearSongs()
        {
            CurrentSong = null;
            NextSongs = null;
        }
    }
}