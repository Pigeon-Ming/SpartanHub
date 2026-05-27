using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class AssetDetail
    {
        [JsonProperty("AssetId")]
        public string AssetId { get; set; }

        [JsonProperty("AssetKind")]
        public int AssetKind { get; set; }

        [JsonProperty("PublicName")]
        public string PublicName { get; set; }

        [JsonProperty("Name")]
        public object Name { get; set; }

        [JsonProperty("Description")]
        public object Description { get; set; }

        [JsonProperty("CurrentVersionId")]
        public string CurrentVersionId { get; set; }

        [JsonProperty("Versions")]
        public AssetVersion[] Versions { get; set; }

        public string GetNameValue(string preferredLang = "zh-CN")
        {
            if (Name is LocalizedString localized)
                return localized.GetValue(preferredLang);
            if (Name is string str)
                return str;
            return PublicName ?? AssetId;
        }
    }

    public class LocalizedString
    {
        [JsonProperty("en-US")]
        public string EnUs { get; set; }

        [JsonProperty("zh-CN")]
        public string ZhCn { get; set; }

        public string GetValue(string preferredLang = "zh-CN")
        {
            if (preferredLang == "zh-CN" && !string.IsNullOrEmpty(ZhCn))
                return ZhCn;
            if (preferredLang == "zh-CN")
                return EnUs ?? string.Empty;
            return EnUs ?? string.Empty;
        }
    }

    public class AssetVersion
    {
        [JsonProperty("VersionId")]
        public string VersionId { get; set; }

        [JsonProperty("PublishedDate")]
        public string PublishedDate { get; set; }

        [JsonProperty("Name")]
        public LocalizedString Name { get; set; }
    }
}
