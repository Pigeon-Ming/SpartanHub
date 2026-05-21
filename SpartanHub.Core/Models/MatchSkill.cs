using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class MatchSkill
    {
        [JsonProperty("TeamId")]
        public int TeamId { get; set; }

        [JsonProperty("TeamMmr")]
        public double TeamMmr { get; set; }

        [JsonProperty("TeamMmrs")]
        public object TeamMmrs { get; set; }

        [JsonProperty("RankRecap")]
        public RankRecap RankRecap { get; set; }
    }

    public class RankRecap
    {
        [JsonProperty("PreMatchCsr")]
        public PlaylistCsr PreMatchCsr { get; set; }

        [JsonProperty("PostMatchCsr")]
        public PlaylistCsr PostMatchCsr { get; set; }
    }
}
