using System;
using System.Windows;
using System.Windows.Controls;
using Kfstorm.DoubanFM.Core;

namespace WpfClientSample
{
    /// <summary>
    /// Interaction logic for FindRelatedWindow.xaml
    /// </summary>
    public partial class FindRelatedWindow
    {
        public Channel Channel { get; set; }

        private readonly IDiscovery _discovery;
        private readonly string _sid;

        public FindRelatedWindow(string sid)
        {
            _sid = sid;
            _discovery = new Discovery(((App)Application.Current).Session);

            InitializeComponent();
        }

        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var songDetail = await _discovery.GetSongDetail(_sid);
            if (songDetail.SimilarSongChannel.HasValue)
            {
                var button = new Button { Content = "Listen similar songs", Tag = songDetail.SimilarSongChannel.Value };
                button.Click += ButtonOnClick;
                SpChoices.Children.Add(button);
            }
            foreach (var channel in songDetail.ArtistChannels)
            {
                var button = new Button { Content = channel.Name, Tag = channel };
                button.Click += ButtonOnClick;
                SpChoices.Children.Add(button);
            }
        }

        private async void ButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var tag = ((Button)sender).Tag;
            if (tag is int)
            {
                Channel = await _discovery.GetChannelInfo((int)tag);
            }
            else
            {
                Channel = (Channel)tag;
            }
            Close();
        }
    }
}
