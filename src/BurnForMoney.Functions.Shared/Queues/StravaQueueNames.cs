namespace BurnForMoney.Functions.Shared.Queues
{
    public static class StravaQueueNames
    {
        private const string PoisonQueueSuffix = "-poison";

        public const string AuthorizationCodes = "authorization-codes";
        public const string AuthorizationCodesPoison = AuthorizationCodes + PoisonQueueSuffix;
        public const string UnauthorizedAthletes = "unauthorized-athletes";

        public const string RefreshStravaToken = "refresh-strava-token";

        public const string StravaEvents = "strava-events";
        public const string StravaEventsActivityAdd = "strava-events-activity-add";
        public const string StravaEventsActivityUpdate = "strava-events-activity-update";
        public const string StravaEventsActivityDelete = "strava-events-activity-delete";

        public const string CollectAthleteActivities = "collect-athlete-activities";

        public const string DeactivateAthleteRequests = "strava-deactivate-athlete-requests";
        public const string ActivateAthleteRequests = "strava-activate-athlete-requests";
    }
}