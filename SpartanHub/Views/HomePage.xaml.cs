using SpartanHub.Service;
using SpartanHub.Core.Clients;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SpartanHub.Views
{
    public sealed partial class HomePage : Page
    {
        private readonly HaloInfiniteClient _haloClient;

        public HomePage()
        {
            InitializeComponent();
            _haloClient = new HaloInfiniteClient(UserSessionService.Instance);
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
                var xuid = session.CurrentUser.Xuid;
                MyNameplateControl.Xuid = xuid;
                ActiveSessionCardControl.Xuid = xuid;
                ResetOverview();
                await Task.WhenAll(
                    UpdateOverviewAsync(session.CurrentUser.Gamertag, xuid),
                    UpdateActiveSessionTrackAsync(xuid));
            }
            else
            {
                MyNameplateControl.Xuid = null;
                ActiveSessionCardControl.Xuid = null;
                ActiveSessionCardControl.TrackId = null;
                ResetOverview();
            }
        }

        private void ResetOverview()
        {
            TotalMatchesTextBlock.Text = "--";
            BattlePassRankTextBlock.Text = "--";
            TotalKillsTextBlock.Text = "--";
            TotalPlayTimeTextBlock.Text = "--";
        }

        private async Task UpdateOverviewAsync(string gamerTag, string xuid)
        {
            TotalMatchesTextBlock.Text = "加载中";
            BattlePassRankTextBlock.Text = "加载中";
            TotalKillsTextBlock.Text = "加载中";
            TotalPlayTimeTextBlock.Text = "加载中";

            var serviceRecordTask = LoadServiceRecordOverviewAsync(gamerTag, xuid);
            var matchCountTask = LoadMatchCountOverviewAsync(xuid);
            var battlePassTask = LoadBattlePassRankOverviewAsync(xuid);

            await Task.WhenAll(serviceRecordTask, matchCountTask, battlePassTask);
        }

        private async Task LoadServiceRecordOverviewAsync(string gamerTag, string xuid)
        {
            try
            {
                var playerId = !string.IsNullOrWhiteSpace(gamerTag)
                    ? gamerTag
                    : SpartanHub.Core.Utilities.XuidUtility.WrapPlayerId(xuid);
                var record = await _haloClient.GetUserServiceRecordAsync(playerId);

                TotalKillsTextBlock.Text = FormatNumber(record?.CoreStats?.Kills);
                TotalPlayTimeTextBlock.Text = FormatIsoDuration(record?.TimePlayed);

                if (TotalMatchesTextBlock.Text == "加载中" && record != null)
                {
                    TotalMatchesTextBlock.Text = FormatNumber(record.MatchesCompleted);
                }
            }
            catch (Exception)
            {
                TotalKillsTextBlock.Text = "--";
                TotalPlayTimeTextBlock.Text = "--";
                if (TotalMatchesTextBlock.Text == "加载中")
                {
                    TotalMatchesTextBlock.Text = "--";
                }
            }
        }

        private async Task LoadMatchCountOverviewAsync(string xuid)
        {
            try
            {
                var count = await _haloClient.GetPlayerMatchCountAsync(xuid);
                TotalMatchesTextBlock.Text = FormatNumber(count?.All);
            }
            catch (Exception)
            {
                if (TotalMatchesTextBlock.Text == "加载中")
                {
                    TotalMatchesTextBlock.Text = "--";
                }
            }
        }

        private async Task LoadBattlePassRankOverviewAsync(string xuid)
        {
            try
            {
                var rewardTracks = await _haloClient.GetPlayerOperationRewardTracksAsync(xuid);
                var activeTrack = rewardTracks?.OperationRewardTracks?.FirstOrDefault(track =>
                    string.Equals(track.RewardTrackPath, rewardTracks.ActiveOperationRewardTrackPath, StringComparison.OrdinalIgnoreCase))
                    ?? rewardTracks?.OperationRewardTracks?.FirstOrDefault();
                var progress = activeTrack?.CurrentProgress;

                BattlePassRankTextBlock.Text = progress == null
                    ? "--"
                    : progress.HasReachedMaxRank
                        ? $"{progress.Rank:N0} 满级"
                        : progress.Rank.ToString("N0");
            }
            catch (Exception)
            {
                BattlePassRankTextBlock.Text = "--";
            }
        }

        private static string FormatIsoDuration(string duration)
        {
            if (string.IsNullOrWhiteSpace(duration))
            {
                return "--";
            }

            try
            {
                var timeSpan = XmlConvert.ToTimeSpan(duration);
                return FormatTimeSpan(timeSpan);
            }
            catch (Exception)
            {
                return duration;
            }
        }

        private static string FormatNumber(int? value)
        {
            return value.HasValue ? value.Value.ToString("N0") : "--";
        }

        private static string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1)
            {
                return $"{(int)timeSpan.TotalDays}天{timeSpan.Hours}时{timeSpan.Minutes}分";
            }

            if (timeSpan.TotalHours >= 1)
            {
                return $"{(int)timeSpan.TotalHours}时{timeSpan.Minutes}分";
            }

            return $"{timeSpan.Minutes}分";
        }

        private async System.Threading.Tasks.Task UpdateActiveSessionTrackAsync(string xuid)
        {
            try
            {
                var rewardTracks = await _haloClient.GetPlayerOperationRewardTracksAsync(xuid);
                ActiveSessionCardControl.TrackId = rewardTracks?.ActiveOperationRewardTrackPath;
            }
            catch (Exception)
            {
                ActiveSessionCardControl.TrackId = null;
            }
        }
    }
}
