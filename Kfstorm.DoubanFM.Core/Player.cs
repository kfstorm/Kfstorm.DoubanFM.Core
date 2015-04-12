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

        private Song _currentSong;

        public ISession Session { get; }

        public IServerConnection ServerConnection { get; }

        public Song CurrentSong
        {
            get { return _currentSong; }
            protected set
            {
                if (_currentSong != value)
                {
                    _currentSong = value;
                    OnCurrentSongChanged(new EventArgs<Song>(_currentSong));
                }
            }
        }

        protected Queue<Song> NextSongs { get; set; }

        public ChannelList ChannelList { get; protected set; }

        public Channel CurrentChannel { get; protected set; }

        public IDictionary<string, object> Config { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public event EventHandler<EventArgs<Song>> CurrentSongChanged;
        public event EventHandler<EventArgs<Channel>> CurrentChannelChanged;

        public Player(IServerConnection serverConnection, ISession session)
        {
            ServerConnection = serverConnection;
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
            await LogExceptionIfAny(Logger, async () =>
            {
                switch (type)
                {
                    case NextCommandType.CurrentSongEnded:
                        await Report(ReportType.CurrentSongEnded);
                        break;
                    case NextCommandType.SkipCurrentSong:
                        await Report(ReportType.SkipCurrentSong);
                        break;
                    case NextCommandType.BanCurrentSong:
                        await Report(ReportType.BanCurrentSong);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("type");
                };
            });
        }

        public async Task ChangeChannel(Channel newChannel)
        {
            await LogExceptionIfAny(Logger, async () =>
            {
                if (CurrentChannel != newChannel)
                {
                    CurrentChannel = newChannel;
                    OnCurrentChannelChanged(new EventArgs<Channel>(CurrentChannel));
                    if (CurrentChannel == null)
                    {
                        ClearSongs();
                    }
                    else
                    {
                        await Report(ReportType.NewChannel);
                    }
                }
            });
        }

        public async Task SetRedHeart(bool redHeart)
        {
            await IgnoreExceptionIfAny(Logger, async () => await Report(redHeart ? ReportType.Like : ReportType.CancelLike));
            CurrentSong.Like = redHeart;
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

        private async Task Report(ReportType type)
        {
            try
            {
                var changeCurrentSong = type == ReportType.SkipCurrentSong || type == ReportType.NewChannel || type == ReportType.BanCurrentSong;
                var newPlayList = await GetPlayList(type);
                if (type == ReportType.CurrentSongEnded) return;
                ChangePlayListSongs(newPlayList);
                if (changeCurrentSong)
                {
                    await ChangeCurrentSong();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to send report to server. Report type: {type}. Current channel: {CurrentChannel}. Current song: {CurrentSong}.", ex);
                throw;
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