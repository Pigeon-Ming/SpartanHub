using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpartanHub.Core.Authentication;
using SpartanHub.Core.Endpoints;
using SpartanHub.Core.Exceptions;
using SpartanHub.Core.Models;
using SpartanHub.Core.Utilities;

namespace SpartanHub.Core.Clients
{
    public class HaloInfiniteClient
    {
        private static readonly SemaphoreSlim FlightConfigurationLock = new SemaphoreSlim(1, 1);
        private static string _flightConfigurationId;
        private static string _flightConfigurationXuid;

        private readonly ISpartanTokenProvider _spartanTokenProvider;
        private readonly IPlayerContextProvider _playerContextProvider;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerSettings _jsonSettings;

        public Action<ApiLogEntry> OnRequestLog { get; set; }

        public HaloInfiniteClient(ISpartanTokenProvider spartanTokenProvider, HttpClient httpClient = null)
        {
            _spartanTokenProvider = spartanTokenProvider ?? throw new ArgumentNullException(nameof(spartanTokenProvider));
            _playerContextProvider = spartanTokenProvider as IPlayerContextProvider;
            _httpClient = httpClient ?? new HttpClient();
            _jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        private async Task<HttpResponseMessage> ExecuteRequestAsync(string url, HttpMethod method, string requestBody = null, bool skipAuth = false, IDictionary<string, string> headers = null, bool skipClearance = false)
        {
            var logEntry = new ApiLogEntry
            {
                Timestamp = DateTime.Now,
                Method = method.Method,
                Url = url,
                RequestBody = requestBody,
                StatusCode = 0,
                IsSuccess = false
            };

            try
            {
                if (!skipAuth && !skipClearance)
                {
                    await EnsureActiveFlightConfigurationForRequestAsync(url).ConfigureAwait(false);
                }

                var request = new HttpRequestMessage(method, url);

                request.Headers.Add("User-Agent", GlobalConstants.HaloPcUserAgent);
                request.Headers.Add("Accept", "application/json");

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                if (!skipClearance && !request.Headers.Contains("343-clearance") && !string.IsNullOrWhiteSpace(_flightConfigurationId))
                {
                    request.Headers.Add("343-clearance", _flightConfigurationId);
                }

                if (!skipAuth)
                {
                    var spartanToken = await _spartanTokenProvider.GetSpartanTokenAsync().ConfigureAwait(false);
                    request.Headers.Add("x-343-authorization-spartan", spartanToken);
                }

                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                logEntry.StatusCode = (int)response.StatusCode;
                logEntry.IsSuccess = response.IsSuccessStatusCode;

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    logEntry.ErrorMessage = $"HTTP {response.StatusCode}: {errorContent}";
                    OnRequestLog?.Invoke(logEntry);
                    throw new HaloApiException(url, response);
                }

                OnRequestLog?.Invoke(logEntry);
                return response;
            }
            catch (Exception ex) when (!(ex is HaloApiException))
            {
                logEntry.ErrorMessage = ex.Message;
                OnRequestLog?.Invoke(logEntry);
                throw;
            }
        }

        private async Task<T> ExecuteJsonRequestAsync<T>(string url, HttpMethod method, string requestBody = null, bool skipAuth = false, IDictionary<string, string> headers = null, bool skipClearance = false)
        {
            var response = await ExecuteRequestAsync(url, method, requestBody, skipAuth, headers, skipClearance).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            
            OnRequestLog?.Invoke(new ApiLogEntry
            {
                Timestamp = DateTime.Now,
                Method = method.Method,
                Url = url,
                RequestBody = requestBody,
                ResponseBody = content,
                StatusCode = 200,
                IsSuccess = true
            });

            return JsonConvert.DeserializeObject<T>(content, _jsonSettings);
        }

