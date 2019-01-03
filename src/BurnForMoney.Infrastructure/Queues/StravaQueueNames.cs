namespace BurnForMoney.Functions.Infrastructure.Queues
{
    public static class StravaQueueNames
    {
        private const string PoisonQueueSuffix = "-poison";

        public const string AuthorizationCodes = "authorization-codes";
        public const string AuthorizationCodesPoison = AuthorizationCodes + PoisonQueueSuffix;
        public const string UnauthorizedAthletes = "unauthorized-athletes";
        public const string RefreshStravaToken = "refresh-strava-token";
        public const string CollectAthleteActivities = "collect-athlete-activities";
    }
}