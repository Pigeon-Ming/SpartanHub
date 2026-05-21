using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class SeasonCalendar
    {
        [JsonProperty("Seasons")]
        public SeasonInfo[] Seasons { get; set; }

        [JsonProperty("Events")]
        public EventInfo[] Events { get; set; }

        [JsonProperty("CareerRank")]
        public CareerRank CareerRank { get; set; }
    }

    public class SeasonInfo
    {
        [JsonProperty("CsrSeasonFilePath")]
        public string CsrSeasonFilePath { get; set; }

        [JsonProperty("OperationTrackPath")]
        public string OperationTrackPath { get; set; }

        [JsonProperty("SeasonMetadata")]
        public string SeasonMetadata { get; set; }

        [JsonProperty("StartDate")]
        public ApiFormattedDate StartDate { get; set; }

        [JsonProperty("EndDate")]
        public ApiFormattedDate EndDate { get; set; }
    }

    public class EventInfo
    {
        [JsonProperty("RewardTrackPath")]
        public string RewardTrackPath { get; set; }

        [JsonProperty("StartDate")]
        public ApiFormattedDate StartDate { get; set; }

        [JsonProperty("EndDate")]
        public ApiFormattedDate EndDate { get; set; }
    }

    public class CareerRank
    {
        [JsonProperty("RewardTrackPath")]
        public string RewardTrackPath { get; set; }
    }
}
