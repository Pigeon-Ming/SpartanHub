using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpartanHub.Core.Clients;
using SpartanHub.Core.Models;

namespace SpartanHub.Service
{
    public class MetadataService
    {
        private static readonly Lazy<MetadataService> _instance = new Lazy<MetadataService>(() => new MetadataService());
        
        public static MetadataService Instance => _instance.Value;

        private readonly HaloInfiniteClient _haloClient;
        
        private CachedMedalsMetadata _cachedMedalsMetadata;
        private readonly Dictionary<string, CachedAssetDetail> _cachedAssetDetails = new Dictionary<string, CachedAssetDetail>();
        private readonly Dictionary<string, CachedPlaylist> _cachedPlaylists = new Dictionary<string, CachedPlaylist>();

        public MedalsMetadataFile MedalsMetadata => _cachedMedalsMetadata?.Metadata;

        private MetadataService()
        {
            _haloClient = new HaloInfiniteClient(UserSessionService.Instance);
        }

        public async Task<MedalsMetadataFile> GetMedalsMetadataAsync()
        {
            if (_cachedMedalsMetadata != null && !_cachedMedalsMetadata.IsExpired)
            {
                return _cachedMedalsMetadata.Metadata;
            }

            try
            {
                var metadata = await _haloClient.GetMedalsMetadataFileAsync();
                
                _cachedMedalsMetadata = new CachedMedalsMetadata
                {
                    Metadata = metadata,
                    ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
                };

                return metadata;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<AssetDetail> GetAssetDetailAsync(string assetType, string assetId)
        {
            if (string.IsNullOrEmpty(assetType) || string.IsNullOrEmpty(assetId))
            {
                return null;
            }

            var cacheKey = $"{assetType}_{assetId}";
            
            if (_cachedAssetDetails.TryGetValue(cacheKey, out var cached) && !cached.IsExpired)
            {
                return cached.AssetDetail;
            }

            try
            {
                var assetDetail = await _haloClient.GetAssetDetailAsync(assetType, assetId);
                
                if (assetDetail != null)
                {
                    _cachedAssetDetails[cacheKey] = new CachedAssetDetail
                    {
                        AssetDetail = assetDetail,
                        ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
                    };
                }

                return assetDetail;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Playlist> GetPlaylistAsync(string playlistId)
        {
            if (string.IsNullOrEmpty(playlistId))
            {
                return null;
            }

            if (_cachedPlaylists.TryGetValue(playlistId, out var cached) && !cached.IsExpired)
            {
                return cached.Playlist;
            }

            try
            {
                var playlist = await _haloClient.GetPlaylistAsync(playlistId);
                
                if (playlist != null)
                {
                    _cachedPlaylists[playlistId] = new CachedPlaylist
                    {
                        Playlist = playlist,
                        ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
                    };
                }

                return playlist;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void ClearAllCache()
        {
            _cachedMedalsMetadata = null;
            _cachedAssetDetails.Clear();
            _cachedPlaylists.Clear();
        }

        private class CachedMedalsMetadata
        {
            public MedalsMetadataFile Metadata { get; set; }
            public DateTimeOffset ExpiresAt { get; set; }
            public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;
        }

        private class CachedAssetDetail
        {
            public AssetDetail AssetDetail { get; set; }
            public DateTimeOffset ExpiresAt { get; set; }
            public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;
        }

        private class CachedPlaylist
        {
            public Playlist Playlist { get; set; }
            public DateTimeOffset ExpiresAt { get; set; }
            public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;
        }
    }
}
