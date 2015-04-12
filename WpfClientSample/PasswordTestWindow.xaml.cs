using System.Windows;
using Kfstorm.DoubanFM.Core;
using Newtonsoft.Json;

namespace WpfClientSample
{
    /// <summary>
    /// Interaction logic for PasswordTestWindow.xaml
    /// </summary>
    public partial class PasswordTestWindow
    {
        public ISession Session { get; } = ((App)Application.Current).Session;

        public PasswordAuthentication PAuth { get; }

        public PasswordTestWindow()
        {
            PAuth = new PasswordAuthentication(((App)Application.Current).ServerConnection);

            InitializeComponent();

            ShowSessionInfo();
        }

        private async void BtnLogOn_Click(object sender, RoutedEventArgs e)
        {
            PAuth.Username = TbUsername.Text;
            PAuth.Password = PbPassword.Password;
            await Session.LogOn(PAuth);
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
