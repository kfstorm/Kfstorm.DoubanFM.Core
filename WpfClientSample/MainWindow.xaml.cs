using System.Windows;

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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var oAuthWindow = new OAuthTestWindow { Owner = this };
            oAuthWindow.Show();
        }
    }
}
