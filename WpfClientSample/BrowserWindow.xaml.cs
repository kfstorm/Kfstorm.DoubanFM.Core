using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace WpfClientSample
{
    /// <summary>
    /// Interaction logic for BrowserWindow.xaml
    /// </summary>
    public partial class BrowserWindow : Window
    {
        public BrowserWindow(Uri navigateUri, Uri expectedRedirectUri)
        {
            InitializeComponent();

            _navigateUri = navigateUri;
            _expectedRedirectUri = expectedRedirectUri;
            _redirectTaskCompletionSource = new TaskCompletionSource<Uri>();

            Browser.Navigated += Browser_Navigated;
        }

        void Browser_Navigated(object sender, NavigationEventArgs e)
        {
            var redirectedUri = new UriBuilder(e.Uri.AbsoluteUri) { Query = string.Empty };
            if (redirectedUri.Uri == _expectedRedirectUri)
            {
                _redirectTaskCompletionSource.SetResult(e.Uri);

                Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _redirectTaskCompletionSource.TrySetResult(null);
        }

        private readonly Uri _navigateUri;

        private readonly Uri _expectedRedirectUri;

        private readonly TaskCompletionSource<Uri> _redirectTaskCompletionSource;

        public async Task<Uri> GetRedirectUri()
        {
            Show();

            Browser.Navigate(_navigateUri);

            return await _redirectTaskCompletionSource.Task.ConfigureAwait(false);
        }
    }
}