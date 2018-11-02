namespace BurnForMoney.Functions.Shared.Queues
{
    public static class AppQueueNames
    {
        public const string CalculateMonthlyResults = "calculate-monthly-results";

        public const string PendingRawActivities = "pending-raw-activities";
        public const string PendingActivities = "pending-activities";
        public const string PendingActivitiesUpdates = "pending-activities-updates";

        public const string NotificationsToSend = "notifications-to-send";
    }
}