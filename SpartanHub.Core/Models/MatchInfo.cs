using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class MatchInfo
    {
        [JsonProperty("MapId")]
        public string MapId { get; set; }

        [JsonProperty("MapVersionId")]
        public string MapVersionId { get; set; }

        [JsonProperty("MapModePairId")]
        public string MapModePairId { get; set; }

        [JsonProperty("MapModePairVersionId")]
        public string MapModePairVersionId { get; set; }

        [JsonProperty("UgcGameVariantId")]
        public string UgcGameVariantId { get; set; }

        [JsonProperty("UgcGameVariantVersionId")]
        public string UgcGameVariantVersionId { get; set; }

        [JsonProperty("PlaylistId")]
        public string PlaylistId { get; set; }

        [JsonProperty("PlaylistVersionId")]
        public string PlaylistVersionId { get; set; }

        [JsonProperty("MatchmakingId")]
        public string MatchmakingId { get; set; }

        [JsonProperty("Category")]
        public int Category { get; set; }

        [JsonProperty("MatchCompletedDate")]
        public string MatchCompletedDate { get; set; }
    }
}
