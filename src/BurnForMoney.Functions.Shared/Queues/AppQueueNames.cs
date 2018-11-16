namespace BurnForMoney.Functions.Shared.Queues
{
    public static class AppQueueNames
    {
        public const string DeleteActivityRequests = "delete-activity-requests";
        public const string AddActivityRequests = "add-activity-requests";
        public const string UpdateActivityRequests = "update-activity-requests";

        public const string NotificationsToSend = "notifications-to-send";
    }
}