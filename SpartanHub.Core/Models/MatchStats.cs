using Newtonsoft.Json;

namespace SpartanHub.Core.Models
{
    public class MatchStats
    {
        [JsonProperty("MatchId")]
        public string MatchId { get; set; }

        [JsonProperty("MatchInfo")]
        public MatchInfo MatchInfo { get; set; }

        [JsonProperty("Teams")]
        public TeamStats[] Teams { get; set; }

        [JsonProperty("Players")]
        public PlayerStats[] Players { get; set; }
    }

    public class TeamStats
    {
        [JsonProperty("TeamId")]
        public int TeamId { get; set; }

        [JsonProperty("Outcome")]
        public int Outcome { get; set; }

        [JsonProperty("Rank")]
        public int Rank { get; set; }

        [JsonProperty("Stats")]
        public MatchTeamCoreStats Stats { get; set; }
    }

    public class MatchTeamCoreStats
    {
        [JsonProperty("CoreStats")]
        public MatchCoreStats CoreStats { get; set; }

        [JsonProperty("OddballStats")]
        public OddballStatsData OddballStats { get; set; }

        [JsonProperty("ZonesStats")]
        public ZonesStatsData ZonesStats { get; set; }

        [JsonProperty("CaptureTheFlagStats")]
        public CaptureTheFlagStatsData CaptureTheFlagStats { get; set; }

        [JsonProperty("EliminationStats")]
        public EliminationStatsData EliminationStats { get; set; }

        [JsonProperty("InfectionStats")]
        public InfectionStatsData InfectionStats { get; set; }

        [JsonProperty("StockpileStats")]
        public StockpileStatsData StockpileStats { get; set; }

        [JsonProperty("VIPStats")]
        public VipStatsData VipStats { get; set; }

        [JsonProperty("ExtractionStats")]
        public ExtractionStatsData ExtractionStats { get; set; }
    }

    public class MatchCoreStats
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

        [JsonProperty("KDA")]
        public double KDA { get; set; }

        [JsonProperty("Suicides")]
        public int Suicides { get; set; }

        [JsonProperty("Betrayals")]
        public int Betrayals { get; set; }

        [JsonProperty("AverageLifeDuration")]
        public string AverageLifeDuration { get; set; }

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

