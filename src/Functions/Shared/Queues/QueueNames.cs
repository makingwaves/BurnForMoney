namespace BurnForMoney.Functions.Shared.Queues
{
    public static class QueueNames
    {
        private const string PoisonQueueSuffix = "-poison";

        public const string AuthorizationCodes = "authorization-codes";
        public const string AuthorizationCodesPoison = AuthorizationCodes + PoisonQueueSuffix;

        public const string NewStravaAthletesRequests = "new-strava-athletes-requests";
        public const string NewStravaAthletesRequestsPoison = NewStravaAthletesRequests + PoisonQueueSuffix;
        public const string RefreshStravaToken = "refresh-strava-token";

        public const string UnauthorizedAccessTokens = "unauthorized-access-tokens";


        public const string CollectAthleteActivities = "collect-athlete-activities";
        public const string CalculateMonthlyResults = "calculate-monthly-results";

        public const string PendingRawActivities = "pending-raw-activities";
        public const string PendingActivities = "pending-activities";
        public const string PendingActivitiesUpdates = "pending-activities-updates";

        public const string StravaEvents = "strava-events";
        public const string StravaEventsActivityAdd = "strava-events-activity-add";
        public const string StravaEventsActivityUpdate = "strava-events-activity-update";
        public const string StravaEventsActivityDelete = "strava-events-activity-delete";
        public const string StravaEventsAthleteDeauthorized = "strava-events-athlete-deauthorized";
    }
}