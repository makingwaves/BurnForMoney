namespace BurnForMoney.Functions.Shared.Queues
{
    public static class AppQueueNames
    {
        public const string DeleteActivityRequests = "delete-activity-requests";
        public const string UpsertRawActivitiesRequests = "upsert-activities-requests";

        public const string NotificationsToSend = "notifications-to-send";
    }
}