using System;
using System.Windows;
using Kfstorm.DoubanFM.Core;

namespace WpfClientSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var redirectUri = new Uri("https://www.example.com");
            var authentication = new OAuthAuthentication(new ServerConnection("0c893b52c05e21d0259bb1f8411a6570", "7d85ce80ded0caf1"), redirectUri)
            {
                GetRedirectedUri = async uri =>
                {
                    var window = new BrowserWindow(uri, redirectUri);
                    return await window.GetRedirectUri();
                }
            };
            var session = new Session();
            if (await session.LogOn(authentication) != null)
            {
                Content = session.UserInfo.Username;
            }
        }
    }
}
