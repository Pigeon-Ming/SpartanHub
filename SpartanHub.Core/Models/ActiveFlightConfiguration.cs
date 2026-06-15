using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class ActiveFlightConfiguration
    {
        [JsonProperty("FlightConfigurationId")]
        public string FlightConfigurationId { get; set; }
    }
}
