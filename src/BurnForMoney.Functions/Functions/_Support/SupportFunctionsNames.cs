using System.Diagnostics.CodeAnalysis;

namespace BurnForMoney.Functions.Functions._Support
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class SupportFunctionsNames
    {
        public const string Prefix = "Support_";

        public const string CollectMonthlyStatistics = Prefix + "CollectMonthlyStatistics";
        public const string ActivateAthlete = Prefix + "ActivateAthlete";
        public const string DeactivateAthlete = Prefix + "DeactivateAthlete";
        public const string DeleteAthlete = Prefix + "DeleteAthlete";
        public const string EncryptText = Prefix + "EncryptText";
        public const string DecryptText = Prefix + "DecryptText";
        public const string ReprocessPoisonQueueMessages = Prefix + "ReprocessPoisonQueueMessages";
    }
}