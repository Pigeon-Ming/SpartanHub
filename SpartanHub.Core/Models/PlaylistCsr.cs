using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class PlaylistCsrContainer
    {
        [JsonProperty("Csr")]
        public PlaylistCsr Csr { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("ResultCode")]
        public int ResultCode { get; set; }
    }

    public class PlaylistCsr
    {
        [JsonProperty("DesignationId")]
        public string DesignationId { get; set; }

        [JsonProperty("Tier")]
        public int Tier { get; set; }

        [JsonProperty("Division")]
        public int Division { get; set; }

        [JsonProperty("Csr")]
        public int CsrValue { get; set; }

        [JsonProperty("PercentToNextTier")]
        public double PercentToNextTier { get; set; }
    }
}
