using System.Diagnostics.CodeAnalysis;
using BurnForMoney.Functions.Shared;

namespace BurnForMoney.Functions.Strava.Functions._Support
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class SupportFunctionsNames
    {
        public const string Prefix = SupportFunctionNameConvention.Prefix;

        public const string PullAthleteActivities = Prefix + "PullAthleteActivities";
        public const string PullAllAthletesActivities = Prefix + "PullAllAthletesActivities";
        public const string ReprocessPoisonQueueMessages = Prefix + "ReprocessPoisonQueueMessages";
        public const string PurgeDurableHubHistory = Prefix + "PurgeDurableHubHistory";
        public const string DeactivateAthlete = Prefix + "DeactivateAthlete";
        public const string ActivateAthlete = Prefix + "ActivateAthlete";
    }
}