using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpartanHub.Core.Clients;
using SpartanHub.Core.Models;

namespace SpartanHub.Service
{
    public class MatchDataCacheService
    {
        private static readonly Lazy<MatchDataCacheService> _instance = new Lazy<MatchDataCacheService>(() => new MatchDataCacheService());
        
        public static MatchDataCacheService Instance => _instance.Value;

        private readonly Dictionary<string, CachedMatchDetail> _matchDetailCache = new Dictionary<string, CachedMatchDetail>();
        private readonly Dictionary<string, CachedMatchHistory> _matchHistoryCache = new Dictionary<string, CachedMatchHistory>();
        private readonly HaloInfiniteClient _haloClient;

        private MatchDataCacheService()
        {
            _haloClient = new HaloInfiniteClient(UserSessionService.Instance);
        }

        public async Task<MatchStats> GetMatchDetailAsync(string matchId)
        {
            if (string.IsNullOrEmpty(matchId))
            {
                return null;
            }

            if (_matchDetailCache.TryGetValue(matchId, out var cached) && !cached.IsExpired)
            {
                return cached.MatchStats;
            }

            try
            {
                var matchStats = await _haloClient.GetMatchStatsAsync(matchId);
                
                if (matchStats != null)
                {
                    _matchDetailCache[matchId] = new CachedMatchDetail
                    {
                        MatchStats = matchStats,
                        ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10)
                    };
                }

                return matchStats;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<PlayerMatchHistory>> GetRecentMatchesAsync(string xuid, int count = 25)
        {
            if (string.IsNullOrEmpty(xuid))
            {
                return Enumerable.Empty<PlayerMatchHistory>();
            }

            if (_matchHistoryCache.TryGetValue(xuid, out var cached) && !cached.IsExpired)
            {
                return cached.Matches.Take(count);
            }

            try
            {
                var history = await _haloClient.GetPlayerMatchesAsync(xuid, MatchType.All, count);
                
                if (history != null)
                {
                    _matchHistoryCache[xuid] = new CachedMatchHistory
                    {
                        Matches = history.ToList(),
                        ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10)
                    };
                }

                return history?.Take(count) ?? Enumerable.Empty<PlayerMatchHistory>();
            }
            catch (Exception)
            {
                return Enumerable.Empty<PlayerMatchHistory>();
            }
        }



        public void CacheMatchDetail(MatchStats matchStats)
        {
            if (matchStats == null || string.IsNullOrEmpty(matchStats.MatchId))
            {
                return;
            }

            _matchDetailCache[matchStats.MatchId] = new CachedMatchDetail
            {
                MatchStats = matchStats,
                ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10)
            };
        }

        public void InvalidateMatchCache(string matchId)
        {
            if (!string.IsNullOrEmpty(matchId))
            {
                _matchDetailCache.Remove(matchId);
            }
        }

        public void InvalidateHistoryCache(string xuid)
        {
            if (!string.IsNullOrEmpty(xuid))
            {
                _matchHistoryCache.Remove(xuid);
            }
        }

        public void ClearAllCache()
        {
            _matchDetailCache.Clear();
            _matchHistoryCache.Clear();
        }

        private class CachedMatchDetail
        {
            public MatchStats MatchStats { get; set; }
            public DateTimeOffset ExpiresAt { get; set; }
            public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;
        }

        private class CachedMatchHistory
        {
            public List<PlayerMatchHistory> Matches { get; set; }
            public DateTimeOffset ExpiresAt { get; set; }
            public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;
        }
    }
}
