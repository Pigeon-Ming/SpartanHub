using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class BanMessage
    {
        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Body")]
        public string Body { get; set; }
    }
}
