using System.Diagnostics.CodeAnalysis;

namespace BurnForMoney.Functions.Functions
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

        public const string Q_NotificationsGateway = "Q_NotificationsGateway";
        public const string T_GenerateReport = "T_GenerateReport";
        public const string B_SendNotificationWithLinkToTheReport = "B_SendNotificationWithLinkToTheReport";
    }
}