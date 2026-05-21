using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class CsrSeasonCalendar
    {
        [JsonProperty("Seasons")]
        public CsrSeasonInfo[] Seasons { get; set; }
    }

    public class CsrSeasonInfo
    {
        [JsonProperty("CsrSeasonFilePath")]
        public string CsrSeasonFilePath { get; set; }

        [JsonProperty("StartDate")]
        public ApiFormattedDate StartDate { get; set; }

        [JsonProperty("EndDate")]
        public ApiFormattedDate EndDate { get; set; }
    }
}
