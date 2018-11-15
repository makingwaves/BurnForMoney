using System.Diagnostics.CodeAnalysis;

namespace BurnForMoney.Functions.Strava
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class SupportFunctionsNames
    {
        public const string Prefix = "_Support_";

        public const string PullAthleteActivities = Prefix + "PullAthleteActivities";
        public const string PullAllAthletesActivities = Prefix + "PullAllAthletesActivities";
        public const string ReprocessPoisonQueueMessages = Prefix + "ReprocessPoisonQueueMessages";
        public const string PurgeDurableHubHistory = Prefix + "PurgeDurableHubHistory";
    }
}