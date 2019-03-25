using BurnForMoney.Functions.Shared;

namespace BurnForMoney.Functions.InternalApi.Functions
{
    public static class FunctionsNames
    {
        public const string HttpTriggerPrefix = FunctionNameConvention.HttpTriggerPrefix;

        public const string AddActivity = HttpTriggerPrefix + "AddActivity";
        public const string UpdateActivity = HttpTriggerPrefix + "UpdateActivity";
        public const string DeleteActivity = HttpTriggerPrefix + "DeleteActivity";
        public const string SignInAthlete = HttpTriggerPrefix + "SignInAthlete";
        public const string AsignActiveDirectoryIdToAthlete = HttpTriggerPrefix + "AsignActiveDirectoryIdToAthlete";

        public const string GetAthletes = HttpTriggerPrefix + "GetAthletes";
        public const string GetCurrentAthelte = HttpTriggerPrefix + "GetCurrentAthelte";
        public const string GetAthleteActivities = HttpTriggerPrefix + "GetAthleteActivities";
        public const string GetActivityCategories = HttpTriggerPrefix + "GetActivityCategories";

        public const string GetTopAthletesForGivenActivityType = HttpTriggerPrefix + "GetTopAthletesForGivenActivityType";
    }
}