        [JsonProperty("ObjectivesCompleted")]
        public int ObjectivesCompleted { get; set; }
    }

    public class PlayerStats
    {
        [JsonProperty("PlayerId")]
        public string PlayerId { get; set; }

        [JsonProperty("PlayerType")]
        public int PlayerType { get; set; }

        [JsonProperty("BotAttributes")]
        public object BotAttributes { get; set; }

        [JsonProperty("LastTeamId")]
        public int LastTeamId { get; set; }

        [JsonProperty("Outcome")]
        public MatchOutcome Outcome { get; set; }

        [JsonProperty("Rank")]
        public int Rank { get; set; }

        [JsonProperty("ParticipationInfo")]
        public ParticipationInfo ParticipationInfo { get; set; }

        [JsonProperty("PlayerTeamStats")]
        public PlayerTeamStat[] PlayerTeamStats { get; set; }
    }

    public class ParticipationInfo
    {
        [JsonProperty("FirstJoinedTime")]
        public string FirstJoinedTime { get; set; }

        [JsonProperty("LastLeaveTime")]
        public string LastLeaveTime { get; set; }

        [JsonProperty("PresentAtBeginning")]
        public bool PresentAtBeginning { get; set; }

        [JsonProperty("JoinedInProgress")]
        public bool JoinedInProgress { get; set; }

        [JsonProperty("LeftInProgress")]
        public bool LeftInProgress { get; set; }

        [JsonProperty("PresentAtCompletion")]
        public bool PresentAtCompletion { get; set; }

        [JsonProperty("TimePlayed")]
        public string TimePlayed { get; set; }

        [JsonProperty("ConfirmedParticipation")]
        public bool? ConfirmedParticipation { get; set; }
    }

    public class PlayerTeamStat
    {
        [JsonProperty("TeamId")]
        public int TeamId { get; set; }

        [JsonProperty("Stats")]
        public MatchPlayerCoreStats Stats { get; set; }
    }

    public class MatchPlayerCoreStats
    {
        [JsonProperty("CoreStats")]
        public MatchCoreStats CoreStats { get; set; }

        [JsonProperty("OddballStats")]
        public OddballStatsData OddballStats { get; set; }

        [JsonProperty("ZonesStats")]
        public ZonesStatsData ZonesStats { get; set; }

        [JsonProperty("CaptureTheFlagStats")]
        public CaptureTheFlagStatsData CaptureTheFlagStats { get; set; }

        [JsonProperty("EliminationStats")]
        public EliminationStatsData EliminationStats { get; set; }

        [JsonProperty("InfectionStats")]
        public InfectionStatsData InfectionStats { get; set; }

        [JsonProperty("StockpileStats")]
        public StockpileStatsData StockpileStats { get; set; }

        [JsonProperty("VIPStats")]
        public VipStatsData VipStats { get; set; }

        [JsonProperty("ExtractionStats")]
        public ExtractionStatsData ExtractionStats { get; set; }
    }

    public class OddballStatsData
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

    public class ZonesStatsData
    {
        [JsonProperty("StrongholdCaptures")]
        public int StrongholdCaptures { get; set; }

        [JsonProperty("StrongholdDefensiveKills")]
        public int StrongholdDefensiveKills { get; set; }

        [JsonProperty("StrongholdOffensiveKills")]
        public int StrongholdOffensiveKills { get; set; }

        [JsonProperty("StrongholdSecures")]
        public int StrongholdSecures { get; set; }

        [JsonProperty("StrongholdOccupationTime")]
        public string StrongholdOccupationTime { get; set; }

        [JsonProperty("StrongholdScoringTicks")]
        public int StrongholdScoringTicks { get; set; }
    }

    public class CaptureTheFlagStatsData
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

    public class EliminationStatsData
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

    public class InfectionStatsData
    {
        [JsonProperty("AlphasKilled")]
        public int AlphasKilled { get; set; }

        [JsonProperty("SpartansInfected")]
        public int SpartansInfected { get; set; }

        [JsonProperty("SpartansInfectedAsAlpha")]
        public int SpartansInfectedAsAlpha { get; set; }

        [JsonProperty("KillsAsLastSpartanStanding")]
        public int KillsAsLastSpartanStanding { get; set; }

        [JsonProperty("LastSpartansStandingInfected")]
        public int LastSpartansStandingInfected { get; set; }

        [JsonProperty("RoundsAsAlpha")]
        public int RoundsAsAlpha { get; set; }

        [JsonProperty("RoundsAsLastSpartanStanding")]
        public int RoundsAsLastSpartanStanding { get; set; }

        [JsonProperty("RoundsFinishedAsInfected")]
        public int RoundsFinishedAsInfected { get; set; }

        [JsonProperty("RoundsSurvivedAsSpartan")]
        public int RoundsSurvivedAsSpartan { get; set; }

        [JsonProperty("RoundsSurvivedAsLastSpartanStanding")]
        public int RoundsSurvivedAsLastSpartanStanding { get; set; }

        [JsonProperty("TimeAsLastSpartanStanding")]
        public string TimeAsLastSpartanStanding { get; set; }

        [JsonProperty("InfectedKilled")]
        public int InfectedKilled { get; set; }
    }

    public class StockpileStatsData
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

    public class VipStatsData
    {
        [JsonProperty("VipKills")]
        public int VipKills { get; set; }

        [JsonProperty("VipAssists")]
        public int VipAssists { get; set; }

        [JsonProperty("KillsAsVip")]
        public int KillsAsVip { get; set; }

        [JsonProperty("TimesSelectedAsVip")]
        public int TimesSelectedAsVip { get; set; }

        [JsonProperty("MaxKillingSpreeAsVip")]
        public int MaxKillingSpreeAsVip { get; set; }

        [JsonProperty("LongestTimeAsVip")]
        public string LongestTimeAsVip { get; set; }

        [JsonProperty("TimeAsVip")]
        public string TimeAsVip { get; set; }
    }

    public class ExtractionStatsData
    {
        [JsonProperty("SuccessfulExtractions")]
        public int SuccessfulExtractions { get; set; }

        [JsonProperty("ExtractionConversionsDenied")]
        public int ExtractionConversionsDenied { get; set; }

        [JsonProperty("ExtractionConversionsCompleted")]
        public int ExtractionConversionsCompleted { get; set; }

        [JsonProperty("ExtractionInitiationsDenied")]
        public int ExtractionInitiationsDenied { get; set; }

        [JsonProperty("ExtractionInitiationsCompleted")]
        public int ExtractionInitiationsCompleted { get; set; }
    }
}
