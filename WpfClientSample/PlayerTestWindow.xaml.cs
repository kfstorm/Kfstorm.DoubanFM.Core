using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
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

        public IDiscovery Discovery;

        private bool _playing = true;

        // ReSharper disable once NotAccessedField.Local
        private DispatcherTimer _refreshTimer;

        public PlayerTestWindow()
        {
            InitializeComponent();

            _refreshTimer = new DispatcherTimer(TimeSpan.FromSeconds(0.1), DispatcherPriority.Background, (sender, args) =>
            {
                var length = MeAudio.NaturalDuration.HasTimeSpan ? MeAudio.NaturalDuration.TimeSpan : (TimeSpan?)null;
                TbCurrentPosition.Text = MeAudio.Position.ToString(@"mm\:ss");
                TbLength.Text = length?.ToString(@"mm\:ss") ?? "--:--";
                PbProgress.Minimum = 0;
                PbProgress.Maximum = length?.TotalMilliseconds ?? 1;
                PbProgress.Value = MeAudio.Position.TotalMilliseconds;
            }, Dispatcher);

            Player = new Player(((App)Application.Current).Session);
            Player.CurrentChannelChanged += (sender, args) => TbCurrentChannel.Text = args.Object?.ToString();
            Player.CurrentSongChanged += (sender, args) =>
            {
                ImgCover.Source = args.Object != null ? new BitmapImage(new Uri(args.Object.PictureUrl)) : null;
                TbTitle.Text = args.Object?.Title;
                TbArtist.Text = args.Object?.Artist;
                TbAlbum.Text = args.Object?.AlbumTitle;

                TbCurrentSong.Text = args.Object != null ? JsonConvert.SerializeObject(args.Object, Formatting.Indented) : null;
                MeAudio.Source = args.Object == null ? null : new Uri(args.Object.Url);
                MeAudio.Play();
                _playing = true;
            };
            Discovery = new Discovery(((App)Application.Current).Session);
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

        private async void BtnFindRelated_Click(object sender, RoutedEventArgs e)
        {
            var sid = Player.CurrentSong?.Sid;
            if (sid != null)
            {
                var window = new FindRelatedWindow(sid) { Owner = this };
                window.ShowDialog();
                if (window.Channel != null)
                {
                    await Player.ChangeChannel(window.Channel, ChangeChannelCommandType.PlayRelatedSongs);
                }
            }
        }

        private async void LvChannels_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var channel = (Channel)(e.AddedItems.Count > 0 ? e.AddedItems[0] : null);
            if (channel != null)
            {
                LvSearchChannelResult.SelectedItem = null;
                await Player.ChangeChannel(channel);
            }
        }

        private async void LvSearchChannelResult_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var channel = (Channel)(e.AddedItems.Count > 0 ? e.AddedItems[0] : null);
            if (channel != null)
            {
                LvChannels.SelectedItem = null;
                await Player.ChangeChannel(channel);
            }
        }

        private async void BtnRefreshChannelList_Click(object sender, RoutedEventArgs e)
        {
            var channelGroups = await Discovery.GetRecommendedChannels();
            LvChannels.ItemsSource = channelGroups.SelectMany(group => group.Channels).ToArray();
        }

        private int _searchChannelStart;
        private int _searchChannelSize = 20;

        private async void BtnSearchChannel_OnClick(object sender, RoutedEventArgs e)
        {
            _searchChannelStart = 0;
            _searchChannelStart += await SearchChannel(TbSearchChannelQuery.Text, _searchChannelStart, _searchChannelSize);
        }

        private async void BtnSearchChannelResultShowMore_OnClick(object sender, RoutedEventArgs e)
        {
            _searchChannelStart += await SearchChannel(TbSearchChannelQuery.Text, _searchChannelStart, _searchChannelSize);
        }

        private async Task<int> SearchChannel(string query, int start, int size)
        {
            var channels = await Discovery.SearchChannel(query, start, size);
            if (start == 0)
            {
                LvSearchChannelResult.ItemsSource = new ObservableCollection<Channel>(channels.CurrentList);
            }
            else
            {
                foreach (var channel in channels.CurrentList)
                {
                    ((ObservableCollection<Channel>)LvSearchChannelResult.ItemsSource).Add(channel);
                }
            }
            return start + channels.CurrentList.Count;
        }

        private async void MeAudio_MediaEnded(object sender, RoutedEventArgs e)
        {
            await Player.Next(NextCommandType.CurrentSongEnded);
        }
    }
}
