using System;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public interface IPlayer : IDisposable
    {
        ISession Session { get; }
        IServerConnection ServerConnection { get; }
        Song CurrentSong { get; }
        ChannelList ChannelList { get; }
        Channel CurrentChannel { get; }

        event EventHandler<EventArgs<Song>> CurrentSongChanged;
        event EventHandler<EventArgs<Channel>> CurrentChannelChanged;

        void Initialize();
        Task ChangeChannel(Channel newChannel);
        void Next(NextCommandType type);
        void SetRedHeart(bool redHeart);
    }
}