        public async Task<string> EnsureActiveFlightConfigurationAsync(string playerXuid)
        {
            var unwrappedXuid = XuidUtility.UnwrapPlayerId(playerXuid ?? string.Empty);
            if (string.IsNullOrWhiteSpace(unwrappedXuid))
            {
                return _flightConfigurationId;
            }

            if (!string.IsNullOrWhiteSpace(_flightConfigurationId) && _flightConfigurationXuid == unwrappedXuid)
            {
                return _flightConfigurationId;
            }

            await FlightConfigurationLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (!string.IsNullOrWhiteSpace(_flightConfigurationId) && _flightConfigurationXuid == unwrappedXuid)
                {
                    return _flightConfigurationId;
                }

                var wrappedXuid = XuidUtility.WrapPlayerId(unwrappedXuid);
                var url = $"https://{HaloCoreEndpoints.SettingsOrigin}.{HaloCoreEndpoints.ServiceDomain}/oban/flight-configurations/titles/hi/audiences/retail/players/{wrappedXuid}/active";
                var configuration = await ExecuteJsonRequestAsync<ActiveFlightConfiguration>(url, HttpMethod.Get, skipClearance: true).ConfigureAwait(false);

                _flightConfigurationId = configuration?.FlightConfigurationId;
                _flightConfigurationXuid = unwrappedXuid;
                return _flightConfigurationId;
            }
            finally
            {
                FlightConfigurationLock.Release();
            }
        }

