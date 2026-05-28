using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public enum BanType
    {
        Matchmaking = 1,
        Communication = 2,
        Gameplay = 3
    }

    public enum BanScope
    {
        Global = 1,
        Multiplayer = 2,
        Chat = 3
    }

    public class BanSummary
    {
        [JsonProperty("Results")]
        public BanSummaryResult[] Results { get; set; }

        [JsonProperty("Links")]
        public BanSummaryLinks Links { get; set; }
    }

    public class BanSummaryResult
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("ResultCode")]
        public int ResultCode { get; set; }

        [JsonProperty("Result")]
        public BanSummaryData Result { get; set; }
    }

    public class BanSummaryData
    {
        [JsonProperty("BansInEffect")]
        public BanInEffect[] BansInEffect { get; set; }
    }

    public class BanInEffect
    {
        [JsonProperty("Type")]
        public int Type { get; set; }

        [JsonProperty("Scope")]
        public int Scope { get; set; }

        [JsonProperty("MessageId")]
        public object MessageId { get; set; }

        [JsonProperty("MessageUri")]
        public object MessageUri { get; set; }

        [JsonProperty("EnforceUntilUtc")]
        public ApiFormattedDate EnforceUntilUtc { get; set; }

        [JsonProperty("BanMessagePath")]
        public string BanMessagePath { get; set; }
    }

    public class BanSummaryLinks
    {
        [JsonProperty("Self")]
        public BanSummaryLink Self { get; set; }
    }

    public class BanSummaryLink
    {
        [JsonProperty("AuthorityId")]
        public string AuthorityId { get; set; }

        [JsonProperty("Path")]
        public string Path { get; set; }

        [JsonProperty("QueryString")]
        public string QueryString { get; set; }

        [JsonProperty("RetryPolicyId")]
        public string RetryPolicyId { get; set; }

        [JsonProperty("TopicName")]
        public string TopicName { get; set; }

        [JsonProperty("AcknowledgementTypeId")]
        public int AcknowledgementTypeId { get; set; }

        [JsonProperty("AuthenticationLifetimeExtensionSupported")]
        public bool AuthenticationLifetimeExtensionSupported { get; set; }

        [JsonProperty("ClearanceAware")]
        public bool ClearanceAware { get; set; }
    }
}
