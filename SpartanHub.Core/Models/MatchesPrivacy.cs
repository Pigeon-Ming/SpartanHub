using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum Privacy
    {
        Show = 1,
        Hide = 2
    }

    public class MatchesPrivacy
    {
        [JsonProperty("MatchmadeGames")]
        public Privacy MatchmadeGames { get; set; }

        [JsonProperty("OtherGames")]
        public Privacy OtherGames { get; set; }
    }
}
