using SpartanHub.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace SpartanHub.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ControlTestPage : Page
    {
        MatchStats matchStats = new MatchStats();
        public ControlTestPage()
        {
            this.InitializeComponent();

            matchStats = CreateMockMatchStats();
            Bindings.Update();
        }

        private MatchStats CreateMockMatchStats()
        {
            return new MatchStats
            {
                MatchId = "mock-match-12345",
                MatchInfo = new MatchInfo
                {
                    MapId = "map_downtown",
                    MapVersionId = "v1.0",
                    MapModePairId = "mode_pair_001",
                    MapModePairVersionId = "v1.0",
                    UgcGameVariantId = "variant_123",
                    UgcGameVariantVersionId = "v1.0",
                    PlaylistId = "playlist_ranked",
                    PlaylistVersionId = "v1.0",
                    MatchmakingId = "mm_001",
                    Category = 1,
                    MatchCompletedDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                },
                Teams = new TeamStats[]
                {
                    new TeamStats
                    {
                        TeamId = 0,
                        Outcome = 1,
                        Rank = 1,
                        Stats = new MatchTeamCoreStats
                        {
                            CoreStats = new MatchCoreStats
                            {
                                Score = 1000,
                                PersonalScore = 500,
                                RoundsWon = 3,
                                RoundsLost = 1,
                                RoundsTied = 0,
                                Kills = 45,
                                Deaths = 30,
                                Assists = 20,
                                KDA = 2.17,
                                Suicides = 2,
                                Betrayals = 0,
                                AverageLifeDuration = "00:01:30",
                                GrenadeKills = 5,
                                HeadshotKills = 12,
                                MeleeKills = 3,
                                PowerWeaponKills = 8,
                                ShotsFired = 500,
                                ShotsHit = 250,
                                Accuracy = 50.0,
                                DamageDealt = 15000.5,
                                DamageTaken = 12000.3,
                                CalloutAssists = 10,
                                VehicleDestroys = 2,
                                DriverAssists = 5,
                                Hijacks = 1,
                                EmpAssists = 3,
                                MaxKillingSpree = 8,
                                Medals = new Medal[] { new Medal { NameId = 1, Count = 3, TotalPersonalScoreAwarded = 150 } },
                                PersonalScores = new PersonalScore[] { new PersonalScore { NameId = 1001, Count = 5, TotalPersonalScoreAwarded = 200 } },
                                Spawns = 15,
                                ObjectivesCompleted = 5
                            },
                            OddballStats = new OddballStatsData
                            {
                                KillsAsSkullCarrier = 5,
                                LongestTimeAsSkullCarrier = "00:00:45",
                                SkullCarriersKilled = 3,
                                SkullGrabs = 8,
                                TimeAsSkullCarrier = "00:02:30",
                                SkullScoringTicks = 15
                            },
                            ZonesStats = new ZonesStatsData
                            {
                                StrongholdCaptures = 4,
                                StrongholdDefensiveKills = 6,
                                StrongholdOffensiveKills = 8,
                                StrongholdSecures = 3,
                                StrongholdOccupationTime = "00:03:00",
                                StrongholdScoringTicks = 20
                            },
                            CaptureTheFlagStats = new CaptureTheFlagStatsData
                            {
                                FlagCaptureAssists = 2,
                                FlagCaptures = 3,
                                FlagCarriersKilled = 4,
                                FlagGrabs = 5,
                                FlagReturnersKilled = 2,
                                FlagReturns = 6,
                                FlagSecures = 3,
                                FlagSteals = 2,
                                KillsAsFlagCarrier = 5,
                                KillsAsFlagReturner = 3,
                                TimeAsFlagCarrier = "00:01:50"
                            },
                            EliminationStats = new EliminationStatsData
                            {
                                AlliesRevived = 5,
                                EliminationAssists = 8,
                                Eliminations = 12,
                                EnemyRevivesDenied = 3,
                                Executions = 2,
                                KillsAsLastPlayerStanding = 3,
                                LastPlayersStandingKilled = 2,
                                RoundsSurvived = 8,
                                TimesRevivedByAlly = 4
                            },
                            InfectionStats = new InfectionStatsData
                            {
                                AlphasKilled = 3,
                                SpartansInfected = 5,
                                SpartansInfectedAsAlpha = 8,
                                KillsAsLastSpartanStanding = 4,
                                LastSpartansStandingInfected = 2,
                                RoundsAsAlpha = 3,
                                RoundsAsLastSpartanStanding = 5,
                                RoundsFinishedAsInfected = 2,
                                RoundsSurvivedAsSpartan = 10,
                                RoundsSurvivedAsLastSpartanStanding = 3,
                                TimeAsLastSpartanStanding = "00:02:00",
                                InfectedKilled = 15
                            },
                            StockpileStats = new StockpileStatsData
                            {
                                KillsAsPowerSeedCarrier = 4,
                                PowerSeedCarriersKilled = 3,
                                PowerSeedsDeposited = 6,
                                PowerSeedsStolen = 2,
                                TimeAsPowerSeedCarrier = "00:01:30",
                                TimeAsPowerSeedDriver = "00:00:50"
                            },
                            VipStats = new VipStatsData
                            {
                                VipKills = 5,
                                VipAssists = 3,
                                KillsAsVip = 8,
                                TimesSelectedAsVip = 2,
                                MaxKillingSpreeAsVip = 6,
                                LongestTimeAsVip = "00:01:45",
                                TimeAsVip = "00:03:20"
                            },
                            ExtractionStats = new ExtractionStatsData
                            {
                                SuccessfulExtractions = 4,
                                ExtractionConversionsDenied = 2,
                                ExtractionConversionsCompleted = 3,
                                ExtractionInitiationsDenied = 1,
                                ExtractionInitiationsCompleted = 5
                            }
                        }
                    },
                    new TeamStats
                    {
                        TeamId = 1,
                        Outcome = 2,
                        Rank = 2,
                        Stats = new MatchTeamCoreStats
                        {
                            CoreStats = new MatchCoreStats
                            {
                                Score = 800,
                                PersonalScore = 400,
                                RoundsWon = 1,
                                RoundsLost = 3,
                                RoundsTied = 0,
                                Kills = 30,
                                Deaths = 45,
                                Assists = 15,
                                KDA = 1.0,
                                Suicides = 3,
                                Betrayals = 1,
                                AverageLifeDuration = "00:01:15",
                                GrenadeKills = 3,
                                HeadshotKills = 8,
                                MeleeKills = 2,
                                PowerWeaponKills = 5,
                                ShotsFired = 450,
                                ShotsHit = 180,
                                Accuracy = 40.0,
                                DamageDealt = 12000.5,
                                DamageTaken = 15000.3,
                                CalloutAssists = 7,
                                VehicleDestroys = 1,
                                DriverAssists = 3,
                                Hijacks = 0,
                                EmpAssists = 2,
                                MaxKillingSpree = 5,
                                Medals = new Medal[] { new Medal { NameId = 1, Count = 2, TotalPersonalScoreAwarded = 100 } },
                                PersonalScores = new PersonalScore[] { new PersonalScore { NameId = 1001, Count = 3, TotalPersonalScoreAwarded = 150 } },
                                Spawns = 18,
                                ObjectivesCompleted = 3
                            },
                            OddballStats = new OddballStatsData
                            {
                                KillsAsSkullCarrier = 3,
                                LongestTimeAsSkullCarrier = "00:00:30",
                                SkullCarriersKilled = 5,
                                SkullGrabs = 5,
                                TimeAsSkullCarrier = "00:01:30",
                                SkullScoringTicks = 10
                            },
                            ZonesStats = new ZonesStatsData
                            {
                                StrongholdCaptures = 2,
                                StrongholdDefensiveKills = 4,
                                StrongholdOffensiveKills = 5,
                                StrongholdSecures = 2,
                                StrongholdOccupationTime = "00:02:00",
                                StrongholdScoringTicks = 12
                            },
                            CaptureTheFlagStats = new CaptureTheFlagStatsData
                            {
                                FlagCaptureAssists = 1,
                                FlagCaptures = 2,
                                FlagCarriersKilled = 3,
                                FlagGrabs = 4,
                                FlagReturnersKilled = 1,
                                FlagReturns = 4,
                                FlagSecures = 2,
                                FlagSteals = 1,
                                KillsAsFlagCarrier = 3,
                                KillsAsFlagReturner = 2,
                                TimeAsFlagCarrier = "00:01:20"
                            },
                            EliminationStats = new EliminationStatsData
                            {
                                AlliesRevived = 3,
                                EliminationAssists = 5,
                                Eliminations = 8,
                                EnemyRevivesDenied = 2,
                                Executions = 1,
                                KillsAsLastPlayerStanding = 2,
                                LastPlayersStandingKilled = 3,
                                RoundsSurvived = 5,
                                TimesRevivedByAlly = 6
                            },
                            InfectionStats = new InfectionStatsData
                            {
                                AlphasKilled = 2,
                                SpartansInfected = 3,
                                SpartansInfectedAsAlpha = 5,
                                KillsAsLastSpartanStanding = 2,
                                LastSpartansStandingInfected = 3,
                                RoundsAsAlpha = 2,
                                RoundsAsLastSpartanStanding = 3,
                                RoundsFinishedAsInfected = 3,
                                RoundsSurvivedAsSpartan = 7,
                                RoundsSurvivedAsLastSpartanStanding = 2,
                                TimeAsLastSpartanStanding = "00:01:30",
                                InfectedKilled = 10
                            },
                            StockpileStats = new StockpileStatsData
                            {
                                KillsAsPowerSeedCarrier = 2,
                                PowerSeedCarriersKilled = 4,
                                PowerSeedsDeposited = 4,
                                PowerSeedsStolen = 3,
                                TimeAsPowerSeedCarrier = "00:01:00",
                                TimeAsPowerSeedDriver = "00:00:40"
                            },
                            VipStats = new VipStatsData
                            {
                                VipKills = 3,
                                VipAssists = 2,
                                KillsAsVip = 5,
                                TimesSelectedAsVip = 1,
                                MaxKillingSpreeAsVip = 4,
                                LongestTimeAsVip = "00:01:20",
                                TimeAsVip = "00:02:30"
                            },
                            ExtractionStats = new ExtractionStatsData
                            {
                                SuccessfulExtractions = 2,
                                ExtractionConversionsDenied = 3,
                                ExtractionConversionsCompleted = 2,
                                ExtractionInitiationsDenied = 2,
                                ExtractionInitiationsCompleted = 3
                            }
                        }
                    }
                },
                Players = new PlayerStats[]
                {
                    new PlayerStats
                    {
                        PlayerId = "player_001",
                        PlayerType = 1,
                        BotAttributes = null,
                        LastTeamId = 0,
                        Outcome = MatchOutcome.Victory,
                        Rank = 1,
                        ParticipationInfo = new ParticipationInfo
                        {
                            FirstJoinedTime = "2026-01-15T10:00:00Z",
                            LastLeaveTime = "2026-01-15T10:15:00Z",
                            PresentAtBeginning = true,
                            JoinedInProgress = false,
                            LeftInProgress = false,
                            PresentAtCompletion = true,
                            ConfirmedParticipation = true,
                            TimePlayed = "00:15:00"
                        },
                        PlayerTeamStats = new PlayerTeamStat[]
                        {
                            new PlayerTeamStat
                            {
                                TeamId = 0,
                                Stats = new MatchPlayerCoreStats
                                {
                                    CoreStats = new MatchCoreStats
                                    {
                                        Score = 500,
                                        PersonalScore = 250,
                                        RoundsWon = 3,
                                        RoundsLost = 1,
                                        RoundsTied = 0,
                                        Kills = 15,
                                        Deaths = 10,
                                        Assists = 8,
                                        KDA = 2.3,
                                        Suicides = 1,
                                        Betrayals = 0,
                                        AverageLifeDuration = "00:01:30",
                                        GrenadeKills = 2,
                                        HeadshotKills = 5,
                                        MeleeKills = 1,
                                        PowerWeaponKills = 3,
                                        ShotsFired = 200,
                                        ShotsHit = 100,
                                        Accuracy = 50.0,
                                        DamageDealt = 5000.5,
                                        DamageTaken = 4000.3,
                                        CalloutAssists = 4,
                                        VehicleDestroys = 1,
                                        DriverAssists = 2,
                                        Hijacks = 0,
                                        EmpAssists = 1,
                                        MaxKillingSpree = 5,
                                        Medals = new Medal[] { new Medal { NameId = 1, Count = 2, TotalPersonalScoreAwarded = 100 } },
                                        PersonalScores = new PersonalScore[] { new PersonalScore { NameId = 1001, Count = 3, TotalPersonalScoreAwarded = 100 } },
                                        Spawns = 5,
                                        ObjectivesCompleted = 3
                                    },
                                    OddballStats = new OddballStatsData
                                    {
                                        KillsAsSkullCarrier = 2,
                                        LongestTimeAsSkullCarrier = "00:00:30",
                                        SkullCarriersKilled = 1,
                                        SkullGrabs = 3,
                                        TimeAsSkullCarrier = "00:01:00",
                                        SkullScoringTicks = 8
                                    },
                                    ZonesStats = new ZonesStatsData
                                    {
                                        StrongholdCaptures = 2,
                                        StrongholdDefensiveKills = 3,
                                        StrongholdOffensiveKills = 4,
                                        StrongholdSecures = 1,
                                        StrongholdOccupationTime = "00:01:30",
                                        StrongholdScoringTicks = 10
                                    },
                                    CaptureTheFlagStats = new CaptureTheFlagStatsData
                                    {
                                        FlagCaptureAssists = 1,
                                        FlagCaptures = 1,
                                        FlagCarriersKilled = 2,
                                        FlagGrabs = 2,
                                        FlagReturnersKilled = 1,
                                        FlagReturns = 3,
                                        FlagSecures = 1,
                                        FlagSteals = 1,
                                        KillsAsFlagCarrier = 2,
                                        KillsAsFlagReturner = 1,
                                        TimeAsFlagCarrier = "00:01:00"
                                    },
                                    EliminationStats = new EliminationStatsData
                                    {
                                        AlliesRevived = 2,
                                        EliminationAssists = 3,
                                        Eliminations = 5,
                                        EnemyRevivesDenied = 1,
                                        Executions = 1,
                                        KillsAsLastPlayerStanding = 2,
                                        LastPlayersStandingKilled = 1,
                                        RoundsSurvived = 4,
                                        TimesRevivedByAlly = 2
                                    },
                                    InfectionStats = new InfectionStatsData
                                    {
                                        AlphasKilled = 1,
                                        SpartansInfected = 2,
                                        SpartansInfectedAsAlpha = 3,
                                        KillsAsLastSpartanStanding = 2,
                                        LastSpartansStandingInfected = 1,
                                        RoundsAsAlpha = 1,
                                        RoundsAsLastSpartanStanding = 2,
                                        RoundsFinishedAsInfected = 1,
                                        RoundsSurvivedAsSpartan = 5,
                                        RoundsSurvivedAsLastSpartanStanding = 2,
                                        TimeAsLastSpartanStanding = "00:01:00",
                                        InfectedKilled = 6
                                    },
                                    StockpileStats = new StockpileStatsData
                                    {
                                        KillsAsPowerSeedCarrier = 2,
                                        PowerSeedCarriersKilled = 1,
                                        PowerSeedsDeposited = 3,
                                        PowerSeedsStolen = 1,
                                        TimeAsPowerSeedCarrier = "00:00:50",
                                        TimeAsPowerSeedDriver = "00:00:30"
                                    },
                                    VipStats = new VipStatsData
                                    {
                                        VipKills = 2,
                                        VipAssists = 1,
                                        KillsAsVip = 3,
                                        TimesSelectedAsVip = 1,
                                        MaxKillingSpreeAsVip = 3,
                                        LongestTimeAsVip = "00:01:00",
                                        TimeAsVip = "00:01:30"
                                    },
                                    ExtractionStats = new ExtractionStatsData
                                    {
                                        SuccessfulExtractions = 2,
                                        ExtractionConversionsDenied = 1,
                                        ExtractionConversionsCompleted = 1,
                                        ExtractionInitiationsDenied = 0,
                                        ExtractionInitiationsCompleted = 2
                                    }
                                }
                            }
                        }
                    },
                    new PlayerStats
                    {
                        PlayerId = "player_002",
                        PlayerType = 1,
                        BotAttributes = null,
                        LastTeamId = 1,
                        Outcome = MatchOutcome.Defeat,
                        Rank = 2,
                        ParticipationInfo = new ParticipationInfo
                        {
                            FirstJoinedTime = "2026-01-15T10:00:00Z",
                            LastLeaveTime = "2026-01-15T10:15:00Z",
                            PresentAtBeginning = true,
                            JoinedInProgress = false,
                            LeftInProgress = false,
                            PresentAtCompletion = true,
                            ConfirmedParticipation = true,
                            TimePlayed = "00:15:00"
                        },
                        PlayerTeamStats = new PlayerTeamStat[]
                        {
                            new PlayerTeamStat
                            {
                                TeamId = 1,
                                Stats = new MatchPlayerCoreStats
                                {
                                    CoreStats = new MatchCoreStats
                                    {
                                        Score = 400,
                                        PersonalScore = 200,
                                        RoundsWon = 1,
                                        RoundsLost = 3,
                                        RoundsTied = 0,
                                        Kills = 10,
                                        Deaths = 15,
                                        Assists = 5,
                                        KDA = 1.0,
                                        Suicides = 2,
                                        Betrayals = 0,
                                        AverageLifeDuration = "00:01:15",
                                        GrenadeKills = 1,
                                        HeadshotKills = 3,
                                        MeleeKills = 1,
                                        PowerWeaponKills = 2,
                                        ShotsFired = 180,
                                        ShotsHit = 72,
                                        Accuracy = 40.0,
                                        DamageDealt = 4000.5,
                                        DamageTaken = 5000.3,
                                        CalloutAssists = 3,
                                        VehicleDestroys = 0,
                                        DriverAssists = 1,
                                        Hijacks = 0,
                                        EmpAssists = 1,
                                        MaxKillingSpree = 3,
                                        Medals = new Medal[] { new Medal { NameId = 1, Count = 1, TotalPersonalScoreAwarded = 50 } },
                                        PersonalScores = new PersonalScore[] { new PersonalScore { NameId = 1001, Count = 2, TotalPersonalScoreAwarded = 75 } },
                                        Spawns = 6,
                                        ObjectivesCompleted = 2
                                    },
                                    OddballStats = new OddballStatsData
                                    {
                                        KillsAsSkullCarrier = 1,
                                        LongestTimeAsSkullCarrier = "00:00:20",
                                        SkullCarriersKilled = 2,
                                        SkullGrabs = 2,
                                        TimeAsSkullCarrier = "00:00:50",
                                        SkullScoringTicks = 5
                                    },
                                    ZonesStats = new ZonesStatsData
                                    {
                                        StrongholdCaptures = 1,
                                        StrongholdDefensiveKills = 2,
                                        StrongholdOffensiveKills = 3,
                                        StrongholdSecures = 1,
                                        StrongholdOccupationTime = "00:01:00",
                                        StrongholdScoringTicks = 6
                                    },
                                    CaptureTheFlagStats = new CaptureTheFlagStatsData
                                    {
                                        FlagCaptureAssists = 0,
                                        FlagCaptures = 1,
                                        FlagCarriersKilled = 1,
                                        FlagGrabs = 2,
                                        FlagReturnersKilled = 0,
                                        FlagReturns = 2,
                                        FlagSecures = 1,
                                        FlagSteals = 0,
                                        KillsAsFlagCarrier = 1,
                                        KillsAsFlagReturner = 1,
                                        TimeAsFlagCarrier = "00:00:50"
                                    },
                                    EliminationStats = new EliminationStatsData
                                    {
                                        AlliesRevived = 1,
                                        EliminationAssists = 2,
                                        Eliminations = 3,
                                        EnemyRevivesDenied = 1,
                                        Executions = 0,
                                        KillsAsLastPlayerStanding = 1,
                                        LastPlayersStandingKilled = 2,
                                        RoundsSurvived = 2,
                                        TimesRevivedByAlly = 3
                                    },
                                    InfectionStats = new InfectionStatsData
                                    {
                                        AlphasKilled = 1,
                                        SpartansInfected = 1,
                                        SpartansInfectedAsAlpha = 2,
                                        KillsAsLastSpartanStanding = 1,
                                        LastSpartansStandingInfected = 2,
                                        RoundsAsAlpha = 1,
                                        RoundsAsLastSpartanStanding = 1,
                                        RoundsFinishedAsInfected = 2,
                                        RoundsSurvivedAsSpartan = 3,
                                        RoundsSurvivedAsLastSpartanStanding = 1,
                                        TimeAsLastSpartanStanding = "00:00:50",
                                        InfectedKilled = 4
                                    },
                                    StockpileStats = new StockpileStatsData
                                    {
                                        KillsAsPowerSeedCarrier = 1,
                                        PowerSeedCarriersKilled = 2,
                                        PowerSeedsDeposited = 2,
                                        PowerSeedsStolen = 1,
                                        TimeAsPowerSeedCarrier = "00:00:40",
                                        TimeAsPowerSeedDriver = "00:00:20"
                                    },
                                    VipStats = new VipStatsData
                                    {
                                        VipKills = 1,
                                        VipAssists = 1,
                                        KillsAsVip = 2,
                                        TimesSelectedAsVip = 0,
                                        MaxKillingSpreeAsVip = 2,
                                        LongestTimeAsVip = "00:00:50",
                                        TimeAsVip = "00:01:00"
                                    },
                                    ExtractionStats = new ExtractionStatsData
                                    {
                                        SuccessfulExtractions = 1,
                                        ExtractionConversionsDenied = 2,
                                        ExtractionConversionsCompleted = 1,
                                        ExtractionInitiationsDenied = 1,
                                        ExtractionInitiationsCompleted = 1
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
