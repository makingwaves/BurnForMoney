namespace BurnForMoney.Functions.Helpers
{
    public static class QueueNames
    {
        public const string AuthorizationCodes = "queue-authorization-codes";
        public const string AuthorizationCodesPoison = "queue-authorization-codes-poison";
        public const string PendingActivities = "queue-pending-activities";
    }
}