using System.Diagnostics.CodeAnalysis;
using BurnForMoney.Functions.Shared;

namespace BurnForMoney.Functions.Functions
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class FunctionsNames
    {
        public const string QueueTriggerPrefix = FunctionNameConvention.QueueTriggerPrefix;
        public const string TimerTriggerPrefix = FunctionNameConvention.TimerTriggerPrefix;
        public const string BlobTriggerPrefix = FunctionNameConvention.BlobTriggerPrefix;

        public const string Q_AddAthlete = QueueTriggerPrefix + "AddAthlete";
        public const string Q_DeactivateAthlete = QueueTriggerPrefix + "DeactivateAthlete";
        public const string Q_ActivateAthlete = QueueTriggerPrefix + "ActivateAthlete";
        public const string Q_AssignActiveDirectoryIdToAthlete = QueueTriggerPrefix + "AssignActiveDirectoryId";

        public const string Q_AddActivity = QueueTriggerPrefix + "AddActivity";
        public const string Q_UpdateActivity = QueueTriggerPrefix + "UpdateActivity";
        public const string Q_DeleteActivity = QueueTriggerPrefix + "DeleteActivity";

        public const string Q_NotificationsGateway = QueueTriggerPrefix + "NotificationsGateway";
        public const string T_GenerateReport = TimerTriggerPrefix + "GenerateReport";
        public const string B_SendNotificationWithLinkToTheReport = BlobTriggerPrefix + "SendNotificationWithLinkToTheReport";
        
        public const string T_MonitorPoisonQueues = TimerTriggerPrefix + "MonitorPoisonQueues";
    }
}