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

        private void BtnOAuthTest_OnClick(object sender, RoutedEventArgs e)
        {
            var oAuthWindow = new OAuthTestWindow { Owner = this };
            oAuthWindow.ShowDialog();
        }

        private void BtnPasswordTest_OnClick(object sender, RoutedEventArgs e)
        {
            var passwordWindow = new PasswordTestWindow { Owner = this };
            passwordWindow.ShowDialog();
        }
    }
}
