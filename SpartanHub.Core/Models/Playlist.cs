using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class Playlist
    {
        [JsonProperty("NameHint")]
        public string NameHint { get; set; }

        [JsonProperty("UgcPlaylistVersion")]
        public string UgcPlaylistVersion { get; set; }

        [JsonProperty("PlatformMatchmakingHopperId")]
        public string PlatformMatchmakingHopperId { get; set; }

        [JsonProperty("GameStartRulesId")]
        public string GameStartRulesId { get; set; }

        [JsonProperty("ThunderheadContentConfiguration")]
        public string ThunderheadContentConfiguration { get; set; }

        [JsonProperty("ThunderheadVmSize")]
        public string ThunderheadVmSize { get; set; }

        [JsonProperty("TrueMatchSettings")]
        public string TrueMatchSettings { get; set; }

        [JsonProperty("HasCsr")]
        public bool HasCsr { get; set; }

        [JsonProperty("PlaylistExperience")]
        public string PlaylistExperience { get; set; }

        [JsonProperty("MatchmakingDelaySec")]
        public int MatchmakingDelaySec { get; set; }
    }
}
