using System;
using Kfstorm.DoubanFM.Core;
using System.Text;
using System.Windows;

namespace WpfClientSample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public string ClientId { get; } = "02646d3fb69a52ff072d47bf23cef8fd";
        public string ClientSecret { get; } = "cde5d61429abcd7c";
        public Uri RedirectUri { get; } = new Uri("http://www.douban.com/mobile/fm");
        public IServerConnection ServerConnection { get; }
        public ISession Session { get; }

        public App()
        {
            ServerConnection = new ServerConnection(ClientId, ClientSecret, RedirectUri);
            Session = new Session();

            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var sb = new StringBuilder();
            sb.AppendLine(e.Exception.Message);
            sb.AppendLine();
            sb.AppendLine("Detail:");
            sb.AppendLine(e.Exception.ToString());
            MessageBox.Show(sb.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
