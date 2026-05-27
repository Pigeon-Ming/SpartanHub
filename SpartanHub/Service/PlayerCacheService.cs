using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using SpartanHub.Core.Clients;
using SpartanHub.Core.Models;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace SpartanHub.Service
{
    public class PlayerCacheService
    {
        private static readonly Lazy<PlayerCacheService> _instance = new Lazy<PlayerCacheService>(() => new PlayerCacheService());
        
        public static PlayerCacheService Instance => _instance.Value;

        private readonly ConcurrentDictionary<string, CachedPlayerInfo> _playerCache = new ConcurrentDictionary<string, CachedPlayerInfo>();
        private readonly ConcurrentDictionary<string, CachedImage> _avatarCache = new ConcurrentDictionary<string, CachedImage>();
        private readonly HaloInfiniteClient _haloClient;
        private readonly HttpClient _httpClient;

        private PlayerCacheService()
        {
            _haloClient = new HaloInfiniteClient(UserSessionService.Instance);
            _httpClient = new HttpClient();
        }

        public async Task<UserInfo> GetPlayerInfoAsync(string xuid)
        {
            if (string.IsNullOrEmpty(xuid))
            {
                return null;
            }

            if (_playerCache.TryGetValue(xuid, out var cached) && !cached.IsExpired)
            {
                return cached.Info;
            }

            try
            {
                var users = await _haloClient.GetUsersAsync(new[] { xuid });
                var userInfo = users?[0];

                if (userInfo != null)
                {
                    _playerCache[xuid] = new CachedPlayerInfo
                    {
                        Info = userInfo,
                        ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(30)
                    };
                }

                return userInfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> GetPlayerAvatarUrlAsync(string xuid, AvatarSize size = AvatarSize.Large)
        {
            if (string.IsNullOrEmpty(xuid))
            {
                return null;
            }

            var cacheKey = $"{xuid}_{size}";
            
            if (_avatarCache.TryGetValue(cacheKey, out var cached) && !cached.IsExpired)
            {
                return cached.Url;
            }

            try
            {
                var userInfo = await GetPlayerInfoAsync(xuid);
                if (userInfo?.Gamerpic == null)
                {
                    return null;
                }

                var avatarUrl = size switch
                {
                    AvatarSize.Small => userInfo.Gamerpic.Small,
                    AvatarSize.Medium => userInfo.Gamerpic.Medium,
                    AvatarSize.Large => userInfo.Gamerpic.Large,
                    AvatarSize.XLarge => userInfo.Gamerpic.Xlarge,
                    _ => userInfo.Gamerpic.Large
                };

                _avatarCache[cacheKey] = new CachedImage
                {
                    Url = avatarUrl,
                    ExpiresAt = DateTimeOffset.UtcNow.AddHours(1)
                };

                return avatarUrl;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void CachePlayerInfo(UserInfo info)
        {
            if (info == null || string.IsNullOrEmpty(info.Xuid))
            {
                return;
            }

            _playerCache[info.Xuid] = new CachedPlayerInfo
            {
                Info = info,
                ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(30)
            };
        }

        public void ClearExpiredCache()
        {
            var now = DateTimeOffset.UtcNow;
            
            foreach (var kvp in _playerCache)
            {
                if (kvp.Value.IsExpired)
                {
                    _playerCache.TryRemove(kvp.Key, out _);
                }
            }

            foreach (var kvp in _avatarCache)
            {
                if (kvp.Value.IsExpired)
                {
                    _avatarCache.TryRemove(kvp.Key, out _);
                }
            }
        }

        public void ClearAllCache()
        {
            _playerCache.Clear();
            _avatarCache.Clear();
        }

        private class CachedPlayerInfo
        {
            public UserInfo Info { get; set; }
            public DateTimeOffset ExpiresAt { get; set; }
            public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;
        }

        private class CachedImage
        {
            public string Url { get; set; }
            public DateTimeOffset ExpiresAt { get; set; }
            public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;
        }

        public enum AvatarSize
        {
            Small,
            Medium,
            Large,
            XLarge
        }
    }
}
