using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class MatchInfo
    {
        [JsonProperty("StartTime")]
        public string StartTime { get; set; }

        [JsonProperty("EndTime")]
        public string EndTime { get; set; }

        [JsonProperty("Duration")]
        public string Duration { get; set; }

        [JsonProperty("PlayableDuration")]
        public string PlayableDuration { get; set; }

        [JsonProperty("LifecycleMode")]
        public int LifecycleMode { get; set; }

        [JsonProperty("GameVariantCategory")]
        public int GameVariantCategory { get; set; }

        [JsonProperty("Category")]
        public int Category { get; set; }

        [JsonProperty("LevelId")]
        public string LevelId { get; set; }

        [JsonProperty("MapVariant")]
        public AssetInfo MapVariant { get; set; }

        [JsonProperty("MapId")]
        public string MapId { get; set; }

        [JsonProperty("MapVersionId")]
        public string MapVersionId { get; set; }

        [JsonProperty("MapModePairId")]
        public string MapModePairId { get; set; }

        [JsonProperty("MapModePairVersionId")]
        public string MapModePairVersionId { get; set; }

        [JsonProperty("UgcGameVariant")]
        public AssetInfo UgcGameVariant { get; set; }

        [JsonProperty("UgcGameVariantId")]
        public string UgcGameVariantId { get; set; }

        [JsonProperty("UgcGameVariantVersionId")]
        public string UgcGameVariantVersionId { get; set; }

        [JsonProperty("Playlist")]
        public AssetInfo Playlist { get; set; }

        [JsonProperty("PlaylistId")]
        public string PlaylistId { get; set; }

        [JsonProperty("PlaylistVersionId")]
        public string PlaylistVersionId { get; set; }

        [JsonProperty("PlaylistExperience")]
        public int PlaylistExperience { get; set; }

        [JsonProperty("PlaylistMapModePair")]
        public AssetInfo PlaylistMapModePair { get; set; }

        [JsonProperty("SeasonId")]
        public string SeasonId { get; set; }

        [JsonProperty("TeamsEnabled")]
        public bool TeamsEnabled { get; set; }

        [JsonProperty("TeamScoringEnabled")]
        public bool TeamScoringEnabled { get; set; }

        [JsonProperty("GameplayInteraction")]
        public int GameplayInteraction { get; set; }

        [JsonProperty("ClearanceId")]
        public string ClearanceId { get; set; }

        [JsonProperty("MatchmakingId")]
        public string MatchmakingId { get; set; }

        [JsonProperty("MatchCompletedDate")]
        public string MatchCompletedDate { get; set; }
    }

    public class AssetInfo
    {
        [JsonProperty("AssetKind")]
        public int AssetKind { get; set; }

        [JsonProperty("AssetId")]
        public string AssetId { get; set; }

        [JsonProperty("VersionId")]
        public string VersionId { get; set; }
    }
}
