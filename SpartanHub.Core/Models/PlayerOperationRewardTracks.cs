using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class PlayerOperationRewardTracks
    {
        [JsonProperty("ActiveOperationRewardTrackPath")]
        public string ActiveOperationRewardTrackPath { get; set; }

        [JsonProperty("OperationRewardTracks")]
        public OperationRewardTrack[] OperationRewardTracks { get; set; }
    }

    public class OperationRewardTrack
    {
        [JsonProperty("RewardTrackPath")]
        public string RewardTrackPath { get; set; }

        [JsonProperty("TrackType")]
        public string TrackType { get; set; }

        [JsonProperty("CurrentProgress")]
        public RewardTrackProgress CurrentProgress { get; set; }

        [JsonProperty("PreviousProgress")]
        public RewardTrackProgress PreviousProgress { get; set; }

        [JsonProperty("IsOwned")]
        public bool IsOwned { get; set; }

        [JsonProperty("BaseXp")]
        public int? BaseXp { get; set; }

        [JsonProperty("BoostXp")]
        public int? BoostXp { get; set; }
    }

    public class RewardTrackProgress
    {
        [JsonProperty("Rank")]
        public int Rank { get; set; }

        [JsonProperty("PartialProgress")]
        public int PartialProgress { get; set; }

        [JsonProperty("IsOwned")]
        public bool IsOwned { get; set; }

        [JsonProperty("HasReachedMaxRank")]
        public bool HasReachedMaxRank { get; set; }
    }
}
