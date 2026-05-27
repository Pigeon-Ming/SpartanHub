using Newtonsoft.Json;
using SpartanHub.Core;
using SpartanHub.Core.Authentication;
using SpartanHub.Core.Clients;
using SpartanHub.Core.Models;
using SpartanHub.Service;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace SpartanHub
{
    public sealed partial class DevPage : Page
    {
        private HaloInfiniteClient _client;
        private StaticSpartanTokenProvider _tokenProvider;
        private readonly UserSessionService _userSessionService;

        public DevPage()
        {
            this.InitializeComponent();
            _userSessionService = UserSessionService.Instance;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
           AppViewBackButtonVisibility.Visible;

            // 页面加载时检查是否已有登录会话
            this.Loaded += DevPage_Loaded;
        }

        private async void DevPage_Loaded(object sender, RoutedEventArgs e)
        {
            // 检查是否有缓存的 Token
            var token = _userSessionService.SpartanToken;
            if (!string.IsNullOrEmpty(token))
            {
                TokenTextBox.Text = token;
                InitializeClient(token);
                StatusText.Text = "已使用缓存的 Token 初始化";

                // 如果已有用户信息，显示出来
                if (_userSessionService.CurrentUser != null)
                {
                    StatusText.Text = $"已登录：{_userSessionService.CurrentUser.Gamertag}";
                }
            }
        }

        private void InitializeClient(string token)
        {
            _tokenProvider = new StaticSpartanTokenProvider(token);
            _client = new HaloInfiniteClient(_tokenProvider);
            _client.OnRequestLog = OnApiRequestLog;
        }

        private void OnApiRequestLog(ApiLogEntry entry)
        {
            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var logBuilder = new StringBuilder();
                logBuilder.AppendLine($"[{entry.Timestamp:HH:mm:ss.fff}] {entry.Method} {entry.Url}");
                
                if (!string.IsNullOrEmpty(entry.RequestBody))
                {
                    try
                    {
                        var formatted = JsonConvert.DeserializeObject(entry.RequestBody);
                        logBuilder.AppendLine($"请求体：{JsonConvert.SerializeObject(formatted, Formatting.Indented)}");
                    }
                    catch
                    {
                        logBuilder.AppendLine($"请求体：{entry.RequestBody}");
                    }
                }

                if (entry.IsSuccess)
                {
                    logBuilder.AppendLine($"状态码：{entry.StatusCode} (成功)");
                    if (!string.IsNullOrEmpty(entry.ResponseBody))
                    {
                        try
                        {
                            var formatted = JsonConvert.DeserializeObject(entry.ResponseBody);
                            logBuilder.AppendLine($"响应体：{JsonConvert.SerializeObject(formatted, Formatting.Indented)}");
                        }
                        catch
                        {
                            logBuilder.AppendLine($"响应体：{entry.ResponseBody}");
                        }
                    }
                }
                else
                {
                    logBuilder.AppendLine($"状态码：{entry.StatusCode} (失败)");
                    if (!string.IsNullOrEmpty(entry.ErrorMessage))
                    {
                        logBuilder.AppendLine($"错误：{entry.ErrorMessage}");
                    }
                }
                
                logBuilder.AppendLine(new string('-', 50));
                logBuilder.AppendLine();

                ApiLogTextBox.Text += logBuilder.ToString();
                
                ApiLogTextBox.SelectionStart = ApiLogTextBox.Text.Length;
                ApiLogTextBox.SelectionLength = 0;
            });
        }

        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            ApiLogTextBox.Text = string.Empty;
        }

        private bool CheckClient()
        {
            if (_client == null)
            {
                StatusText.Text = "请先设置 Spartan Token";
                return false;
            }
            return true;
        }

        private async void QueryButton_Click(object sender, RoutedEventArgs e)
        {
            var gamerTag = GamerTagTextBox.Text?.Trim();
            
            if (string.IsNullOrEmpty(gamerTag))
            {
                StatusText.Text = "请输入 GamerTag";
                return;
            }

            if (!CheckClient()) return;

            try
            {
                LoadingRing.IsActive = true;
                QueryButton.IsEnabled = false;
                StatusText.Text = $"正在查询玩家 {gamerTag}...";
                ResultBorder.Visibility = Visibility.Collapsed;

                var userInfo = await _client.GetUserAsync(gamerTag);

                GamertagResult.Text = userInfo.Gamertag;
                XuidResult.Text = $"XUID: {userInfo.Xuid}";
                
                if (!string.IsNullOrEmpty(userInfo.Gamerpic?.Large))
                {
                    GamerPicImage.Source = new BitmapImage(new Uri(userInfo.Gamerpic.Large));
                }

                MatchHistoryXuidTextBox.Text = userInfo.Xuid;
                CsrXuidTextBox.Text = userInfo.Xuid;
                ServiceRecordTextBox.Text = gamerTag;
                PrivacyXuidTextBox.Text = userInfo.Xuid;
                BanXuidTextBox.Text = userInfo.Xuid;

                ResultBorder.Visibility = Visibility.Visible;
                StatusText.Text = $"成功获取玩家 {userInfo.Gamertag} 的信息";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"查询失败：{ex.Message}";
                ResultBorder.Visibility = Visibility.Collapsed;
            }
            finally
            {
                LoadingRing.IsActive = false;
                QueryButton.IsEnabled = true;
            }
        }

        private async void MatchHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var xuid = MatchHistoryXuidTextBox.Text?.Trim();
            
            if (string.IsNullOrEmpty(xuid))
            {
                StatusText.Text = "请输入 XUID";
                return;
            }

            if (!CheckClient()) return;

            try
            {
                LoadingRing.IsActive = true;
                MatchHistoryButton.IsEnabled = false;
                StatusText.Text = $"正在查询对战历史...";
                MatchHistoryList.Visibility = Visibility.Collapsed;

                var matches = await _client.GetPlayerMatchesAsync(xuid, type: MatchType.All, count: 20);

                MatchHistoryList.ItemsSource = matches;
                MatchHistoryList.Visibility = Visibility.Visible;
                StatusText.Text = $"成功获取 {matches.Length} 场对战记录";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"查询失败：{ex.Message}";
                MatchHistoryList.Visibility = Visibility.Collapsed;
            }
            finally
            {
                LoadingRing.IsActive = false;
                MatchHistoryButton.IsEnabled = true;
            }
        }

        private async void MatchStatsButton_Click(object sender, RoutedEventArgs e)
        {
            var matchId = MatchIdTextBox.Text?.Trim();
            
            if (string.IsNullOrEmpty(matchId))
            {
                if (sender is Button button && button.DataContext is Core.Models.PlayerMatchHistory match)
                {
                    matchId = match.MatchId;
                    MatchIdTextBox.Text = matchId;
                }
                else
                {
                    StatusText.Text = "请输入 MatchId 或点击对战历史中的'详细'按钮";
                    return;
                }
            }

            if (!CheckClient()) return;

            try
            {
                LoadingRing.IsActive = true;
                if (sender is Button btn)
                {
                    if (btn.DataContext is Core.Models.PlayerMatchHistory)
                        btn.IsEnabled = false;
                    else
                        MatchStatsButton.IsEnabled = false;
                }
                else
                {
                    MatchStatsButton.IsEnabled = false;
                }
                StatusText.Text = $"正在查询比赛数据...";
                MatchStatsBorder.Visibility = Visibility.Collapsed;

                var stats = await _client.GetMatchStatsAsync(matchId);

                var mapName = "查询中...";
                var playlistName = "查询中...";

                if (stats.MatchInfo.MapVariant?.AssetId != null)
                {
                    try
                    {
                        var mapAsset = await _client.GetAssetDetailAsync("maps", stats.MatchInfo.MapVariant.AssetId);
                        mapName = mapAsset.GetNameValue();
                    }
                    catch (Exception mapEx)
                    {
                        mapName = $"{stats.MatchInfo.MapVariant.AssetId} (查询失败：{mapEx.Message})";
                    }
                }

                if (stats.MatchInfo.Playlist?.AssetId != null)
                {
                    try
                    {
                        var playlistAsset = await _client.GetPlaylistAsync(stats.MatchInfo.Playlist.AssetId);
                        playlistName = playlistAsset?.NameHint ?? stats.MatchInfo.Playlist.AssetId;
                    }
                    catch (Exception playlistEx)
                    {
                        playlistName = $"{stats.MatchInfo.Playlist.AssetId} (查询失败：{playlistEx.Message})";
                    }
                }

                var endTime = !string.IsNullOrEmpty(stats.MatchInfo.EndTime)
                    ? DateTime.Parse(stats.MatchInfo.EndTime).ToString("yyyy-MM-dd HH:mm")
                    : "未知";
                var duration = !string.IsNullOrEmpty(stats.MatchInfo.Duration)
                    ? stats.MatchInfo.Duration
                    : "未知";

                var sb = new StringBuilder();
                sb.AppendLine($"比赛 ID: {stats.MatchId}");
                sb.AppendLine($"地图：{mapName}");
                sb.AppendLine($"玩法：{playlistName}");
                sb.AppendLine($"日期：{endTime}");
                sb.AppendLine($"时长：{duration}");
                sb.AppendLine();
                sb.AppendLine("--- 队伍统计 ---");
                foreach (var team in stats.Teams)
                {
                    string outcomeText;
                    switch (team.Outcome)
                    {
                        case 1:
                            outcomeText = "胜利";
                            break;
                        case 2:
                            outcomeText = "失败";
                            break;
                        case 3:
                            outcomeText = "平局";
                            break;
                        default:
                            outcomeText = team.Outcome.ToString();
                            break;
                    }
                    sb.AppendLine($"队伍 {team.TeamId}: 排名 {team.Rank}, 结果 {outcomeText}");
                    sb.AppendLine($"  击杀：{team.Stats.CoreStats.Kills} | 死亡：{team.Stats.CoreStats.Deaths} | 助攻：{team.Stats.CoreStats.Assists}");
                    sb.AppendLine($"  爆头：{team.Stats.CoreStats.HeadshotKills} | 精准度：{team.Stats.CoreStats.Accuracy}%");
                }
                sb.AppendLine();
                sb.AppendLine("--- 玩家统计 (前 10) ---");
                var topPlayers = stats.Players.OrderByDescending(p => p.PlayerTeamStats.FirstOrDefault()?.Stats?.CoreStats?.Kills ?? 0).Take(10);
                foreach (var player in topPlayers)
                {
                    var teamStats = player.PlayerTeamStats.FirstOrDefault()?.Stats?.CoreStats;
                    if (teamStats != null)
                    {
                        sb.AppendLine($"玩家：{player.PlayerId}");
                        sb.AppendLine($"  击杀：{teamStats.Kills} | 死亡：{teamStats.Deaths} | 助攻：{teamStats.Assists} | KDA: {teamStats.KDA}");
                        sb.AppendLine($"  爆头：{teamStats.HeadshotKills} | 精准度：{teamStats.Accuracy}% | 最大连杀：{teamStats.MaxKillingSpree}");
                        sb.AppendLine($"  伤害输出：{teamStats.DamageDealt} | 伤害承受：{teamStats.DamageTaken}");
                        sb.AppendLine();
                    }
                }

                MatchStatsResult.Text = sb.ToString();
                MatchStatsBorder.Visibility = Visibility.Visible;
                StatusText.Text = "成功获取比赛详细数据";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"查询失败：{ex.Message}";
                MatchStatsBorder.Visibility = Visibility.Collapsed;
            }
            finally
            {
                LoadingRing.IsActive = false;
                if (sender is Button btn && btn.DataContext is Core.Models.PlayerMatchHistory)
                    btn.IsEnabled = true;
                else
                    MatchStatsButton.IsEnabled = true;
            }
        }

        private async void CsrQueryButton_Click(object sender, RoutedEventArgs e)
        {
            var xuid = CsrXuidTextBox.Text?.Trim();
            var playlistId = CsrPlaylistIdTextBox.Text?.Trim();
            
            if (string.IsNullOrEmpty(xuid) || string.IsNullOrEmpty(playlistId))
            {
                StatusText.Text = "请输入 XUID 和 PlaylistId";
                return;
            }

            if (!CheckClient()) return;

            try
            {
                LoadingRing.IsActive = true;
                CsrQueryButton.IsEnabled = false;
                StatusText.Text = $"正在查询 CSR 段位...";
                CsrResultBorder.Visibility = Visibility.Collapsed;

                var csrs = await _client.GetPlaylistCsrAsync(playlistId, new[] { xuid });

                var sb = new StringBuilder();
                foreach (var csr in csrs)
                {
                    sb.AppendLine($"玩家：{csr.Id}");
                    if (csr.ResultCode == 1 && csr.Csr != null)
                    {
                        sb.AppendLine($"  段位：{csr.Csr.DesignationId}");
                        sb.AppendLine($"  等级：Tier {csr.Csr.Tier}, Division {csr.Csr.Division}");
                        sb.AppendLine($"  CSR 值：{csr.Csr.CsrValue}");
                        sb.AppendLine($"  下一级进度：{csr.Csr.PercentToNextTier}%");
                    }
                    else
                    {
                        sb.AppendLine($"  暂无段位数据");
                    }
                    sb.AppendLine();
                }

                CsrResult.Text = sb.ToString();
                CsrResultBorder.Visibility = Visibility.Visible;
                StatusText.Text = "成功获取 CSR 段位信息";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"查询失败：{ex.Message}";
                CsrResultBorder.Visibility = Visibility.Collapsed;
            }
            finally
            {
                LoadingRing.IsActive = false;
                CsrQueryButton.IsEnabled = true;
            }
        }

        private async void ServiceRecordButton_Click(object sender, RoutedEventArgs e)
        {
            var gamerTag = ServiceRecordTextBox.Text?.Trim();
            
            if (string.IsNullOrEmpty(gamerTag))
            {
                StatusText.Text = "请输入 GamerTag";
                return;
            }

            if (!CheckClient()) return;

            try
            {
                LoadingRing.IsActive = true;
                ServiceRecordButton.IsEnabled = false;
                StatusText.Text = $"正在查询战绩记录...";
                ServiceRecordBorder.Visibility = Visibility.Collapsed;

                var record = await _client.GetUserServiceRecordAsync(gamerTag);

                var sb = new StringBuilder();
                sb.AppendLine($"--- 总体战绩 ---");
                sb.AppendLine($"游戏时长：{record.TimePlayed}");
                sb.AppendLine($"完成比赛：{record.MatchesCompleted}");
                sb.AppendLine($"胜利：{record.Wins} | 失败：{record.Losses} | 平局：{record.Ties}");
                sb.AppendLine();
                sb.AppendLine("--- 核心数据 ---");
                sb.AppendLine($"击杀：{record.CoreStats.Kills}");
                sb.AppendLine($"死亡：{record.CoreStats.Deaths}");
                sb.AppendLine($"助攻：{record.CoreStats.Assists}");
                sb.AppendLine($"KDA: {record.CoreStats.AverageKDA}");
                sb.AppendLine($"爆头：{record.CoreStats.HeadshotKills}");
                sb.AppendLine($"精准度：{record.CoreStats.Accuracy}%");
                sb.AppendLine($"伤害输出：{record.CoreStats.DamageDealt}");
                sb.AppendLine($"最大连杀：{record.CoreStats.MaxKillingSpree}");
                sb.AppendLine();
                sb.AppendLine("--- 模式统计 ---");
                sb.AppendLine($"夺旗 - 夺旗：{record.CaptureTheFlagStats?.FlagCaptures ?? 0} | 抢旗：{record.CaptureTheFlagStats?.FlagGrabs ?? 0}");
                sb.AppendLine($"据点 - 占领：{record.ZonesStats?.ZoneCaptures ?? 0} | 防守击杀：{record.ZonesStats?.ZoneDefensiveKills ?? 0}");
                sb.AppendLine($"山丘之王 - 骷髅持有时间：{record.OddballStats?.TimeAsSkullCarrier ?? "N/A"}");
                sb.AppendLine($"淘汰 - 淘汰：{record.EliminationStats?.Eliminations ?? 0} | 执行：{record.EliminationStats?.Executions ?? 0}");

                ServiceRecordResult.Text = sb.ToString();
                ServiceRecordBorder.Visibility = Visibility.Visible;
                StatusText.Text = "成功获取战绩记录";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"查询失败：{ex.Message}";
                ServiceRecordBorder.Visibility = Visibility.Collapsed;
            }
            finally
            {
                LoadingRing.IsActive = false;
                ServiceRecordButton.IsEnabled = true;
            }
        }

        private async void PrivacyGetButton_Click(object sender, RoutedEventArgs e)
        {
            var xuid = PrivacyXuidTextBox.Text?.Trim();
            
            if (string.IsNullOrEmpty(xuid))
            {
                StatusText.Text = "请输入 XUID";
                return;
            }

            if (!CheckClient()) return;

            try
            {
                LoadingRing.IsActive = true;
                PrivacyGetButton.IsEnabled = false;
                StatusText.Text = $"正在查询隐私设置...";
                PrivacyResultBorder.Visibility = Visibility.Collapsed;

                var privacy = await _client.GetMatchesPrivacyAsync(xuid);

                var sb = new StringBuilder();
                sb.AppendLine($"匹配对战：{privacy.MatchmadeGames}");
                sb.AppendLine($"其他对战：{privacy.OtherGames}");

                PrivacyResult.Text = sb.ToString();
                PrivacyResultBorder.Visibility = Visibility.Visible;
                StatusText.Text = "成功获取隐私设置";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"查询失败：{ex.Message}";
                PrivacyResultBorder.Visibility = Visibility.Collapsed;
            }
            finally
            {
                LoadingRing.IsActive = false;
                PrivacyGetButton.IsEnabled = true;
            }
        }

        private async void PrivacySetButton_Click(object sender, RoutedEventArgs e)
        {
            var xuid = PrivacyXuidTextBox.Text?.Trim();
            
            if (string.IsNullOrEmpty(xuid))
            {
                StatusText.Text = "请输入 XUID";
                return;
            }

            if (!CheckClient()) return;

            try
            {
                LoadingRing.IsActive = true;
                PrivacySetButton.IsEnabled = false;
                StatusText.Text = $"正在更新隐私设置...";
                PrivacyResultBorder.Visibility = Visibility.Collapsed;

                var newPrivacy = new Core.Models.MatchesPrivacy
                {
                    MatchmadeGames = Core.Models.Privacy.Show,
                    OtherGames = Core.Models.Privacy.Show
                };

                var privacy = await _client.UpdateMatchesPrivacyAsync(xuid, newPrivacy);

                var sb = new StringBuilder();
                sb.AppendLine($"更新成功!");
                sb.AppendLine($"匹配对战：{privacy.MatchmadeGames}");
                sb.AppendLine($"其他对战：{privacy.OtherGames}");

                PrivacyResult.Text = sb.ToString();
                PrivacyResultBorder.Visibility = Visibility.Visible;
                StatusText.Text = "隐私设置更新成功";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"更新失败：{ex.Message}";
                PrivacyResultBorder.Visibility = Visibility.Collapsed;
            }
            finally
            {
                LoadingRing.IsActive = false;
                PrivacySetButton.IsEnabled = true;
            }
        }

        private async void BanQueryButton_Click(object sender, RoutedEventArgs e)
        {
            var xuid = BanXuidTextBox.Text?.Trim();
            
            if (string.IsNullOrEmpty(xuid))
            {
                StatusText.Text = "请输入 XUID";
                return;
            }

            if (!CheckClient()) return;

            try
            {
                LoadingRing.IsActive = true;
                BanQueryButton.IsEnabled = false;
                StatusText.Text = $"正在查询封禁信息...";
                BanResultBorder.Visibility = Visibility.Collapsed;

                var banSummary = await _client.GetBanSummaryAsync(new[] { xuid });

                var sb = new StringBuilder();
                if (banSummary.Results?.Length > 0)
                {
                    foreach (var result in banSummary.Results)
                    {
                        if (result.Result?.BansInEffect?.Length > 0)
                        {
                            foreach (var ban in result.Result.BansInEffect)
                            {
                                sb.AppendLine($"玩家：{result.Id}");
                                sb.AppendLine($"状态：生效中");
                                if (!string.IsNullOrEmpty(ban.BanMessagePath))
                                {
                                    sb.AppendLine($"封禁消息：{ban.BanMessagePath}");
                                }
                                sb.AppendLine($"类型：{(BanType)ban.Type}");
                                sb.AppendLine($"范围：{(BanScope)ban.Scope}");
                                if (ban.EnforceUntilUtc != null)
                                {
                                    sb.AppendLine($"截止时间：{ban.EnforceUntilUtc.ISO8601Date}");
                                }
                                sb.AppendLine();
                            }
                        }
                        else
                        {
                            sb.AppendLine($"玩家：{result.Id}");
                            sb.AppendLine("无封禁记录");
                            sb.AppendLine();
                        }
                    }
                }
                else
                {
                    sb.AppendLine("无封禁记录");
                }

                BanResult.Text = sb.ToString();
                BanResultBorder.Visibility = Visibility.Visible;
                StatusText.Text = "成功获取封禁信息";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"查询失败：{ex.Message}";
                BanResultBorder.Visibility = Visibility.Collapsed;
            }
            finally
            {
                LoadingRing.IsActive = false;
                BanQueryButton.IsEnabled = true;
            }
        }

        private async void MedalsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckClient()) return;

            try
            {
                LoadingRing.IsActive = true;
                MedalsButton.IsEnabled = false;
                StatusText.Text = $"正在获取勋章元数据...";
                MedalsResultBorder.Visibility = Visibility.Collapsed;

                var medals = await _client.GetMedalsMetadataFileAsync();

                var sb = new StringBuilder();
                sb.AppendLine($"--- 勋章列表 (前 20 个) ---");
                foreach (var medal in medals.Medals.Take(20))
                {
                    sb.AppendLine($"勋章：{medal.Name?.Value ?? "未知"}");
                    sb.AppendLine($"  ID: {medal.NameId}");
                    sb.AppendLine($"  描述：{medal.Description?.Value ?? "无描述"}");
                    sb.AppendLine($"  排序权重：{medal.SortingWeight}");
                    sb.AppendLine();
                }

                MedalsResult.Text = sb.ToString();
                MedalsResultBorder.Visibility = Visibility.Visible;
                StatusText.Text = $"成功获取 {medals.Medals.Length} 个勋章信息";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"查询失败：{ex.Message}";
                MedalsResultBorder.Visibility = Visibility.Collapsed;
            }
            finally
            {
                LoadingRing.IsActive = false;
                MedalsButton.IsEnabled = true;
            }
        }

        private async void SeasonButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckClient()) return;

            try
            {
                LoadingRing.IsActive = true;
                SeasonButton.IsEnabled = false;
                StatusText.Text = $"正在获取赛季信息...";
                SeasonResultBorder.Visibility = Visibility.Collapsed;

                var season = await _client.GetSeasonCalendarAsync();

                var sb = new StringBuilder();
                sb.AppendLine($"--- 赛季信息 ---");
                foreach (var s in season.Seasons)
                {
                    sb.AppendLine($"赛季:");
                    sb.AppendLine($"  开始：{s.StartDate?.ISO8601Date}");
                    sb.AppendLine($"  结束：{s.EndDate?.ISO8601Date}");
                    sb.AppendLine($"  赛季元数据：{s.SeasonMetadata}");
                }
                sb.AppendLine();
                sb.AppendLine($"--- 活动 ---");
                foreach (var evt in season.Events)
                {
                    sb.AppendLine($"活动:");
                    sb.AppendLine($"  开始：{evt.StartDate?.ISO8601Date}");
                    sb.AppendLine($"  结束：{evt.EndDate?.ISO8601Date}");
                    sb.AppendLine($"  奖励轨道：{evt.RewardTrackPath}");
                }

                SeasonResult.Text = sb.ToString();
                SeasonResultBorder.Visibility = Visibility.Visible;
                StatusText.Text = "成功获取赛季信息";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"查询失败：{ex.Message}";
                SeasonResultBorder.Visibility = Visibility.Collapsed;
            }
            finally
            {
                LoadingRing.IsActive = false;
                SeasonButton.IsEnabled = true;
            }
        }

        private async void CsrSeasonButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckClient()) return;

            try
            {
                LoadingRing.IsActive = true;
                CsrSeasonButton.IsEnabled = false;
                StatusText.Text = $"正在获取 CSR 赛季信息...";
                SeasonResultBorder.Visibility = Visibility.Collapsed;

                var season = await _client.GetCsrSeasonCalendarAsync();

                var sb = new StringBuilder();
                sb.AppendLine($"--- CSR 赛季信息 ---");
                foreach (var s in season.Seasons)
                {
                    sb.AppendLine($"CSR 赛季:");
                    sb.AppendLine($"  开始：{s.StartDate?.ISO8601Date}");
                    sb.AppendLine($"  结束：{s.EndDate?.ISO8601Date}");
                    sb.AppendLine($"  文件：{s.CsrSeasonFilePath}");
                }

                SeasonResult.Text = sb.ToString();
                SeasonResultBorder.Visibility = Visibility.Visible;
                StatusText.Text = "成功获取 CSR 赛季信息";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"查询失败：{ex.Message}";
                SeasonResultBorder.Visibility = Visibility.Collapsed;
            }
            finally
            {
                LoadingRing.IsActive = false;
                CsrSeasonButton.IsEnabled = true;
            }
        }

        private async void SetTokenButton_Click(object sender, RoutedEventArgs e)
        {
            var token = TokenTextBox.Text?.Trim();
            
            if (string.IsNullOrEmpty(token))
            {
                StatusText.Text = "请输入 Spartan Token";
                return;
            }

            try
            {
                // 初始化客户端
                InitializeClient(token);
                
                // 验证 Token 并获取当前用户信息
                var currentUser = await _client.GetCurrentUserAsync();
                
                // 保存到 UserSessionService（会自动持久化到 PasswordVault）
                await _userSessionService.LoginAsync(token);
                
                StatusText.Text = $"认证成功！当前用户 XUID: {currentUser.xuid}";
                
                // 自动填充用户信息（使用 XUID 查询玩家信息获取 Gamertag）
                try
                {
                    var userInfo = await _client.GetUserAsync(currentUser.xuid);
                    GamerTagTextBox.Text = userInfo.Gamertag;
                }
                catch
                {
                    // 如果无法获取玩家信息，至少显示 XUID
                    GamerTagTextBox.Text = currentUser.xuid;
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Token 设置失败：{ex.Message}";
                _client = null;
            }
        }
    }
}
