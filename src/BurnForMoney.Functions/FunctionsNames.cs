using System.Diagnostics.CodeAnalysis;

namespace BurnForMoney.Functions
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class FunctionsNames
    {
        public const string Q_ProcessRawActivity = "Q_ProcessRawActivity";
        public const string Q_ProcessRawUpdatedActivity = "Q_ProcessRawUpdatedActivity";
        public const string Q_DeleteActivity = "Q_DeleteActivity";
        public const string Q_SubmitAthleteActivity = "Q_SubmitAthleteActivity";
        public const string Q_UpdateAthleteActivity = "Q_UpdateAthleteActivity";

        public const string T_CalculateMonthlyAthleteResults = "T_CalculateMonthlyAthleteResults";
        public const string T_CalculateMonthlyAthleteResultsFromPreviousMonth = "T_CalculateMonthlyAthleteResultsFromPreviousMonth";
        public const string Q_CalculateMonthlyAthleteResults = "Q_CalculateMonthlyAthleteResults";

        public const string Support_EncryptString = "Support_EncryptString";
        public const string Support_DecryptString = "Support_DecryptString";
        public const string Support_Activities_CollectMonthlyStatistics = "Support_Activities_CollectMonthlyStatistics";
        public const string Support_Activities_Add = "Support_Activities_Add";
        public const string Support_Athlete_Deactivate = "Support_Athlete_Deactivate";
        public const string Support_Athlete_Activate = "Support_Athlete_Activate";
        public const string Support_Athlete_Delete = "Support_Athlete_Delete";

        public const string Support_ReprocessPoisonQueueMessages = "Support_ReprocessPoisonQueueMessages";
        public const string NotificationsGateway = "NotificationsGateway";
        public const string T_GenerateReport = "T_GenerateReport";
        public const string B_SendNotificationWithLinkToTheReport = "B_SendNotificationWithLinkToTheReport";
    }
}