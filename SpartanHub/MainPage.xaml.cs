using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SpartanHub
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void NavigateToDevPage_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DevPage));
        }
    }
}
