using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    public interface IPlayer : IDisposable
    {
        Song CurrentSong { get; }
        Queue<Song> NextSongs { get; }
        ChannelList ChannelList { get; }
        Channel CurrentChannel { get; set; }
        PlayerState State { get; }

        event EventHandler<EventArgs<Song>> SongChanged;
        event EventHandler<EventArgs<Channel>> ChannelChanged;
        event EventHandler<EventArgs<PlayerState>> StateChanged;

        Task StartInitialize();
        void Skip();
        void Ban();
        void Next();
        void SetRedHeart(bool redHeart);
    }
}