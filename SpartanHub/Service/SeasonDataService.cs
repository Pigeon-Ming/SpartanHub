using System;
using System.Threading.Tasks;
using SpartanHub.Core.Clients;
using SpartanHub.Core.Models;

namespace SpartanHub.Service
{
    public class SeasonDataService
    {
        private static readonly Lazy<SeasonDataService> _instance = new Lazy<SeasonDataService>(() => new SeasonDataService());
        
        public static SeasonDataService Instance => _instance.Value;

        private readonly HaloInfiniteClient _haloClient;
        
        private CachedSeasonData _cachedSeasonData;
        private CachedProgressionData _cachedProgressionData;

        public SeasonCalendar CurrentSeason => _cachedSeasonData?.Calendar;
        public ProgressionFile Progression => _cachedProgressionData?.Progression;

        private SeasonDataService()
        {
            _haloClient = new HaloInfiniteClient(UserSessionService.Instance);
        }

        public async Task<SeasonCalendar> GetCurrentSeasonAsync()
        {
            if (_cachedSeasonData != null && !_cachedSeasonData.IsExpired)
            {
                return _cachedSeasonData.Calendar;
            }

            try
            {
                var seasonCalendar = await _haloClient.GetSeasonCalendarAsync();
                
                _cachedSeasonData = new CachedSeasonData
                {
                    Calendar = seasonCalendar,
                    ExpiresAt = DateTimeOffset.UtcNow.AddHours(1)
                };

                return seasonCalendar;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ProgressionFile> GetProgressionAsync()
        {
            if (_cachedProgressionData != null && !_cachedProgressionData.IsExpired)
            {
                return _cachedProgressionData.Progression;
            }

            try
            {
                var progression = await _haloClient.GetProgressionFileAsync("progression.json");
                
                _cachedProgressionData = new CachedProgressionData
                {
                    Progression = progression,
                    ExpiresAt = DateTimeOffset.UtcNow.AddHours(1)
                };

                return progression;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<CsrSeasonCalendar> GetCurrentCsrSeasonAsync()
        {
            try
            {
                var csrSeasonCalendar = await _haloClient.GetCsrSeasonCalendarAsync();
                
                return csrSeasonCalendar;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void RefreshSeasonData()
        {
            _cachedSeasonData = null;
            _cachedProgressionData = null;
        }

        public void ClearCache()
        {
            _cachedSeasonData = null;
            _cachedProgressionData = null;
        }

        private class CachedSeasonData
        {
            public SeasonCalendar Calendar { get; set; }
            public DateTimeOffset ExpiresAt { get; set; }
            public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;
        }

        private class CachedProgressionData
        {
            public ProgressionFile Progression { get; set; }
            public DateTimeOffset ExpiresAt { get; set; }
            public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;
        }
    }
}
