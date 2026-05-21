using System;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using SpartanHub.Core.Authentication;
using SpartanHub.Core.Clients;

namespace SpartanHub
{
    public sealed partial class DevPage : Page
    {
        private HaloInfiniteClient _client;
        private StaticSpartanTokenProvider _tokenProvider;

        public DevPage()
        {
            this.InitializeComponent();
        }

        private void InitializeClient(string token)
        {
            _tokenProvider = new StaticSpartanTokenProvider(token);
            _client = new HaloInfiniteClient(_tokenProvider);
        }

        private bool CheckClient()
        {
            if (_client == null)
            {
                StatusText.Text = "请先设置Spartan Token";
                return false;
            }
            return true;
        }

        private async void QueryButton_Click(object sender, RoutedEventArgs e)
        {
            var gamerTag = GamerTagTextBox.Text?.Trim();
            
            if (string.IsNullOrEmpty(gamerTag))
            {
                StatusText.Text = "请输入GamerTag";
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
                StatusText.Text = $"查询失败: {ex.Message}";
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
                StatusText.Text = "请输入XUID";
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
                StatusText.Text = $"查询失败: {ex.Message}";
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
                    StatusText.Text = "请输入MatchId或点击对战历史中的'详细'按钮";
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

                var sb = new StringBuilder();
                sb.AppendLine($"比赛ID: {stats.MatchId}");
                sb.AppendLine($"地图: {stats.MatchInfo.MapId}");
                sb.AppendLine($"玩法: {stats.MatchInfo.PlaylistId}");
                sb.AppendLine($"日期: {stats.MatchInfo.MatchCompletedDate}");
                sb.AppendLine();
                sb.AppendLine("--- 队伍统计 ---");
                foreach (var team in stats.Teams)
                {
                    sb.AppendLine($"队伍 {team.TeamId}: 排名 {team.Rank}, 结果 {team.Outcome}");
                    sb.AppendLine($"  击杀: {team.Stats.CoreStats.Kills} | 死亡: {team.Stats.CoreStats.Deaths} | 助攻: {team.Stats.CoreStats.Assists}");
                    sb.AppendLine($"  爆头: {team.Stats.CoreStats.HeadshotKills} | 精准度: {team.Stats.CoreStats.Accuracy}%");
                }
                sb.AppendLine();
                sb.AppendLine("--- 玩家统计 (前10) ---");
                var topPlayers = stats.Players.OrderByDescending(p => p.PlayerTeamStats.FirstOrDefault()?.Stats?.CoreStats?.Kills ?? 0).Take(10);
                foreach (var player in topPlayers)
                {
                    var teamStats = player.PlayerTeamStats.FirstOrDefault()?.Stats?.CoreStats;
                    if (teamStats != null)
                    {
                        sb.AppendLine($"玩家: {player.PlayerId}");
                        sb.AppendLine($"  击杀: {teamStats.Kills} | 死亡: {teamStats.Deaths} | 助攻: {teamStats.Assists} | KDA: {teamStats.KDA}");
                        sb.AppendLine($"  爆头: {teamStats.HeadshotKills} | 精准度: {teamStats.Accuracy}% | 最大连杀: {teamStats.MaxKillingSpree}");
                        sb.AppendLine($"  伤害输出: {teamStats.DamageDealt} | 伤害承受: {teamStats.DamageTaken}");
                        sb.AppendLine();
                    }
                }

                MatchStatsResult.Text = sb.ToString();
                MatchStatsBorder.Visibility = Visibility.Visible;
                StatusText.Text = "成功获取比赛详细数据";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"查询失败: {ex.Message}";
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
                StatusText.Text = "请输入XUID和PlaylistId";
                return;
            }

            if (!CheckClient()) return;

            try
            {
                LoadingRing.IsActive = true;
                CsrQueryButton.IsEnabled = false;
                StatusText.Text = $"正在查询CSR段位...";
                CsrResultBorder.Visibility = Visibility.Collapsed;

                var csrs = await _client.GetPlaylistCsrAsync(playlistId, new[] { xuid });

                var sb = new StringBuilder();
                foreach (var csr in csrs)
                {
                    sb.AppendLine($"玩家: {csr.Id}");
                    if (csr.ResultCode == 1 && csr.Csr != null)
                    {
                        sb.AppendLine($"  段位: {csr.Csr.DesignationId}");
                        sb.AppendLine($"  等级: Tier {csr.Csr.Tier}, Division {csr.Csr.Division}");
                        sb.AppendLine($"  CSR值: {csr.Csr.CsrValue}");
                        sb.AppendLine($"  下一级进度: {csr.Csr.PercentToNextTier}%");
                    }
                    else
                    {
                        sb.AppendLine($"  暂无段位数据");
                    }
                    sb.AppendLine();
                }

                CsrResult.Text = sb.ToString();
                CsrResultBorder.Visibility = Visibility.Visible;
                StatusText.Text = "成功获取CSR段位信息";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"查询失败: {ex.Message}";
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
                StatusText.Text = "请输入GamerTag";
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
                sb.AppendLine($"游戏时长: {record.TimePlayed}");
                sb.AppendLine($"完成比赛: {record.MatchesCompleted}");
                sb.AppendLine($"胜利: {record.Wins} | 失败: {record.Losses} | 平局: {record.Ties}");
                sb.AppendLine();
                sb.AppendLine("--- 核心数据 ---");
                sb.AppendLine($"击杀: {record.CoreStats.Kills}");
                sb.AppendLine($"死亡: {record.CoreStats.Deaths}");
                sb.AppendLine($"助攻: {record.CoreStats.Assists}");
                sb.AppendLine($"KDA: {record.CoreStats.AverageKDA}");
                sb.AppendLine($"爆头: {record.CoreStats.HeadshotKills}");
                sb.AppendLine($"精准度: {record.CoreStats.Accuracy}%");
                sb.AppendLine($"伤害输出: {record.CoreStats.DamageDealt}");
                sb.AppendLine($"最大连杀: {record.CoreStats.MaxKillingSpree}");
                sb.AppendLine();
                sb.AppendLine("--- 模式统计 ---");
                sb.AppendLine($"夺旗 - 夺旗: {record.CaptureTheFlagStats?.FlagCaptures ?? 0} | 抢旗: {record.CaptureTheFlagStats?.FlagGrabs ?? 0}");
                sb.AppendLine($"据点 - 占领: {record.ZonesStats?.ZoneCaptures ?? 0} | 防守击杀: {record.ZonesStats?.ZoneDefensiveKills ?? 0}");
                sb.AppendLine($"山丘之王 - 骷髅持有时间: {record.OddballStats?.TimeAsSkullCarrier ?? "N/A"}");
                sb.AppendLine($"淘汰 - 淘汰: {record.EliminationStats?.Eliminations ?? 0} | 执行: {record.EliminationStats?.Executions ?? 0}");

                ServiceRecordResult.Text = sb.ToString();
                ServiceRecordBorder.Visibility = Visibility.Visible;
                StatusText.Text = "成功获取战绩记录";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"查询失败: {ex.Message}";
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
                StatusText.Text = "请输入XUID";
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
                sb.AppendLine($"匹配对战: {privacy.MatchmadeGames}");
                sb.AppendLine($"其他对战: {privacy.OtherGames}");

                PrivacyResult.Text = sb.ToString();
                PrivacyResultBorder.Visibility = Visibility.Visible;
                StatusText.Text = "成功获取隐私设置";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"查询失败: {ex.Message}";
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
                StatusText.Text = "请输入XUID";
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
                sb.AppendLine($"匹配对战: {privacy.MatchmadeGames}");
                sb.AppendLine($"其他对战: {privacy.OtherGames}");

                PrivacyResult.Text = sb.ToString();
                PrivacyResultBorder.Visibility = Visibility.Visible;
                StatusText.Text = "成功更新隐私设置";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"更新失败: {ex.Message}";
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
                StatusText.Text = "请输入XUID";
                return;
            }

            if (!CheckClient()) return;

            try
            {
                LoadingRing.IsActive = true;
                BanQueryButton.IsEnabled = false;
                StatusText.Text = $"正在查询封禁状态...";
                BanResultBorder.Visibility = Visibility.Collapsed;

                var banSummary = await _client.GetBanSummaryAsync(new[] { xuid });

                var sb = new StringBuilder();
                foreach (var result in banSummary.Results)
                {
                    sb.AppendLine($"玩家: {result.Id}");
                    if (result.Result != null && result.Result.BansInEffect != null)
                    {
                        if (result.Result.BansInEffect.Length > 0)
                        {
                            foreach (var ban in result.Result.BansInEffect)
                            {
                                sb.AppendLine($"  类型: {ban.Type} | 范围: {ban.Scope}");
                                sb.AppendLine($"  封禁至: {ban.EnforceUntilUtc?.ISO8601Date}");
                                sb.AppendLine($"  消息路径: {ban.BanMessagePath}");
                            }
                        }
                        else
                        {
                            sb.AppendLine($"  无封禁记录");
                        }
                    }
                    sb.AppendLine();
                }

                BanResult.Text = sb.ToString();
                BanResultBorder.Visibility = Visibility.Visible;
                StatusText.Text = "成功获取封禁信息";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"查询失败: {ex.Message}";
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
                StatusText.Text = $"正在获取勋章列表...";
                MedalsResultBorder.Visibility = Visibility.Collapsed;

                var medals = await _client.GetMedalsMetadataFileAsync();

                var sb = new StringBuilder();
                sb.AppendLine($"勋章总数: {medals.Medals.Length}");
                sb.AppendLine();
                sb.AppendLine("--- 前20个勋章 ---");
                var topMedals = medals.Medals.Take(20);
                foreach (var medal in topMedals)
                {
                    sb.AppendLine($"  {medal.Name?.Value ?? medal.NameId.ToString()}: {medal.Description?.Value}");
                }

                MedalsResult.Text = sb.ToString();
                MedalsResultBorder.Visibility = Visibility.Visible;
                StatusText.Text = "成功获取勋章列表";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"查询失败: {ex.Message}";
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
                    sb.AppendLine($"  开始: {s.StartDate?.ISO8601Date}");
                    sb.AppendLine($"  结束: {s.EndDate?.ISO8601Date}");
                    sb.AppendLine($"  赛季元数据: {s.SeasonMetadata}");
                }
                sb.AppendLine();
                sb.AppendLine($"--- 活动 ---");
                foreach (var evt in season.Events)
                {
                    sb.AppendLine($"活动:");
                    sb.AppendLine($"  开始: {evt.StartDate?.ISO8601Date}");
                    sb.AppendLine($"  结束: {evt.EndDate?.ISO8601Date}");
                    sb.AppendLine($"  奖励轨道: {evt.RewardTrackPath}");
                }

                SeasonResult.Text = sb.ToString();
                SeasonResultBorder.Visibility = Visibility.Visible;
                StatusText.Text = "成功获取赛季信息";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"查询失败: {ex.Message}";
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
                StatusText.Text = $"正在获取CSR赛季信息...";
                SeasonResultBorder.Visibility = Visibility.Collapsed;

                var season = await _client.GetCsrSeasonCalendarAsync();

                var sb = new StringBuilder();
                sb.AppendLine($"--- CSR赛季信息 ---");
                foreach (var s in season.Seasons)
                {
                    sb.AppendLine($"CSR赛季:");
                    sb.AppendLine($"  开始: {s.StartDate?.ISO8601Date}");
                    sb.AppendLine($"  结束: {s.EndDate?.ISO8601Date}");
                    sb.AppendLine($"  文件: {s.CsrSeasonFilePath}");
                }

                SeasonResult.Text = sb.ToString();
                SeasonResultBorder.Visibility = Visibility.Visible;
                StatusText.Text = "成功获取CSR赛季信息";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"查询失败: {ex.Message}";
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
                StatusText.Text = "请输入Spartan Token";
                return;
            }

            try
            {
                InitializeClient(token);
                StatusText.Text = "Token设置成功，现在可以查询玩家信息";
                
                var currentUser = await _client.GetCurrentUserAsync();
                StatusText.Text = $"认证成功，当前用户XUID: {currentUser.xuid}";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Token设置失败: {ex.Message}";
            }
        }
    }
}
