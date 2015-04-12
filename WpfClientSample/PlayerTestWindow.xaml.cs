using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Kfstorm.DoubanFM.Core;
using Newtonsoft.Json;

namespace WpfClientSample
{
    /// <summary>
    /// Interaction logic for PlayerTestWindow.xaml
    /// </summary>
    public partial class PlayerTestWindow : Window
    {
        public IPlayer Player;

        public PlayerTestWindow()
        {
            InitializeComponent();

            Player = new Player(((App)Application.Current).ServerConnection, ((App)Application.Current).Session);
        }

        private void RefreshChannelList()
        {
            LvChannels.ItemsSource = Player.ChannelList?.ChannelGroups?.SelectMany(group => group.Channels).ToArray();
        }

        private void RefreshSongInfo()
        {
            TbCurrentSong.Text = Player.CurrentSong != null ? JsonConvert.SerializeObject(Player.CurrentSong, Formatting.Indented) : null;
        }

        private async void BtnLike_Click(object sender, RoutedEventArgs e)
        {
            await Player.SetRedHeart(!Player.CurrentSong.Like);
            RefreshSongInfo();
        }

        private async void BtnBan_Click(object sender, RoutedEventArgs e)
        {
            await Player.Next(NextCommandType.BanCurrentSong);
            RefreshSongInfo();
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private async void Next_Click(object sender, RoutedEventArgs e)
        {
            await Player.Next(NextCommandType.SkipCurrentSong);
            RefreshSongInfo();
        }

        private async void LvChannels_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var channel = (Channel)(e.AddedItems.Count > 0 ? e.AddedItems[0] : null);
            await Player.ChangeChannel(channel);
            RefreshSongInfo();
        }

        private async void BtnRefreshChannelList_Click(object sender, RoutedEventArgs e)
        {
            await Player.RefreshChannelList();
            RefreshChannelList();
        }
    }
}
