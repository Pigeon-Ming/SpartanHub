using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class SpartanToken
    {
        [JsonProperty("SpartanToken")]
        public string SpartanTokenValue { get; set; }

        [JsonProperty("ExpiresUtc")]
        public ExpiresUtc ExpiresUtc { get; set; }
    }

    public class ExpiresUtc
    {
        [JsonProperty("ISO8601Date")]
        public string ISO8601Date { get; set; }
    }
}
