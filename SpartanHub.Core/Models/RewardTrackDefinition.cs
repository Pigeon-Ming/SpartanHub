using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class RewardTrackDefinition
    {
        [JsonProperty("TrackId")]
        public string TrackId { get; set; }

        [JsonProperty("XpPerRank")]
        public int XpPerRank { get; set; }

        [JsonProperty("HideIfNotOwned")]
        public bool HideIfNotOwned { get; set; }

        [JsonProperty("Ranks")]
        public RewardTrackRank[] Ranks { get; set; }

        [JsonProperty("Name")]
        public LocalizedText Name { get; set; }

        [JsonProperty("Description")]
        public LocalizedText Description { get; set; }

        [JsonProperty("OperationNumber")]
        public int? OperationNumber { get; set; }

        [JsonProperty("DateRange")]
        public LocalizedText DateRange { get; set; }

        [JsonProperty("IsRitual")]
        public bool IsRitual { get; set; }

        [JsonProperty("SummaryImagePath")]
        public string SummaryImagePath { get; set; }

        [JsonProperty("WeekNumber")]
        public int? WeekNumber { get; set; }

        [JsonProperty("BackgroundImagePath")]
        public string BackgroundImagePath { get; set; }
    }

    public class RewardTrackRank
    {
        [JsonProperty("Rank")]
        public int Rank { get; set; }

        [JsonProperty("FreeRewards")]
        public RewardTrackRewards FreeRewards { get; set; }

        [JsonProperty("PaidRewards")]
        public RewardTrackRewards PaidRewards { get; set; }
    }

    public class RewardTrackRewards
    {
        [JsonProperty("InventoryRewards")]
        public RewardTrackInventoryReward[] InventoryRewards { get; set; }

        [JsonProperty("CurrencyRewards")]
        public RewardTrackCurrencyReward[] CurrencyRewards { get; set; }
    }

    public class RewardTrackInventoryReward
    {
        [JsonProperty("InventoryItemPath")]
        public string InventoryItemPath { get; set; }

        [JsonProperty("Amount")]
        public int Amount { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }
    }

    public class RewardTrackCurrencyReward
    {
        [JsonProperty("CurrencyPath")]
        public string CurrencyPath { get; set; }

        [JsonProperty("Amount")]
        public int Amount { get; set; }
    }
}
