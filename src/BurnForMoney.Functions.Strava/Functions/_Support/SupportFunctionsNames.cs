using System.Diagnostics.CodeAnalysis;

namespace BurnForMoney.Functions.Strava.Functions._Support
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class SupportFunctionsNames
    {
        public const string Prefix = "Support_";

        public const string PullAthleteActivities = Prefix + "PullAthleteActivities";
        public const string PullAllAthletesActivities = Prefix + "PullAllAthletesActivities";
        public const string ReprocessPoisonQueueMessages = Prefix + "ReprocessPoisonQueueMessages";
        public const string PurgeDurableHubHistory = Prefix + "PurgeDurableHubHistory";
    }
}