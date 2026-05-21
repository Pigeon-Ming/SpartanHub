using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class UserInfo
    {
        [JsonProperty("xuid")]
        public string Xuid { get; set; }

        [JsonProperty("gamertag")]
        public string Gamertag { get; set; }

        [JsonProperty("gamerpic")]
        public GamerPic Gamerpic { get; set; }
    }

    public class GamerPic
    {
        [JsonProperty("small")]
        public string Small { get; set; }

        [JsonProperty("medium")]
        public string Medium { get; set; }

        [JsonProperty("large")]
        public string Large { get; set; }

        [JsonProperty("xlarge")]
        public string Xlarge { get; set; }
    }
}
