using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public interface IPlayer
    {
        ISession Session { get; }
        IServerConnection ServerConnection { get; }
        Song CurrentSong { get; }
        ChannelList ChannelList { get; }
        Channel CurrentChannel { get; }
        IDictionary<string, object> Config { get; }

        event EventHandler<EventArgs<Song>> CurrentSongChanged;
        event EventHandler<EventArgs<Channel>> CurrentChannelChanged;

        Task RefreshChannelList();
        Task ChangeChannel(Channel newChannel);
        Task Next(NextCommandType type);
        Task SetRedHeart(bool redHeart);
    }
}