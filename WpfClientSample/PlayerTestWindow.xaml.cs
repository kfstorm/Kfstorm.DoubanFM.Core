using System;
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
    public partial class PlayerTestWindow
    {
        public IPlayer Player;

        private bool _playing = true;

        public PlayerTestWindow()
        {
            InitializeComponent();

            Player = new Player(((App)Application.Current).ServerConnection, ((App)Application.Current).Session);
            Player.CurrentChannelChanged += (sender, args) => TbCurrentChannel.Text = args.Object?.ToString();
            Player.CurrentSongChanged += (sender, args) =>
            {
                TbCurrentSong.Text = args.Object != null ? JsonConvert.SerializeObject(args.Object, Formatting.Indented) : null;
                MeAudio.Source = args.Object == null ? null : new Uri(args.Object.Url);
                MeAudio.Play();
                _playing = true;
            };
        }

        private async void BtnLike_Click(object sender, RoutedEventArgs e)
        {
            await Player.SetRedHeart(!Player.CurrentSong.Like);
            TbCurrentSong.Text = Player.CurrentSong != null ? JsonConvert.SerializeObject(Player.CurrentSong, Formatting.Indented) : null;
        }

        private async void BtnBan_Click(object sender, RoutedEventArgs e)
        {
            await Player.Next(NextCommandType.BanCurrentSong);
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (_playing)
            {
                MeAudio.Pause();
            }
            else
            {
                MeAudio.Play();
            }
            _playing = !_playing;
        }

        private async void Next_Click(object sender, RoutedEventArgs e)
        {
            await Player.Next(NextCommandType.SkipCurrentSong);
        }

        private async void LvChannels_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var channel = (Channel)(e.AddedItems.Count > 0 ? e.AddedItems[0] : null);
            await Player.ChangeChannel(channel);
        }

        private async void BtnRefreshChannelList_Click(object sender, RoutedEventArgs e)
        {
            await Player.RefreshChannelList();
            LvChannels.ItemsSource = Player.ChannelList?.ChannelGroups?.SelectMany(group => group.Channels).ToArray();
        }
    }
}
