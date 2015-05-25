using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// The default implementation of <see cref="IPlayer" /> interface
    /// </summary>
    public class Player : IPlayer
    {
        private volatile Song _currentSong;

        private volatile Channel _currentChannel;

        /// <summary>
        /// The state lock
        /// </summary>
        protected object StateLock = new object();

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public ISession Session { get; }

        /// <summary>
        /// Gets the server connection.
        /// </summary>
        /// <value>
        /// The server connection.
        /// </value>
        public IServerConnection ServerConnection => Session.ServerConnection;

        /// <summary>
        /// Gets the current song.
        /// </summary>
        /// <value>
        /// The current song.
        /// </value>
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

        /// <summary>
        /// Stores a list of songs will be played next.
        /// </summary>
        private Queue<Song> _pendingSongs;

        /// <summary>
        /// Gets the current channel.
        /// </summary>
        /// <value>
        /// The current channel.
        /// </value>
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

        /// <summary>
        /// Gets or sets the asynchronous expected channel ID.
        /// </summary>
        /// <value>
        /// The asynchronous expected channel ID.
        /// </value>
        /// <remarks>
        /// This property will be set to the new channel ID when changing current channel. Any async operation related to current channel should check this property before anything take effect, to avoid outdated operation takes effect.
        /// For example, when changing channel from A to B asynchronously (call it operation 1), user triggerred next command (call it operation 2, which will update the pending songs based on current channel).
        /// Assume below execution timeline:
        /// Operation 1 start -> operation 2 start -> operation 1 finish -> operation 2 finish.
        /// When operation 1 finishes, current channel will be switched to channel B. And when operation 2 finishes, it will try to update the pending songs based on channel A. Now check the consistence of this property (B) and the channel ID in operation 2 (A) can prevent outdated server response.
        /// </remarks>
        protected int? AsyncExpectedChannelId { get; set; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        /// <remarks>
        /// The configuration stores information needed by player. Such as music format and bit rate, which will be used when communiating with server.
        /// </remarks>
        public IDictionary<string, object> Config { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Occurs when current song changed.
        /// </summary>
        public event EventHandler<EventArgs<Song>> CurrentSongChanged;
        /// <summary>
        /// Occurs when current channel changed.
        /// </summary>
        public event EventHandler<EventArgs<Channel>> CurrentChannelChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player" /> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public Player(ISession session)
        {
            Session = session;
        }

        /// <summary>
        /// Switch to next song.
        /// </summary>
        /// <param name="type">the type of next operation.</param>
        /// <returns></returns>
        public async Task Next(NextCommandType type)
        {
            ThrowExceptionIfCurrentChannelIsNull();
            ReportType reportType;
            if (CurrentSong == null)
            {
                reportType = ReportType.PlayListEmpty;
            }
            else
            {
                switch (type)
                {
                    case NextCommandType.CurrentSongEnded:
                        reportType = ReportType.CurrentSongEnded;
                        break;
                    case NextCommandType.SkipCurrentSong:
                        reportType = ReportType.SkipCurrentSong;
                        break;
                    case NextCommandType.BanCurrentSong:
                        reportType = ReportType.BanCurrentSong;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }

            await Report(reportType, CurrentChannel.Id, CurrentSong?.Sid, null /* should not pass the start song code here */);
        }

        /// <summary>
        /// Changes the channel.
        /// </summary>
        /// <param name="newChannel">The new channel.</param>
        /// <param name="type">The type of operation.</param>
        /// <returns></returns>
        public async Task ChangeChannel(Channel newChannel, ChangeChannelCommandType type = ChangeChannelCommandType.Normal)
        {
            AsyncExpectedChannelId = newChannel?.Id;
            CurrentChannel = null;
            CurrentSong = null;
            if (newChannel != null)
            {
                var start = type == ChangeChannelCommandType.Normal ? newChannel.Start : null;
                await Report(ReportType.CurrentChannelChanged, newChannel.Id, CurrentSong?.Sid, start);

                /*
                        If user called ChangeChannel twice in a short time, say call 1 and call 2.
                        But call 2 responded before call 1. Then we want to use call 2's response.
                        So we need to check AsyncExpectedChannelId here because it should be call 2's ID.
                        */
                if (AsyncExpectedChannelId == newChannel.Id)
                {
                    CurrentChannel = newChannel;
                }
            }
        }

        /// <summary>
        /// Sets the red heart.
        /// </summary>
        /// <param name="redHeart">if set to <c>true</c> then current song will be marked with red heart, indicating that user likes this song. Otherwise remove the red heart mark.</param>
        /// <returns></returns>
        /// <remarks>Set redHeart to false doesn't means user dislike current song.</remarks>
        public async Task SetRedHeart(bool redHeart)
        {
            ThrowExceptionIfCurrentChannelIsNull();
            ThrowExceptionIfCurrentSongIsNull();
            var sid = CurrentSong.Sid;
            await Report(redHeart ? ReportType.Like : ReportType.CancelLike, CurrentChannel.Id, CurrentSong?.Sid, null);
            if (CurrentSong != null && CurrentSong.Sid == sid)
            {
                CurrentSong.Like = redHeart;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CurrentSongChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="Kfstorm.DoubanFM.Core.EventArgs{T}" /> instance containing the event data.</param>
        protected virtual void OnCurrentSongChanged(EventArgs<Song> e)
        {
            CurrentSongChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:CurrentChannelChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="Kfstorm.DoubanFM.Core.EventArgs{T}" /> instance containing the event data.</param>
        protected virtual void OnCurrentChannelChanged(EventArgs<Channel> e)
        {
            CurrentChannelChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Send a report to server.
        /// </summary>
        /// <param name="type">The type of report.</param>
        /// <param name="channelId">The channel ID.</param>
        /// <param name="sid">The SID of current song.</param>
        /// <param name="start">The start song code.</param>
        /// <returns></returns>
        private async Task Report(ReportType type, int channelId, string sid, string start)
        {
            var changeCurrentSong = !(type == ReportType.Like || type == ReportType.CancelLike);
            if (changeCurrentSong)
            {
                CurrentSong = null;
            }
            var uri = ServerConnection.CreateGetPlayListUri(channelId, type: type, sid: sid, start: start, formats: null, kbps: null, playedTime: null, mode: null, excludedSids: null, max: null);
            var jsonContent = await ServerConnection.Get(uri, ServerConnection.SetSessionInfoToRequest);
            var newPlayList = ServerRequests.ParseGetPlayListResult(jsonContent);
            if (newPlayList.Length == 0)
            {
                if (type != ReportType.CurrentSongEnded)
                {
                    throw new NoAvailableSongsException();
                }
                if (_pendingSongs.Count == 0)
                {
                    await Report(ReportType.PlayListEmpty, channelId, sid, start);
                    return;
                }
            }
            if (channelId == AsyncExpectedChannelId)
            {
                if (newPlayList.Length != 0)
                {
                    if (_pendingSongs == null)
                    {
                        _pendingSongs = new Queue<Song>();
                    }
                    _pendingSongs.Clear();
                    foreach (var song in newPlayList)
                    {
                        _pendingSongs.Enqueue(song);
                    }
                }
                if (changeCurrentSong)
                {
                    CurrentSong = _pendingSongs.Dequeue();
                }
            }
            else
            {
                // TODO: throw exception or not?
            }
        }

        /// <summary>
        /// Throws the exception if current channel is null.
        /// </summary>
        private void ThrowExceptionIfCurrentChannelIsNull()
        {
            if (CurrentChannel == null)
            {
                throw new ChannelNotSelectedException();
            }
        }

        /// <summary>
        /// Throws the exception if current song is null.
        /// </summary>
        private void ThrowExceptionIfCurrentSongIsNull()
        {
            if (CurrentSong == null)
            {
                throw new SongNotSelectedException();
            }
        }
    }
}