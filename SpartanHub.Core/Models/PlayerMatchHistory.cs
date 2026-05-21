using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class PlayerMatchHistory
    {
        [JsonProperty("MatchId")]
        public string MatchId { get; set; }

        [JsonProperty("LastTeamId")]
        public int LastTeamId { get; set; }

        [JsonProperty("Outcome")]
        public MatchOutcome Outcome { get; set; }

        [JsonProperty("Rank")]
        public int Rank { get; set; }

        [JsonProperty("PresentAtEndOfMatch")]
        public bool PresentAtEndOfMatch { get; set; }

        [JsonProperty("MatchInfo")]
        public MatchInfo MatchInfo { get; set; }
    }
}
