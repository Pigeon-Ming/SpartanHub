using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class ServiceRecord
    {
        [JsonProperty("Subqueries")]
        public Subqueries Subqueries { get; set; }

        [JsonProperty("TimePlayed")]
        public string TimePlayed { get; set; }

        [JsonProperty("MatchesCompleted")]
        public int MatchesCompleted { get; set; }

        [JsonProperty("Wins")]
        public int Wins { get; set; }

        [JsonProperty("Losses")]
        public int Losses { get; set; }

        [JsonProperty("Ties")]
        public int Ties { get; set; }

        [JsonProperty("CoreStats")]
        public CoreStats CoreStats { get; set; }

        [JsonProperty("BombStats")]
        public object BombStats { get; set; }

        [JsonProperty("CaptureTheFlagStats")]
        public CaptureTheFlagStats CaptureTheFlagStats { get; set; }

        [JsonProperty("EliminationStats")]
        public EliminationStats EliminationStats { get; set; }

        [JsonProperty("ExtractionStats")]
        public object ExtractionStats { get; set; }

        [JsonProperty("InfectionStats")]
        public object InfectionStats { get; set; }

        [JsonProperty("OddballStats")]
        public OddballStats OddballStats { get; set; }

        [JsonProperty("ZonesStats")]
        public ZonesStats ZonesStats { get; set; }

        [JsonProperty("StockpileStats")]
        public StockpileStats StockpileStats { get; set; }
    }

    public class Subqueries
    {
        [JsonProperty("SeasonIds")]
        public string[] SeasonIds { get; set; }

        [JsonProperty("GameVariantCategories")]
        public int[] GameVariantCategories { get; set; }

        [JsonProperty("IsRanked")]
        public bool[] IsRanked { get; set; }

        [JsonProperty("PlaylistAssetIds")]
        public string[] PlaylistAssetIds { get; set; }
    }

    public class CoreStats
    {
        [JsonProperty("Score")]
        public int Score { get; set; }

        [JsonProperty("PersonalScore")]
        public int PersonalScore { get; set; }

        [JsonProperty("RoundsWon")]
        public int RoundsWon { get; set; }

        [JsonProperty("RoundsLost")]
        public int RoundsLost { get; set; }

        [JsonProperty("RoundsTied")]
        public int RoundsTied { get; set; }

        [JsonProperty("Kills")]
        public int Kills { get; set; }

        [JsonProperty("Deaths")]
        public int Deaths { get; set; }

        [JsonProperty("Assists")]
        public int Assists { get; set; }

        [JsonProperty("AverageKDA")]
        public double AverageKDA { get; set; }

        [JsonProperty("Suicides")]
        public int Suicides { get; set; }

        [JsonProperty("Betrayals")]
        public int Betrayals { get; set; }

        [JsonProperty("GrenadeKills")]
        public int GrenadeKills { get; set; }

        [JsonProperty("HeadshotKills")]
        public int HeadshotKills { get; set; }

        [JsonProperty("MeleeKills")]
        public int MeleeKills { get; set; }

        [JsonProperty("PowerWeaponKills")]
        public int PowerWeaponKills { get; set; }

        [JsonProperty("ShotsFired")]
        public int ShotsFired { get; set; }

        [JsonProperty("ShotsHit")]
        public int ShotsHit { get; set; }

        [JsonProperty("Accuracy")]
        public double Accuracy { get; set; }

        [JsonProperty("DamageDealt")]
        public double DamageDealt { get; set; }

        [JsonProperty("DamageTaken")]
        public double DamageTaken { get; set; }

        [JsonProperty("CalloutAssists")]
        public int CalloutAssists { get; set; }

        [JsonProperty("VehicleDestroys")]
        public int VehicleDestroys { get; set; }

        [JsonProperty("DriverAssists")]
        public int DriverAssists { get; set; }

        [JsonProperty("Hijacks")]
        public int Hijacks { get; set; }

        [JsonProperty("EmpAssists")]
        public int EmpAssists { get; set; }

        [JsonProperty("MaxKillingSpree")]
        public int MaxKillingSpree { get; set; }

        [JsonProperty("Medals")]
        public Medal[] Medals { get; set; }

        [JsonProperty("PersonalScores")]
        public PersonalScore[] PersonalScores { get; set; }

        [JsonProperty("Spawns")]
        public int Spawns { get; set; }
    }

    public class Medal
    {
        [JsonProperty("NameId")]
        public long NameId { get; set; }

        [JsonProperty("Count")]
        public int Count { get; set; }

        [JsonProperty("TotalPersonalScoreAwarded")]
        public int TotalPersonalScoreAwarded { get; set; }
    }

    public class PersonalScore
    {
        [JsonProperty("NameId")]
        public long NameId { get; set; }

        [JsonProperty("Count")]
        public int Count { get; set; }

        [JsonProperty("TotalPersonalScoreAwarded")]
        public int TotalPersonalScoreAwarded { get; set; }
    }

    public class CaptureTheFlagStats
    {
        [JsonProperty("FlagCaptureAssists")]
        public int FlagCaptureAssists { get; set; }

        [JsonProperty("FlagCaptures")]
        public int FlagCaptures { get; set; }

        [JsonProperty("FlagCarriersKilled")]
        public int FlagCarriersKilled { get; set; }

        [JsonProperty("FlagGrabs")]
        public int FlagGrabs { get; set; }

        [JsonProperty("FlagReturnersKilled")]
        public int FlagReturnersKilled { get; set; }

        [JsonProperty("FlagReturns")]
        public int FlagReturns { get; set; }

        [JsonProperty("FlagSecures")]
        public int FlagSecures { get; set; }

        [JsonProperty("FlagSteals")]
        public int FlagSteals { get; set; }

        [JsonProperty("KillsAsFlagCarrier")]
        public int KillsAsFlagCarrier { get; set; }

        [JsonProperty("KillsAsFlagReturner")]
        public int KillsAsFlagReturner { get; set; }

        [JsonProperty("TimeAsFlagCarrier")]
        public string TimeAsFlagCarrier { get; set; }
    }

    public class EliminationStats
    {
        [JsonProperty("AlliesRevived")]
        public int AlliesRevived { get; set; }

        [JsonProperty("EliminationAssists")]
        public int EliminationAssists { get; set; }

        [JsonProperty("Eliminations")]
        public int Eliminations { get; set; }

        [JsonProperty("EnemyRevivesDenied")]
        public int EnemyRevivesDenied { get; set; }

        [JsonProperty("Executions")]
        public int Executions { get; set; }

        [JsonProperty("KillsAsLastPlayerStanding")]
        public int KillsAsLastPlayerStanding { get; set; }

        [JsonProperty("LastPlayersStandingKilled")]
        public int LastPlayersStandingKilled { get; set; }

        [JsonProperty("RoundsSurvived")]
        public int RoundsSurvived { get; set; }

        [JsonProperty("TimesRevivedByAlly")]
        public int TimesRevivedByAlly { get; set; }
    }

    public class OddballStats
    {
        [JsonProperty("KillsAsSkullCarrier")]
        public int KillsAsSkullCarrier { get; set; }

        [JsonProperty("LongestTimeAsSkullCarrier")]
        public string LongestTimeAsSkullCarrier { get; set; }

        [JsonProperty("SkullCarriersKilled")]
        public int SkullCarriersKilled { get; set; }

        [JsonProperty("SkullGrabs")]
        public int SkullGrabs { get; set; }

        [JsonProperty("TimeAsSkullCarrier")]
        public string TimeAsSkullCarrier { get; set; }

        [JsonProperty("SkullScoringTicks")]
        public int SkullScoringTicks { get; set; }
    }

    public class ZonesStats
    {
        [JsonProperty("ZoneCaptures")]
        public int ZoneCaptures { get; set; }

        [JsonProperty("ZoneDefensiveKills")]
        public int ZoneDefensiveKills { get; set; }

        [JsonProperty("ZoneOffensiveKills")]
        public int ZoneOffensiveKills { get; set; }

        [JsonProperty("ZoneSecures")]
        public int ZoneSecures { get; set; }

        [JsonProperty("TotalZoneOccupationTime")]
        public string TotalZoneOccupationTime { get; set; }

        [JsonProperty("ZoneScoringTicks")]
        public int ZoneScoringTicks { get; set; }
    }

    public class StockpileStats
    {
        [JsonProperty("KillsAsPowerSeedCarrier")]
        public int KillsAsPowerSeedCarrier { get; set; }

        [JsonProperty("PowerSeedCarriersKilled")]
        public int PowerSeedCarriersKilled { get; set; }

        [JsonProperty("PowerSeedsDeposited")]
        public int PowerSeedsDeposited { get; set; }

        [JsonProperty("PowerSeedsStolen")]
        public int PowerSeedsStolen { get; set; }

        [JsonProperty("TimeAsPowerSeedCarrier")]
        public string TimeAsPowerSeedCarrier { get; set; }

        [JsonProperty("TimeAsPowerSeedDriver")]
        public string TimeAsPowerSeedDriver { get; set; }
    }
}
