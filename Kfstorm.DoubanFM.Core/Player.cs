using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using static Kfstorm.DoubanFM.Core.ExceptionHelper;

namespace Kfstorm.DoubanFM.Core
{
    public partial class Player : IPlayer
    {
        protected ILog Logger = LogManager.GetLogger(typeof(Player));

        private volatile Song _currentSong;

        private volatile Channel _currentChannel;

        protected object StateLock = new object();

        public ISession Session { get; }

        public IServerConnection ServerConnection { get; }

        public Song CurrentSong
        {
            get { return _currentSong; }
            protected set
            {
                if (_currentSong != value)
                {
                    lock (StateLock)
                    {
                        _currentSong = value;
                    }
                    OnCurrentSongChanged(new EventArgs<Song>(_currentSong));
                }
            }
        }

        private Queue<Song> _nextSongs;

        public ChannelList ChannelList { get; protected set; }

        public Channel CurrentChannel
        {
            get { return _currentChannel; }
            protected set
            {
                if (_currentChannel != value)
                {
                    _currentChannel = value;
                    OnCurrentChannelChanged(new EventArgs<Channel>(_currentChannel));
                }
            }
        }

        protected int? AsyncExpectedChannelId { get; set; }

        public IDictionary<string, object> Config { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public event EventHandler<EventArgs<Song>> CurrentSongChanged;
        public event EventHandler<EventArgs<Channel>> CurrentChannelChanged;

        public Player(ISession session)
        {
            ServerConnection = session.ServerConnection;
            Session = session;
        }

        public async Task Initialize()
        {
            if (ChannelList != null)
            {
                throw new InvalidOperationException("Already initialized");
            }
            while (ChannelList == null)
            {
                await LogExceptionIfAny(Logger, RefreshChannelList, "Failed to initialize.");
            }
            Logger.Info("Initialized.");
        }

        public async Task RefreshChannelList()
        {
            ChannelList = await GetChannelList();
        }

        public async Task Next(NextCommandType type)
        {
            ThrowExceptionIfNoChannel();
            ThrowExceptionIfNoSong();
            await LogExceptionIfAny(Logger, async () =>
            {
                switch (type)
                {
                    case NextCommandType.CurrentSongEnded:
                        await Report(ReportType.CurrentSongEnded, CurrentChannel.Id, CurrentSong?.Sid);
                        break;
                    case NextCommandType.SkipCurrentSong:
                        await Report(ReportType.SkipCurrentSong, CurrentChannel.Id, CurrentSong?.Sid);
                        break;
                    case NextCommandType.BanCurrentSong:
                        await Report(ReportType.BanCurrentSong, CurrentChannel.Id, CurrentSong?.Sid);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type));
                }
            });
        }

        public async Task ChangeChannel(Channel newChannel)
        {
            if (CurrentChannel != newChannel)
            {
                AsyncExpectedChannelId = newChannel?.Id;
                CurrentChannel = null;
                CurrentSong = null;
                if (newChannel != null)
                {
                    await LogExceptionIfAny(Logger, async () =>
                    {
                        await Report(ReportType.NewChannel, newChannel.Id, CurrentSong?.Sid);

                        /*
                        If user called ChangeChannel twice in a short time, say call 1 and call 2.
                        But call 2 responded before call 1. Then we want to use call 2's response.
                        So we need to check AsyncExpectedChannelId here because it should be call 2's ID.
                        */
                        if (AsyncExpectedChannelId == newChannel.Id)
                        {
                            CurrentChannel = newChannel;
                        }
                    });
                }
            }
        }

        public async Task SetRedHeart(bool redHeart)
        {
            ThrowExceptionIfNoChannel();
            ThrowExceptionIfNoSong();
            var sid = CurrentSong.Sid;
            await IgnoreExceptionIfAny(Logger, async () => await Report(redHeart ? ReportType.Like : ReportType.CancelLike, CurrentChannel.Id, CurrentSong?.Sid));
            if (CurrentSong != null && CurrentSong.Sid == sid)
            {
                CurrentSong.Like = redHeart;
            }
        }

        public void Dispose()
        {
            // TODO
        }

        protected virtual void OnCurrentSongChanged(EventArgs<Song> e)
        {
            Logger.Info($"Current song changed. {e.Object}");
            CurrentSongChanged?.Invoke(this, e);
        }

        protected virtual void OnCurrentChannelChanged(EventArgs<Channel> e)
        {
            Logger.Info($"Current channel changed. {e.Object}");
            CurrentChannelChanged?.Invoke(this, e);
        }

        private async Task Report(ReportType type, int channelId, string sid)
        {
            try
            {
                var changeCurrentSong = type == ReportType.SkipCurrentSong || type == ReportType.NewChannel || type == ReportType.BanCurrentSong || type == ReportType.CurrentSongEnded;
                if (changeCurrentSong)
                {
                    CurrentSong = null;
                }
                var newPlayList = await GetPlayList(type, channelId, sid);
                if (channelId == AsyncExpectedChannelId)
                {
                    if (type != ReportType.CurrentSongEnded)
                    {
                        ChangePlayListSongs(newPlayList);
                    }
                    if (changeCurrentSong)
                    {
                        await ChangeCurrentSong(channelId, sid);
                    }
                }
                else
                {
                    // TODO: throw exception or not?
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to send report to server. Report type: {type}. Current channel: {CurrentChannel}. Current song: {CurrentSong}.", ex);
                throw;
            }
        }

        private async Task ChangeCurrentSong(int channelId, string sid)
        {
            CurrentSong = null;
            while (_nextSongs == null || _nextSongs.Count == 0)
            {
                var newPlayList = await GetPlayList(ReportType.PlayListEmpty, channelId, sid);
                if (channelId == AsyncExpectedChannelId)
                {
                    ChangePlayListSongs(newPlayList);
                }
                else
                {
                    // TODO: throw exception or not?
                    return;
                }
            }
            CurrentSong = _nextSongs.Dequeue();
        }

        private void ChangePlayListSongs(Song[] newPlayList)
        {
            if (_nextSongs == null)
            {
                _nextSongs = new Queue<Song>();
            }
            _nextSongs.Clear();
            foreach (var song in newPlayList)
            {
                _nextSongs.Enqueue(song);
            }
        }

        private void ThrowExceptionIfNoChannel()
        {
            if (CurrentChannel == null)
            {
                throw new ChannelNotSelectedException();
            }
        }

        private void ThrowExceptionIfNoSong()
        {
            if (CurrentSong == null)
            {
                throw new SongNotSelectedException();
            }
        }
    }
}