        private async Task EnsureActiveFlightConfigurationForRequestAsync(string url)
        {
            var xuid = _playerContextProvider?.PlayerXuid;
            if (string.IsNullOrWhiteSpace(xuid))
            {
                xuid = TryExtractXuidFromUrl(url);
            }

            var unwrappedXuid = XuidUtility.UnwrapPlayerId(xuid ?? string.Empty);
            if (!string.IsNullOrWhiteSpace(_flightConfigurationId) &&
                (string.IsNullOrWhiteSpace(unwrappedXuid) || _flightConfigurationXuid == unwrappedXuid))
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(xuid))
            {
                await EnsureActiveFlightConfigurationAsync(xuid).ConfigureAwait(false);
            }
        }

        private static string TryExtractXuidFromUrl(string url)
        {
            const string prefix = "xuid(";
            var start = url.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
            if (start < 0)
            {
                return null;
            }

            start += prefix.Length;
            var end = url.IndexOf(")", start, StringComparison.Ordinal);
            return end > start
                ? url.Substring(start, end - start)
                : null;
        }

        public async Task<UserInfo> GetUserAsync(string gamerTag)
        {
            var url = $"https://{HaloCoreEndpoints.Profile}.{HaloCoreEndpoints.ServiceDomain}/users/gt({gamerTag})";
            return await ExecuteJsonRequestAsync<UserInfo>(url, HttpMethod.Get).ConfigureAwait(false);
        }

        public async Task<UserInfo[]> GetUsersAsync(string[] xuids)
        {
            var xuidList = string.Join(",", xuids.Select(x => XuidUtility.UnwrapPlayerId(x)));
            var url = $"https://{HaloCoreEndpoints.Profile}.{HaloCoreEndpoints.ServiceDomain}/users?xuids={xuidList}";
            return await ExecuteJsonRequestAsync<UserInfo[]>(url, HttpMethod.Get).ConfigureAwait(false);
        }

        public async Task<PlaylistCsrContainer[]> GetPlaylistCsrAsync(string playlistId, string[] playerIds, string seasonId = null)
        {
            var playerIdList = string.Join(",", playerIds.Select(XuidUtility.WrapPlayerId));
            var urlParams = new StringBuilder($"players={playerIdList}");
            if (!string.IsNullOrEmpty(seasonId))
            {
                urlParams.Append($"&season={seasonId}");
            }

            var url = $"https://{HaloCoreEndpoints.SkillOrigin}.{HaloCoreEndpoints.ServiceDomain}/hi/playlist/{playlistId}/csrs?{urlParams}";
            var response = await ExecuteJsonRequestAsync<ResultsContainer<PlaylistCsrContainer>>(url, HttpMethod.Get).ConfigureAwait(false);
            return response.Value.ToArray();
        }

        public async Task<ServiceRecord> GetUserServiceRecordAsync(string gamerTagOrWrappedXuid, string seasonId = null, string playlistAssetId = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(seasonId)) queryParams.Add($"seasonId={seasonId}");
            if (!string.IsNullOrEmpty(playlistAssetId)) queryParams.Add($"playlistAssetId={playlistAssetId}");
            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";

            var url = $"https://{HaloCoreEndpoints.StatsOrigin}.{HaloCoreEndpoints.ServiceDomain}/hi/players/{gamerTagOrWrappedXuid}/Matchmade/servicerecord{queryString}";
            return await ExecuteJsonRequestAsync<ServiceRecord>(url, HttpMethod.Get).ConfigureAwait(false);
        }

        public async Task<Playlist> GetPlaylistAsync(string playlistId)
        {
            var url = $"https://{HaloCoreEndpoints.GameCmsOrigin}.{HaloCoreEndpoints.ServiceDomain}/hi/multiplayer/file/playlists/assets/{playlistId}.json";
            return await ExecuteJsonRequestAsync<Playlist>(url, HttpMethod.Get).ConfigureAwait(false);
        }

        public async Task<PlayerMatchHistory[]> GetPlayerMatchesAsync(string playerXuid, MatchType type = MatchType.All, int count = 25, int start = 0)
        {
            var queryParams = new List<string> { $"count={count}", $"start={start}" };
            if (type != MatchType.All)
            {
                queryParams.Add($"type={(int)type}");
            }
            var queryString = string.Join("&", queryParams);

            var url = $"https://{HaloCoreEndpoints.StatsOrigin}.{HaloCoreEndpoints.ServiceDomain}/hi/players/{XuidUtility.WrapPlayerId(playerXuid)}/matches?{queryString}";
            var response = await ExecuteJsonRequestAsync<PaginationContainer<PlayerMatchHistory>>(url, HttpMethod.Get).ConfigureAwait(false);
            return response.Results.ToArray();
        }

        public async Task<MatchCount> GetPlayerMatchCountAsync(string playerXuid)
        {
            var url = $"https://{HaloCoreEndpoints.StatsOrigin}.{HaloCoreEndpoints.ServiceDomain}/hi/players/{XuidUtility.WrapPlayerId(playerXuid)}/matches/count";
            return await ExecuteJsonRequestAsync<MatchCount>(url, HttpMethod.Get).ConfigureAwait(false);
        }

        public async Task<CurrentUserInfo> GetCurrentUserAsync()
        {
            var url = $"https://{HaloCoreEndpoints.CommsOrigin}.{HaloCoreEndpoints.ServiceDomain}/users/me";
            return await ExecuteJsonRequestAsync<CurrentUserInfo>(url, HttpMethod.Get).ConfigureAwait(false);
        }

        /// <summary>
        /// 获取玩家公开自定义外观信息，包括 ServiceTag。
        /// </summary>
        public async Task<PlayerCustomization> GetPlayerCustomizationAsync(string playerXuid, PlayerCustomizationView view = PlayerCustomizationView.Public)
        {
            var wrappedXuid = XuidUtility.WrapPlayerId(XuidUtility.UnwrapPlayerId(playerXuid));
            var viewValue = view.ToString().ToLowerInvariant();
            var url = $"https://{HaloCoreEndpoints.EconomyOrigin}.{HaloCoreEndpoints.ServiceDomainWithoutPort}/hi/players/{wrappedXuid}/customization?view={viewValue}";
            return await ExecuteJsonRequestAsync<PlayerCustomization>(url, HttpMethod.Get).ConfigureAwait(false);
        }

        /// <summary>
        /// 获取玩家 Operation 通行证奖励轨道进度。
        /// </summary>
        public async Task<PlayerOperationRewardTracks> GetPlayerOperationRewardTracksAsync(string playerXuid)
        {
            var wrappedXuid = XuidUtility.WrapPlayerId(XuidUtility.UnwrapPlayerId(playerXuid));
            var url = $"https://{HaloCoreEndpoints.EconomyOrigin}.{HaloCoreEndpoints.ServiceDomainWithoutPort}/hi/players/{wrappedXuid}/rewardtracks/operations?view=all";
            return await ExecuteJsonRequestAsync<PlayerOperationRewardTracks>(url, HttpMethod.Get).ConfigureAwait(false);
        }

        public async Task<MatchStats> GetMatchStatsAsync(string matchId)
        {
            var url = $"https://{HaloCoreEndpoints.StatsOrigin}.{HaloCoreEndpoints.ServiceDomain}/hi/matches/{matchId}/stats";
            return await ExecuteJsonRequestAsync<MatchStats>(url, HttpMethod.Get).ConfigureAwait(false);
        }

        public async Task<MatchSkill[]> GetMatchSkillAsync(string matchId, string[] playerIds)
        {
            var playerIdList = string.Join(",", playerIds.Select(XuidUtility.WrapPlayerId));
            var url = $"https://{HaloCoreEndpoints.SkillOrigin}.{HaloCoreEndpoints.ServiceDomain}/hi/matches/{matchId}/skill?players={playerIdList}";
            var response = await ExecuteJsonRequestAsync<ResultsContainer<MatchSkill>>(url, HttpMethod.Get).ConfigureAwait(false);
            return response.Value.ToArray();
        }

        /// <summary>
        /// 获取玩家对战隐私设置
        /// </summary>
        public async Task<MatchesPrivacy> GetMatchesPrivacyAsync(string playerXuid)
        {
            var url = $"https://{HaloCoreEndpoints.StatsOrigin}.{HaloCoreEndpoints.ServiceDomain}/hi/players/{XuidUtility.WrapPlayerId(playerXuid)}/matches-privacy";
            return await ExecuteJsonRequestAsync<MatchesPrivacy>(url, HttpMethod.Get).ConfigureAwait(false);
        }

        /// <summary>
        /// 更新玩家对战隐私设置
        /// </summary>
        public async Task<MatchesPrivacy> UpdateMatchesPrivacyAsync(string playerXuid, MatchesPrivacy matchesPrivacy)
        {
            var url = $"https://{HaloCoreEndpoints.StatsOrigin}.{HaloCoreEndpoints.ServiceDomain}/hi/players/{XuidUtility.WrapPlayerId(playerXuid)}/matches-privacy";
            var requestBody = JsonConvert.SerializeObject(new { matchesPrivacy });
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, url) { Content = content };
            var response = await ExecuteRequestWithContentAsync(request, requestBody).ConfigureAwait(false);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<MatchesPrivacy>(responseContent, _jsonSettings);
        }

        /// <summary>
        /// 获取进度文件
        /// </summary>
        public async Task<ProgressionFile> GetProgressionFileAsync(string filename)
        {
            var url = $"https://{HaloCoreEndpoints.GameCmsOrigin}.{HaloCoreEndpoints.ServiceDomain}/hi/Progression/file/{filename}";
            return await ExecuteJsonRequestAsync<ProgressionFile>(url, HttpMethod.Get).ConfigureAwait(false);
        }

        /// <summary>
        /// 获取勋章元数据文件
        /// </summary>
        public async Task<MedalsMetadataFile> GetMedalsMetadataFileAsync()
        {
            var url = $"https://{HaloCoreEndpoints.GameCmsOrigin}.{HaloCoreEndpoints.ServiceDomain}/hi/Waypoint/file/medals/metadata.json";
            return await ExecuteJsonRequestAsync<MedalsMetadataFile>(url, HttpMethod.Get).ConfigureAwait(false);
        }

        /// <summary>
        /// 获取玩家封禁摘要
        /// </summary>
        public async Task<BanSummary> GetBanSummaryAsync(string[] xuids)
        {
            var xuidList = string.Join(",", xuids.Select(XuidUtility.WrapPlayerId));
            var url = $"https://{HaloCoreEndpoints.BanProcessorOrigin}.{HaloCoreEndpoints.ServiceDomain}/hi/bansummary?targets={xuidList}";
            return await ExecuteJsonRequestAsync<BanSummary>(url, HttpMethod.Get).ConfigureAwait(false);
        }

        /// <summary>
        /// 获取封禁消息
        /// </summary>
        public async Task<BanMessage> GetBanMessageAsync(string banPath)
        {
            var url = $"https://{HaloCoreEndpoints.GameCmsOrigin}.{HaloCoreEndpoints.ServiceDomain}/hi/banning/file/{banPath}";
            return await ExecuteJsonRequestAsync<BanMessage>(url, HttpMethod.Get).ConfigureAwait(false);
        }

        /// <summary>
        /// 获取赛季日历
        /// </summary>
        public async Task<SeasonCalendar> GetSeasonCalendarAsync()
        {
            var url = $"https://{HaloCoreEndpoints.GameCmsOrigin}.{HaloCoreEndpoints.ServiceDomain}/hi/progression/file/calendars/seasons/seasoncalendar.json";
            return await ExecuteJsonRequestAsync<SeasonCalendar>(url, HttpMethod.Get).ConfigureAwait(false);
        }

        /// <summary>
        /// 获取CSR赛季日历
        /// </summary>
        public async Task<CsrSeasonCalendar> GetCsrSeasonCalendarAsync()
        {
            var url = $"https://{HaloCoreEndpoints.GameCmsOrigin}.{HaloCoreEndpoints.ServiceDomain}/hi/progression/file/calendars/csrseasons/csrseasoncalendar.json";
            return await ExecuteJsonRequestAsync<CsrSeasonCalendar>(url, HttpMethod.Get).ConfigureAwait(false);
        }

        /// <summary>
        /// 获取资产详情（地图、玩法等）
        /// </summary>
        public async Task<AssetDetail> GetAssetDetailAsync(string assetType, string assetId)
        {
            var url = $"https://{HaloCoreEndpoints.DiscoveryOrigin}.{HaloCoreEndpoints.ServiceDomain}/hi/{assetType}/{assetId}";
            return await ExecuteJsonRequestAsync<AssetDetail>(url, HttpMethod.Get).ConfigureAwait(false);
        }

        /// <summary>
        /// 获取指定版本的资产详情
        /// </summary>
        public async Task<AssetDetail> GetAssetVersionDetailAsync(string assetType, string assetId, string versionId)
        {
            var url = $"https://{HaloCoreEndpoints.DiscoveryOrigin}.{HaloCoreEndpoints.ServiceDomain}/hi/{assetType}/{assetId}/versions/{versionId}";
            return await ExecuteJsonRequestAsync<AssetDetail>(url, HttpMethod.Get).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> ExecuteRequestWithContentAsync(HttpRequestMessage request, string requestBody = null)
        {
            var logEntry = new ApiLogEntry
            {
                Timestamp = DateTime.Now,
                Method = request.Method.Method,
                Url = request.RequestUri.ToString(),
                RequestBody = requestBody,
                StatusCode = 0,
                IsSuccess = false
            };

            try
            {
                await EnsureActiveFlightConfigurationForRequestAsync(request.RequestUri.ToString()).ConfigureAwait(false);

                request.Headers.Add("User-Agent", GlobalConstants.HaloPcUserAgent);
                request.Headers.Add("Accept", "application/json");

                if (!request.Headers.Contains("343-clearance") && !string.IsNullOrWhiteSpace(_flightConfigurationId))
                {
                    request.Headers.Add("343-clearance", _flightConfigurationId);
                }

                var spartanToken = await _spartanTokenProvider.GetSpartanTokenAsync().ConfigureAwait(false);
                request.Headers.Add("x-343-authorization-spartan", spartanToken);

                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                logEntry.StatusCode = (int)response.StatusCode;
                logEntry.IsSuccess = response.IsSuccessStatusCode;

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    logEntry.ErrorMessage = $"HTTP {response.StatusCode}: {errorContent}";
                    OnRequestLog?.Invoke(logEntry);
                    throw new HaloApiException(request.RequestUri.ToString(), response);
                }

                OnRequestLog?.Invoke(logEntry);
                return response;
            }
            catch (Exception ex) when (!(ex is HaloApiException))
            {
                logEntry.ErrorMessage = ex.Message;
                OnRequestLog?.Invoke(logEntry);
                throw;
            }
        }
    }

    public class ResultsContainer<T>
    {
        [JsonProperty("Value")]
        public List<T> Value { get; set; }
    }

    public class PaginationContainer<T>
    {
        [JsonProperty("Start")]
        public int Start { get; set; }

        [JsonProperty("Count")]
        public int Count { get; set; }

        [JsonProperty("ResultCount")]
        public int ResultCount { get; set; }

        [JsonProperty("Results")]
        public List<T> Results { get; set; }
    }

    public class CurrentUserInfo
    {
        [JsonProperty("xuid")]
        public string xuid { get; set; }

        [JsonProperty("notificationsReadDate")]
        public string notificationsReadDate { get; set; }
    }

    public class MatchCount
    {
        [JsonProperty("All")]
        public int All { get; set; }

        [JsonProperty("Matchmade")]
        public int Matchmade { get; set; }

        [JsonProperty("Custom")]
        public int Custom { get; set; }

        [JsonProperty("Local")]
        public int Local { get; set; }
    }

    public enum MatchType
    {
        All = 0,
        Matchmade = 1,
        Custom = 2,
        Local = 3
    }
}
