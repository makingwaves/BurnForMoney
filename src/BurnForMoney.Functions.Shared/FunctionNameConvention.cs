namespace BurnForMoney.Functions.Shared
{
    public static class FunctionNameConvention
    {
        public const string HttpTriggerPrefix = "Http_";
        public const string QueueTriggerPrefix = "Q_";
        public const string TimerTriggerPrefix = "T_";
        public const string BlobTriggerPrefix = "B_";
        public const string DurableActivityPrefix = "A_";
        public const string DurableOrchestratorPrefix = "O_";
    }
}