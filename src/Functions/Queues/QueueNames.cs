namespace BurnForMoney.Functions.Queues
{
    public static class QueueNames
    {
        private const string PoisonQueueSuffix = "-poison";

        public const string AuthorizationCodes = "queue-authorization-codes";
        public const string AuthorizationCodesPoison = AuthorizationCodes + PoisonQueueSuffix;
        public const string PendingRawActivities = "queue-pending-raw-activities";
        public const string PendingActivities = "queue-pending-activities";
    }
}