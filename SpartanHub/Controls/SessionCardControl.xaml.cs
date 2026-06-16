using SpartanHub.Core.Clients;
using SpartanHub.Core.Models;
using SpartanHub.Core.Utilities;
using SpartanHub.Service;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SpartanHub.Controls
{
    public sealed partial class SessionCardControl : UserControl
    {
        public static readonly DependencyProperty TrackIdProperty =
            DependencyProperty.Register(
                nameof(TrackId),
                typeof(string),
                typeof(SessionCardControl),
                new PropertyMetadata(null, OnTrackContextChanged));

        public static readonly DependencyProperty XuidProperty =
            DependencyProperty.Register(
                nameof(Xuid),
                typeof(string),
                typeof(SessionCardControl),
                new PropertyMetadata(null, OnTrackContextChanged));

        public static readonly DependencyProperty AutoLoadProperty =
            DependencyProperty.Register(
                nameof(AutoLoad),
                typeof(bool),
                typeof(SessionCardControl),
                new PropertyMetadata(true, OnTrackContextChanged));

        private readonly HaloInfiniteClient _haloClient;
        private int _loadVersion;
        private bool _isLoaded;

        public SessionCardControl()
        {
            InitializeComponent();
            _haloClient = new HaloInfiniteClient(UserSessionService.Instance);
            Loaded += SessionCardControl_Loaded;
            Unloaded += SessionCardControl_Unloaded;
        }

        public string TrackId
        {
            get => (string)GetValue(TrackIdProperty);
            set => SetValue(TrackIdProperty, value);
        }

        public string Xuid
        {
            get => (string)GetValue(XuidProperty);
            set => SetValue(XuidProperty, value);
        }

        public bool AutoLoad
        {
            get => (bool)GetValue(AutoLoadProperty);
            set => SetValue(AutoLoadProperty, value);
        }

        public Task ReloadAsync()
        {
            return LoadTrackAsync();
        }

        private static void OnTrackContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SessionCardControl)d).RequestLoad();
        }

        private void SessionCardControl_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
            RequestLoad();
        }

        private void SessionCardControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = false;
            _loadVersion++;
        }

        private void RequestLoad()
        {
            if (!_isLoaded || !AutoLoad)
            {
                return;
            }

            _ = LoadTrackAsync();
        }

        private async Task LoadTrackAsync()
        {
            var trackId = NormalizeText(TrackId);
            if (string.IsNullOrWhiteSpace(trackId))
            {
                ShowEmpty();
                return;
            }

            var xuid = ResolveXuid();
            if (string.IsNullOrWhiteSpace(xuid))
            {
                ShowMessage(trackId, "未登录", "请先登录账户");
                return;
            }

            var currentVersion = ++_loadVersion;
            ShowMessage(trackId, "加载中", string.Empty);

            try
            {
                var definition = await _haloClient.GetOperationRewardTrackDefinitionAsync(trackId).ConfigureAwait(false);
                var progressContainer = await _haloClient.GetPlayerOperationRewardTracksAsync(xuid).ConfigureAwait(false);

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (!_isLoaded || currentVersion != _loadVersion)
                    {
                        return;
                    }

                    var progress = FindTrackProgress(progressContainer, definition?.TrackId ?? trackId);
                    ShowTrack(definition, progress, trackId);
                });
            }
            catch (Exception ex)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (!_isLoaded || currentVersion != _loadVersion)
                    {
                        return;
                    }

                    ShowMessage(trackId, "加载失败", ex.Message);
                });
            }
        }

        private void ShowTrack(RewardTrackDefinition definition, OperationRewardTrack progressTrack, string fallbackTrackId)
        {
            var progress = progressTrack?.CurrentProgress;
            var rank = progress?.Rank ?? 0;
            var xpPerRank = definition?.XpPerRank > 0 ? definition.XpPerRank : 1000;
            var currentXp = Math.Max(0, Math.Min(progress?.PartialProgress ?? 0, xpPerRank));
            var remainingXp = progress?.HasReachedMaxRank == true ? 0 : Math.Max(0, xpPerRank - currentXp);

            TrackNameTextBlock.Text = GetLocalizedValue(definition?.Name, fallbackTrackId);
            RankTextBlock.Text = rank.ToString(CultureInfo.InvariantCulture);
            CurrentXpTextBlock.Text = $"{currentXp}XP";
            RemainingXpTextBlock.Text = $"{remainingXp}XP";
            RankProgressBar.Maximum = xpPerRank;
            RankProgressBar.Value = currentXp;
        }

        private OperationRewardTrack FindTrackProgress(PlayerOperationRewardTracks progressContainer, string trackId)
        {
            var expectedPath = NormalizeTrackPath(trackId);
            var expectedFileName = GetTrackFileName(trackId);

            return progressContainer?.OperationRewardTracks?.FirstOrDefault(track =>
                string.Equals(NormalizeTrackPath(track.RewardTrackPath), expectedPath, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(GetTrackFileName(track.RewardTrackPath), expectedFileName, StringComparison.OrdinalIgnoreCase));
        }

        private string ResolveXuid()
        {
            var xuid = NormalizeText(Xuid);
            if (!string.IsNullOrWhiteSpace(xuid))
            {
                return XuidUtility.UnwrapPlayerId(xuid);
            }

            return UserSessionService.Instance.CurrentUser?.Xuid;
        }

        private void ShowEmpty()
        {
            TrackNameTextBlock.Text = "未选择通行证";
            RankTextBlock.Text = "-";
            CurrentXpTextBlock.Text = "0XP";
            RemainingXpTextBlock.Text = "0XP";
            RankProgressBar.Maximum = 1000;
            RankProgressBar.Value = 0;
        }

        private void ShowMessage(string title, string leftText, string rightText)
        {
            TrackNameTextBlock.Text = string.IsNullOrWhiteSpace(title) ? "通行证" : title;
            RankTextBlock.Text = "-";
            CurrentXpTextBlock.Text = leftText;
            RemainingXpTextBlock.Text = rightText;
            RankProgressBar.Maximum = 1000;
            RankProgressBar.Value = 0;
        }

        private static string GetLocalizedValue(LocalizedText text, string fallback)
        {
            if (text == null)
            {
                return fallback;
            }

            var languages = ApplicationLanguages.Languages;
            if (text.Translations != null && languages != null)
            {
                foreach (var language in languages)
                {
                    if (text.Translations.TryGetValue(language, out var exact) && !string.IsNullOrWhiteSpace(exact))
                    {
                        return exact;
                    }

                    var normalized = NormalizeLanguage(language);
                    if (text.Translations.TryGetValue(normalized, out var normalizedValue) && !string.IsNullOrWhiteSpace(normalizedValue))
                    {
                        return normalizedValue;
                    }
                }
            }

            return string.IsNullOrWhiteSpace(text.Value) ? fallback : text.Value;
        }

        private static string NormalizeLanguage(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                return language;
            }

            if (language.StartsWith("zh-Hans", StringComparison.OrdinalIgnoreCase))
            {
                return "zh-CN";
            }

            if (language.StartsWith("zh-Hant", StringComparison.OrdinalIgnoreCase))
            {
                return "zh-TW";
            }

            return language;
        }

        private static string NormalizeTrackPath(string trackIdOrPath)
        {
            var value = NormalizeText(trackIdOrPath)?.Replace("\\", "/");
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (value.IndexOf("/", StringComparison.Ordinal) >= 0)
            {
                return value.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                    ? value
                    : $"{value}.json";
            }

            return $"RewardTracks/Operations/{value}.json";
        }

        private static string GetTrackFileName(string trackIdOrPath)
        {
            var path = NormalizeTrackPath(trackIdOrPath);
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            var slashIndex = path.LastIndexOf("/", StringComparison.Ordinal);
            var fileName = slashIndex >= 0 ? path.Substring(slashIndex + 1) : path;
            return fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                ? fileName.Substring(0, fileName.Length - 5)
                : fileName;
        }

        private static string NormalizeText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
