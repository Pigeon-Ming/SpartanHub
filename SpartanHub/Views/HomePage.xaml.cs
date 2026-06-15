using SpartanHub.Service;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SpartanHub.Views
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await UpdateSignedInPlayerAsync();
        }

        private async System.Threading.Tasks.Task UpdateSignedInPlayerAsync()
        {
            var session = UserSessionService.Instance;

            if (!session.IsLoggedIn)
            {
                await session.LoadPersistedSessionAsync();
            }

            if (session.IsLoggedIn && !string.IsNullOrWhiteSpace(session.CurrentUser?.Xuid))
            {
                MyNameplateControl.Xuid = session.CurrentUser.Xuid;
            }
            else
            {
                MyNameplateControl.Xuid = null;
            }
        }
    }
}
