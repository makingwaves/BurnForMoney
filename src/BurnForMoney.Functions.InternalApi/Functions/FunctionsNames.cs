using BurnForMoney.Functions.Shared;

namespace BurnForMoney.Functions.InternalApi.Functions
{
    public static class FunctionsNames
    {
        public const string HttpTriggerPrefix = FunctionNameConvention.HttpTriggerPrefix;

        public const string AddActivity = HttpTriggerPrefix + "AddActivity";
        public const string UpdateActivity = HttpTriggerPrefix + "UpdateActivity";
        public const string DeleteActivity = HttpTriggerPrefix + "DeleteActivity";
        public const string CreateAthlete = HttpTriggerPrefix + "CreateAthlete";
        public const string AssignActiveDirectoryIdToAthlete = HttpTriggerPrefix + "AssignActiveDirectoryIdToAthlete";

        public const string GetAthletes = HttpTriggerPrefix + "GetAthletes";
        public const string GetAthlete = HttpTriggerPrefix + "GetAthlete";
        public const string GetAthleteActivities = HttpTriggerPrefix + "GetAthleteActivities";
        public const string GetActivityCategories = HttpTriggerPrefix + "GetActivityCategories";

        public const string GetTopAthletesForGivenActivityType = HttpTriggerPrefix + "GetTopAthletesForGivenActivityType";
        public const string GetDashboardTop = HttpTriggerPrefix + "GetDashboardTop";
    }
}