using System;
using SpartanHub.Service;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SpartanHub.Views
{
    public sealed partial class AuthPage : Page
    {
        private const string HaloWaypointUrl = "https://www.halowaypoint.com/sign-in?path=/halo-infinite/progression";
        private readonly UserSessionService _sessionService;

        public AuthPage()
        {
            InitializeComponent();
            _sessionService = UserSessionService.Instance;
            Loaded += AuthPage_Loaded;
        }

        private void AuthPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (LoginWebView.Source == null)
            {
                LoginWebView.Navigate(new Uri(HaloWaypointUrl));
            }
        }

        private void LoginWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            LoadingRing.IsActive = true;
            CompleteLoginButton.IsEnabled = false;
            AddressText.Text = args.Uri?.ToString() ?? string.Empty;
            StatusText.Text = "页面加载中...";
        }

        private void LoginWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            LoadingRing.IsActive = false;
            CompleteLoginButton.IsEnabled = true;
            BackButton.IsEnabled = LoginWebView.CanGoBack;
            AddressText.Text = LoginWebView.Source?.ToString() ?? string.Empty;

            if (!args.IsSuccess)
            {
                StatusText.Text = $"页面加载失败：{args.WebErrorStatus}";
                return;
            }

            StatusText.Text = "页面已返回 HaloWaypoint。若已经完成 Microsoft 登录，请点击完成登录。";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoginWebView.CanGoBack)
            {
                LoginWebView.GoBack();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWebView.Refresh();
        }

        private async void CompleteLoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoadingRing.IsActive = true;
            CompleteLoginButton.IsEnabled = false;
            StatusText.Text = "正在读取登录 Cookie 并验证 Token...";

            try
            {
                var result = await _sessionService.CompleteHaloWaypointLoginAsync();
                if (!result.IsSuccess)
                {
                    StatusText.Text = result.ErrorMessage;
                    return;
                }

                StatusText.Text = BuildSuccessMessage(result.Session?.Gamertag, result.Session?.Xuid);

                if (Frame != null)
                {
                    Frame.Navigate(typeof(HomePage));
                }
            }
            finally
            {
                LoadingRing.IsActive = false;
                CompleteLoginButton.IsEnabled = true;
            }
        }

        private async void ClearLoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoadingRing.IsActive = true;
            StatusText.Text = "正在清除登录状态...";

            try
            {
                await _sessionService.LogoutAsync();
                LoginWebView.Navigate(new Uri(HaloWaypointUrl));
                StatusText.Text = "登录状态已清除。";
            }
            finally
            {
                LoadingRing.IsActive = false;
            }
        }

        private static string BuildSuccessMessage(string gamertag, string xuid)
        {
            if (!string.IsNullOrWhiteSpace(gamertag))
            {
                return $"登录成功：{gamertag}";
            }

            return $"登录成功，当前用户 XUID：{xuid}";
        }
    }
}
