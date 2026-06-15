using System;
using System.Linq;
using System.Threading.Tasks;
using SpartanHub.Core.Models;
using SpartanHub.Core.Utilities;
using SpartanHub.Service;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace SpartanHub.Controls
{
    public sealed partial class PlayerInfoNameplateControl : UserControl
    {
        public static readonly DependencyProperty PlayerIdProperty =
            DependencyProperty.Register(
                nameof(PlayerId),
                typeof(string),
                typeof(PlayerInfoNameplateControl),
                new PropertyMetadata(null, OnPlayerIdentityChanged));

        public static readonly DependencyProperty GamerTagProperty =
            DependencyProperty.Register(
                nameof(GamerTag),
                typeof(string),
                typeof(PlayerInfoNameplateControl),
                new PropertyMetadata(null, OnPlayerIdentityChanged));

        public static readonly DependencyProperty XuidProperty =
            DependencyProperty.Register(
                nameof(Xuid),
                typeof(string),
                typeof(PlayerInfoNameplateControl),
                new PropertyMetadata(null, OnPlayerIdentityChanged));

        public static readonly DependencyProperty StatusTextProperty =
            DependencyProperty.Register(
                nameof(StatusText),
                typeof(string),
                typeof(PlayerInfoNameplateControl),
                new PropertyMetadata(null, OnStatusTextChanged));

        public static readonly DependencyProperty StatusDetailTextProperty =
            DependencyProperty.Register(
                nameof(StatusDetailText),
                typeof(string),
                typeof(PlayerInfoNameplateControl),
                new PropertyMetadata(null, OnStatusTextChanged));

        public static readonly DependencyProperty AutoLoadProperty =
            DependencyProperty.Register(
                nameof(AutoLoad),
                typeof(bool),
                typeof(PlayerInfoNameplateControl),
                new PropertyMetadata(true, OnAutoLoadChanged));

        private int _loadVersion;
        private bool _isLoaded;
        private bool _isLoading;

        public event EventHandler<UserInfo> PlayerLoaded;

        public PlayerInfoNameplateControl()
        {
            InitializeComponent();
            Loaded += PlayerInfoNameplateControl_Loaded;
            Unloaded += PlayerInfoNameplateControl_Unloaded;
        }

        public string PlayerId
        {
            get => (string)GetValue(PlayerIdProperty);
            set => SetValue(PlayerIdProperty, value);
        }

        public string GamerTag
        {
            get => (string)GetValue(GamerTagProperty);
            set => SetValue(GamerTagProperty, value);
        }

        public string Xuid
        {
            get => (string)GetValue(XuidProperty);
            set => SetValue(XuidProperty, value);
        }

        public string StatusText
        {
            get => (string)GetValue(StatusTextProperty);
            set => SetValue(StatusTextProperty, value);
        }

        public string StatusDetailText
        {
            get => (string)GetValue(StatusDetailTextProperty);
            set => SetValue(StatusDetailTextProperty, value);
        }

        public bool AutoLoad
        {
            get => (bool)GetValue(AutoLoadProperty);
            set => SetValue(AutoLoadProperty, value);
        }

        public UserInfo PlayerInfo { get; private set; }

        public Task ReloadAsync()
        {
            return LoadPlayerAsync();
        }

        private static void OnPlayerIdentityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlayerInfoNameplateControl)d).RequestLoad();
        }

        private static void OnAutoLoadChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlayerInfoNameplateControl)d).RequestLoad();
        }

        private static void OnStatusTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PlayerInfoNameplateControl)d;
            if (!control._isLoading)
            {
                control.UpdateStatusText();
            }
        }

        private void PlayerInfoNameplateControl_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
            RequestLoad();
        }

        private void PlayerInfoNameplateControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = false;
            _loadVersion++;
        }

        private void RequestLoad()
        {
            if (!_isLoaded)
            {
                return;
            }

            if (!AutoLoad)
            {
                UpdateStatusText();
                return;
            }

            _ = LoadPlayerAsync();
        }

        private async Task LoadPlayerAsync()
        {
            var identity = ResolveIdentity();
            if (string.IsNullOrWhiteSpace(identity.Xuid) && string.IsNullOrWhiteSpace(identity.GamerTag))
            {
                ClearPlayer();
                return;
            }

            var currentVersion = ++_loadVersion;
            SetLoading(true);

            var player = string.IsNullOrWhiteSpace(identity.Xuid)
                ? await PlayerCacheService.Instance.GetPlayerInfoByGamerTagAsync(identity.GamerTag)
                : await PlayerCacheService.Instance.GetPlayerInfoAsync(identity.Xuid);

            if (!_isLoaded || currentVersion != _loadVersion)
            {
                return;
            }

            if (player == null)
            {
                SetLoading(false);
                ShowLoadFailed();
                return;
            }

            var serviceTag = await PlayerCacheService.Instance.GetPlayerServiceTagAsync(player.Xuid);

            if (!_isLoaded || currentVersion != _loadVersion)
            {
                return;
            }

            SetLoading(false);
            ShowPlayer(player, serviceTag);
            PlayerLoaded?.Invoke(this, player);
        }

        private (string Xuid, string GamerTag) ResolveIdentity()
        {
            var xuid = NormalizeText(Xuid);
            var gamerTag = NormalizeText(GamerTag);
            var playerId = NormalizeText(PlayerId);

            if (!string.IsNullOrWhiteSpace(xuid))
            {
                return (XuidUtility.UnwrapPlayerId(xuid), gamerTag);
            }

            if (!string.IsNullOrWhiteSpace(gamerTag))
            {
                return (null, gamerTag);
            }

            if (LooksLikeXuid(playerId))
            {
                return (XuidUtility.UnwrapPlayerId(playerId), null);
            }

            return (null, playerId);
        }

        private void ShowPlayer(UserInfo player, string serviceTag)
        {
            PlayerInfo = player;

            GamerTagTextBlock.Text = string.IsNullOrWhiteSpace(player.Gamertag) ? "未知玩家" : player.Gamertag;
            IdentifierTextBlock.Text = string.IsNullOrWhiteSpace(serviceTag) ? "ServiceTag --" : serviceTag.Trim();
            AvatarInitialsText.Text = GetInitials(player.Gamertag);
            AvatarInitialsText.Visibility = Visibility.Visible;

            var avatarUrl = GetBestAvatarUrl(player);
            if (!string.IsNullOrWhiteSpace(avatarUrl) && Uri.TryCreate(avatarUrl, UriKind.Absolute, out var uri))
            {
                AvatarBrush.ImageSource = new BitmapImage(uri);
                AvatarInitialsText.Visibility = Visibility.Collapsed;
            }
            else
            {
                AvatarBrush.ImageSource = null;
            }

            UpdateStatusText();
        }

        private void ClearPlayer()
        {
            PlayerInfo = null;
            SetLoading(false);
            AvatarBrush.ImageSource = null;
            AvatarInitialsText.Text = "?";
            AvatarInitialsText.Visibility = Visibility.Visible;
            GamerTagTextBlock.Text = "玩家资料";
            IdentifierTextBlock.Text = "传入 GamerTag 或 XUID";
            StatusTextBlock.Text = "未加载";
            StatusDetailTextBlock.Text = string.Empty;
            StatusDetailTextBlock.Visibility = Visibility.Collapsed;
        }

        private void ShowLoadFailed()
        {
            PlayerInfo = null;
            AvatarBrush.ImageSource = null;
            AvatarInitialsText.Text = "!";
            AvatarInitialsText.Visibility = Visibility.Visible;
            GamerTagTextBlock.Text = "加载失败";
            IdentifierTextBlock.Text = "请检查玩家标识或登录状态";
            StatusTextBlock.Text = "失败";
            StatusDetailTextBlock.Text = string.Empty;
            StatusDetailTextBlock.Visibility = Visibility.Collapsed;
        }

        private void SetLoading(bool isLoading)
        {
            _isLoading = isLoading;
            LoadingRing.IsActive = isLoading;
            LoadingRing.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;

            if (isLoading)
            {
                StatusTextBlock.Text = "加载中";
                StatusDetailTextBlock.Text = string.Empty;
                StatusDetailTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateStatusText()
        {
            StatusTextBlock.Text = string.IsNullOrWhiteSpace(StatusText) ? "已加载" : StatusText.Trim();

            var detail = string.IsNullOrWhiteSpace(StatusDetailText) ? string.Empty : StatusDetailText.Trim();
            StatusDetailTextBlock.Text = detail;
            StatusDetailTextBlock.Visibility = string.IsNullOrWhiteSpace(detail)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private static string NormalizeText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static bool LooksLikeXuid(string value)
        {
            value = NormalizeText(value);
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            value = XuidUtility.UnwrapPlayerId(value);
            return value.Length >= 12 && value.All(char.IsDigit);
        }

        private static string GetBestAvatarUrl(UserInfo player)
        {
            return player?.Gamerpic?.Large
                ?? player?.Gamerpic?.Medium
                ?? player?.Gamerpic?.Small
                ?? player?.Gamerpic?.Xlarge;
        }

        private static string GetInitials(string gamerTag)
        {
            gamerTag = NormalizeText(gamerTag);
            return string.IsNullOrWhiteSpace(gamerTag) ? "?" : gamerTag.Substring(0, 1).ToUpperInvariant();
        }
    }
}
