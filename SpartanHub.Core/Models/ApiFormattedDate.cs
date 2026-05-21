using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class ApiFormattedDate
    {
        [JsonProperty("ISO8601Date")]
        public string ISO8601Date { get; set; }
    }
}
