namespace Kfstorm.DoubanFM.Core
{
    public interface IPlayer
    {
        Song CurerntSong { get; }
        Song[] NextSongs { get; }
        ChannelList ChannelList { get; }

        void Skip();
        void Ban();
        void Next();
        void SetRedHeart(bool redHeart);
    }
}