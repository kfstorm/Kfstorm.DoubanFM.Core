using System.Windows;
using Kfstorm.DoubanFM.Core;
using Newtonsoft.Json;

namespace WpfClientSample
{
    /// <summary>
    /// Interaction logic for OAuthTestWindow.xaml
    /// </summary>
    public partial class OAuthTestWindow
    {
        public ISession Session { get; } = ((App)Application.Current).Session;

        public OAuthAuthentication OAuth { get; }

        public OAuthTestWindow()
        {
            OAuth = new OAuthAuthentication(((App)Application.Current).ServerConnection)
            {
                GetRedirectedUri = async uri =>
                {
                    var window = new BrowserWindow(uri, OAuth.RedirectUri);
                    return await window.GetRedirectUri();
                }
            };

            InitializeComponent();

            ShowSessionInfo();
        }

        private async void BtnLogOn_Click(object sender, RoutedEventArgs e)
        {
            await Session.LogOn(OAuth);
            ShowSessionInfo();
        }

        private void BtnLogOff_Click(object sender, RoutedEventArgs e)
        {
            Session.LogOff();
            ShowSessionInfo();
        }

        private void ShowSessionInfo()
        {
            TbSessionInfo.Text = JsonConvert.SerializeObject(Session, Formatting.Indented);
        }
    }
}
