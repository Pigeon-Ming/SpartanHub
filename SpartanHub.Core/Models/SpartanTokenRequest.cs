using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class SpartanTokenRequest
    {
        [JsonProperty("Audience")]
        public string Audience { get; set; }

        [JsonProperty("MinVersion")]
        public string MinVersion { get; set; }

        [JsonProperty("Proof")]
        public ProofItem[] Proof { get; set; }
    }

    public class ProofItem
    {
        [JsonProperty("Token")]
        public string Token { get; set; }

        [JsonProperty("TokenType")]
        public string TokenType { get; set; }
    }
}
