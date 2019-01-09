using System.Diagnostics.CodeAnalysis;
using BurnForMoney.Functions.Shared;

namespace BurnForMoney.Functions.Functions._Support
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class SupportFunctionsNames
    {
        public const string Prefix = SupportFunctionNameConvention.Prefix;
        public const string ReprocessPoisonQueueMessages = Prefix + "ReprocessPoisonQueueMessages";
    }
}