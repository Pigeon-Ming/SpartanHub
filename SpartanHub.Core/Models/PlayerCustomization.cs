using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class PlayerCustomization
    {
        [JsonProperty("ArmorCores")]
        public ArmorCoresContainer ArmorCores { get; set; }

        [JsonProperty("SpartanBody")]
        public SpartanBodyCustomization SpartanBody { get; set; }

        [JsonProperty("Appearance")]
        public SpartanAppearance Appearance { get; set; }

        [JsonProperty("WeaponCores")]
        public WeaponCoresContainer WeaponCores { get; set; }

        [JsonProperty("AiCores")]
        public AiCoresContainer AiCores { get; set; }

        [JsonProperty("VehicleCores")]
        public VehicleCoresContainer VehicleCores { get; set; }
    }

    public class ArmorCoresContainer
    {
        [JsonProperty("ArmorCores")]
        public ArmorCoreCustomization[] ArmorCores { get; set; }
    }

    public class WeaponCoresContainer
    {
        [JsonProperty("WeaponCores")]
        public WeaponCoreCustomization[] WeaponCores { get; set; }
    }

    public class AiCoresContainer
    {
        [JsonProperty("AiCores")]
        public AiCoreCustomization[] AiCores { get; set; }
    }

    public class VehicleCoresContainer
    {
        [JsonProperty("VehicleCores")]
        public VehicleCoreCustomization[] VehicleCores { get; set; }
    }

    public class ArmorCoreCustomization
    {
        [JsonProperty("CorePath")]
        public string CorePath { get; set; }

        [JsonProperty("IsEquipped")]
        public bool? IsEquipped { get; set; }

        [JsonProperty("Themes")]
        public ArmorThemeCustomization[] Themes { get; set; }

        [JsonProperty("CoreId")]
        public string CoreId { get; set; }

        [JsonProperty("CoreType")]
        public string CoreType { get; set; }
    }

    public class WeaponCoreCustomization
    {
        [JsonProperty("CorePath")]
        public string CorePath { get; set; }

        [JsonProperty("IsEquipped")]
        public bool? IsEquipped { get; set; }

        [JsonProperty("Themes")]
        public WeaponThemeCustomization[] Themes { get; set; }

        [JsonProperty("CoreId")]
        public string CoreId { get; set; }

        [JsonProperty("CoreType")]
        public string CoreType { get; set; }
    }

    public class AiCoreCustomization
    {
        [JsonProperty("CorePath")]
        public string CorePath { get; set; }

        [JsonProperty("IsEquipped")]
        public bool? IsEquipped { get; set; }

        [JsonProperty("Themes")]
        public AiThemeCustomization[] Themes { get; set; }

        [JsonProperty("CoreId")]
        public string CoreId { get; set; }

        [JsonProperty("CoreType")]
        public string CoreType { get; set; }
    }

    public class VehicleCoreCustomization
    {
        [JsonProperty("CoreId")]
        public string CoreId { get; set; }

        [JsonProperty("CorePath")]
        public string CorePath { get; set; }

        [JsonProperty("IsEquipped")]
        public bool? IsEquipped { get; set; }

        [JsonProperty("Themes")]
        public VehicleThemeCustomization[] Themes { get; set; }

        [JsonProperty("CoreType")]
        public string CoreType { get; set; }
    }

    public class ArmorThemeCustomization
    {
        [JsonProperty("FirstModifiedDateUtc")]
        public ApiFormattedDate FirstModifiedDateUtc { get; set; }

        [JsonProperty("LastModifiedDateUtc")]
        public ApiFormattedDate LastModifiedDateUtc { get; set; }

        [JsonProperty("IsEquipped")]
        public bool IsEquipped { get; set; }

        [JsonProperty("IsDefault")]
        public bool IsDefault { get; set; }

        [JsonProperty("ThemePath")]
        public string ThemePath { get; set; }

        [JsonProperty("CoatingPath")]
        public string CoatingPath { get; set; }

        [JsonProperty("GlovePath")]
        public string GlovePath { get; set; }

        [JsonProperty("HelmetPath")]
        public string HelmetPath { get; set; }

        [JsonProperty("HelmetAttachmentPath")]
        public string HelmetAttachmentPath { get; set; }

        [JsonProperty("ChestAttachmentPath")]
        public string ChestAttachmentPath { get; set; }

        [JsonProperty("KneePadPath")]
        public string KneePadPath { get; set; }

        [JsonProperty("LeftShoulderPadPath")]
        public string LeftShoulderPadPath { get; set; }

        [JsonProperty("RightShoulderPadPath")]
        public string RightShoulderPadPath { get; set; }

        [JsonProperty("Emblems")]
        public CustomizationEmblem[] Emblems { get; set; }

        [JsonProperty("ArmorFxPath")]
        public string ArmorFxPath { get; set; }

        [JsonProperty("MythicFxPath")]
        public string MythicFxPath { get; set; }

        [JsonProperty("VisorPath")]
        public string VisorPath { get; set; }

        [JsonProperty("HipAttachmentPath")]
        public string HipAttachmentPath { get; set; }

        [JsonProperty("WristAttachmentPath")]
        public string WristAttachmentPath { get; set; }

        [JsonProperty("ArmorFxPaths")]
        public string[] ArmorFxPaths { get; set; }
    }

    public class WeaponThemeCustomization
    {
        [JsonProperty("FirstModifiedDateUtc")]
        public ApiFormattedDate FirstModifiedDateUtc { get; set; }

        [JsonProperty("LastModifiedDateUtc")]
        public ApiFormattedDate LastModifiedDateUtc { get; set; }

        [JsonProperty("IsEquipped")]
        public bool IsEquipped { get; set; }

        [JsonProperty("IsDefault")]
        public bool IsDefault { get; set; }

        [JsonProperty("ThemePath")]
        public string ThemePath { get; set; }

        [JsonProperty("CoatingPath")]
        public string CoatingPath { get; set; }

        [JsonProperty("Emblems")]
        public CustomizationEmblem[] Emblems { get; set; }

        [JsonProperty("DeathFxPath")]
        public string DeathFxPath { get; set; }

        [JsonProperty("AmmoCounterColorPath")]
        public string AmmoCounterColorPath { get; set; }

        [JsonProperty("StatTrackerPath")]
        public string StatTrackerPath { get; set; }

        [JsonProperty("WeaponCharmPath")]
        public string WeaponCharmPath { get; set; }

        [JsonProperty("AlternateGeometryRegionPath")]
        public string AlternateGeometryRegionPath { get; set; }
    }

    public class AiThemeCustomization
    {
        [JsonProperty("FirstModifiedDateUtc")]
        public ApiFormattedDate FirstModifiedDateUtc { get; set; }

        [JsonProperty("LastModifiedDateUtc")]
        public ApiFormattedDate LastModifiedDateUtc { get; set; }

        [JsonProperty("IsEquipped")]
        public bool IsEquipped { get; set; }

        [JsonProperty("IsDefault")]
        public bool IsDefault { get; set; }

        [JsonProperty("ThemePath")]
        public string ThemePath { get; set; }

        [JsonProperty("ModelPath")]
        public string ModelPath { get; set; }

        [JsonProperty("ColorPath")]
        public string ColorPath { get; set; }
    }

    public class VehicleThemeCustomization
    {
        [JsonProperty("FirstModifiedDateUtc")]
        public ApiFormattedDate FirstModifiedDateUtc { get; set; }

        [JsonProperty("LastModifiedDateUtc")]
        public ApiFormattedDate LastModifiedDateUtc { get; set; }

        [JsonProperty("IsEquipped")]
        public bool IsEquipped { get; set; }

        [JsonProperty("IsDefault")]
        public bool IsDefault { get; set; }

        [JsonProperty("ThemePath")]
        public string ThemePath { get; set; }

        [JsonProperty("CoatingPath")]
        public string CoatingPath { get; set; }

        [JsonProperty("HornPath")]
        public string HornPath { get; set; }

        [JsonProperty("VehicleFxPath")]
        public string VehicleFxPath { get; set; }

        [JsonProperty("VehicleCharmPath")]
        public string VehicleCharmPath { get; set; }

        [JsonProperty("Emblems")]
        public CustomizationEmblem[] Emblems { get; set; }

        [JsonProperty("AlternateGeometryRegionPath")]
        public string AlternateGeometryRegionPath { get; set; }
    }

    public class SpartanBodyCustomization
    {
        [JsonProperty("LastModifiedDateUtc")]
        public ApiFormattedDate LastModifiedDateUtc { get; set; }

        [JsonProperty("LeftArm")]
        public string LeftArm { get; set; }

        [JsonProperty("RightArm")]
        public string RightArm { get; set; }

        [JsonProperty("LeftLeg")]
        public string LeftLeg { get; set; }

        [JsonProperty("RightLeg")]
        public string RightLeg { get; set; }

        [JsonProperty("BodyType")]
        public string BodyType { get; set; }

        [JsonProperty("VoicePath")]
        public string VoicePath { get; set; }
    }

    public class SpartanAppearance
    {
        [JsonProperty("LastModifiedDateUtc")]
        public ApiFormattedDate LastModifiedDateUtc { get; set; }

        [JsonProperty("ActionPosePath")]
        public string ActionPosePath { get; set; }

        [JsonProperty("BackdropImagePath")]
        public string BackdropImagePath { get; set; }

        [JsonProperty("Emblem")]
        public CustomizationEmblem Emblem { get; set; }

        [JsonProperty("ServiceTag")]
        public string ServiceTag { get; set; }

        [JsonProperty("IntroEmotePath")]
        public string IntroEmotePath { get; set; }

        [JsonProperty("PlayerTitlePath")]
        public string PlayerTitlePath { get; set; }
    }

    public class CustomizationEmblem
    {
        [JsonProperty("EmblemPath")]
        public string EmblemPath { get; set; }

        [JsonProperty("Path")]
        public string Path { get; set; }

        [JsonProperty("LocationId")]
        public int? LocationId { get; set; }

        [JsonProperty("ConfigurationId")]
        public long ConfigurationId { get; set; }
    }

    public enum PlayerCustomizationView
    {
        Public
    }
}
