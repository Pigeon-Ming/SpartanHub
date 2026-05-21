using Newtonsoft.Json;
using System.Collections.Generic;

namespace SpartanHub.Core.Models
{
    public class Settings
    {
        [JsonProperty("Authorities")]
        public Dictionary<string, Authority> Authorities { get; set; }

        [JsonProperty("RetryPolicies")]
        public Dictionary<string, RetryPolicy> RetryPolicies { get; set; }

        [JsonProperty("Settings")]
        public Dictionary<string, string> SettingsValues { get; set; }

        [JsonProperty("Endpoints")]
        public Dictionary<string, Endpoint> Endpoints { get; set; }
    }

    public class Authority
    {
        [JsonProperty("AuthorityId")]
        public string AuthorityId { get; set; }

        [JsonProperty("Scheme")]
        public int Scheme { get; set; }

        [JsonProperty("Hostname")]
        public string Hostname { get; set; }

        [JsonProperty("Port")]
        public int Port { get; set; }

        [JsonProperty("AuthenticationMethods")]
        public int[] AuthenticationMethods { get; set; }
    }

    public class RetryPolicy
    {
        [JsonProperty("RetryPolicyId")]
        public string RetryPolicyId { get; set; }

        [JsonProperty("TimeoutMs")]
        public int TimeoutMs { get; set; }

        [JsonProperty("RetryOptions")]
        public RetryOptions RetryOptions { get; set; }
    }

    public class RetryOptions
    {
        [JsonProperty("MaxRetryCount")]
        public int MaxRetryCount { get; set; }

        [JsonProperty("RetryDelayMs")]
        public int RetryDelayMs { get; set; }

        [JsonProperty("RetryGrowth")]
        public int RetryGrowth { get; set; }

        [JsonProperty("RetryJitterMs")]
        public int RetryJitterMs { get; set; }

        [JsonProperty("RetryIfNotFound")]
        public bool RetryIfNotFound { get; set; }
    }

    public class Endpoint
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
