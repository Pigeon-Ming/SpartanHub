using Newtonsoft.Json;
using System.Collections.Generic;

namespace SpartanHub.Core.Models
{
    public class MedalsMetadataFile
    {
        [JsonProperty("difficulties")]
        public string[] Difficulties { get; set; }

        [JsonProperty("types")]
        public string[] Types { get; set; }

        [JsonProperty("sprites")]
        public Dictionary<string, MedalSprite> Sprites { get; set; }

        [JsonProperty("medals")]
        public MedalMetadata[] Medals { get; set; }
    }

    public class MedalSprite
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("columns")]
        public int Columns { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }
    }

    public class MedalMetadata
    {
        [JsonProperty("name")]
        public LocalizedText Name { get; set; }

        [JsonProperty("description")]
        public LocalizedText Description { get; set; }

        [JsonProperty("spriteIndex")]
        public int SpriteIndex { get; set; }

        [JsonProperty("sortingWeight")]
        public int SortingWeight { get; set; }

        [JsonProperty("difficultyIndex")]
        public int DifficultyIndex { get; set; }

        [JsonProperty("typeIndex")]
        public int TypeIndex { get; set; }

        [JsonProperty("personalScore")]
        public int PersonalScore { get; set; }

        [JsonProperty("nameId")]
        public long NameId { get; set; }
    }

    public class LocalizedText
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("translations")]
        public Dictionary<string, string> Translations { get; set; }
    }
}
